using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Utils.NET.IO;

namespace Utils.NET.Net.Udp.Reliability
{
    public class ReliableChannel : IPacketChannel
    {
        /// <summary>
        /// Dictionary containing all sent packets awaiting acknowledgement
        /// </summary>
        private ConcurrentDictionary<uint, Packet> sentPackets = new ConcurrentDictionary<uint, Packet>();

        public void ReceivePacket(BitReader r, Packet packet)
        {

        }

        public void SendPacket(BitWriter w, Packet packet)
        {

        }
    }
}
