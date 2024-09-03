using Terraria.ModLoader;
using Terraria;
using Arathia.Content.Buffs;

namespace Arathia.Common.GlobalNPCs
{
    public class HolyFlameGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public float damageMultiplier = 1f;

        public override void ResetEffects(NPC npc)
        {
            if (!npc.HasBuff(ModContent.BuffType<HolyFlameDebuff>()))
            {
                damageMultiplier = 1f;
            }
        }
    }
}
