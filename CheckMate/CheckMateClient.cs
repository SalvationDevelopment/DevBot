using System.Collections.Generic;
using System.Threading;
using DevBot.Dev.Data;
using DevBot.Game;

namespace DevBot.Checkmate
{
    public class CheckmateClient
    {
        private const string Address = "173.224.211.157";
        private const int Port = 21001;

        private readonly ServerInfo m_server;
        private const int MaxGames = 10;

        private IList<GameClient> m_gameClients;
        private IList<GameClient> m_addedClients;
        private IList<GameClient> m_removedClients;

        public CheckmateClient()
        {
            m_server = new ServerInfo("Checkmate", Address, Port);
        }

        public void Run()
        {
            m_gameClients = new List<GameClient>();
            m_addedClients = new List<GameClient>();
            m_removedClients = new List<GameClient>();

            StartDuel();

            while (true)
            {

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

        public void OnStart()
        {
            StartDuel();
        }

        public void OnWin()
        {
            StartDuel();
        }

        private void StartDuel()
        {
            if (m_gameClients.Count <= MaxGames)
            {
                GameClient game = new GameClient(this, m_server);
                game.Start();
                AddGameClient(game);
                Logger.WriteLine("Checkmate game created.");
            }
        }
    }
}
