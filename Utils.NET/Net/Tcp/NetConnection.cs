using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using Utils.NET.IO;

namespace Utils.NET.Net.Tcp
{
    public class NetConnection<TPacket> where TPacket : Packet
    {
        private enum ReceiveState
        {
            Size,
            Payload
        }

        private Socket socket;

        private IO.Buffer buffer;

        private PacketFactory<TPacket> packetFactory;

        public NetConnection(Socket socket, PacketFactory<TPacket> packetFactory)
        {
            this.socket = socket;
            this.packetFactory = packetFactory;
            SetupSocket();
            buffer = new IO.Buffer(4);
        }

        private void SetupSocket()
        {
            socket.NoDelay = true;
        }

        public void BeginRead()
        {
            BeginReadSize();
        }

        private void ReceivedSize(int size)
        {
            buffer.Reset(size);
            BeginReadPayload();
        }

        private void ReceivedPayload(byte[] data)
        {
            BitReader w = new BitReader(data, data.Length);
            TPacket packet = packetFactory.CreatePacket(w.ReadUInt8());
            if (packet == null) return;
            packetFactory.HandlePacket(packet);
        }

        private bool TryEndRead(IAsyncResult ar)
        {
            int length = socket.EndReceive(ar, out SocketError error);
            if (error != SocketError.Success)
            {
                Console.WriteLine("SocketError received on EndRead: " + error);
                return false;
            }

            if (length <= 0) // closed socket, disconnect
            {

                return false;
            }

            buffer.size += length; // data was read into the buffer, increment the size accordingly
            return true;
        }

        private void BeginReadSize()
        {
            socket.BeginReceive(buffer.data, buffer.size, buffer.RemainingLength, SocketFlags.None, out SocketError error, OnReadSizeCallback, null);
            if (error == SocketError.Success) return;
            Console.WriteLine("SocketError received on BeginRead: " + error);
        }

        private void OnReadSizeCallback(IAsyncResult ar)
        {
            if (!TryEndRead(ar))
                return;

            if (buffer.RemainingLength > 0)
            {
                BeginReadSize(); // still need more data
            }
            else
            {
                int size = (buffer.data[0] << 24) |
                    (buffer.data[1] << 16) |
                    (buffer.data[2] << 8) |
                    (buffer.data[3]);
                ReceivedSize(size);
            }
        }

        private void BeginReadPayload()
        {
            socket.BeginReceive(buffer.data, buffer.size, buffer.RemainingLength, SocketFlags.None, out SocketError error, OnReadPayloadCallback, null);
            if (error == SocketError.Success) return;
            Console.WriteLine("SocketError received on BeginRead: " + error);
        }

        private void OnReadPayloadCallback(IAsyncResult ar)
        {
            if (!TryEndRead(ar))
                return;

            if (buffer.RemainingLength > 0)
            {
                BeginReadPayload(); // need more data
            }
            else
            {
                ReceivedPayload(buffer.data);
                buffer.Reset(4);
                BeginReadSize();
            }
        }
    }
}
