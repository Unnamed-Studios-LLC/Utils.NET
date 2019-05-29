using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Utils.NET.Net.Tcp
{
    public abstract class NetConnectionFactory<TPacket> where TPacket : Packet
    {
        public abstract NetConnection<TPacket> CreateConnection(Socket socket);
        public abstract void HandleConnection(NetConnection<TPacket> connection);
    }
}
