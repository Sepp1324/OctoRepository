﻿using CommandManagementSystem;
using NLog;
using OctoAwesome.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.GameServer
{
    public class ServerHandler : IObserver<Package>
    {
        public SimulationManager SimulationManager { get; set; }

        private readonly Logger logger;
        private readonly Server server;
        private readonly DefaultCommandManager<ushort, byte[], byte[]> defaultManager;
                
        public ServerHandler()
        {
            logger = LogManager.GetCurrentClassLogger();

            server = new Server();
            SimulationManager = new SimulationManager(new Settings());
            defaultManager = new DefaultCommandManager<ushort, byte[], byte[]>(typeof(ServerHandler).Namespace + ".Commands");
        }

        public void Start()
        {
            server.Start(IPAddress.Any, 8888);
            server.OnClientConnected += ServerOnClientConnected;
        }
        
        private void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            logger.Debug("Hurra ein neuer Spieler");
            e.Subscribe(this);
        }

        public void OnNext(Package value)
        {
            Task.Run(() =>
            {
                if (value.Command == 0 && value.Payload.Length == 0)
                {
                    logger.Debug("Received null package");
                    return;
                }
                logger.Trace("Received a new Package with ID: " + value.UId);
                try
                {
                    value.Payload = defaultManager.Dispatch(value.Command, value.Payload) ?? new byte[0];
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Dispatch failed in Command " + value.Command);
                    return;
                }

                logger.Trace(value.Command);
                value.BaseClient.SendPackage(value);
            });
        }

        public void OnError(Exception error) 
            => throw error;

        public void OnCompleted()
        {
        }
    }
}