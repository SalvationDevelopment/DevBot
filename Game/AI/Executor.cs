﻿using System;
using System.Collections.Generic;
using System.Reflection;
using DevBot.Game.Enums;

namespace DevBot.Game.AI
{
    public abstract class Executor
    {
        public abstract string Deck { get; }
        public Duel Duel { get; private set; }
        public IList<CardExecutor> Executors { get; private set; }
        public GameAI AI { get; private set; }

        protected MainPhase Main { get; private set; }
        protected BattlePhase Battle { get; private set; }

        protected ExecutorType Type { get; private set; }
        protected ClientCard Card { get; private set; }
        protected int ActivateDescription { get; private set; }

        protected int LastChainPlayer { get; private set; }
        protected IList<ClientCard> CurrentChain { get; private set; } 

        protected Executor(GameAI ai, Duel duel)
        {
            Duel = duel;
            AI = ai;
            Executors = new List<CardExecutor>();
            CurrentChain = new List<ClientCard>();
        }

        public virtual BattlePhaseAction OnBattle(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            if (attackers.Count == 0)
                return AI.ToMainPhase2();

            if (defenders.Count == 0)
                return AI.Attack(attackers[0], null);

            for (int i = defenders.Count - 1; i >= 0; --i)
            {
                ClientCard defender = defenders[i];
                int value = defender.GetDefensePower();
                for (int j = 0; j < attackers.Count; ++j)
                {
                    ClientCard attacker = attackers[j];
                    if (!OnPreBattleBetween(attacker, defender))
                        continue;
                    if (attacker.Attack > value || (attacker.Attack >= value && j == attackers.Count - 1))
                        return AI.Attack(attacker, defender);
                }
            }

            if (!Battle.CanMainPhaseTwo)
                return AI.Attack(attackers[attackers.Count - 1], defenders[0]);

            return AI.ToMainPhase2();
        }

        public virtual bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (defender.IsMonsterInvincible())
            {
                if (defender.IsMonsterDangerous() || defender.IsDefense())
                    return false;
            }
            return true;
        }

        public virtual void OnChaining(int player, ClientCard card)
        {
            CurrentChain.Add(card);
            LastChainPlayer = player;
        }

        public virtual void OnChainEnd()
        {
            LastChainPlayer = -1;
            CurrentChain.Clear();
        }

        public virtual IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, bool cancelable)
        {
            return null;
        }

        public virtual bool OnSelectYesNo(int desc)
        {
            return true;
        }

        public bool ChainContainsCard(int id)
        {
            foreach (ClientCard card in CurrentChain)
            {
                if (card.Id == id)
                    return true;
            }
            return false;
        }

        public int ChainCountPlayer(int player)
        {
            int count = 0;
            foreach (ClientCard card in CurrentChain)
            {
                if (card.Controller == player)
                    count++;
            }
            return count;
        }

        public bool HasChainedTrap(int player)
        {
            foreach (ClientCard card in CurrentChain)
            {
                if (card.Controller == player && card.HasType(CardType.Trap))
                    return true;
            }
            return false;
        }

        public ClientCard GetLastChainCard()
        {
            if (CurrentChain.Count > 0)
                return CurrentChain[CurrentChain.Count - 1];
            return null;
        }

        public void SetMain(MainPhase main)
        {
            Main = main;
        }

        public void SetBattle(BattlePhase battle)
        {
            Battle = battle;
        }

        public void SetCard(ExecutorType type, ClientCard card, int description)
        {
            Type = type;
            Card = card;
            ActivateDescription = description;
        }

        public void AddExecutor(ExecutorType type, int cardId, Func<bool> func)
        {
            Executors.Add(new CardExecutor(type, cardId, func));
        }

        public void AddExecutor(ExecutorType type, int cardId)
        {
            Executors.Add(new CardExecutor(type, cardId, null));
        }

        public void AddExecutor(ExecutorType type, Func<bool> func)
        {
            Executors.Add(new CardExecutor(type, -1, func));
        }

        public void AddExecutor(ExecutorType type)
        {
            Executors.Add(new CardExecutor(type, -1, DefaultNoExecutor));
        }

        private bool DefaultNoExecutor()
        {
            foreach (CardExecutor exec in Executors)
            {
                if (exec.Type == Type && exec.CardId == Card.Id)
                    return false;
            }
            return true;
        }

        protected bool DefaultSpellSet()
        {
            return Card.IsTrap() && Duel.Fields[0].GetSpellCountWithoutField() < 4;
        }

        protected bool DefaultTributeSummon()
        {
            int tributecount = (int)Math.Ceiling((Card.Level - 4.0d) / 2.0d);
            for (int j = 0; j < 5; ++j)
            {
                ClientCard tributeCard = Duel.Fields[0].MonsterZone[j];
                if (tributeCard == null) continue;
                if (tributeCard.Attack < Card.Attack)
                    tributecount--;
            }
            return tributecount <= 0;
        }

        protected bool DefaultField()
        {
            return Duel.Fields[0].SpellZone[5] == null;
        }

        protected bool DefaultMonsterRepos()
        {
            bool ennemyBetter = AI.Utils.IsEnnemyBetter(true, true);

            if (Card.IsAttack() && ennemyBetter)
                return true;
            if (Card.IsDefense() && !ennemyBetter && Card.Attack >= Card.Defense)
                return true;
            return false;
        }

        protected bool DefaultTrap()
        {
            return !HasChainedTrap(0);
        }

        private static Type[] m_executors;
        private static Random m_rand;

        public static Executor GetExecutor(GameAI ai, Duel duel)
        {
            if (m_executors == null)
            {
                List<Type> decks = GetDecks();
                List<Type> enabledDecks = new List<Type>();
                foreach (Type deck in decks)
                {
                    GameDeck conDeck = Program.Config.GetDeck(deck.Name);
                    if (conDeck == null || conDeck.Enabled)
                        enabledDecks.Add(deck);
                }
                m_executors = enabledDecks.ToArray();
                m_rand = new Random();
            }
            return (Executor)Activator.CreateInstance(m_executors[m_rand.Next(0, m_executors.Length)], ai, duel);
        }

        public static List<Type> GetDecks()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            List<Type> classlist = new List<Type>();

            foreach (Type type in asm.GetTypes())
            {
                if (type.BaseType == typeof(Executor))
                    classlist.Add(type);
            }

            return classlist;
        }
    }
}