using OctoAwesome.Network.Pooling;
using OctoAwesome.Pooling;
using OctoAwesome.Threading;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public abstract class BaseClient : IAsyncObservable<Package>
    {
        private static uint nextId;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly ConcurrentBag<IAsyncObserver<Package>> observers;
        private readonly PackagePool packagePool;
        protected readonly SocketAsyncEventArgs ReceiveArgs;
        private readonly SocketAsyncEventArgs sendArgs;
        private readonly object sendLock;

        private readonly (byte[] data, int len)[] sendQueue;
        private Package currentPackage;
        private byte nextSendQueueWriteIndex;

        private byte readSendQueueIndex;
        private bool sending;

        protected Socket Socket;

        static BaseClient()
        {
            nextId = 0;
        }

        protected BaseClient()
        {
            sendQueue = new (byte[] data, int len)[256];
            sendLock = new object();
            ReceiveArgs = new SocketAsyncEventArgs();
            ReceiveArgs.Completed += OnReceived;
            ReceiveArgs.SetBuffer(ArrayPool<byte>.Shared.Rent(1024 * 1024), 0, 1024 * 1024);
            packagePool = TypeContainer.Get<PackagePool>();

            sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += OnSent;

            observers = new ConcurrentBag<IAsyncObserver<Package>>();
            cancellationTokenSource = new CancellationTokenSource();

            Id = NextId;
        }

        protected BaseClient(Socket socket) : this()
        {
            Socket = socket;
            Socket.NoDelay = true;
        }

        private static uint NextId => ++nextId;
        public uint Id { get; }

        public Task<IDisposable> Subscribe(IAsyncObserver<Package> observer)
        {
            observers.Add(observer);
            return Task.FromResult(new Subscription<Package>(this, observer) as IDisposable);
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
            {
                observer.OnCompleted();
            }

            cancellationTokenSource.Cancel();
        }

        public Task SendAsync(byte[] data, int len)
        {
            lock (sendLock)
            {
                if (sending)
                {
                    sendQueue[nextSendQueueWriteIndex++] = (data, len);
                    return Task.CompletedTask;
                }

                sending = true;
            }

            return Task.Run(() => SendInternal(data, len));
        }

        public async Task SendPackageAsync(Package package)
        {
            var bytes = new byte[package.Payload.Length + Package.HEAD_LENGTH];
            package.SerializePackage(bytes, 0);
            await SendAsync(bytes, bytes.Length);
        }

        public async Task SendPackageAndRelaseAsync(Package package)
        {
            await SendPackageAsync(package);
            package.Release();
        }

        public void SendPackageAndRelase(Package package)
        {
            var task = Task.Run(async () => await SendPackageAsync(package));
            task.Wait();
            package.Release();
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

        private void OnReceived(object sender, SocketAsyncEventArgs e)
        {
            Receive(e);
        }

        protected void Receive(SocketAsyncEventArgs e)
        {
            do
            {
                if (e.BytesTransferred < 1)
                    return;

                var offset = 0;

                do
                {
                    offset += DataReceived(e.Buffer, e.BytesTransferred, offset);
                } while (offset < e.BytesTransferred);
            } while (!Socket.ReceiveAsync(e));
        }

        private int DataReceived(byte[] buffer, int length, int bufferOffset)
        {
            var offset = 0;

            if (currentPackage == null)
            {
                currentPackage = packagePool.GetBlank();
                currentPackage.BaseClient = this;

                if (length - bufferOffset < Package.HEAD_LENGTH)
                {
                    var ex = new Exception(
                        $"Buffer is to small for package head deserialization [length: {length} | offset: {bufferOffset}]");
                    ex.Data.Add(nameof(length), length);
                    ex.Data.Add(nameof(bufferOffset), bufferOffset);
                    throw ex;
                }
                else
                {
                    if (currentPackage.TryDeserializeHeader(buffer, bufferOffset))
                    {
                        offset += Package.HEAD_LENGTH;
                    }
                    else
                    {
                        throw new InvalidCastException("Can not deserialize header with these bytes :(");
                    }
                }
            }

            offset += currentPackage.DeserializePayload(buffer, bufferOffset + offset,
                length - (bufferOffset + offset));

            if (currentPackage.IsComplete)
            {
                var package = currentPackage;
                Task.Run(() =>
                {
                    foreach (var observer in observers)
                        observer.OnNext(package);

                    package.Release();
                });

                currentPackage = null;
            }

            return offset;
        }
    }
}