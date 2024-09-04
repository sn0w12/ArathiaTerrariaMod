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

            foreach (var damageClass in damageClasses)
            {
                totalAdditiveDamage += player.GetDamage(damageClass).Additive;
            }

            return totalAdditiveDamage;
        }
    }
}