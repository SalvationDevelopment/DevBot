using System;

namespace DevBot.Dev.Data
{
    [Serializable]
    public class ServerInfo
    {
        public string ServerName { get; set; }
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }

        public ServerInfo(string serverName, string serverAddress, int serverPort)
        {
            ServerName = serverName;
            ServerAddress = serverAddress;
            ServerPort = serverPort;
        }
    }
}