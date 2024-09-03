using Arathia.Common.GlobalNPCs;
using Arathia.Content.Dusts;
using Arathia.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Arathia.Content.Buffs
{
    public class HolyFlameDebuff : ModBuff
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
            // Retrieve the stored melee damage multiplier from the NPC's global data
            float damageMultiplier = npc.GetGlobalNPC<HolyFlameGlobalNPC>().damageMultiplier;
            DebuffHelper.DamagePerSecond(npc, buffIndex, 400, damageMultiplier);

            if (Main.rand.NextBool(4)) // 25% chance per tick to spawn dust
            {
                DustHelper.SpawnEllipseDust(npc.Center, ModContent.DustType<HolyFlameDust>(), npc.width / 2, npc.width, npc.height, 0);
            }
        }
    }
}
