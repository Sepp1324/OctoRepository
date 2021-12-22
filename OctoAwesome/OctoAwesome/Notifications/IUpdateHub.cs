using System;

namespace OctoAwesome.Notifications
{
    public interface IUpdateHub
    {

        /// <summary>
        /// Listens on a given Channel
        /// </summary>
        /// <param name="channel">Channel to listen on</param>
        /// <returns>Observer of a given Channel</returns>
        IObservable<Notification> ListenOn(string channel);

        IDisposable AddSource(IObservable<Notification> notification, string channel);
    }
}