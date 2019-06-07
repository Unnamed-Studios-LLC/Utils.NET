using System;
using Utils.NET.IO;

namespace Utils.NET.Net.Udp.Packets
{
    public class UdpAwaitingState : UdpPacket
    {
        public override UdpPacketType Type => UdpPacketType.AwaitingState;

        /// <summary>
        /// The packet type that the other connection is awaiting
        /// </summary>
        public UdpPacketType awaitingType;

        public UdpAwaitingState() { }

        public UdpAwaitingState(UdpPacketType awaitingType) => this.awaitingType = awaitingType;

        protected override void Read(BitReader r)
        {
            awaitingType = (UdpPacketType)r.ReadUInt8();
        }

        protected override void Write(BitWriter w)
        {
            w.Write((byte)awaitingType);
        }
    }
}
