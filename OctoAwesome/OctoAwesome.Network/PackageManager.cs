using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class PackageManager : IObserver<OctoNetworkEventArgs>
    {
        public event EventHandler<OctoPackageAvailableEventArgs> PackageAvailable;

        public List<Subscription<OctoNetworkEventArgs>> Subscriptions { get; set; }

        private readonly ConcurrentQueue<OctoNetworkEventArgs> _receivingQueue;
        private CancellationTokenSource _cancellationTokenSource;
        private Dictionary<BaseClient, Package> _packages;

        public PackageManager()
        {
            _packages = new Dictionary<BaseClient, Package>();
            Subscriptions = new List<Subscription<OctoNetworkEventArgs>>();
            _receivingQueue = new ConcurrentQueue<OctoNetworkEventArgs>();
            _cancellationTokenSource = new CancellationTokenSource();
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

            byte[] bytes;
            bytes = new byte[e.DataCount];

            if (!_packages.TryGetValue(baseClient, out Package package))
            {
                package = new Package();
                _packages.Add(baseClient, package);

                int current = 0;

                current += e.NetworkStream.Read(bytes, current, Package.HEAD_LENGTH - current);

                if (current != Package.HEAD_LENGTH)
                {
                    Console.WriteLine($"Package was not complete, only got: {current} bytes");
                    _packages.Remove(baseClient);
                    return;
                }
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
            var task =  new Task(InternalProcess, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            task.Start(TaskScheduler.Default);
            return task;
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private void InternalProcess()
        {
            while(!_cancellationTokenSource.IsCancellationRequested)
            {
                if (_receivingQueue.IsEmpty)
                    continue;

                if (_receivingQueue.TryDequeue(out OctoNetworkEventArgs eventArgs))
                    ClientDataAvailable(eventArgs);
            }
        }
    }
}
