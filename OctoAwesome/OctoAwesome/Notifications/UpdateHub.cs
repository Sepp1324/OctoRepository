using System;
using System.Collections.Generic;
using OctoAwesome.Rx;
using OctoAwesome.Threading;

namespace OctoAwesome.Notifications
{
    public class UpdateHub : IDisposable, IUpdateHub
    {
        private readonly Dictionary<string, ConcurrentRelay<Notification>> _channels;
        private readonly LockSemaphore _lockSemaphore;

        public UpdateHub()
        {
            _lockSemaphore = new(1, 1);
            _channels = new();
        }

        /// <summary>
        /// Listens on a given Channel
        /// </summary>
        /// <param name="channel">Channel to listen on</param>
        /// <returns>Observer of a given Channel</returns>
        public IObservable<Notification> ListenOn(string channel) => GetChannelRelay(channel);

        public IDisposable AddSource(IObservable<Notification> notification, string channel) => notification.Subscribe(GetChannelRelay(channel));

        private ConcurrentRelay<Notification> GetChannelRelay(string channel)
        {
            using var scope = _lockSemaphore.Wait();
            if (!_channels.TryGetValue(channel, out var channelRelay))
            {
                channelRelay = new();
                _channels.Add(channel, channelRelay);
            }

            return channelRelay;
        }

        public void Dispose()
        {
            foreach (var channel in _channels)
                channel.Value.Dispose();

            _channels.Clear();
            _lockSemaphore.Dispose();
        }
    }
}