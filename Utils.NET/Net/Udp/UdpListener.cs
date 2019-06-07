using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Utils.NET.Net.Udp
{
    public class UdpListener<TCon, TPacket> 
        where TPacket : Packet 
        where TCon : UdpClient<TPacket>
    {
        /// <summary>
        /// The port that the listener listens on
        /// </summary>
        private int port;

        /// <summary>
        /// A queue consisting of all available ports
        /// </summary>
        private ConcurrentQueue<int> availablePorts = new ConcurrentQueue<int>();

        /// <summary>
        /// Socket used to receive connection packets
        /// </summary>
        private Socket socket;

        #region Init

        public UdpListener(ushort port, int maxClients)
        {
            this.port = port;
            InitPorts(maxClients);
            InitSocket();
        }

        /// <summary>
        /// Fills the available ports queue will all ports
        /// </summary>
        /// <param name="maxClients"></param>
        private void InitPorts(int maxClients)
        {
            for (int i = 1; i <= maxClients; i++)
            {
                availablePorts.Enqueue(port + i);
            }
        }

        /// <summary>
        /// Initializes the socket for catching incoming connections
        /// </summary>
        private void InitSocket()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        #endregion

        #region Reading

        /// <summary>
        /// Starts accepting new clients
        /// </summary>
        public void Start()
        {

        }

        #endregion

    }
}
