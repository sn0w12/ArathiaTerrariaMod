using System;
using Terraria;
using Terraria.ModLoader;

namespace Arathia.Utilities
{
    /// <summary>
    /// Helper functions for debuffs.
    /// </summary>
    public static class DebuffHelper
    {
        /// <summary>
        /// Applies a certain damage per second to an npc.
        /// </summary>
        /// <param name="npc">Npc to damage.</param>
        /// <param name="buffIndex">Index of the buff, used to get the time of the buff.</param>
        /// <param name="baseDamage">The base damage to apply per second.</param>
        /// <param name="damageMultiplier">Float to multiple damage with.</param>
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