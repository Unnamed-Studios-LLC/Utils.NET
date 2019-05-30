using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.NET.Logging;
using Utils.NET.Net.Tcp;

namespace Utils.NET.Net
{
    public class PacketHandlerFactory<TCon, THandler, TPacket> 
        where TPacket : Packet
        where TCon : NetConnection<TPacket>
        where THandler : IPacketHandler<TCon, TPacket>
    {
        private Dictionary<byte, IPacketHandler<TCon, TPacket>> handlerTypes;

        public PacketHandlerFactory()
        {
            var handler = typeof(THandler).GetGenericTypeDefinition();
            handlerTypes = handler.Assembly.GetTypes().Where(_ => IsPacketHandler(_, handler)).Select(_ => (IPacketHandler<TCon, TPacket>)Activator.CreateInstance(_)).ToDictionary(_ => _.Id);
        }

        private bool IsPacketHandler(Type sub, Type baseClass)
        {
            var baseType = sub.BaseType;
            if (!baseType.IsAbstract) return false;
            return baseType.IsGenericType && (baseType.GetGenericTypeDefinition() == baseClass);
        }

        public void Handle(TPacket packet, TCon connection)
        {
            if (!handlerTypes.TryGetValue(packet.Id, out var handler)) return;
            handler.Handle(packet, connection);
        }
    }
}
