using Arathia.Common.GlobalNPCs;
using Arathia.Content.Dusts;
using Arathia.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Arathia.Content.Buffs
{
    public class VoidTaintDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true; // This will make the debuff act as a debuff (not a buff)
            Main.pvpBuff[Type] = true; // This makes it work in PvP
            Main.buffNoSave[Type] = true; // The debuff won't be saved when exiting the game
            Main.buffNoTimeDisplay[Type] = false; // The duration of the debuff will be displayed
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            VoidTaintGlobalNPC globalNPC = npc.GetGlobalNPC<VoidTaintGlobalNPC>();

            // Apply defense reduction only once
            if (!globalNPC.voidTaintApplied)
            {
                globalNPC.originalDefense = npc.defDefense; // Store the original defense value
                npc.defDefense = (int)(npc.defDefense * 0.75f); // Reduce defense
                globalNPC.voidTaintApplied = true; // Mark as applied
            }

            DebuffHelper.DamagePerSecond(npc, buffIndex, 100, globalNPC.damageMultiplier);

            // Spawn dust 50% of the time
            if (Main.rand.NextBool(2))
            {
                DustHelper.SpawnEllipseDust(npc.Center, ModContent.DustType<VoidDust>(), npc.width / 5, npc.width, npc.height, 1f);
            }
        }
    }
}
