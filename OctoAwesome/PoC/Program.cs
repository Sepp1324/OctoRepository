using System;
using System.Threading.Tasks;
using OctoAwesome.Notifications;
using OctoAwesome.PoC.Rx;

namespace OctoAwesome.PoC
{
    public static class Program
    {
        private static void Main()
        {
            using var networkRelay = new Relay<Notification>();
            using var updateHub = new UpdateHub();

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    networkRelay.OnNext(new BlockChangedNotification());
                }
            });

            using var sub = updateHub.AddSource(networkRelay, DefaultChannels.NETWORK);
            using var sub2 = updateHub.ListenOn(DefaultChannels.NETWORK).Subscribe(n => Console.WriteLine(n is not null));
            using var sub3 = updateHub.ListenOn(DefaultChannels.NETWORK).Subscribe(n => throw new ArgumentException());
            using var sub4 = updateHub.ListenOn(DefaultChannels.NETWORK).Subscribe(n => Console.WriteLine("Irgendwas"));

            Console.ReadLine();
        }
    }
}
