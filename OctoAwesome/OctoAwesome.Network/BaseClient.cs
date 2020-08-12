using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public abstract class BaseClient : IObservable<Package>
    {
        protected readonly Socket Socket;
        protected readonly SocketAsyncEventArgs ReceiveArgs;

        private byte readSendQueueIndex;
        private byte nextSendQueueWriteIndex;
        private bool sending;

        private readonly ConcurrentBag<IObserver<Package>> observers;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly SocketAsyncEventArgs sendArgs;

        private readonly (byte[] data, int len)[] sendQueue;
        private readonly object sendLock;

        protected BaseClient(Socket socket)
        {
            sendQueue = new (byte[] data, int len)[256];
            sendLock = new object();

            Socket = socket;
            Socket.NoDelay = true;

            ReceiveArgs = new SocketAsyncEventArgs();
            ReceiveArgs.Completed += OnReceived;
            ReceiveArgs.SetBuffer(ArrayPool<byte>.Shared.Rent(1024 * 1024), 0, 1024 * 1024);

            sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += OnSent;

            observers = new ConcurrentBag<IObserver<Package>>();
            cancellationTokenSource = new CancellationTokenSource();
        }

        public Task Start()
        {
            return Task.Run(() =>
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;

                Receive(ReceiveArgs);
            }, cancellationTokenSource.Token);
        }

        public void Stop()
        {
            foreach (var observer in observers)
                observer.OnCompleted();

            cancellationTokenSource.Cancel();
        }

        public void SendAsync(byte[] data, int len)
        {
            lock (sendLock)
            {
                if (sending)
                {
                    sendQueue[nextSendQueueWriteIndex++] = (data, len);
                    return;
                }
                sending = true;
            }
            SendInternal(data, len);
        }

        public void SendPackage(Package package)
        {
            byte[] bytes = new byte[package.Payload.Length + Package.HEAD_LENGTH];
            package.SerializePackage(bytes, 0);
            SendAsync(bytes, bytes.Length);
        }


        public IDisposable Subscribe(IObserver<Package> observer)
        {
            observers.Add(observer);
            return new Subscription<Package>(this, observer);
        }

        private void SendInternal(byte[] data, int len)
        {
            while (true)
            {
                sendArgs.SetBuffer(data, 0, len);

                if (Socket.SendAsync(sendArgs))
                    return;

                lock (sendLock)
                {
                    if (readSendQueueIndex < nextSendQueueWriteIndex)
                    {
                        (data, len) = sendQueue[readSendQueueIndex++];
                    }
                    else
                    {
                        nextSendQueueWriteIndex = 0;
                        readSendQueueIndex = 0;
                        sending = false;
                        return;
                    }
                }
            }
        }

        private void OnSent(object sender, SocketAsyncEventArgs e)
        {
            byte[] data;
            int len;

            lock (sendLock)
            {
                if (readSendQueueIndex < nextSendQueueWriteIndex)
                {
                    (data, len) = sendQueue[readSendQueueIndex++];
                }
                else
                {
                    nextSendQueueWriteIndex = 0;
                    readSendQueueIndex = 0;
                    sending = false;
                    return;
                }
            }
            SendInternal(data, len);
        }

        protected void Receive(SocketAsyncEventArgs e)
        {
            do
            {
                if (e.BytesTransferred < 1)
                    return;

                int offset = 0;

                do
                {
                    offset += DataReceived(e.Buffer, e.BytesTransferred, offset);
                } while (offset < e.BytesTransferred);
            } while (!Socket.ReceiveAsync(e));
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
