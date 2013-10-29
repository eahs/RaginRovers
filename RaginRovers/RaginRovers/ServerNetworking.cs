                                using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using RaginRoversLibrary;
using System.Diagnostics;

namespace RaginRovers
{
    class ServerNetworking
    {
        static NetPeerConfiguration config;
        NetServer server; 

        NetConnection recipient;
        NetPeer netpeer;

        // schedule initial sending of position updates
        double nextSendUpdates;

        public ServerNetworking()
        {
            config = new NetPeerConfiguration("raginrovers");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = 14242;

            server = new NetServer(config);
            server.Start();

            nextSendUpdates = NetTime.Now;

            //recipient = new NetConnection();
            //netpeer = new NetPeer(config);

            //recipient = new NetConnection();

            //recipient.Peer.DiscoverLocalPeers(config.Port);
        }

        public void Receive()
        {
            NetIncomingMessage msg;
            Dictionary<string, string> Stuff;

            while ((msg = server.ReadMessage()) != null)
            {
                Stuff = null;

                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryRequest:
                        //
                        // Server received a discovery request from a client; send a discovery response (with no extra data attached)
                        //
                        server.SendDiscoveryResponse(null, msg.SenderEndPoint);
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        //
                        // Just print diagnostic messages to console
                        //
                        Debug.WriteLine(msg.ReadString());
                        break;
                    case NetIncomingMessageType.StatusChanged:

                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                        if (status == NetConnectionStatus.Connected)
                        {
                            //
                            // A new player just connected!
                            //
                            Debug.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");

                            // randomize his position and store in connection tag
                            /*
                            msg.SenderConnection.Tag = new int[] {
									NetRandom.Instance.Next(10, 100),
									NetRandom.Instance.Next(10, 100)
								};
                            */
                        }

                        break;
                    case NetIncomingMessageType.Data:
                        //
                        // The client sent input to the server
                        //
                        /*
                        int xinput = msg.ReadInt32();
                        int yinput = msg.ReadInt32();

                        int[] pos = msg.SenderConnection.Tag as int[];

                        // fancy movement logic goes here; we just append input to position
                        pos[0] += xinput;
                        pos[1] += yinput;
                        */
                        Stuff = DeserializeData(msg.ReadString());

                        break;
                    default:
                        //Transfer Data from message into string
                        Stuff = DeserializeData(msg.ReadString());
                        //CannonManager.ShootDoggy();
                        break;
                }


                //
                // send position updates 30 times per second
                //
                double now = NetTime.Now;
                if (now > nextSendUpdates)
                {
                    // Yes, it's time to send position updates

                    // for each player...
                    foreach (NetConnection player in server.Connections)
                    {
                        // ... send information about every other player (actually including self)
                        foreach (NetConnection otherPlayer in server.Connections)
                        {
                            /*
                            // send position update about 'otherPlayer' to 'player'
                            NetOutgoingMessage om = server.CreateMessage();

                            // write who this position is for
                            om.Write(otherPlayer.RemoteUniqueIdentifier);

                            if (otherPlayer.Tag == null)
                                otherPlayer.Tag = new int[2];

                            int[] pos = otherPlayer.Tag as int[];
                            om.Write(pos[0]);
                            om.Write(pos[1]);

                            // send message
                            server.SendMessage(om, player, NetDeliveryMethod.Unreliable);
                            */
                            
                        }

                        if (Stuff != null)
                            //Send(player, "msg=shoot;client=1;vel=303.2;rotation=40");
                            Send(player, SerializeData(Stuff));
                    }

                    // schedule next update
                    nextSendUpdates += (1.0 / 30.0);
                }


                server.Recycle(msg); //what this do
            }
        }
        public void Send(NetConnection recipient, string serializedData)
        {
            NetOutgoingMessage sendMsg = server.CreateMessage();

            sendMsg.Write(serializedData);

            server.SendMessage(sendMsg, recipient, NetDeliveryMethod.Unreliable);
        }

        public static string SerializeData(Dictionary<string, string> data)
        {
            string pairs = "";

            foreach (string key in data.Keys)
            {
                pairs += ";" + key + "=" + data[key];
            }

            return pairs.TrimStart(';');
        }

        public static Dictionary<string, string> DeserializeData(string SerializedData)
        {
            Dictionary<string, string> DeserializeData = new Dictionary<string, string>();

            string[] pairs = SerializedData.Split(';');

            for (int i = 0; i < pairs.Length; i++)
            {
                string[] kv = pairs[i].Split('=');
                DeserializeData.Add(kv[0], kv[1]);
            }

            return DeserializeData;
        }
    }
}
