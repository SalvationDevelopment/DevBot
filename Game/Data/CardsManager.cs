using System;
using System.Collections.Generic;
#if __MonoCS__
using Mono.Data.Sqlite;
using SQLiteConnection = Mono.Data.Sqlite.SqliteConnection;
using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
using SQLiteDataReader = Mono.Data.Sqlite.SqliteDataReader;
#else
using System.Data.SQLite;
#endif
using System.IO;

namespace DevBot.Game.Data
{
    public class CardsManager
    {
        private static IDictionary<int, CardData> m_cards;

        public static bool Init()
        {
            try
            {
                m_cards = new Dictionary<int, CardData>();

                string currentPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                currentPath = Path.GetDirectoryName(currentPath) ?? "";
                string absolutePath = Path.Combine(currentPath, "Content/cards.cdb");

                if (!File.Exists(absolutePath))
                    return false;

                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + absolutePath))
                {
                    connection.Open();

                    const string select =
                        "SELECT datas.id, alias, type, level, race, attribute, atk, def, name, desc " +
                        "FROM datas INNER JOIN texts ON datas.id = texts.id";

                    SQLiteCommand command = new SQLiteCommand(select, connection);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                        InitCards(reader);
                    command.Dispose();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void InitCards(SQLiteDataReader reader)
        {
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                CardData card = new CardData(id)
                {
                    AliasId = reader.GetInt32(1),
                    Type = reader.GetInt32(2),
                    Level = reader.GetInt32(3),
                    Race = reader.GetInt32(4),
                    Attribute = reader.GetInt32(5),
                    Atk = reader.GetInt32(6),
                    Def = reader.GetInt32(7),
                    Name = reader.GetString(8),
                    Description = reader.GetString(9)
                };
                m_cards.Add(id, card);

            }
        }

        public static int GetCount()
        {
            return m_cards.Count;
        }

        public static CardData GetCard(int id)
        {
            if (m_cards.ContainsKey(id))
                return m_cards[id];
            return null;
        }
    }
}
