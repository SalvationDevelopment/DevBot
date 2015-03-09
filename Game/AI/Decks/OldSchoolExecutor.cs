namespace DevBot.Game.AI.Decks
{
    public class OldSchoolExecutor : Executor
    {
        public override string Deck
        {
            get { return "OldSchool"; }
        }

        public OldSchoolExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, 19613556, HeavyStorm);
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Activate, 53129443, DarkHole);
            AddExecutor(ExecutorType.Activate, 26412047, HammerShot);
            AddExecutor(ExecutorType.Activate, 66788016);
            AddExecutor(ExecutorType.Activate, 72302403, SwordsOfRevealingLight);
            AddExecutor(ExecutorType.Activate, 43422537, DoubleSummon);

            AddExecutor(ExecutorType.Summon, 83104731, DefaultTributeSummon);
            AddExecutor(ExecutorType.Summon, 6631034, DefaultTributeSummon);
            AddExecutor(ExecutorType.SummonOrSet, 43096270);
            AddExecutor(ExecutorType.SummonOrSet, 69247929);
            AddExecutor(ExecutorType.MonsterSet, 30190809);
            AddExecutor(ExecutorType.SummonOrSet, 77542832);
            AddExecutor(ExecutorType.SummonOrSet, 11091375);
            AddExecutor(ExecutorType.SummonOrSet, 35052053);
            AddExecutor(ExecutorType.SummonOrSet, 49881766);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);

            AddExecutor(ExecutorType.Activate, 44095762, DefaultTrap);
            AddExecutor(ExecutorType.Activate, 70342110, DefaultTrap);
        }

        private int m_lastDoubleSummon;

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (defender.IsMonsterInvincible() && !defender.IsMonsterDangerous() && attacker.Id == 83104731)
                return true;
            return base.OnPreBattleBetween(attacker, defender);
        }

        private bool HeavyStorm()
        {
            return Duel.Fields[0].GetSpellCount() < Duel.Fields[1].GetSpellCount();
        }

        private bool HammerShot()
        {
            return AI.Utils.IsEnnemyBetter(true, false);
        }

        private bool DarkHole()
        {
            return AI.Utils.IsEnnemyBetter(false, false);
        }

        private bool DoubleSummon()
        {
            if (m_lastDoubleSummon == Duel.Turn)
                return false;

            if (Main.SummonableCards.Count == 0)
                return false;

            if (Main.SummonableCards.Count == 1 && Main.SummonableCards[0].Level < 5)
            {
                bool canTribute = false;
                foreach (ClientCard handCard in Duel.Fields[0].Hand)
                {
                    if (handCard.IsMonster() && handCard.Level > 4 && handCard.Level < 6)
                        canTribute = true;
                }
                if (!canTribute)
                    return false;
            }

            int monsters = 0;
            foreach (ClientCard handCard in Duel.Fields[0].Hand)
            {
                if (handCard.IsMonster())
                    monsters++;
            }
            if (monsters <= 1)
                return false;

            m_lastDoubleSummon = Duel.Turn;
            return true;
        }

        private bool SwordsOfRevealingLight()
        {
            foreach (ClientCard handCard in Duel.Fields[1].GetMonsters())
            {
                if (handCard.IsFacedown())
                    return true;
            }
            return AI.Utils.IsEnnemyBetter(true, false);
        }
    }
}