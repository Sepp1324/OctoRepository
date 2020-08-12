using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class PackageManager : IObserver<OctoNetworkEventArgs>
    {
        public event EventHandler<OctoPackageAvailableEventArgs> PackageAvailable;

        public List<Subscription<OctoNetworkEventArgs>> Subscriptions { get; private set; }

        private readonly ConcurrentQueue<OctoNetworkEventArgs> _receivingQueue;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Dictionary<BaseClient, Package> _packages;
        private readonly MemoryStream _backupStream;

        public PackageManager()
        {
            _packages = new Dictionary<BaseClient, Package>();
            Subscriptions = new List<Subscription<OctoNetworkEventArgs>>();
            _receivingQueue = new ConcurrentQueue<OctoNetworkEventArgs>();
            _cancellationTokenSource = new CancellationTokenSource();
            _backupStream = new MemoryStream();
        }

        public void AddConnectedClient(BaseClient client)
        {
            client.Subscribe(this);
        }

        public void SendPackage(Package package, BaseClient client)
        {
            byte[] bytes = new byte[package.Payload.Length + Package.HEAD_LENGTH];
            package.SerializePackage(bytes);
            client.SendAsync(bytes, bytes.Length);
        }

        private void ClientDataAvailable(OctoNetworkEventArgs e)
        {
            var baseClient = e.Client;
            var data = e.NetworkStream.DataAvailable(Package.HEAD_LENGTH);

            byte[] bytes;
            bytes = new byte[e.DataCount];

            if (!_packages.TryGetValue(baseClient, out Package package))
            {
                int offset = 0;

                if (_backupStream.Length > 0)
                {
                    e.DataCount += (int)_backupStream.Length;
                    _backupStream.Read(bytes, 0, (int)_backupStream.Length);
                    offset = (int)_backupStream.Length;
                    _backupStream.Position = 0;
                    _backupStream.SetLength(0);
                }

                data += offset;

                if (data < Package.HEAD_LENGTH)
                {
                    e.NetworkStream.Read(bytes, offset, data);
                    _backupStream.Write(bytes, 0, data);
                    _backupStream.Position = 0;
                    return;
                }

                package = new Package();
                _packages.Add(baseClient, package);

                offset += e.NetworkStream.Read(bytes, offset, Package.HEAD_LENGTH - offset);

                package.TryDeserializeHeader(bytes);
                e.DataCount -= Package.HEAD_LENGTH;
            }

            if (e.DataCount > 0)
                e.NetworkStream.Read(bytes, 0, e.DataCount);

            var count = package.DeserializePayload(bytes, 0, e.DataCount);

            if (package.IsComplete)
            {
                _packages.Remove(baseClient);
                PackageAvailable?.Invoke(this, new OctoPackageAvailableEventArgs { BaseClient = baseClient, Package = package });

                if (e.DataCount - count > 0)
                    ClientDataAvailable(new OctoNetworkEventArgs() { Client = baseClient, DataCount = e.DataCount - count, NetworkStream = e.NetworkStream });
            }
        }

        public void OnNext(OctoNetworkEventArgs value) => _receivingQueue.Enqueue(value);

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public Task Start()
        {
            var task = new Task(InternalProcess, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            task.Start(TaskScheduler.Default);
            return task;
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private void InternalProcess()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                if (_receivingQueue.IsEmpty)
                    continue;

                if (_receivingQueue.TryDequeue(out OctoNetworkEventArgs eventArgs))
                    ClientDataAvailable(eventArgs);
            }
        }
    }
}
