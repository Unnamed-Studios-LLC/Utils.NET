using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Utils.NET.Net.Tcp
{
    public class NetListener<TCon, TPacket> 
        where TPacket : Packet 
        where TCon : NetConnection<TPacket>
    {
        public EndPoint localEndPoint;

        private Socket socket;

        private NetConnectionFactory<TCon, TPacket> connectionFactory;

        public NetListener(ushort port, NetConnectionFactory<TCon, TPacket> connectionFactory)
        {
            this.connectionFactory = connectionFactory;

            localEndPoint = new IPEndPoint(IPAddress.Any, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(localEndPoint);

            socket.Listen(5);
        }

        public void Start()
        {
            socket.BeginAccept(OnAcceptCallback, null);
        }

        public void Stop()
        {
            socket.Close();
        }

        private void OnAcceptCallback(IAsyncResult ar)
        {
            Socket remoteSocket = socket.EndAccept(ar);
            TCon connection = connectionFactory.CreateConnection(remoteSocket);
            if (connection == null)
            {
                remoteSocket.Close();
                return;
            }
            connectionFactory.HandleConnection(connection);
        }
    }
}
