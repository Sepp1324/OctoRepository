﻿using System;
using System.Buffers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using OctoAwesome.Network.Pooling;
using OctoAwesome.Rx;

namespace OctoAwesome.Network
{
    public abstract class BaseClient : IDisposable
    {
        private static uint _nextId;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly PackagePool _packagePool;
        protected readonly SocketAsyncEventArgs ReceiveArgs;
        private readonly SocketAsyncEventArgs _sendArgs;
        private readonly object _sendLock;

        private readonly (byte[] data, int len)[] _sendQueue;
        private Package _currentPackage;
        private byte _nextSendQueueWriteIndex;

        private byte _readSendQueueIndex;
        private bool _sending;

        protected Socket Socket;

        private readonly ConcurrentRelay<Package> _packages;

        static BaseClient() => _nextId = 0;

        protected BaseClient()
        {
            _packages = new();
            _sendQueue = new (byte[] data, int len)[256];
            _sendLock = new();
            ReceiveArgs = new();
            ReceiveArgs.Completed += OnReceived;
            ReceiveArgs.SetBuffer(ArrayPool<byte>.Shared.Rent(1024 * 1024), 0, 1024 * 1024);
            _packagePool = TypeContainer.Get<PackagePool>();

            _sendArgs = new();
            _sendArgs.Completed += OnSent;

            _cancellationTokenSource = new();

            Id = NextId;
        }

        protected BaseClient(Socket socket) : this()
        {
            Socket = socket;
            Socket.NoDelay = true;
        }

        private static uint NextId => ++_nextId;

        public uint Id { get; }

        public IObservable<Package> Packages => _packages;

        public Task Start()
        {
            return Task.Run(() =>
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;

                Receive(ReceiveArgs);
            }, _cancellationTokenSource.Token);
        }

        public void Stop() => _cancellationTokenSource.Cancel();

        public Task SendAsync(byte[] data, int len)
        {
            lock (_sendLock)
            {
                if (_sending)
                {
                    _sendQueue[_nextSendQueueWriteIndex++] = (data, len);
                    return Task.CompletedTask;
                }

                _sending = true;
            }

            return Task.Run(() => SendInternal(data, len));
        }

        public async Task SendPackageAsync(Package package)
        {
            var bytes = new byte[package.Payload.Length + Package.HEAD_LENGTH];
            package.SerializePackage(bytes, 0);
            await SendAsync(bytes, bytes.Length);
        }

        public async Task SendPackageAndReleaseAsync(Package package)
        {
            await SendPackageAsync(package);
            package.Release();
        }

        public void SendPackageAndRelease(Package package)
        {
            var task = Task.Run(async () => await SendPackageAsync(package));
            task.Wait();
            package.Release();
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

        private void OnReceived(object sender, SocketAsyncEventArgs e) => Task.Run(() => Receive(e));

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

            if (_currentPackage == null)
            {
                _currentPackage = _packagePool.GetBlank();
                _currentPackage.BaseClient = this;

                if (length - bufferOffset < Package.HEAD_LENGTH)
                {
                    var ex = new Exception($"Buffer is to small for package head deserialization [length: {length} | offset: {bufferOffset}]");
                    ex.Data.Add(nameof(length), length);
                    ex.Data.Add(nameof(bufferOffset), bufferOffset);
                    throw ex;
                }

                if (_currentPackage.TryDeserializeHeader(buffer, bufferOffset))
                    offset += Package.HEAD_LENGTH;
                else
                    throw new InvalidCastException("Cannot deserialize header with these bytes :(");
            }

            offset += _currentPackage.DeserializePayload(buffer, bufferOffset + offset, length - (bufferOffset + offset));

            if (_currentPackage.IsComplete)
            {
                _packages.OnNext(_currentPackage);
                _currentPackage = null;
            }

            return offset;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            ReceiveArgs?.Dispose();
            _sendArgs?.Dispose();
            _packages?.Dispose();
        }
    }
}