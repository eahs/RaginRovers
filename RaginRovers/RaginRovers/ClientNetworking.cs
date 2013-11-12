using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace RaginRovers
{
    class ClientNetworking
    {
        public NetClient client;
        public NetPeerConfiguration config;
        public float reconnectTimer = 0f;
        public float reconnectTime = 15000f;

        public Dictionary<string, EventHandler> ActionHandler = new Dictionary<string, EventHandler>();

        public ClientNetworking()
        {
            config = new NetPeerConfiguration("raginrovers");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
        }

        public void Connect()
        {
            client = new NetClient(config);
            client.Start();

            client.DiscoverLocalPeers(14242);
        }

        public void Disconnect()
        {
            client.Shutdown("bye"); 
        }

        public bool Connected
        {
            get
            {
                if (client == null)
                    return false;
              
                return client.ConnectionStatus == NetConnectionStatus.Connected;
            }
        }

        public void SendMessage(string msg)
        {
            if (Connected)
            {
                NetOutgoingMessage om = client.CreateMessage();
                om.Write(msg);
                client.SendMessage(om, NetDeliveryMethod.ReliableUnordered);
            }
        }

        public void Update(GameTime gameTime)
        {
            HandleIncomingNetworkData();

            if (!Connected)
            {
                reconnectTimer += (float)gameTime.ElapsedGameTime.Milliseconds;

                if (reconnectTimer > reconnectTime)
                {
                    reconnectTimer = 0f;
                    Debug.WriteLine("Trying to connect again..");
                    Connect();
                }
            }


        }

        private void HandleIncomingNetworkData()
        {
            // read messages
            NetIncomingMessage msg;
            while ((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        // just connect to first server discovered
                        client.Connect(msg.SenderEndPoint);
                        break;

                    case NetIncomingMessageType.Data:
                        // server sent a position update

                        Dictionary<string, string> data = ServerNetworking.DeserializeData(msg.ReadString());

                        //action=shoot;cannonGroup=1;rotation=
                        if (data.ContainsKey("action"))
                        {
                            string key = data["action"];

                            if (ActionHandler.ContainsKey(key))
                            {
                                ActionHandler[key]((object)data, null);
                            }
                            
                        }


                        break;
                }
            }
        }
    }
}
