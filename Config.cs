using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using DevBot.Game.AI.Decks;

namespace DevBot
{
    public class GameDeck
    {
        public string DeckName;
        public bool Enabled;
    }

    public class Config
    {
        public string Username = "DevBot";
        public string Password = "Password";
        public string BotOwner = "DevPro";
        public string OwnerPass = "SomethingRandom";
	public bool CheckmateEnabled = false;
	public List<GameDeck> Decks = new List<GameDeck>();

        public void LoadDecks()
        {
            AddDeck(typeof(ChaosDragonExecutor).Name, true);
            AddDeck(typeof(DamageBurnExecutor).Name, false);
            AddDeck(typeof(DragunityExecutor).Name, false);
            AddDeck(typeof(FrogExecutor).Name, false);
            AddDeck(typeof(HorusExecutor).Name, false);
            AddDeck(typeof(OldSchoolExecutor).Name, false);
            AddDeck(typeof(SuperRoboExecutor).Name, false);
        }

        public void AddDeck(string name, bool enabled)
        {
            if (GetDeck(name) == null)
                Decks.Add(new GameDeck{ DeckName = name, Enabled = enabled});
        }

        public bool RemoveDeck(string name)
        {
            foreach (GameDeck deck in Decks)
            {
                if (deck.DeckName == name)
                    return Decks.Remove(deck);
            }
            return false;
        }

        public bool RemoveDeck(GameDeck deck)
        {
            return Decks.Remove(deck);
        }

        public GameDeck GetDeck(string name)
        {
            foreach (GameDeck deck in Decks)
            {
                if (deck.DeckName == name)
                    return deck;
            }
            return null;
        }

        public static void SaveConfig(Config config)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Config));
                TextWriter textWriter = new StreamWriter("config.conf");
                serializer.Serialize(textWriter, config);
                textWriter.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
        }

        public static Config LoadConfig()
        {
            if (!File.Exists("config.conf"))
            {
                SaveConfig(new Config());
            }
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Config));
                TextReader textReader = new StreamReader("config.conf");
                Config config = (Config)deserializer.Deserialize(textReader);
                textReader.Close();
                return config;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
            return new Config();
        }
    }
}
