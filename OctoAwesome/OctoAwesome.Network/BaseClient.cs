using System;
using System.Buffers;
using System.Net.Sockets;

namespace OctoAwesome.Network
{
    public abstract class BaseClient
    {
        public event EventHandler<OctoNetworkEventArgs> DataAvailable;

        protected readonly Socket Socket;
        protected readonly SocketAsyncEventArgs ReceiveArgs;

        protected readonly OctoNetworkStream _internalSendStream;
        protected readonly OctoNetworkStream _internalRecivedStream;

        private byte _readSendQueueIndex;
        private byte _nextSendQueueWriteIndex;
        private bool _sending;

        private readonly SocketAsyncEventArgs _sendArgs;

        private readonly (byte[] data, int len)[] _sendQueue;
        private readonly object _sendLock;

        protected BaseClient(Socket socket)
        {
            _sendQueue = new (byte[] data, int len)[256];
            _sendLock = new object();

            Socket = socket;
            Socket.NoDelay = true;

            ReceiveArgs = new SocketAsyncEventArgs();
            ReceiveArgs.Completed += OnReceived;
            ReceiveArgs.SetBuffer(ArrayPool<byte>.Shared.Rent(2048), 0, 2048);

            _sendArgs = new SocketAsyncEventArgs();
            _sendArgs.Completed += OnSent;

            _internalSendStream = new OctoNetworkStream();
            _internalRecivedStream = new OctoNetworkStream();

        }

        public void Start()
        {
            while (true)
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;
                Receive(ReceiveArgs);
            }
        }

        public void SendAsync(byte[] data, int len)
        {
            lock (_sendLock)
            {
                if (_sending)
                {
                    _sendQueue[_nextSendQueueWriteIndex++] = (data, len);
                    return;
                }
                _sending = true;
            }
            SendInternal(data, len);
        }

        private void SendInternal(byte[] data, int len)
        {
            while (true)
            {
                _sendArgs.SetBuffer(data, 0, len);

                if (Socket.SendAsync(_sendArgs))
                    return;

                lock (_sendLock)
                {
                    if (_readSendQueueIndex < _nextSendQueueWriteIndex)
                    {
                        (data, len) = _sendQueue[_readSendQueueIndex++];
                    }
                    else
                    {
                        _nextSendQueueWriteIndex = 0;
                        _readSendQueueIndex = 0;
                        _sending = false;
                        return;
                    }
                }
            }
        }

        private void OnSent(object sender, SocketAsyncEventArgs e)
        {
            byte[] data;
            int len;

            lock (_sendLock)
            {
                if (_readSendQueueIndex < _nextSendQueueWriteIndex)
                {
                    (data, len) = _sendQueue[_readSendQueueIndex++];
                }
                else
                {
                    _nextSendQueueWriteIndex = 0;
                    _readSendQueueIndex = 0;
                    _sending = false;
                    return;
                }
            }
            SendInternal(data, len);
        }

        protected void Receive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred < 1)
                return;

            int offset = 0;
            do
            {
                int count = _internalRecivedStream.Write(e.Buffer, offset, e.BytesTransferred - offset);

                if (count > 0)
                    DataAvailable?.Invoke(this, new OctoNetworkEventArgs { NetworkStream = _internalRecivedStream, DataCount = count });
                else
                    Console.WriteLine("Bytes: " + e.BytesTransferred);

                offset += count;
            } while (offset < e.BytesTransferred);
        }

        private void OnReceived(object sender, SocketAsyncEventArgs e)
        {
            Receive(e);

            while (Socket.Connected)
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;

                Receive(ReceiveArgs);
            }
        }
    }
}
