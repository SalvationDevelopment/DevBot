using System;
using System.Collections.Generic;
using DevBot.Dev.Data;
using DevBot.Dev.Enums;
using DevBot.Game;
using Newtonsoft.Json;

namespace DevBot.Dev
{
    public class DevBehavior
    {
        public DevClient Client { get; private set; }
        public DevConnection Chat { get; private set; }

        private IDictionary<DevClientPacket, Action<string>> m_packets;

        public DevBehavior(DevClient client)
        {
            Client = client;
            Chat = client.ChatClient;

            m_packets = new Dictionary<DevClientPacket, Action<string>>();
            RegisterPackets();
        }

        public void OnPacket(DevClientPacket id, string data)
        {
            if (m_packets.ContainsKey(id))
                m_packets[id](data);
        }

        private void RegisterPackets()
        {
            m_packets.Add(DevClientPacket.LoginAccepted, OnLoginAccepted);
            m_packets.Add(DevClientPacket.GameServers, OnGameServers);
            m_packets.Add(DevClientPacket.AddServer, OnAddServer);
            m_packets.Add(DevClientPacket.RemoveServer, OnRemoveServer);
            m_packets.Add(DevClientPacket.Message, OnMessage);
            m_packets.Add(DevClientPacket.DuelRequest, OnDuelRequest);
            m_packets.Add(DevClientPacket.StartDuel, OnStartDuel);
        }

        private void OnLoginAccepted(string data)
        {
            LoginData login = JsonConvert.DeserializeObject<LoginData>(data);
            Client.Token = login.LoginKey;
            Logger.WriteLine("Connected with account " + Program.Config.Username + ". Login token is " + Client.Token + ".");

            Chat.Send(DevServerPacket.JoinChannel, DevClient.Channel);
        }
       
        private void OnGameServers(string data)
        {
            Client.Servers.Clear();
            ServerInfo[] servers = JsonConvert.DeserializeObject<ServerInfo[]>(data);
            foreach (ServerInfo server in servers)
                Client.Servers.Add(server);
        }

        private void OnAddServer(string data)
        {
            ServerInfo server = JsonConvert.DeserializeObject<ServerInfo>(data);
            Client.Servers.Add(server);
        }

        private void OnRemoveServer(string data)
        {
            foreach (ServerInfo server in Client.Servers)
            {
                if (server.ServerName == data)
                {
                    Client.Servers.Remove(server);
                    break;
                }
            }
        }

        private void OnMessage(string data)
        {
            ChatMessage message = JsonConvert.DeserializeObject<ChatMessage>(data);
            if (message.From == null) return;
            if (message.From.Username.Equals(Program.Config.Username)) return;

            if (message.Message.ToLower().Contains(Program.Config.Username.ToLower()) && message.Message.ToLower().Contains("duel") &&
                (Client.HostEnabled || message.From.Username.Equals(Program.Config.BotOwner)))
            {
                ServerInfo server = Client.GetServer();
                if (server != null)
                {
                    DuelRequest request = new DuelRequest(message.From.Username, server.ServerName, Client.GetRandomDuelName(true));
                    Chat.Send(DevServerPacket.RequestDuel, JsonConvert.SerializeObject(request));
                }
            }
            if (message.Type == (int)MessageType.PrivateMessage)
            {
                if (message.From.Username.Equals(Program.Config.BotOwner))
                {
                    if (message.Message.ToLower().Equals("autohost"))
                    {
                        Client.HostEnabled = !Client.HostEnabled;
                        Client.SendChatPrivateMessage(message.From.Username, "Auto host is now " + (Client.HostEnabled ? "enabled" : "disabled") + ".");
                    }
                    else if (message.Message.ToLower().StartsWith("msg "))
                        Client.SendChatMessage(message.Message.Substring(4));
                }
                else
                {
                    if (message.Message.ToLower().StartsWith("login "))
                    {
                        string passEntered = message.Message.Substring(6);
                        if (passEntered == Program.Config.OwnerPass)
                        {
                            Program.Config.BotOwner = message.From.Username;
                            Config.SaveConfig(Program.Config);
                            Client.SendChatPrivateMessage(message.From.Username, "You are now the bot owner.");
                        }
                    }
                }
            }
        }

        private void OnDuelRequest(string data)
        {
            DuelRequest duel = JsonConvert.DeserializeObject<DuelRequest>(data);
            if (duel.Username.Equals(Program.Config.BotOwner))
            {
                Chat.Send(DevServerPacket.AcceptDuel);
            }
            else if (!Client.HostEnabled)
            {
                Client.SendChatPrivateMessage(duel.Username, "Sorry but duels are currently disabled. Try again later.");
                Chat.Send(DevServerPacket.RefuseDuel);
            }
            else if (duel.DuelFormatString.StartsWith("200OOO8000,0,5,1,UL,"))
            {
                Chat.Send(DevServerPacket.AcceptDuel);
            }
            else
            {
                Client.SendChatPrivateMessage(duel.Username, "Sorry, I only accept allowed single TCG/OCG duels. Please change your request settings. (Options > Request Settings)");
                Chat.Send(DevServerPacket.RefuseDuel);
            }
        }

        private void OnStartDuel(string data)
        {
            DuelRequest duel = JsonConvert.DeserializeObject<DuelRequest>(data);
            ServerInfo server = Client.GetServer(duel.Server);
            if (server != null)
            {
                Logger.WriteLine("Duel requested. Room informations are " + duel.DuelFormatString + ".");
                GameClient game = new GameClient(Client, server, duel.DuelFormatString);
                game.Start();
                Client.AddGameClient(game);
            }
        }
    }
}
