using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Utils.NET.Net.Tcp
{
    public class NetListener<TPacket> where TPacket : Packet
    {
        public EndPoint localEndPoint;

        private Socket socket;

        private NetConnectionFactory<TPacket> connectionFactory;

        public NetListener(ushort port, NetConnectionFactory<TPacket> connectionFactory)
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

        private void OnAcceptCallback(IAsyncResult ar)
        {
            Socket remoteSocket = socket.EndAccept(ar);
            NetConnection<TPacket> connection = connectionFactory.CreateConnection(remoteSocket);
            if (connection == null)
            {
                remoteSocket.Close();
                return;
            }
            connectionFactory.HandleConnection(connection);
        }
    }
}
