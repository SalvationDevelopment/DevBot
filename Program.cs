using System;
using System.Diagnostics;
using System.IO;
using DevBot.Checkmate;
using DevBot.Dev;
using DevBot.Game.Data;

namespace DevBot
{
    public class Program
    {
        public const string Version = "0.2.2.0";
        public static Config Config;
        public static Random Rand;

        public static void Main()
        {
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += OnCrash;
#endif
            DrawUselessThings();

            Config = Config.LoadConfig();
            Config.LoadDecks();
            Config.SaveConfig(Config);
            Rand = new Random();

            if (!CardsManager.Init())
            {
                Logger.WriteLine("Could not load cards database. Exiting.");
                return;
            }
            
            Logger.WriteLine("Initialized cards database, "+ CardsManager.GetCount() + " cards loaded.");

            ServerInfos infos = new ServerInfos();
            if (!infos.Retrieve())
            {
                Logger.WriteLine("Could not retrieve server informations. Exiting.");
                return;
            }
            
            Logger.WriteLine("Received server informations, connecting to " + infos.Address + ":" + infos.ChatPort + ".");

	    if (!Config.CheckmateEnabled)
            {
                DevClient devclient = new DevClient(infos);
                devclient.Run();
            }
            else
            {
                CheckmateClient Cekclient = new CheckmateClient();
                Cekclient.Run();
            }

        }

        private static void DrawUselessThings()
        {
            Console.Title = "DevBot v" + Version;

            Console.WriteLine(@"      ____  ____  _  _  ____   __  ____ ");
            Console.WriteLine(@"     (    \(  __)/ )( \(  _ \ /  \(_  _)");
            Console.WriteLine(@"      ) D ( ) _) \ \/ / ) _ ((  O ) )(  ");
            Console.WriteLine(@"     (____/(____) \__/ (____/ \__/ (__)  v" + Version);
            Console.WriteLine();
        }

        private static void OnCrash(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception ?? new Exception();
            File.WriteAllText("crash_" + DateTime.UtcNow.ToString("dd-MM-yyyy-hh-mm-ss") + ".log", ex.ToString());
            Process.GetCurrentProcess().Kill();
        }
    }
}
