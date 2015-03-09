using System.Collections.Generic;
using DevBot.Game.AI.Enums;
using DevBot.Game.Enums;

namespace DevBot.Game.AI.Decks
{
    public class ChaosDragonExecutor : Executor
    {
        public override string Deck
        {
            get { return "ChaosDragon"; }
        }
        
        private int RykoDesc = 344044737;
        public int LastChainer = -1;

        public enum CardId
        {
            REDMD = 88264978,
            GalaxyEyesPhotonDragon = 93717133,
            BLSEotB = 72989439,
            DAD = 65192027,
            Gorz = 44330098,
            Lightpulsar = 99365553,
            Darkflare = 25460258,
            ChaosSorc = 9596126,
            EclipseWyvern = 51858306,
            DivineDragon = 20277376,
            TKRO = 71564252,
            CardTrooper = 85087012,
            Calc = 51196174,
            Fader = 19665973,
            Ryko = 21502796,
            Reasoning = 58577036,
            MST = 5318639,
            FoolishBurial = 81439173,
            DDR = 9622164,
            Charge = 94886282,
            Allure = 1475311,
            DarkHole = 53129443,
            MirrorForce = 44095762,
            Torrential = 53582587,
            Warning = 84749824,
            EscapeFromTheDarkDimension = 31550470,
            Compulsory = 94192409,
        }

        public ChaosDragonExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Execute spells
            AddExecutor(ExecutorType.Activate, (int)CardId.MST, MysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.Reasoning);
            AddExecutor(ExecutorType.Activate, (int)CardId.FoolishBurial, FoolishBurial);
            AddExecutor(ExecutorType.Activate, (int)CardId.Charge, ChargeoftheLightBrigade);
            AddExecutor(ExecutorType.Activate, (int)CardId.DDR, DDR);

            //Execute Traps
            AddExecutor(ExecutorType.Activate, (int)CardId.EscapeFromTheDarkDimension, EscapefromtheDarkDimension);

            // Execute monsters
            AddExecutor(ExecutorType.Activate, (int)CardId.REDMD, REDMDEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Darkflare, DarkflareEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.CardTrooper, CardTrooperEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.ChaosSorc, ChaosSorcEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.DivineDragon, DivineDragonEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.DAD, DADEffect);

            // Special Summon
            AddExecutor(ExecutorType.SpSummon, (int)CardId.REDMD, REDMDSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.DAD);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Lightpulsar, LightpulsarSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Darkflare, ChaosSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.BLSEotB, ChaosSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.ChaosSorc, ChaosSummon);

            // Use draw effects if we can't do anything else
            AddExecutor(ExecutorType.Activate, (int)CardId.Allure, AllureofDarkness);

            // Normal Summon
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.Ryko);
            AddExecutor(ExecutorType.Summon, (int)CardId.Calc, Calculator);
            AddExecutor(ExecutorType.Summon, (int)CardId.CardTrooper);
            AddExecutor(ExecutorType.Summon, (int)CardId.TKRO);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.DivineDragon);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.EclipseWyvern);

            // Chain traps and monsters
            AddExecutor(ExecutorType.Activate, (int)CardId.Warning, SolemnWarning);
            AddExecutor(ExecutorType.Activate, (int)CardId.Torrential, TorrentialTribute);
            AddExecutor(ExecutorType.Activate, (int)CardId.MirrorForce, MirrorForce);
            AddExecutor(ExecutorType.Activate, (int)CardId.Compulsory, CompulsoryEvac);
            AddExecutor(ExecutorType.Activate, (int)CardId.GalaxyEyesPhotonDragon, GEPDEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.BLSEotB, BLSEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Lightpulsar, LightpulsarEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Gorz);
            AddExecutor(ExecutorType.Activate, (int)CardId.Fader, BattleFader);
            AddExecutor(ExecutorType.Activate, (int)CardId.TKRO, SolemnWarning);

            // Set traps
            AddExecutor(ExecutorType.SpellSet, SetOurTraps);
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, bool cancelable)
        {
            //I reccomend using GetLastChainCard to work out what card effect is in action
            //Only use this when the cards might not be targetable

            //Always return base.OnSelectCard if nothing is done
            return base.OnSelectCard(cards, min, max, cancelable);
        }

        public override void OnChaining(int player, ClientCard card)
        {
            base.OnChaining(player, card);
            LastChainer = player;
            if (card.Id == (int)CardId.Ryko)
                RykoEffect();
            else if (card.Id == (int)CardId.EclipseWyvern)
                WyvernEffect(card);
        }

        public override bool OnSelectYesNo(int desc)
        {
            if (desc == RykoDesc) 
                return RykoEffect();
            return true;
        }

        private bool BattleFader()
        {
            return DefaultTrap();
        }

        private bool SetOurTraps()
        {
            if (Duel.Phase == Phase.Main2 || (Duel.Phase == Phase.Main1 && (Duel.Turn == 1 || !Duel.MainPhase.CanBattlePhase)))
                if (Card.IsTrap() && Duel.Fields[0].GetSpellCountWithoutField() < 4)
                    return true;
                else if (Card.IsSpell() && Card.Id == (int)CardId.MST && Duel.Fields[0].GetSpellCountWithoutField() < 4)
                    return true;
            return false;
        }

        private bool MysticalSpaceTyphoon()
        {
            List<ClientCard> spells = Duel.Fields[1].GetSpells();
            if (spells.Count == 0)
                return false;
            ClientCard selected = null;
            foreach (ClientCard card in spells)
            {
                if (Duel.Player == 1 && !card.HasType(CardType.Continuous) && !card.HasType(CardType.Equip))
                    continue;
                selected = card;
                if (Duel.Player == 0 && card.IsFacedown())
                    break;
            }
            if (selected == null)
                return false;
            AI.SelectCard(selected);
            return DefaultTrap();
        }

        private bool DarkHole() 
        {
            return AI.Utils.IsEnnemyBetter(false, false);
        }

        private bool MirrorForce()
        {
            if (AI.Utils.IsEnnemyBetter(true, false))
                return DefaultTrap();
            return false;
        }

        private bool TorrentialTribute()
        {
            if (AI.Utils.IsEnnemyBetter(false, false))
                return DefaultTrap();
            return false;
        }

        private bool FoolishBurial() 
        {
            int remaining = 3;
            foreach (ClientCard card in Duel.Fields[0].Banished)
                if (card.Id == (int)CardId.EclipseWyvern)
                    remaining--;
            foreach (ClientCard card in Duel.Fields[0].Graveyard)
                if (card.Id == (int)CardId.EclipseWyvern)
                    remaining--;
            foreach (ClientCard card in Duel.Fields[0].Hand)
                if (card.Id == (int)CardId.EclipseWyvern)
                    remaining--;
            if (Duel.Fields[0].GetMonsterCount() > 0)
                foreach (ClientCard card in Duel.Fields[0].MonsterZone)
                    if (card != null && card.Id == (int)CardId.EclipseWyvern)
                        remaining--;
            if (remaining > 0) 
            {
                AI.SelectCard((int)CardId.EclipseWyvern);
                return true;
            }
            remaining = 2;
            foreach (ClientCard card in Duel.Fields[0].Banished)
                if (card.Id == (int)CardId.Lightpulsar)
                    remaining--;
            foreach (ClientCard card in Duel.Fields[0].Graveyard)
                if (card.Id == (int)CardId.Lightpulsar)
                    remaining--;
            foreach (ClientCard card in Duel.Fields[0].Hand)
                if (card.Id == (int)CardId.Lightpulsar)
                    remaining--;
            if (Duel.Fields[0].GetMonsterCount() > 0)
                foreach (ClientCard card in Duel.Fields[0].MonsterZone)
                    if (card != null && card.Id == (int)CardId.Lightpulsar)
                        remaining--;
            if (remaining > 0)
            {
                AI.SelectCard((int)CardId.Lightpulsar);
                return true;
            }
            remaining = 3;
            foreach (ClientCard card in Duel.Fields[0].Banished)
                if (card.Id == (int)CardId.DivineDragon)
                    remaining--;
            foreach (ClientCard card in Duel.Fields[0].Graveyard)
                if (card.Id == (int)CardId.DivineDragon)
                    remaining--;
            foreach (ClientCard card in Duel.Fields[0].Hand)
                if (card.Id == (int)CardId.DivineDragon)
                    remaining--;
            if (Duel.Fields[0].GetMonsterCount() > 0)
                foreach (ClientCard card in Duel.Fields[0].MonsterZone)
                    if (card != null && card.Id == (int)CardId.DivineDragon)
                        remaining--;
            if (remaining > 0)
            {
                AI.SelectCard((int)CardId.DivineDragon);
                return true;
            }
            return false;
        }

        private bool ChargeoftheLightBrigade() 
        {
            int remaining = 2;
            foreach (ClientCard card in Duel.Fields[0].Banished)
                if (card.Id == (int)CardId.Ryko)
                    remaining--;
            foreach (ClientCard card in Duel.Fields[0].Graveyard)
                if (card.Id == (int)CardId.Ryko)
                    remaining--;
            foreach (ClientCard card in Duel.Fields[0].Hand)
                if (card.Id == (int)CardId.Ryko)
                    remaining--;
            if (Duel.Fields[0].GetMonsterCount() > 0)
                foreach (ClientCard card in Duel.Fields[0].MonsterZone)
                    if (card != null && card.Id == (int)CardId.Ryko)
                        remaining--;
            if (remaining > 0) 
            {
                AI.SelectCard((int)CardId.Ryko);
                return true;
            }
            return false;
        }

        private bool DDR() 
        {
            ClientField field = Duel.Fields[0];

            int tributeId = -1;
            if (field.HasInHand((int)CardId.EclipseWyvern)) 
                tributeId = (int)CardId.EclipseWyvern; 
            else if (field.HasInHand((int)CardId.DivineDragon)) 
                tributeId = (int)CardId.DivineDragon; 
            else if (field.HasInHand((int)CardId.Calc)) 
                tributeId = (int)CardId.Calc; 
            else if (field.HasInHand((int)CardId.Darkflare)) 
                tributeId = (int)CardId.Darkflare; 
            else if (field.HasInHand((int)CardId.Gorz)) 
                tributeId = (int)CardId.Gorz; 
            else if (field.HasInHand((int)CardId.ChaosSorc)) 
                tributeId = (int)CardId.ChaosSorc; 
            else 
                return false;

            foreach (ClientCard card in Duel.Fields[0].Banished) 
                if (card.Id == (int)CardId.REDMD) 
                {
                    AI.SelectCard(tributeId); 
                    AI.SelectNextCard((int)CardId.REDMD); 
                    return true; 
                }
            foreach (ClientCard card in Duel.Fields[0].Banished) 
                if (card.Id == (int)CardId.Lightpulsar) 
                { 
                    AI.SelectCard(tributeId); 
                    AI.SelectNextCard((int)CardId.Lightpulsar); 
                    return true; 
                }
            foreach (ClientCard card in Duel.Fields[0].Banished) 
                if (card.Id == (int)CardId.GalaxyEyesPhotonDragon) 
                { 
                    AI.SelectCard(tributeId); 
                    AI.SelectNextCard((int)CardId.GalaxyEyesPhotonDragon); 
                    return true; 
                }
            foreach (ClientCard card in Duel.Fields[0].Banished) 
                if (card.Id == (int)CardId.Darkflare) 
                { 
                    AI.SelectCard(tributeId); 
                    AI.SelectNextCard((int)CardId.Darkflare); 
                    return true; 
                }
            foreach (ClientCard card in Duel.Fields[0].Banished) 
                if (card.Id == (int)CardId.Gorz) 
                { 
                    AI.SelectCard(tributeId); 
                    AI.SelectNextCard((int)CardId.Gorz); 
                    return true; 
                }
            return false;
        }

        private bool EscapefromtheDarkDimension() 
        {
            foreach (ClientCard card in Duel.Fields[0].Banished) 
                if (card.Id == (int)CardId.REDMD) 
                { 
                    AI.SelectCard((int)CardId.REDMD); 
                    return true; 
                }
            foreach (ClientCard card in Duel.Fields[0].Banished) 
                if (card.Id == (int)CardId.Darkflare) 
                { 
                    AI.SelectCard((int)CardId.Darkflare); 
                    return true; 
                }
            foreach (ClientCard card in Duel.Fields[0].Banished) 
                if (card.Id == (int)CardId.REDMD) 
                { 
                    AI.SelectCard((int)CardId.Gorz); 
                    return true; 
                }
            return false;
        }

        private bool REDMDEffect() 
        {
            ClientField field = Duel.Fields[0];
            int selected = 0;
            if ((field.HasInGraveyard((int)CardId.GalaxyEyesPhotonDragon) || field.HasInHand((int)CardId.GalaxyEyesPhotonDragon)) && field.HasInMonstersZone((int)CardId.Lightpulsar))
                selected = (int)CardId.GalaxyEyesPhotonDragon;
            else if ((field.HasInGraveyard((int)CardId.GalaxyEyesPhotonDragon) || field.HasInHand((int)CardId.GalaxyEyesPhotonDragon)) && (!field.HasInGraveyard((int)CardId.Lightpulsar) && !field.HasInHand((int)CardId.Lightpulsar) && !field.HasInMonstersZone((int)CardId.Lightpulsar)))
                selected = (int)CardId.GalaxyEyesPhotonDragon;
            else if (field.HasInGraveyard((int)CardId.Lightpulsar) || field.HasInHand((int)CardId.Lightpulsar))
                selected = (int)CardId.Lightpulsar;
            else if (field.HasInGraveyard((int)CardId.Darkflare) || field.HasInHand((int)CardId.Darkflare))
                selected = (int)CardId.Darkflare;
            else if (field.HasInGraveyard((int)CardId.EclipseWyvern) || field.HasInHand((int)CardId.EclipseWyvern))
                selected = (int)CardId.EclipseWyvern;
            else if (field.HasInGraveyard((int)CardId.DivineDragon) || field.HasInHand((int)CardId.DivineDragon))
                selected = (int)CardId.DivineDragon;
            else
                return false;
            AI.SelectCard(selected);
            return true;
        }

        private bool DarkflareEffect()
        {
            ClientField field = Duel.Fields[0];
            int handcardId = -1;
            int deckcardId = -1;
            int banishId = -1;
            if (field.HasInHand((int)CardId.EclipseWyvern))
                handcardId = (int)CardId.EclipseWyvern;
            else if (field.HasInHand((int)CardId.DivineDragon))
                handcardId = (int)CardId.DivineDragon;
            else if (field.HasInHand((int)CardId.DAD))
                handcardId = (int)CardId.DAD;
            else if (field.HasInHand((int)CardId.Darkflare))
                handcardId = (int)CardId.Darkflare;
            else if (field.HasInHand((int)CardId.GalaxyEyesPhotonDragon))
                handcardId = (int)CardId.GalaxyEyesPhotonDragon;
            else
                return false;

            if (handcardId != (int)CardId.EclipseWyvern)
            {
                int remaining = 2;
                foreach (ClientCard card in Duel.Fields[0].Graveyard)
                    if (card.Id == (int)CardId.EclipseWyvern) 
                        remaining--;
                foreach (ClientCard card in Duel.Fields[0].Hand)
                    if (card.Id == (int)CardId.EclipseWyvern) 
                        remaining--; 
                foreach (ClientCard card in Duel.Fields[0].Banished)
                    if (card.Id == (int)CardId.EclipseWyvern) 
                        remaining--;

                if (Duel.Fields[0].GetMonsterCount() > 0)
                    foreach (ClientCard card in Duel.Fields[0].MonsterZone)
                        if (card != null)
                            if (card.Id == (int)CardId.EclipseWyvern) 
                                remaining--; 
                       
                if (remaining > 0) 
                    deckcardId = (int)CardId.EclipseWyvern;
            }
            else if (handcardId == (int)CardId.EclipseWyvern || handcardId == (int)CardId.Lightpulsar || handcardId == (int)CardId.GalaxyEyesPhotonDragon)
            {
                if (deckcardId < 0) //Seach for Divine Dragon
                {
                    int remaining = 3;
                    foreach (ClientCard card in Duel.Fields[0].Graveyard)
                        if (card.Id == (int)CardId.DivineDragon) 
                            remaining--;
                    foreach (ClientCard card in Duel.Fields[0].Hand)
                        if (card.Id == (int)CardId.DivineDragon) 
                            remaining--;
                    foreach (ClientCard card in Duel.Fields[0].Banished)
                        if (card.Id == (int)CardId.DivineDragon) 
                            remaining--;
                    if (Duel.Fields[0].GetMonsterCount() > 0)
                        foreach (ClientCard card in Duel.Fields[0].MonsterZone)
                            if (card != null && card.Id == (int)CardId.DivineDragon)
                                remaining--;
                    if (remaining > 0) 
                        deckcardId = (int)CardId.DivineDragon;
                }
                if (deckcardId < 0) //Seach for Darkflare
                {
                    int remaining = 2;
                    foreach (ClientCard card in Duel.Fields[0].Graveyard)
                        if (card.Id == (int)CardId.Darkflare) 
                            remaining--;
                    foreach (ClientCard card in Duel.Fields[0].Hand)
                        if (card.Id == (int)CardId.Darkflare) 
                            remaining--;
                    foreach (ClientCard card in Duel.Fields[0].Banished)
                        if (card.Id == (int)CardId.Darkflare) 
                            remaining--;
                    if (Duel.Fields[0].GetMonsterCount() > 0) 
                        foreach (ClientCard card in Duel.Fields[0].MonsterZone)
                            if (card != null && card.Id == (int)CardId.Darkflare) 
                                remaining--;
                    if (remaining > 0) 
                        deckcardId = (int)CardId.Darkflare;
                }
                if (deckcardId < 0) 
                    return false;

            }
            else if (handcardId == (int)CardId.Darkflare || handcardId == (int)CardId.DivineDragon)
            {
                if (deckcardId < 0) //Seach for Galaxy-Eyes Photon Dragon
                {
                    int remaining = 3;
                    foreach (ClientCard card in Duel.Fields[0].Graveyard)
                        if (card.Id == (int)CardId.GalaxyEyesPhotonDragon) 
                            remaining--;
                    foreach (ClientCard card in Duel.Fields[0].Hand)
                        if (card.Id == (int)CardId.GalaxyEyesPhotonDragon) 
                            remaining--;
                    foreach (ClientCard card in Duel.Fields[0].Banished)
                        if (card.Id == (int)CardId.GalaxyEyesPhotonDragon) 
                            remaining--;
                    if (Duel.Fields[0].GetMonsterCount() > 0)
                        foreach (ClientCard card in Duel.Fields[0].MonsterZone)
                            if (card != null && card.Id == (int)CardId.GalaxyEyesPhotonDragon) 
                                remaining--;
                    if (remaining > 0) 
                        deckcardId = (int)CardId.GalaxyEyesPhotonDragon;
                }
                if (deckcardId < 0) //Seach for Lightpulsar
                {
                    int remaining = 2;
                    foreach (ClientCard card in Duel.Fields[0].Graveyard)
                        if (card.Id == (int)CardId.Lightpulsar) 
                            remaining--;
                    foreach (ClientCard card in Duel.Fields[0].Hand)
                        if (card.Id == (int)CardId.Lightpulsar) 
                            remaining--;
                    foreach (ClientCard card in Duel.Fields[0].Banished)
                        if (card.Id == (int)CardId.Lightpulsar) 
                            remaining--;
                    if (Duel.Fields[0].GetMonsterCount() > 0)
                        foreach (ClientCard card in Duel.Fields[0].MonsterZone)
                            if (card != null && card.Id == (int)CardId.Lightpulsar) 
                                remaining--;
                    if (remaining > 0) 
                        deckcardId = (int)CardId.Lightpulsar;
                }
                if (deckcardId < 0) 
                    return false;
            }

            //Now we have to banish 1 card. If deckcardId and handcardId != Eclipse Wyvern, we want to banish wyvern, else banish anything else.
            if ((deckcardId != (int)CardId.EclipseWyvern && handcardId != (int)CardId.EclipseWyvern) && field.HasInGraveyard((int)CardId.EclipseWyvern))
                banishId = (int)CardId.EclipseWyvern;
            else
                if (handcardId == (int)CardId.Lightpulsar) 
                    banishId = deckcardId;
                else if (deckcardId == (int)CardId.Lightpulsar) 
                    banishId = handcardId;
                else 
                    banishId = deckcardId;
            if (handcardId > 0 && deckcardId > 0 && banishId > 0)
            {
                AI.SelectCard(handcardId); 
                AI.SelectNextCard(deckcardId); 
                AI.SelectNextCard(banishId); 
                return true;
            }
            return false;
        }

        private bool CardTrooperEffect()
        {
            int Number = 0;
            if (Duel.Fields[0].Deck.Count >= 18) 
                Number = 3;
            else if (Duel.Fields[0].Deck.Count == 17) 
                Number = 2;
            else if (Duel.Fields[0].Deck.Count == 16) 
                Number = 1;
            
            if (Number == 0) 
                return false;
            AI.SelectNumber(Number);
            return true;
        }

        private bool ChaosSorcEffect()
        {
            List<ClientCard> monsters = Duel.Fields[1].GetMonsters();
            ClientCard selected = null;
            if (monsters.Count > 0)
            {
                int highestAttack = 0;
                foreach (ClientCard card in monsters)
                {
                    if (card.IsFacedown())
                        continue;
                    if (card.Attack > highestAttack)
                    {
                        highestAttack = card.Attack;
                        selected = card;
                    }
                }
                if (selected != null)
                {
                    AI.SelectCard(selected);
                    return true;
                }
            }
            return false;
        }

        private bool DivineDragonEffect()
        {
            ClientField field = Duel.Fields[0];
            int discardId = -1;
            int selectId = -1;

            if (field.HasInGraveyard((int)CardId.REDMD)) 
                selectId = (int)CardId.REDMD;
            else if (field.HasInGraveyard((int)CardId.Lightpulsar)) 
                selectId = (int)CardId.Lightpulsar;
            else 
                return false;

            if (field.HasInHand((int)CardId.EclipseWyvern)) 
               discardId = (int)CardId.EclipseWyvern;
            else if (field.HasInHand((int)CardId.GalaxyEyesPhotonDragon)) 
                discardId = (int)CardId.GalaxyEyesPhotonDragon; 
            else if (field.HasInHand((int)CardId.DAD)) 
                discardId = (int)CardId.DAD; 
            else if (field.HasInHand((int)CardId.DivineDragon)) 
                discardId = (int)CardId.DivineDragon; 
            else if (field.HasInHand((int)CardId.Darkflare))
                discardId = (int)CardId.Darkflare; 
            else if (field.HasInHand((int)CardId.Ryko)) 
                discardId = (int)CardId.Ryko; 
            else 
                return false;

            if (discardId == 0 || selectId == 0)
                return false;
            AI.SelectCard(discardId); 
            AI.SelectNextCard(selectId); 
            return true;
        }

        private bool REDMDSummon()
        {
            ClientField field = Duel.Fields[0];
            int banish = -1;
            if (field.HasInMonstersZone((int)CardId.DivineDragon)) 
                banish = (int)CardId.DivineDragon; 
            else if (field.HasInMonstersZone((int)CardId.Darkflare)) 
                banish = (int)CardId.Darkflare; 
            else if (field.HasInMonstersZone((int)CardId.DAD)) 
                banish = (int)CardId.DAD; 
            else if (field.HasInMonstersZone((int)CardId.EclipseWyvern)) 
                banish = (int)CardId.EclipseWyvern; 
            else if (field.HasInMonstersZone((int)CardId.Lightpulsar) && (field.HasInGraveyard((int)CardId.Lightpulsar) || (field.HasInHand((int)CardId.Lightpulsar)))) 
                banish = (int)CardId.Lightpulsar;
            else 
                return false;
            if (banish == 0)
                return false;
            AI.SelectCard(banish); 
            return true; 
        }

        private bool LightpulsarSummon()
        {
            ClientField field = Duel.Fields[0];
            int light = -1;
            int dark = -1;
            if (Card.Location == CardLocation.Hand) //Summoning from Hand
                return ChaosSummon();
            else if (Card.Location == CardLocation.Grave)
            {
                //Select a light monster in hand to discard.
                if (field.HasInHand((int)CardId.EclipseWyvern)) 
                    light = (int)CardId.EclipseWyvern; 
                else if (field.HasInHand((int)CardId.Ryko)) 
                    light = (int)CardId.Ryko; 
                else if (field.HasInHand((int)CardId.Calc)) 
                    light = (int)CardId.Calc; 
                else if (field.HasInHand((int)CardId.TKRO)) 
                    light = (int)CardId.TKRO; 
                else if (field.HasInHand((int)CardId.GalaxyEyesPhotonDragon) && field.HasInHand((int)CardId.DDR)) 
                    light = (int)CardId.GalaxyEyesPhotonDragon;
                else 
                    return false;
                //Select Dark Monster to discard
                if (field.HasInHand((int)CardId.DivineDragon)) 
                    dark = (int)CardId.DivineDragon; 
                else if (field.HasInHand((int)CardId.DAD)) 
                    dark = (int)CardId.DAD; 
                else if (field.HasInHand((int)CardId.Gorz)) 
                    dark = (int)CardId.Gorz; 
                else if (field.HasInHand((int)CardId.Fader)) 
                    dark = (int)CardId.Fader; 
                else if (field.HasInHand((int)CardId.ChaosSorc)) 
                    dark = (int)CardId.ChaosSorc; 
                else if (field.HasInHand((int)CardId.Darkflare)) 
                    dark = (int)CardId.Darkflare; 
                else 
                    return false;
                AI.SelectCard(light); 
                AI.SelectNextCard(dark); 
                return true;
            }
            return false;
        }

        private bool ChaosSummon()
        {
            ClientField field = Duel.Fields[0];
            int light = -1;
            int dark = -1;
            
            if (field.HasInGraveyard((int)CardId.EclipseWyvern)) 
                light = (int)CardId.EclipseWyvern; 
            else if (field.HasInGraveyard((int)CardId.Ryko)) 
                light = (int)CardId.Ryko; 
            else if (field.HasInGraveyard((int)CardId.Calc)) 
                light = (int)CardId.Calc;
            else if (field.HasInGraveyard((int)CardId.TKRO)) 
                light = (int)CardId.TKRO;
            else if (field.HasInGraveyard((int)CardId.GalaxyEyesPhotonDragon) && field.HasInHand((int)CardId.DDR)) 
                light = (int)CardId.GalaxyEyesPhotonDragon;
            else 
                return false;
            
            if (field.HasInGraveyard((int)CardId.DivineDragon)) 
                dark = (int)CardId.DivineDragon;
            else if (field.HasInGraveyard((int)CardId.DAD)) 
                dark = (int)CardId.DAD;
            else if (field.HasInGraveyard((int)CardId.Gorz)) 
                dark = (int)CardId.Gorz; 
            else if (field.HasInGraveyard((int)CardId.Fader)) 
                dark = (int)CardId.Fader; 
            else if (field.HasInGraveyard((int)CardId.ChaosSorc)) 
                dark = (int)CardId.ChaosSorc;
            else if (field.HasInGraveyard((int)CardId.Darkflare)) 
                dark = (int)CardId.Darkflare;
            else 
                return false;
            AI.SelectCard(light); 
            AI.SelectNextCard(dark); 
            return true;
        }

        private bool AllureofDarkness()
        {
            ClientField field = Duel.Fields[0];
            int banish = -1;
            if (field.HasInHand((int)CardId.DivineDragon)) 
                banish = (int)CardId.DivineDragon;
            else if (field.HasInHand((int)CardId.Gorz)) 
                banish = (int)CardId.Gorz; 
            else if (field.HasInHand((int)CardId.DAD)) 
                banish = (int)CardId.DAD; 
            else if (field.HasInHand((int)CardId.Fader)) 
                banish = (int)CardId.Fader; 
            else if (field.HasInHand((int)CardId.ChaosSorc)) 
                banish = (int)CardId.Fader; 
            else if (field.HasInHand((int)CardId.Darkflare)) 
                banish = (int)CardId.Darkflare; 
            if (banish > 0)
                return true;
            return false;
        }

        private bool SolemnWarning()
        {
            if (AI.Utils.IsEnnemyBetter(true, false))
                return DefaultTrap();
            return false;
        }

        private bool WyvernEffect(ClientCard Card)
        {
            if (Card.Location == CardLocation.Grave)
            {
                ClientField field = Duel.Fields[0];

                //Search for Red-Eyes Darkness Metal Dragon
                bool redmdsearch = true;
                if (field.HasInHand((int)CardId.REDMD)) 
                    redmdsearch = false;
                else if (field.HasInGraveyard((int)CardId.REDMD)) 
                    redmdsearch = false;
                else if (field.HasInMonstersZone((int)CardId.REDMD)) 
                    redmdsearch = false;
                else
                    foreach (ClientCard card in field.Banished)
                        if (card.Id == (int)CardId.REDMD) 
                            redmdsearch = false;
                if (redmdsearch) { 
                    AI.SelectCard((int)CardId.REDMD); 
                    return true; 
                }
                
                //Search for Galaxy-Eyes Photon Dragon
                int GEPD = (int)CardId.GalaxyEyesPhotonDragon;
                int remaining = 3;
                foreach (ClientCard card in field.Hand)
                    if (card.Id == GEPD) remaining--;
                foreach (ClientCard card in field.Graveyard)
                    if (card.Id == GEPD) remaining--;
                foreach (ClientCard card in field.Banished)
                    if (card.Id == GEPD) remaining--;
                foreach (ClientCard card in field.MonsterZone)
                    if (card != null && card.Id == GEPD) 
                        remaining--;
                if (remaining > 0) { 
                    AI.SelectCard(GEPD); 
                    return true; 
                }

                //Search for Dark Armed Dragon
                bool dadsearch = true;
                if (field.HasInHand((int)CardId.DAD)) 
                    dadsearch = false;
                else if (field.HasInGraveyard((int)CardId.DAD)) 
                    dadsearch = false;
                else if (field.HasInMonstersZone((int)CardId.DAD)) 
                    dadsearch = false;
                else
                    foreach (ClientCard card in field.Banished)
                        if (card.Id == (int)CardId.DAD) 
                            dadsearch = false;
                if (dadsearch == true) { 
                    AI.SelectCard((int)CardId.DAD); 
                    return true; 
                }

                //Else return False
                return false;
            }
            else if (Card.Location == CardLocation.Removed) 
                return true;
            return false;
        }
        private bool DADEffect()
        {
            ClientField field = Duel.Fields[0];
            int banishId = -1;
            if (field.HasInGraveyard((int)CardId.ChaosSorc)) 
                banishId = (int)CardId.ChaosSorc; 
            else if (field.HasInGraveyard((int)CardId.Fader)) 
                banishId = (int)CardId.Fader; 
            else if (field.HasInGraveyard((int)CardId.Gorz)) 
                banishId = (int)CardId.Gorz; 
            else if (field.HasInGraveyard((int)CardId.DivineDragon)) 
                banishId = (int)CardId.DivineDragon; 
            else if (field.HasInGraveyard((int)CardId.Darkflare)) 
                banishId = (int)CardId.Darkflare; 
            else if (field.HasInGraveyard((int)CardId.REDMD) && (field.HasInSpellZone((int)CardId.EscapeFromTheDarkDimension) || field.HasInHand((int)CardId.DDR) || field.HasInHand((int)CardId.EscapeFromTheDarkDimension))) 
                banishId = (int)CardId.REDMD;
            else 
                return false;

            if (Duel.Fields[1].IsFieldEmpty())
                return false;
            int selectedId = 0;

            List<ClientCard> spells = Duel.Fields[1].GetSpells();
            List<ClientCard> monsters = Duel.Fields[1].GetMonsters();
            if (Duel.Fields[1].HasInSpellZone((int)NegatesSummons.Necrovalley))
                selectedId = (int)NegatesSummons.Necrovalley;
            else if (Duel.Fields[1].HasInSpellZone((int)NegatesSummons.ImperialIronWall))
                selectedId = (int)NegatesSummons.ImperialIronWall;
            else if (Duel.Fields[1].HasInSpellZone((int)NegatesSummons.DnaSurgery))
                selectedId = (int)NegatesSummons.DnaSurgery;
            else if (Duel.Fields[1].HasInSpellZone((int)NegatesSummons.ZombieWorld))
                selectedId = (int)NegatesSummons.ZombieWorld;
            else if (Duel.Fields[1].HasInSpellZone((int)NegateAttackSpell.SavageColosseum))
                selectedId = (int)NegateAttackSpell.SavageColosseum;
            else if (Duel.Fields[1].HasInMonstersZone((int)NegatesSpells.HorusLv6))
                selectedId = (int)NegatesSpells.HorusLv6;
            else if (Duel.Fields[1].HasInMonstersZone((int)NegatesSpells.HorusLv8))
                selectedId = (int)NegatesSpells.HorusLv8;
            else if (Duel.Fields[1].HasInMonstersZone((int)NegatesTraps.Jinzo))
                selectedId = (int)NegatesTraps.Jinzo;
            else if (Duel.Fields[1].HasInMonstersZone((int)NegatesTraps.RoyalDecree))
                selectedId = (int)NegatesTraps.RoyalDecree;
            else if (Duel.Fields[1].HasInMonstersZone((int)InvincibleMonster.LionHeart))
                selectedId = (int)InvincibleMonster.LionHeart;
            else if (Duel.Fields[1].HasInMonstersZone((int)InvincibleMonster.Marshmallon))
                selectedId = (int)InvincibleMonster.Marshmallon;
            else if (Duel.Fields[1].HasInMonstersZone((int)InvincibleMonster.SpiritReaper))
                selectedId = (int)InvincibleMonster.SpiritReaper;
            else if (Duel.Fields[1].HasInSpellZone((int)NegateAttackSpell.MessengerOfPeace))
                selectedId = (int)NegateAttackSpell.MessengerOfPeace;
            else if (Duel.Fields[1].HasInSpellZone((int)NegateAttackSpell.GravityBind))
                selectedId = (int)NegateAttackSpell.GravityBind;
            else if (Duel.Fields[1].HasInSpellZone((int)NegatesEffects.SoulDrain))
                selectedId = (int)NegatesEffects.SoulDrain;
            else if (Duel.Fields[1].HasInSpellZone((int)NegatesSummons.DimensionalFissure))
                selectedId = (int)NegatesSummons.DimensionalFissure;
            else if (Duel.Fields[1].HasInSpellZone((int)NegatesSummons.MacroCosmos))
                selectedId = (int)NegatesSummons.MacroCosmos;

            if (selectedId > 0)
            {
                AI.SelectCard(banishId);
                AI.SelectNextCard(selectedId);
            }
            else
            {
                ClientCard selected = null;
                foreach (ClientCard card in spells)
                    if (card.IsFacedown() && selected == null)
                        selected = card;

                foreach (ClientCard card in monsters)
                    if (card.IsFacedown() && selected == null)
                        selected = card;

                if (monsters.Count > 0 && selected == null)
                {
                    selected = monsters.GetHighestAttackMonster();
                    if (selected == null)
                        selected = monsters[Program.Rand.Next(0, monsters.Count)];
                }

                if (selected == null && spells.Count > 0)
                    selected = spells[Program.Rand.Next(0, spells.Count)];
                AI.SelectCard(banishId);
                AI.SelectNextCard(selected);
            }
            return true;
        }
        private bool RykoEffect()
        {
            if (Duel.Fields[1].IsFieldEmpty())
                return false;
            int selectedId = 0;
            
            List<ClientCard> spells = Duel.Fields[1].GetSpells();
            List<ClientCard> monsters = Duel.Fields[1].GetMonsters();
            if (Duel.Fields[1].HasInSpellZone((int)NegatesSummons.Necrovalley))
                selectedId = (int)NegatesSummons.Necrovalley;
            else if (Duel.Fields[1].HasInSpellZone((int)NegatesSummons.ImperialIronWall))
                selectedId = (int)NegatesSummons.ImperialIronWall;
            else if (Duel.Fields[1].HasInSpellZone((int)NegatesSummons.DnaSurgery))
                selectedId = (int)NegatesSummons.DnaSurgery;
            else if (Duel.Fields[1].HasInSpellZone((int)NegatesSummons.ZombieWorld))
                selectedId = (int)NegatesSummons.ZombieWorld;
            else if (Duel.Fields[1].HasInSpellZone((int)NegateAttackSpell.SavageColosseum))
                selectedId = (int)NegateAttackSpell.SavageColosseum;
            else if (Duel.Fields[1].HasInMonstersZone((int)NegatesSpells.HorusLv6))
                selectedId = (int)NegatesSpells.HorusLv6;
            else if (Duel.Fields[1].HasInMonstersZone((int)NegatesSpells.HorusLv8))
                selectedId = (int)NegatesSpells.HorusLv8;
            else if (Duel.Fields[1].HasInMonstersZone((int)NegatesTraps.Jinzo))
                selectedId = (int)NegatesTraps.Jinzo;
            else if (Duel.Fields[1].HasInMonstersZone((int)NegatesTraps.RoyalDecree))
                selectedId = (int)NegatesTraps.RoyalDecree;
            else if (Duel.Fields[1].HasInMonstersZone((int)InvincibleMonster.LionHeart))
                selectedId = (int)InvincibleMonster.LionHeart;
            else if (Duel.Fields[1].HasInMonstersZone((int)InvincibleMonster.Marshmallon))
                selectedId = (int)InvincibleMonster.Marshmallon;
            else if (Duel.Fields[1].HasInMonstersZone((int)InvincibleMonster.SpiritReaper))
                selectedId = (int)InvincibleMonster.SpiritReaper;
            else if (Duel.Fields[1].HasInSpellZone((int)NegateAttackSpell.MessengerOfPeace))
                selectedId = (int)NegateAttackSpell.MessengerOfPeace;
            else if (Duel.Fields[1].HasInSpellZone((int)NegateAttackSpell.GravityBind))
                selectedId = (int)NegateAttackSpell.GravityBind;
            else if (Duel.Fields[1].HasInSpellZone((int)NegatesEffects.SoulDrain))
                selectedId = (int)NegatesEffects.SoulDrain;
            else if (Duel.Fields[1].HasInSpellZone((int)NegatesSummons.DimensionalFissure))
                selectedId = (int)NegatesSummons.DimensionalFissure;
            else if (Duel.Fields[1].HasInSpellZone((int)NegatesSummons.MacroCosmos))
                selectedId = (int)NegatesSummons.MacroCosmos;
            
            if (selectedId > 0)
                AI.SelectCard(selectedId);
            else
            {
                ClientCard selected = null;
                foreach (ClientCard card in spells)
                    if (card.IsFacedown() && selected == null)
                        selected = card;

                foreach (ClientCard card in monsters)
                    if (card.IsFacedown() && selected == null)
                        selected = card;

                if (monsters.Count > 0 && selected == null)
                {
                    selected = monsters.GetHighestAttackMonster();
                    if (selected == null && monsters.Count > 0)
                        selected = monsters[Program.Rand.Next(0, monsters.Count)];
                }

                if (selected == null && spells.Count > 0)
                    selected = spells[Program.Rand.Next(0, spells.Count)];

                AI.SelectCard(selected);
            }
            return true;
        }

        private bool LightpulsarEffect()
        {
            ClientField field = Duel.Fields[0];
            if (field.HasInGraveyard((int)CardId.REDMD)) 
            {
                AI.SelectCard((int)CardId.REDMD);
                return true; 
            }
            else if (field.HasInGraveyard((int)CardId.Darkflare)) 
            {
                AI.SelectCard((int)CardId.Darkflare); 
                return true; 
            }
            else 
                return false;
        }

        private bool GEPDEffect()
        {
            return false;
        }

        private bool BLSEffect()
        {
            if (Duel.Phase == Phase.Battle) 
                return true;
            return false;
        }

        private bool Calculator() //Check if it is viable to summon The Calculator
        {
            ClientField field = Duel.Fields[0];
            if (!field.HasInHand((int)CardId.Calc)) 
                return false; 
            int leveltotal = 0;
            if (field.GetMonsterCount() > 0)
                foreach (ClientCard card in field.MonsterZone)
                    if (card != null)
                        if (!card.IsFacedown())
                            leveltotal = leveltotal + card.Level;
            if (leveltotal > 5)
                return true;
            return false;
        }

        private bool CompulsoryEvac()
        {
            List<ClientCard> monsters = Duel.Fields[1].GetMonsters();
            if (monsters.Count > 0)
            {
                ClientCard selected = monsters.GetHighestAttackMonster();
                if (selected == null)
                    selected = monsters[Program.Rand.Next(0, monsters.Count)];
                if (selected == null)
                    return false;
                AI.SelectCard(selected);
                return DefaultTrap();
            }
            return false;
        }
    }
}