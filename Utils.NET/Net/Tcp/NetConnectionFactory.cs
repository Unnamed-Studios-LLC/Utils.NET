using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Utils.NET.Net.Tcp
{
    public abstract class NetConnectionFactory<Tcon, TPacket> 
        where TPacket : Packet
        where Tcon : NetConnection<TPacket>
    {
        public abstract void HandleConnection(Tcon connection);
    }
}
