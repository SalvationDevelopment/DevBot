using System;

namespace DevBot
{
    public static class Logger
    {
        public static void WriteLine(string message)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message);
        }
    }
}