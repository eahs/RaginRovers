using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using RaginRoversLibrary;

namespace RaginRovers
{
    class Networking
    {
        static NetPeerConfiguration config;
        NetServer server; 

        NetConnection recipient;
        NetPeer netpeer; 

        public Networking()
        {
            config = new NetPeerConfiguration("test_console");
            config.Port = 14242;
            server = new NetServer(config);
            //recipient = new NetConnection();
            netpeer = new NetPeer(config);

            recipient = new NetConnection();

            recipient.Peer.DiscoverLocalPeers(config.Port);
        }

        public void Receive()
        {
            NetIncomingMessage msg;
            List<string> Stuff = new List<string>();
            while ((msg = server.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    default:
                        //Transfer Data from message into string
                        Stuff = DeserializeData(msg.ReadString());
                        //CannonManager.ShootDoggy();
                        break;
                }
                server.Recycle(msg); //what this do
            }
        }
        public void Send(string serializedData)
        {
            NetOutgoingMessage sendMsg = server.CreateMessage();

            sendMsg.Write(serializedData);

            server.SendMessage(sendMsg, recipient, NetDeliveryMethod.ReliableUnordered);
        }
        public List<string> DeserializeData(string SerializedData)
        {
            List<string> DeserializeData = new List<string>();
            return DeserializeData;
        }
    }
}
