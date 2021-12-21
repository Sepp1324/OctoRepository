﻿using System;
using System.Text;
using CommandManagementSystem.Attributes;
using engenious;
using OctoAwesome.EntityComponents;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;

namespace OctoAwesome.GameServer.Commands
{
    public static class PlayerCommands
    {
        [Command((ushort)OfficialCommand.Whoami)]
        public static byte[] Whoami(CommandParameter parameter)
        {
            var updateHub = TypeContainer.Get<IUpdateHub>();
            var playerName = Encoding.UTF8.GetString(parameter.Data);
            var player = new Player();
            var entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();
            var entityNotification = entityNotificationPool.Get();
            entityNotification.Entity = player;
            entityNotification.Type = EntityNotification.ActionType.Add;

            updateHub.Push(entityNotification, DefaultChannels.SIMULATION);
            entityNotification.Release();


            var remotePlayer = new RemoteEntity(player);
            remotePlayer.Components.AddComponent(new PositionComponent
                { Position = new Coordinate(0, new Index3(0, 0, 78), new Vector3(0, 0)) });
            remotePlayer.Components.AddComponent(
                new RenderComponent { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 },
                true);
            remotePlayer.Components.AddComponent(new BodyComponent { Mass = 50f, Height = 2f, Radius = 1.5f });

            Console.WriteLine(playerName);
            entityNotification = entityNotificationPool.Get();
            entityNotification.Entity = remotePlayer;
            entityNotification.Type = EntityNotification.ActionType.Add;

            updateHub.Push(entityNotification, DefaultChannels.NETWORK);
            entityNotification.Release();
            return Serializer.Serialize(player);
        }
    }
}