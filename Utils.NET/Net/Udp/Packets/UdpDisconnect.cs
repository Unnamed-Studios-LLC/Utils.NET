using System;
using System.Collections.Generic;
using System.Text;
using Utils.NET.IO;

namespace Utils.NET.Net.Udp.Packets
{
    public class UdpDisconnect : UdpPacket
    {
        public override UdpPacketType Type => UdpPacketType.Disconnect;

        /// <summary>
        /// Generated salt from server/client
        /// </summary>
        public ulong salt;

        public UdpDisconnect() { }

        public UdpDisconnect(ulong salt) => this.salt = salt;

        protected override void Read(BitReader r)
        {
            salt = r.ReadUInt64();
        }

        protected override void Write(BitWriter w)
        {
            w.Write(salt);
        }
    }
}
