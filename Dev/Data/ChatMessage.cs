using System;
using DevBot.Dev.Enums;

namespace DevBot.Dev.Data
{
    [Serializable]
    public class ChatMessage
    {
        public string Message { get; set; }
        public string Channel { get; set; }
        public UserData From { get; set; }
        public int Type { get; set; } 
        public int Command { get; set; }

        public ChatMessage(MessageType type, CommandType command, string channel, string message)
        {
            Message = message;
            Channel = channel;
            Type = (int) type;
            Command = (int) command;
        }
    }
}