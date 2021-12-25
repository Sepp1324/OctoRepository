using System;
using System.Text;
using CommandManagementSystem.Attributes;
using OctoAwesome.EntityComponents;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;

namespace OctoAwesome.GameServer.Commands
{
    public static class PlayerCommands
    {
        private static readonly ConcurrentRelay<Notification> simulationChannel;
        private static readonly ConcurrentRelay<Notification> networkChannel;

        private static readonly IDisposable simulationChannelSub;
        private static readonly IDisposable networkChannelSub;

        static PlayerCommands()
        {
            var updateHub = TypeContainer.Get<IUpdateHub>();

            simulationChannelSub = updateHub.AddSource(simulationChannel, DefaultChannels.SIMULATION);
            networkChannelSub = updateHub.AddSource(networkChannel, DefaultChannels.NETWORK);
        }

        [Command((ushort)OfficialCommand.Whoami)]
        public static byte[] Whoami(CommandParameter parameter)
        {
            var playerName = Encoding.UTF8.GetString(parameter.Data);
            var player = new Player();
            var entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();
            var entityNotification = entityNotificationPool.Get();
            entityNotification.Entity = player;
            entityNotification.Type = EntityNotification.ActionType.Add;

            simulationChannel.OnNext(entityNotification);
            entityNotification.Release();

            var remotePlayer = new RemoteEntity(player);
            remotePlayer.Components.AddComponent(new PositionComponent { Position = new(0, new(0, 0, 78), new(0, 0)) });
            remotePlayer.Components.AddComponent(new RenderComponent { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            remotePlayer.Components.AddComponent(new BodyComponent { Mass = 50f, Height = 2f, Radius = 1.5f });

            Console.WriteLine(playerName);
            entityNotification = entityNotificationPool.Get();
            entityNotification.Entity = remotePlayer;
            entityNotification.Type = EntityNotification.ActionType.Add;

            networkChannel.OnNext(entityNotification);
            entityNotification.Release();
            return Serializer.Serialize(player);
        }
    }
}