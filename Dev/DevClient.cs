using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using DevBot.Dev.Data;
using DevBot.Dev.Enums;
using DevBot.Game;
using Newtonsoft.Json;

namespace DevBot.Dev
{
    public class DevClient
    {
        public const string Channel = "DevPro-English";
        private const string UID = "";

        private const int HostCount = 0;

        public ServerInfos Server { get; private set; }
        public DevConnection ChatClient { get; private set; }
        public int Token { get; set; }
        public IList<ServerInfo> Servers { get; private set; }

        public bool HostEnabled { get; set; }

        private Random m_random;

        private IList<GameClient> m_gameClients;
        private IList<GameClient> m_addedClients;
        private IList<GameClient> m_removedClients;
        private GameClient[] m_infClient;
        private DevBehavior m_behavior;

        public DevClient(ServerInfos infos)
        {
            Server = infos;
            m_random = new Random();
            m_infClient = new GameClient[HostCount];
            Servers = new List<ServerInfo>();
            HostEnabled = true;
        }

        private DateTime m_latestfix = DateTime.Now;

        public void Run()
        {
            ChatClient = new DevConnection(Server.Address, Server.ChatPort);
            ChatClient.Send(DevServerPacket.Login, GetJsonLoginString());

            m_gameClients = new List<GameClient>();
            m_addedClients = new List<GameClient>();
            m_removedClients = new List<GameClient>();
            m_behavior = new DevBehavior(this);

            while (ChatClient.IsConnected)
            {
                while (ChatClient.HasPacket())
                {
                    DevPacket packet = ChatClient.Receive();
                    HandlePacket(packet);
                }

                foreach (GameClient game in m_gameClients)
                {
#if !DEBUG
                    try
                    {
                        game.Tick();
                    }
                    catch (Exception ex)
                    {
                        game.Connection.Close();
                        File.WriteAllText("game_" + DateTime.UtcNow.ToString("dd-MM-yyyy-hh-mm-ss") + ".log", ex.ToString());
                    }
#else
                    game.Tick();
#endif
                }

                foreach (GameClient game in m_addedClients)
                    m_gameClients.Add(game);
                m_addedClients.Clear();

                foreach (GameClient game in m_removedClients)
                    m_gameClients.Remove(game);
                m_removedClients.Clear();

                if (HostEnabled)
                {
                    for (int i = 0; i < m_infClient.Length; ++i)
                    {
                        if (m_infClient[i] == null || !m_gameClients.Contains(m_infClient[i]))
                        {
                            Thread.Sleep(500);
                            ServerInfo server = GetServer();
                            if (server != null)
                            {
                                m_infClient[i] = new GameClient(this, server, GetRandomDuelName(false));
                                m_infClient[i].Start();
                                m_gameClients.Add(m_infClient[i]);
                            }
                            else
                                HostEnabled = false;
                            break;
                        }
                    }
                }

                if ((DateTime.Now - m_latestfix).TotalSeconds > 30)
                {
                    ChatClient.Send(DevServerPacket.RefuseDuel);
                    m_latestfix = DateTime.Now;
                }

                Thread.Sleep(1);
            }
        }

        public void AddGameClient(GameClient client)
        {
            m_addedClients.Add(client);
        }

        public void RemoveGameClient(GameClient client)
        {
            m_removedClients.Add(client);
        }

        public void SendChatMessage(string message)
        {
            ChatMessage msg = new ChatMessage(MessageType.Message, CommandType.None, Channel, message);
            ChatClient.Send(DevServerPacket.ChatMessage, JsonConvert.SerializeObject(msg));
        }

        public void SendChatPrivateMessage(string username, string message)
        {
            ChatMessage msg = new ChatMessage(MessageType.PrivateMessage, CommandType.None, username, message);
            ChatClient.Send(DevServerPacket.ChatMessage, JsonConvert.SerializeObject(msg));
        }

        public string GetRandomDuelName(bool locked)
        {
            const string allowed = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int size = locked ? 4 : 5;
            StringBuilder sb = new StringBuilder(size);
            sb.Append("200OOO8000,0,5,1,U");
            sb.Append(locked ? "L," : ",");
            for (int i = 0; i < size; ++i)
                sb.Append(allowed[m_random.Next(allowed.Length)]);
            return sb.ToString();
        }

        public ServerInfo GetServer()
        {
            if (Servers.Count == 0)
                return null;
            return Servers[m_random.Next(Servers.Count)];
        }

        public ServerInfo GetServer(string name)
        {
            foreach (ServerInfo server in Servers)
            {
                if (server.ServerName == name)
                    return server;
            }
            return null;
        }

        private void HandlePacket(DevPacket packet)
        {
            if (packet.Data == null) return;
            DevClientPacket id = (DevClientPacket)packet.Id;
            string data = Encoding.UTF8.GetString(packet.Data);
            m_behavior.OnPacket(id, data);
        }

        private string GetJsonLoginString()
        {
            LoginRequest login = new LoginRequest(Program.Config.Username, EncodePassword(Program.Config.Password), UID);
            return JsonConvert.SerializeObject(login);
        }

        private static string EncodePassword(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("&^%\x00a3$Ugdsgs:;");
            byte[] buffer = Encoding.UTF8.GetBytes(password);
            HMACMD5 md5 = new HMACMD5(bytes);
            return Convert.ToBase64String(md5.ComputeHash(buffer));
        }
    }
}