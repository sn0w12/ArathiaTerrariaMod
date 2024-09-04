using Terraria;
using Terraria.ModLoader;

namespace Arathia.Utilities
{
    public static class DebuffHelper
    {
        public static void DamagePerSecond(NPC npc, int buffIndex, int baseDamage, float damageMultiplier = 1f)
        {
            int damagePerSecond = (int)(baseDamage * damageMultiplier);
            if (damagePerSecond > 0)
            {
                if (npc.buffTime[buffIndex] % 60 == 0) // Apply direct damage every second
                {
                    NPC.HitInfo hitInfo = new()
                    {
                        Damage = damagePerSecond,
                    };
                    npc.StrikeNPC(hitInfo);
                }
            }
        }

        public static float GetTotalAdditiveDamage(Player player, params DamageClass[] damageClasses)
        {
            float totalAdditiveDamage = 0f;

            // If no damage classes are provided, get all additive damage
            if (damageClasses.Length == 0)
            {
                damageClasses =
                    [
                        DamageClass.Generic,
                        DamageClass.Melee,
                        DamageClass.Ranged,
                        DamageClass.Magic,
                        DamageClass.Summon,
                        DamageClass.Throwing,
                    ];
            }

            foreach (var damageClass in damageClasses)
            {
                totalAdditiveDamage += player.GetDamage(damageClass).Additive - 1;
            }

            return 1 + totalAdditiveDamage;
        }
    }
}