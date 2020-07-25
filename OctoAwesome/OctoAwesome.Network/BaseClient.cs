using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace OctoAwesome.Network
{
    public abstract class BaseClient
    {
        //public delegate int ReceiveDelegate(object sender, (byte[] Data, int Offset, int Count) eventArgs);
        //public event ReceiveDelegate OnMessageRecived;
        public event EventHandler<Package> PackageReceived;

        protected readonly Socket Socket;
        protected readonly SocketAsyncEventArgs ReceiveArgs;

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
        }

        public void Start()
        {
            while (true)
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;
                int offset = 0;
                do
                {
                    offset += ProcessInternal(ReceiveArgs.Buffer, offset, ReceiveArgs.BytesTransferred - offset);
                } while (offset < ReceiveArgs.BytesTransferred);
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
        public void SendAsync(Package package)
        {
            throw new NotImplementedException(); //TODO: FIX

            //var buffer = new byte[2048];
            //int /*offset = 0,*/read = 0;
            //do
            //{
            //    read = package.Read(buffer, 0, buffer.Length);
            //    //offset += read;
            //    SendAsync(buffer, read);

            //} while (read >= buffer.Length);
        }

        public Package SendAndReceive(Package package)
        {
            Package returnPackage = new Package();

            SendAsync(package);
            _packageReceived.WaitOne();

            lock (_receivedPackages)
            {
                return _receivedPackages.Dequeue();
            }
        }

        private Package _returnPackage = new Package();
        private Queue<Package> _receivedPackages = new Queue<Package>();
        private AutoResetEvent _packageReceived = new AutoResetEvent(false);

        protected virtual int ProcessInternal(byte[] receiveArgsBuffer, int receiveOffset, int receiveArgsCount)
        {
            throw new NotImplementedException(); //TODO: FIX
            //int read = returnPackage.Write(receiveArgsBuffer, receiveOffset, receiveArgsCount);

            //if (read < receiveArgsBuffer.Length - receiveOffset)
            //{
            //    lock (receivedPackages)
            //    {
            //        receivedPackages.Enqueue(returnPackage);
            //    }
            //    PackageReceived?.Invoke(this, returnPackage);
            //    packageReceived.Set();
            //    returnPackage = new Package();
            //}
            //return read;
        }

        //protected int OnMessageReceivedInvoke(byte[] receiveArgsBuffer,int receiveOffset, int receiveArgsCount)
        //    => OnMessageRecived?.Invoke(this, (receiveArgsBuffer, receiveOffset, receiveArgsCount)) ?? 0;

        private void SendInternal(byte[] data, int len)
        {
            while (true)
            {
                _sendArgs.SetBuffer(data, 0, len);

                if (Socket.SendAsync(_sendArgs))
                    return;

                ArrayPool<byte>.Shared.Return(data);

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

            ArrayPool<byte>.Shared.Return(e.Buffer);

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

        private void OnReceived(object sender, SocketAsyncEventArgs e)
        {
            int offset = 0;
            do
            {
                offset += ProcessInternal(e.Buffer, offset, e.BytesTransferred - offset);
            } while (offset < e.BytesTransferred);
            while (Socket.Connected)
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;
                offset = 0;
                do
                {
                    offset += ProcessInternal(ReceiveArgs.Buffer, offset, ReceiveArgs.BytesTransferred - offset);
                } while (offset < ReceiveArgs.BytesTransferred);
            }
        }
    }
}