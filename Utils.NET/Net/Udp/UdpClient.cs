using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Utils.NET.IO;
using Utils.NET.Logging;

namespace Utils.NET.Net.Udp
{
    public class UdpClient<TPacket> where TPacket : Packet
    {
        private enum ConnectionState
        {
            Disconnected = 0,
            Connected = 1,
            Connecting = 2
        }

        private const int Max_Packet_Size = 980;

        /// <summary>
        /// Underlying system socket used to send and receive data
        /// </summary>
        private Socket socket;

        /// <summary>
        /// The EndPoint that this client is connecting or connected to
        /// </summary>
        private EndPoint remoteEndPoint;

        /// <summary>
        /// The state of the virtual connection
        /// </summary>
        private ConnectionState state = ConnectionState.Disconnected;

        /// <summary>
        /// Factory used to create packets
        /// </summary>
        private PacketFactory<TPacket> packetFactory;

        /// <summary>
        /// Buffer used to hold received data
        /// </summary>
        private IO.Buffer buffer;

        /// <summary>
        /// Object used to sync sending states
        /// </summary>
        private object sendSync = new object();

        /// <summary>
        /// Bool determining if the client is currently sending
        /// </summary>
        private bool sending = false;

        /// <summary>
        /// Queue used to store data ready to send
        /// </summary>
        private Queue<TPacket> sendQueue = new Queue<TPacket>();

        #region Init

        public UdpClient()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Init();
        }

        public UdpClient(Socket socket)
        {
            this.socket = socket;
            Init();
        }

        private void Init()
        {
            packetFactory = new PacketFactory<TPacket>();
            buffer = new IO.Buffer(Max_Packet_Size);
        }

        #endregion

        #region Connection

        /// <summary>
        /// Connects to a given string ip address and port
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Connect(string ip, int port)
        {
            Connect(new IPEndPoint(IPAddress.Parse(ip), port));
        }

        /// <summary>
        /// Connects to a given ip and port
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Connect(long ip, int port)
        {
            Connect(new IPEndPoint(ip, port));
        }

        /// <summary>
        /// Connects to a given EndPoint
        /// </summary>
        /// <param name="endpoint"></param>
        public void Connect(EndPoint endpoint)
        {
            remoteEndPoint = endpoint;
        }

        #endregion

        #region Reading

        /// <summary>
        /// Starts reading for incoming packets
        /// </summary>
        public void StartRead()
        {
            if (state != ConnectionState.Connected) return;
            BeginRead();
        }

        private void BeginRead()
        {
            socket.BeginReceiveFrom(buffer.data, 0, Max_Packet_Size, SocketFlags.None, ref remoteEndPoint, OnRead, state);
        }

        private void OnRead(IAsyncResult ar)
        {
            int length = socket.EndReceiveFrom(ar, ref remoteEndPoint);

            byte[] data = buffer.data;
            BitReader r = new BitReader(data, length);
            byte id = r.ReadUInt8();
            TPacket packet = packetFactory.CreatePacket(id);
            if (packet == null)
            {
                Log.Error($"No {typeof(TPacket).Name} for id: {id}");
                return;
            }
            packet.ReadPacket(r);
            HandlePacket(packet);
        }

        /// <summary>
        /// Handles a received packet
        /// </summary>
        /// <param name="packet">Packet.</param>
        private void HandlePacket(TPacket packet)
        {

        }

        #endregion

        #region Sending

        /// <summary>
        /// Sends a given packet
        /// </summary>
        /// <param name="packet">Packet.</param>
        public void Send(TPacket packet)
        {
            lock (sendSync)
            {
                if (sending)
                {
                    sendQueue.Enqueue(packet);
                    return;
                }
                sending = true;
            }

            SendPacket(packet);
        }

        /// <summary>
        /// Sends a packet to the remote 
        /// </summary>
        /// <param name="packet">Packet.</param>
        private void SendPacket(TPacket packet)
        {
            var package = PackagePacket(packet);
            socket.BeginSendTo(package.data, 0, package.size, SocketFlags.None, remoteEndPoint, OnSend, null);
        }

        /// <summary>
        /// Callback function for socket send calls
        /// </summary>
        /// <param name="ar">Ar.</param>
        private void OnSend(IAsyncResult ar)
        {
            int sent = socket.EndSendTo(ar);

            TPacket nextPacket;
            lock (sendSync)
            {
                if (sendQueue.Count == 0)
                {
                    sending = false;
                    return;
                }
                nextPacket = sendQueue.Dequeue();
            }

            SendPacket(nextPacket);
        }

        /// <summary>
        /// Packages a packet into a buffer to send
        /// </summary>
        /// <returns>The packet.</returns>
        /// <param name="packet">Packet.</param>
        private IO.Buffer PackagePacket(TPacket packet)
        {
            var w = new BitWriter();
            packet.WritePacket(w);
            return w.GetData();
        }

        #endregion
    }
}
