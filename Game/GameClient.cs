using System.Net;
using System.Text;
using DevBot.Checkmate;
using DevBot.Dev;
using DevBot.Dev.Data;
using DevBot.Game.Network;
using DevBot.Game.Network.Enums;

namespace DevBot.Game
{
    public class GameClient
    {
        public const short Version = 0x1330;

        public CheckmateClient CheckmateClient { get; private set; }
        public DevClient DevClient { get; private set; }
        public GameConnection Connection { get; private set; }

        private ServerInfo m_server;
        private string m_roomInfos;

        private GameBehavior m_behavior;
        public bool IsCheckmate { get; private set; }

        public GameClient(DevClient devClient, ServerInfo server, string roomInfos)
        {
            DevClient = devClient;
            m_server = server;
            m_roomInfos = roomInfos;
        }

        public GameClient(CheckmateClient client, ServerInfo server)
        {
            CheckmateClient = client;
            m_server = server;
            m_roomInfos = string.Empty;
            IsCheckmate = true;
        }

        public void Start()
        {
            Connection = new GameConnection(IPAddress.Parse(m_server.ServerAddress), m_server.ServerPort);
            m_behavior = new GameBehavior(this);

            GameClientPacket packet = new GameClientPacket(CtosMessage.PlayerInfo);
            packet.Write(Program.Config.Username + "$" + (IsCheckmate ? Program.Config.Password : DevClient.Token.ToString()), 20);
            Connection.Send(packet);

            byte[] junk = {0xCC, 0xCC, 0x00, 0x00, 0x00, 0x00};
            packet = new GameClientPacket(CtosMessage.JoinGame);
            packet.Write(Version);
            packet.Write(junk);
            packet.Write(m_roomInfos, 30);
            Connection.Send(packet);
        }

        public void Tick()
        {
            if (!Connection.IsConnected)
            {
		if (!IsCheckmate)
                    DevClient.RemoveGameClient(this);
                else
                    CheckmateClient.RemoveGameClient(this);
                return;
            }
            while (Connection.HasPacket())
            {
                GameServerPacket packet = Connection.Receive();
                m_behavior.OnPacket(packet);
            }
        }

        public void Chat(string message)
        {
            byte[] content = Encoding.Unicode.GetBytes(message + "\0");
            GameClientPacket chat = new GameClientPacket(CtosMessage.Chat);
            chat.Write(content);
            Connection.Send(chat);
        }
    }
}
