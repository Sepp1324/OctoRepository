using System.Collections.Generic;
using OctoAwesome.Notifications;

namespace OctoAwesome.Network.ServerNotifications
{
    public class ServerDataNotification : Notification
    {
        public ServerDataNotification()
        {
            PlayerIds = new HashSet<int>();
        }

        public byte[] Data { get; set; }
        public OfficialCommand OfficialCommand { get; set; }

        public HashSet<int> PlayerIds { get; set; }

        public override bool Match<T>(T filter)
        {
            if (PlayerIds.Count < 1)
                return true;

            if (filter is int playerId)
                return PlayerIds.Contains(playerId);

            return false;
        }
    }
}