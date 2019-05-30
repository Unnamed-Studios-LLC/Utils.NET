﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using Utils.NET.IO;
using Utils.NET.Logging;

namespace Utils.NET.Net.Tcp
{
    public abstract class NetConnection<TPacket> where TPacket : Packet
    {
        public delegate void OnConnectCallback(bool success, NetConnection<TPacket> connection);

        public delegate void OnDisconnectCallback(NetConnection<TPacket> connection);

        private enum ReceiveState
        {
            Size,
            Payload
        }

        /// <summary>
        /// System socket used to send and receive data
        /// </summary>
        private Socket socket;

        /// <summary>
        /// Buffer used to hold received data
        /// </summary>
        private IO.Buffer buffer;

        /// <summary>
        /// Factory used to create packets from received data
        /// </summary>
        private PacketFactory<TPacket> packetFactory;

        /// <summary>
        /// Value used to syncronize disconnection calls
        /// </summary>
        private int disconnected = 0;

        /// <summary>
        /// True if this connection has been disconnected
        /// </summary>
        public bool Disconnected => disconnected == 1;

        /// <summary>
        /// Delegate to be called upon disconnect
        /// </summary>
        private OnDisconnectCallback disconnectCallback;

        public NetConnection(Socket socket)
        {
            this.socket = socket;
            Init();
        }

        public NetConnection()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Init();
        }

        private void Init()
        {
            packetFactory = new PacketFactory<TPacket>();
            socket.NoDelay = true;
            buffer = new IO.Buffer(4);
        }

        #region Connection

        #region Connect

        public bool Connect(string host, int port)
        {
            try
            {
                socket.Connect(host, port);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Connect(EndPoint endPoint)
        {
            try
            {
                socket.Connect(endPoint);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ConnectAsync(string host, int port, OnConnectCallback callback)
        {
            try
            {
                socket.BeginConnect(host, port, OnConnect, callback);
            }
            catch (SocketException)
            {
                callback(false, this);
            }
            catch (ObjectDisposedException)
            {
                callback(false, this);
            }
        }

        public void ConnectAsync(EndPoint endPoint, OnConnectCallback callback)
        {
            try
            {
                socket.BeginConnect(endPoint, OnConnect, callback);
            }
            catch (ObjectDisposedException)
            {
                callback(false, this);
            }
        }

        public void OnConnect(IAsyncResult ar)
        {
            var callback = (OnConnectCallback)ar.AsyncState;
            try
            {
                socket.EndConnect(ar);
            }
            catch (ObjectDisposedException) { }
            catch (SocketException) { }
            callback(socket.Connected, this);
        }

        #endregion

        #region Disconnect 

        public bool Disconnect()
        {
            if (Interlocked.CompareExchange(ref disconnected, 1, 0) == 1) return false; // return if this method was already called
            DoDisconnect();
            return true;
        }

        protected virtual void DoDisconnect()
        {
            disconnectCallback?.Invoke(this);
            socket.Close();
        }

        /// <summary>
        /// Sets the callback to be called upon disconnection
        /// </summary>
        /// <param name="callback"></param>
        public void SetDisconnectCallback(OnDisconnectCallback callback) => disconnectCallback = callback;

        #endregion

        #endregion

        #region Sending

        private IO.Buffer PackagePacket(TPacket packet)
        {
            BitWriter w = new BitWriter();
            w.Write((int)0); // reserve size int space
            packet.WritePacket(w);

            var payload = w.GetData();
            System.Buffer.BlockCopy(BitConverter.GetBytes(payload.size - 4), 0, payload.data, 0, 4); // insert size int to the start

            return payload;
        }

        #region Sync

        public void Send(TPacket packet)
        {
            var payload = PackagePacket(packet);
            socket.Send(payload.data, 0, payload.size, SocketFlags.None, out SocketError error);
            if (CheckError(error))
            {
                Log.Error("SocketError received on Send: " + error);
                Disconnect();
                return;
            }
        }

        #endregion

        #region Async

        public void SendAsync(TPacket packet)
        {

        }

        #endregion

        #endregion

        #region Reading

        public abstract void HandlePacket(TPacket packet);

        private void ReceivedSize()
        {
            int size = BitConverter.ToInt32(buffer.data, 0);
            buffer.Reset(size);
        }

        private void ReceivedPayload()
        {
            byte[] data = buffer.data;
            try
            {

                BitReader r = new BitReader(data, data.Length);
                byte id = r.ReadUInt8();
                TPacket packet = packetFactory.CreatePacket(id);
                if (packet == null)
                {
                    Log.Error($"No {typeof(TPacket).Name} for id: {id}");
                    return;
                }
                packet.ReadPacket(r);
                HandlePacket(packet);
            }
            finally
            {
                buffer.Reset(4);
            }
        }

        #region Sync

        public void Read()
        {
            while (socket.Connected)
            {
                ReadSize();
                ReadPayload();
            }
        }

        public void ReadSize()
        {
            if (disconnected == 1) return;
            while (buffer.RemainingLength > 0)
            {
                try
                {
                    socket.Receive(buffer.data, buffer.size, buffer.RemainingLength, SocketFlags.None, out SocketError error);
                    if (CheckError(error))
                    {
                        if (Disconnect())
                            Log.Error("SocketError received on ReadSize: " + error);
                        return;
                    }
                }
                catch (ObjectDisposedException disposed) // socket was already disposed
                {
                    return;
                }
            }

            ReceivedSize();
        }

        public void ReadPayload()
        {
            if (disconnected == 1) return;
            while (buffer.RemainingLength > 0)
            {
                try
                {
                    socket.Receive(buffer.data, buffer.size, buffer.RemainingLength, SocketFlags.None, out SocketError error);
                    if (CheckError(error))
                    {
                        if (Disconnect())
                            Log.Error("SocketError received on ReadPayload: " + error);
                        return;
                    }
                }
                catch (ObjectDisposedException disposed) // socket was already disposed
                {
                    return;
                }
            }

            ReceivedPayload();
        }

        #endregion

        #region Async

        public void ReadAsync()
        {
            BeginReadSize();
        }

        private bool TryEndRead(IAsyncResult ar)
        {
            if (disconnected == 1) return false;
            int length = 0;
            try
            {
                length = socket.EndReceive(ar, out SocketError error);
                if (CheckError(error))
                {
                    if (Disconnect())
                        Log.Error("SocketError received on TryEndRead: " + error);
                    return false;
                }
            }
            catch (ObjectDisposedException disposed) // socket was already disposed
            {
                return false;
            }

            if (length <= 0) // closed socket, disconnect
            {
                Disconnect();
                return false;
            }

            buffer.size += length; // data was read into the buffer, increment the size accordingly
            return true;
        }

        private void BeginReadSize()
        {
            if (disconnected == 1) return;
            try
            {
                socket.BeginReceive(buffer.data, buffer.size, buffer.RemainingLength, SocketFlags.None, out SocketError error, OnReadSizeCallback, null);
                if (CheckError(error))
                {
                    if (Disconnect())
                        Log.Error("SocketError received on BeginReadSize: " + error);
                    return;
                }
            }
            catch (ObjectDisposedException disposed) // socket was already disposed
            {
                return;
            }
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
                ReceivedSize();
                BeginReadPayload();
            }
        }

        private void BeginReadPayload()
        {
            if (disconnected == 1) return;
            try
            {
                socket.BeginReceive(buffer.data, buffer.size, buffer.RemainingLength, SocketFlags.None, out SocketError error, OnReadPayloadCallback, null);
                if (CheckError(error))
                {
                    if (Disconnect())
                        Log.Error("SocketError received on BeginReadPayload: " + error);
                    return;
                }
            }
            catch (ObjectDisposedException disposed) // socket was already disposed
            {
                return;
            }
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
                ReceivedPayload();
                BeginReadSize();
            }
        }

        #endregion

        #endregion

        #region Error Handling

        /// <summary>
        /// Checks if the given error qualifies for socket close
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        private bool CheckError(SocketError error)
        {
            switch (error)
            {
                case SocketError.Success:
                case SocketError.IOPending:
                case SocketError.WouldBlock:
                    return false;
                default:
                    return true;
            }
        }

        #endregion
    }
}
