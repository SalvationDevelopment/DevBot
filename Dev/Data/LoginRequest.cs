using System;

namespace DevBot.Dev.Data
{
    [Serializable]
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int Version { get; set; }
        public string UID { get; set; }

        public LoginRequest(string username, string password, string uid)
        {
            Username = username;
            Password = password;
            Version = int.MaxValue;
            UID = uid;
        }
    }
}