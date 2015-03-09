using System;

namespace DevBot.Dev.Data
{
    [Serializable]
    public class LoginData
    {
        public int LoginKey { get; set; }
        public int UserRank { get; set; } 
    }
}