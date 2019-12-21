using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Utils.NET.Logging;

namespace Utils.NET.Net.Tcp
{
    public abstract class NetListener<TCon, TPacket> 
        where TPacket : Packet
        where TCon : NetConnection<TPacket>
    {
        public IPEndPoint localEndPoint;

        private Socket socket;

        public NetListener(int port)
        {
            localEndPoint = new IPEndPoint(IPAddress.Any, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(localEndPoint);

            socket.Listen(5);
        }

        public virtual void Start()
        {
            Log.Write(LogEntry.Init(this).Append(" is listening on port: " + localEndPoint.Port));
            socket.BeginAccept(OnAcceptCallback, null);
        }

        public virtual void Stop()
        {
            socket.Close();
        }

        private void OnAcceptCallback(IAsyncResult ar)
        {
            Socket remoteSocket = socket.EndAccept(ar);
            TCon connection = (TCon)Activator.CreateInstance(typeof(TCon), remoteSocket);
            if (connection == null)
            {
                remoteSocket.Close();
                return;
            }
            HandleConnection(connection);
        }

        protected abstract void HandleConnection(TCon connection);
    }
}
