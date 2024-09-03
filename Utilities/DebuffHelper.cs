using Terraria;

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
    }
}