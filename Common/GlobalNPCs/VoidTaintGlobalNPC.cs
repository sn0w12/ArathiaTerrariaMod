using Arathia.Content.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace Arathia.Common.GlobalNPCs
{
    public class VoidTaintGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int originalDefense;
        public bool voidTaintApplied;

        public override void ResetEffects(NPC npc)
        {
            if (!npc.HasBuff(ModContent.BuffType<VoidTaintDebuff>()))
            {
                npc.defDefense = originalDefense;
                voidTaintApplied = false;
            }
        }
    }
}
