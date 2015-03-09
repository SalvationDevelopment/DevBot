using System;
using DevBot.Game.AI.Enums;

namespace DevBot.Game.AI
{
    public static class CardExtension
    {
        public static bool IsMonsterInvincible(this ClientCard card)
        {
            return Enum.IsDefined(typeof(InvincibleMonster), card.Id);
        }

        public static bool IsMonsterDangerous(this ClientCard card)
        {
            if (card.Id == (int)InvincibleMonster.Marshmallon ||
                card.Id == (int)InvincibleMonster.SpiritReaper)
                return false;
            return true;
        }

        public static bool IsSpellNegateAttack(this ClientCard card)
        {
            return Enum.IsDefined(typeof(NegateAttackSpell), card.Id);
        }
    }
}