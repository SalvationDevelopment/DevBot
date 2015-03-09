using System;

namespace DevBot.Dev.Data
{
    [Serializable]
    public class DuelRequest
    {
        public string Username { get; set; }
        public string Server { get; set; }
        public string DuelFormatString { get; set; }

        public DuelRequest(string username, string server, string duelFormat)
        {
            Username = username;
            Server = server;
            DuelFormatString = duelFormat;
        }
    }
}