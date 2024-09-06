using Terraria;
using Terraria.ModLoader;

namespace Arathia.Utilities
{
    class DamageHelper
    {
        private static readonly DamageClass[] allDamageClasses =
            [
                DamageClass.Generic,
                DamageClass.Melee,
                DamageClass.Ranged,
                DamageClass.Magic,
                DamageClass.Summon,
                DamageClass.Throwing,
            ];

        /// <summary>
        /// Gets the total additive damage based on the damage classes, if no damage classes get total from all of them.
        /// </summary>
        /// <returns>Float representing a damage multiplier</returns>
        public static float GetTotalAdditiveDamage(Player player, params DamageClass[] damageClasses)
        {
            float totalAdditiveDamage = 0f;

            // If no damage classes are provided, get all additive damage
            if (damageClasses.Length == 0)
            {
                damageClasses = allDamageClasses;
            }

            foreach (var damageClass in damageClasses)
            {
                // Minus one for actual % difference
                totalAdditiveDamage += player.GetDamage(damageClass).Additive - 1;
            }

            return 1 + totalAdditiveDamage;
        }
    }
}