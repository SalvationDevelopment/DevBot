using System.Collections.Generic;
using DevBot.Game.Enums;

namespace DevBot.Game.AI.Decks
{
    public class SuperRoboExecutor : Executor
    {
        public enum CardId
        {
            //MainDeck
            //Monsters
            SuperRoboElephan = 45496268,
            SuperRoboLeo = 4596268,
            SuperRoboMonkei = 71880877,
            GearspinrgSpirit = 45458027,
            MechinaCannon = 39284521,
            MechinaGearFrame = 42940404,
            CardTrooper = 85087012,
            //Spells
            TradeIn = 38120068,
            DarkHole = 53129443,
            IronCall = 64662453,
            MysticalSpaceTyphoon = 5318639,
            LimiterRemoval = 23171610,
            //Traps
            BottomlessTrapHole = 29401950,
            TorrentialTribute = 53582587,
            DimensionalPrison = 70342110,
            LimitReverse = 27551,
            SolemnWarning = 84749824,
            CallOfTheHuanted = 97077563,
            CompulsoryEvacuationDevice = 94192409,
            //ExtraDeck
            Number107 = 88177324,
            SunDragonOverlord = 64332231,
            Number40 = 75433814,
            Number15 = 88120966,
            GearGigantX = 28912357,
            Number50 = 51735257,
            SymphonyDjinn = 25341652,
            GagagaCowBoy = 12014404,
            Felgrand = 1639384,
            GarbageEyes = 77799846,
            Zenmaines = 78156759
        }

        public override string Deck
        {
            get { return "SUPAROBO-X"; }
        }

        public SuperRoboExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            
            AddExecutor(ExecutorType.Activate,(int)CardId.MysticalSpaceTyphoon,MysticalSpaceTyphoon);

            //Monsters
            AddExecutor(ExecutorType.Summon,(int)CardId.MechinaGearFrame, SummonGearFrame);
            AddExecutor(ExecutorType.Activate,(int)CardId.MechinaGearFrame,GearFrameEffect);
            
            AddExecutor(ExecutorType.SummonOrSet,(int)CardId.CardTrooper);
            AddExecutor(ExecutorType.Activate,(int)CardId.CardTrooper,CardTrooperEffect);

            AddExecutor(ExecutorType.Summon,(int)CardId.SuperRoboElephan, SummonSuperRoboElephan);
            AddExecutor(ExecutorType.Summon,(int)CardId.SuperRoboMonkei,SummonSuperRoboMonkei);
            AddExecutor(ExecutorType.Summon, (int)CardId.SuperRoboLeo, SummonSuperRoboLio);

            

            AddExecutor(ExecutorType.SpSummon, (int) CardId.GearspinrgSpirit, SummonGearspring);

            AddExecutor(ExecutorType.SpSummon,(int)CardId.MechinaCannon, SummonMechinaCannon);
            
            AddExecutor(ExecutorType.SpellSet, SetTrapsAndSpells);
        }

        //public override void OnChaining(int player, ClientCard card)
        //{
        //    base.OnChaining(player,card);

        //    //if(card.Id == (int))

        //}

        protected bool SetTrapsAndSpells()
        {
            return !Duel.Fields[0].HasAttackingMonster() && Duel.Fields[0].GetSpellCountWithoutField() < 4 ||
                Duel.Phase == Phase.Main2 && (Card.IsTrap()|| Card.Id == (int)CardId.MysticalSpaceTyphoon ||
                Card.Id == (int)CardId.LimitReverse) && Duel.Fields[0].GetSpellCountWithoutField() < 4;
        }

        protected bool MysticalSpaceTyphoon()
        {
            IList<ClientCard> cards = Duel.Fields[1].GetSpells();
            if (Duel.Phase == Phase.End && cards.Count > 0)
            {
                AI.SelectCard(cards[Program.Rand.Next(0,cards.Count)]);
                return true;
            }
            return false;
        }

        protected bool SummonGearFrame()
        {
            return true;
        }

        protected bool GearFrameEffect()
        {
            AI.SelectCard((int)CardId.MechinaCannon);
            return true;
        }

        protected bool SummonSuperRoboMonkei()
        {
            if (Duel.Fields[0].MonsterZone.ContainsMonsterWithLevel(Card.Level))
                return true;
            if (Duel.Fields[0].Hand.ContainsCardWithID((int)CardId.SuperRoboElephan) ||
                Duel.Fields[0].MonsterZone.ContainsCardWithID((int)CardId.SuperRoboElephan))
                return true;
            if (Duel.Fields[0].Hand.ContainsCardWithID((int)CardId.IronCall) &&
                Duel.Fields[0].Graveyard.ContainsMonsterWithLevel(Card.Level))
                return true;

            return false;
        }

        protected bool SummonSuperRoboLio()
        {
            if (Duel.Fields[0].GetMonsters().ContainsMonsterWithLevel(Card.Level))
                return true;

            if (Duel.Fields[0].Hand.ContainsCardWithID((int)CardId.SuperRoboElephan) ||
                Duel.Fields[0].MonsterZone.ContainsCardWithID((int)CardId.SuperRoboElephan) ||
                Duel.Fields[0].MonsterZone.ContainsCardWithID((int)CardId.SuperRoboLeo))
                return true;

            if (Duel.Fields[0].Hand.ContainsCardWithID((int)CardId.IronCall) &&
                Duel.Fields[0].Graveyard.ContainsMonsterWithLevel(Card.Level))
                return true;

            if (Duel.Fields[0].Hand.GetCardCount((int)CardId.SuperRoboLeo) > 1)
                return true;

            return false;
        }

        protected bool SummonMechinaCannon()
        {
            if (Duel.Fields[0].MonsterZone.ContainsMonsterWithLevel(Card.Level))
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }

            return false;
        }

        protected bool SummonGearspring()
        {
            if (Duel.Fields[0].MonsterZone.ContainsMonsterWithLevel(Card.Level))
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }

            IList<ClientCard> otherPlayerMonsters = Duel.Fields[1].MonsterZone;
            IList<ClientCard> myMonsters = Duel.Fields[0].MonsterZone;

            if (otherPlayerMonsters.Count > 0 && myMonsters.Count > 0)
            {
                if (otherPlayerMonsters.GetHighestAttackMonster().Attack >=
                    myMonsters.GetHighestAttackMonster().Attack)
                {
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                    return true;
                }
            }

            if (otherPlayerMonsters.Count > 0)
            {
                if (otherPlayerMonsters.GetHighestAttackMonster().Attack >= 2000)
                {
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                    return true;
                }
            }

            if (Duel.Fields[0].Hand.ContainsCardWithID((int)CardId.MechinaCannon) &&
                Duel.Fields[0].Hand.GetMonsters().Count > 2)
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }


            return false;
        }

        protected bool CardTrooperEffect()
        {
            AI.SelectNumber(3);
            return false;
        }

        protected bool SummonSuperRoboElephan()
        {

            //IList<ClientCard> myMonsters = Duel.Fields[0].GetMonsters();

            //if(AI.Utils.ContainsCardWithID(myMonsters,(int)CardId.SuperRoboLeo) ||
            //    AI.Utils.ContainsCardWithID(myMonsters, (int)CardId.SuperRoboMonkei) && )

            return false;
        }
    }
}