﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace OctoAwesome.Runtime
{
    public sealed class Server
    {
        #region Singleton

        private static Server instance;

        public static Server Instance
        {
            get
            {
                if (instance == null)
                    instance = new Server();

                return instance;
            }
        }

        #endregion

        ServiceHost host;

        private List<Client> clients = new List<Client>();

        public Server()
        {

        }

        public void Open()
        {
            string server = "localhost";
            int port = 8888;
            string name = "Octo";

            string address = string.Format("net.tcp://{0}:{1}/{2}", server, port, name);

            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            host = new ServiceHost(typeof(Client), new Uri(address));
            host.AddServiceEndpoint(typeof(IClient), binding, address);
            host.Open();
        }

        public void Close()
        {
            if (host == null)
                return;

            //TODO: Call all DIsconnects
            lock (clients)
            {
                foreach (var client in clients)
                {
                    try
                    {
                        client.Callback.Disconnect();
                    }
                    catch (Exception) { }

                    clients.Remove(client);

                    if (OnDeregister != null)
                        OnDeregister(client);
                }
            }
            host.Close();
        }

        internal void Register(Client client)
        {
            lock (clients)
            {
                clients.Add(client);

                if (OnRegister != null)
                    OnRegister(client);
            }
        }

        internal void Deregister(Client client)
        {
            lock (clients)
            {
                try
                {
                    client.Callback.Disconnect();
                }
                catch (Exception) { }

                clients.Remove(client);

                if (OnDeregister != null)
                    OnDeregister(client);
            }
        }

        public IEnumerable<string> Clients
        {
            get
            {
                lock (clients)
                {
                    return clients.Select(c => c.Playername);
                }
            }
        }

        public event RegisterDelegate OnRegister;

        public event RegisterDelegate OnDeregister;

        public delegate void RegisterDelegate(Client info);
    }
}
