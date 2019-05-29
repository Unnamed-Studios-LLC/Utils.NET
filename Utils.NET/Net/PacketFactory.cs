using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.NET.IO;

namespace Utils.NET.Net
{
    public abstract class PacketFactory<TPacket> where TPacket : Packet
    {
        private Dictionary<byte, Type> packetTypes;

        public PacketFactory()
        {
            packetTypes = new Dictionary<byte, Type>();
            var t = typeof(TPacket);
            packetTypes = t.Assembly.GetTypes().Where(_ => _.IsSubclassOf(t)).ToDictionary(_ => ((TPacket)Activator.CreateInstance(_)).Id);
        }

        public TPacket CreatePacket(byte id)
        {
            if (!packetTypes.TryGetValue(id, out var type))
                return null;
            return (TPacket)Activator.CreateInstance(type);
        }

        public abstract void HandlePacket(TPacket packet);
    }
}
