using OctoAwesome.Notifications;
using System;
using System.Collections.Generic;

namespace OctoAwesome.PoC.Rx
{
    public class UpdateHub : IDisposable
    {
        private readonly Dictionary<string, Relay<Notification>> _channels;

        public UpdateHub() => _channels = new();

        public IObservable<Notification> ListenOn(string channel) => GetChannelRelay(channel);

        public IDisposable AddSource(IObservable<Notification> notification, string channel) => notification.Subscribe(GetChannelRelay(channel));

        private Relay<Notification> GetChannelRelay(string channel)
        {
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
        }
    }
}