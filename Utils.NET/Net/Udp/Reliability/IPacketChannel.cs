using System;
using System.Collections.Generic;
using System.Text;
using Utils.NET.IO;

namespace Utils.NET.Net.Udp.Reliability
{
    public interface IPacketChannel
    {
        void SendPacket(BitWriter w, Packet packet);

        void ReceivePacket(BitReader r, Packet packet);
    }
}
