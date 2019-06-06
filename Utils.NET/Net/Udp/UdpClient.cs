using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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

        #region Init

        public UdpClient()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
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
        }

        #endregion

        #region Sending

        private void Send()
        {

        }

        #endregion
    }
}
