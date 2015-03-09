using System;

namespace DevBot.Dev.Data
{
    [Serializable]
    public class UserData
    {
        public string Username { get; set; }
        public int LoginKey { get; set; }
        public int Rank { get; set; }
        public int LoginID { get; set; }
        public int A { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public string Team { get; set; }
        public int TeamRank { get; set; }
        public bool Online { get; set; }
    }
}