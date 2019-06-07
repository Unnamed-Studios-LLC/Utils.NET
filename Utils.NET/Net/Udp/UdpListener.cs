using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Utils.NET.Net.Udp
{
    public class UdpListener<TCon, TPacket> 
        where TPacket : Packet 
        where TCon : UdpClient<TPacket>
    {
        /// <summary>
        /// The max amount of clients that can connect
        /// </summary>
        private int maxClients;

        /// <summary>
        /// The port that the listener listens on
        /// </summary>
        private ushort port;

        public UdpListener(ushort port, int maxClients)
        {

        }
    }
}
