using System.Collections.Generic;
namespace DevBot.Game.AI
{
    public class Dialogs
    {
        private GameClient m_game;
        private string[] m_duelstart;
        private string[] m_newturn;
        private string[] m_endturn;
        private string[] m_directattack;
        private string[] m_attack;
        private string[] m_activate;
        private string[] m_summon;
        private string[] m_setmonster;
        private string[] m_chaining;
        
        public Dialogs(GameClient game)
        {
            m_game = game;
            m_duelstart = new string[]
                {
                    "Good luck, have fun."
                };
            m_newturn = new string[]
                {
                    "It's my turn, draw.",
                    "My turn, draw.",
                    "I draw a card."
                };
            m_endturn = new string[]
                {
                    "I end my turn.",
                    "My turn is over.",
                    "Your turn."
                };
            m_directattack = new string[]
                {
                    "{0}, direct attack!",
                    "{0}, attack him directly!",
                    "{0}, he's defenseless, attack!",
                    "My {0} is going to smash your life points!"
                };
            m_attack = new string[]
                {
                    "{0}, attack this {1}!",
                    "{0}, destroy this {1}!",
                    "{0}, unleash your power on this {1}!"
                };
            m_activate = new string[]
                {
                    "I'm activating {0}.",
                    "I'm using the effect of {0}.",
                    "I use the power of {0}."
                };
            m_summon = new string[]
                {
                    "I'm summoning {0}.",
                    "Come on, {0}!",
                    "I summon the powerful {0}."
                };
            m_setmonster = new string[]
                {
                    "I'm setting a monster."
                };
            m_chaining = new string[]
                {
                    "Look at that! I'm activating {0}.",
                    "I use the power of {0}.",
                    "Get ready! I use {0}."
                };
        }

        public void SendDuelStart()
        {
            InternalSendMessage(m_duelstart);
        }

        public void SendNewTurn()
        {
            InternalSendMessage(m_newturn);
        }

        public void SendEndTurn()
        {
            InternalSendMessage(m_endturn);
        }

        public void SendDirectAttack(string attacker)
        {
            InternalSendMessage(m_directattack, attacker);
        }

        public void SendAttack(string attacker, string defender)
        {
            InternalSendMessage(m_attack, attacker, defender);
        }

        public void SendActivate(string spell)
        {
            InternalSendMessage(m_activate, spell);
        }

        public void SendSummon(string monster)
        {
            InternalSendMessage(m_summon, monster);
        }

        public void SendSetMonster()
        {
            InternalSendMessage(m_setmonster);
        }

        public void SendChaining(string card)
        {
            InternalSendMessage(m_chaining, card);
        }

        private void InternalSendMessage(IList<string> array, params object[] opts)
        {
            string message = string.Format(array[Program.Rand.Next(array.Count)], opts);
	    if(m_game.IsCheckmate)
		message = "-" + message;
            m_game.Chat(message);
        }
    }
}