using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Utils.NET.Net.Tcp
{
    public abstract class NetConnectionFactory<TCon, TPacket>
        where TPacket : Packet
        where TCon : NetConnection<TPacket>
    {
        public abstract TCon CreateConnection(Socket socket);
        public abstract void HandleConnection(TCon connection);
    }
}
