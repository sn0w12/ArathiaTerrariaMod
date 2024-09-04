using Arathia.Common.GlobalNPCs;
using Arathia.Content.Dusts;
using Arathia.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Arathia.Content.Items.Accessories
{
    public class StelarusEmbrace : ModItem
    {
        // By declaring these here, changing the values will alter the effect, and the tooltip
        public static readonly int AdditiveDamageBonus = 15;
        public static readonly int MeleeSpeedBonus = 15;
        public static readonly int MeleeCritBonus = 10;
        public static readonly int DefenseBonus = 15;
        public static readonly int HealthBonus = 50;

        // Insert the modifier values into the tooltip localization. More info on this approach can be found on the wiki: https://github.com/tModLoader/tModLoader/wiki/Localization#binding-values-to-localizations
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AdditiveDamageBonus, MeleeSpeedBonus, MeleeCritBonus, DefenseBonus, HealthBonus);

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.accessory = true;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += AdditiveDamageBonus / 100f;
            player.GetAttackSpeed(DamageClass.Melee) += MeleeSpeedBonus / 100f;
            player.GetCritChance(DamageClass.Melee) += MeleeCritBonus;
            player.statLifeMax2 += HealthBonus;
            player.statDefense += DefenseBonus;

            player.GetModPlayer<StelarusPlayer>().holyFlame = true;

            if (!hideVisual && Main.rand.NextBool(50))
            {
                DustHelper.SpawnCircleDust(player.Center, ModContent.DustType<SolarDust>(), Main.rand.Next(2, 6));
            }
        }
    }

    public class StelarusPlayer : ModPlayer
    {
        public bool holyFlame;

        public override void ResetEffects()
        {
            holyFlame = false; // Reset the effect each frame
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (holyFlame && item.DamageType == DamageClass.Melee)
            {
                target.GetGlobalNPC<HolyFlameGlobalNPC>().damageMultiplier = DebuffHelper.GetTotalAdditiveDamage(Player, DamageClass.Generic, DamageClass.Melee);
                target.AddBuff(ModContent.BuffType<Buffs.HolyFlameDebuff>(), 480); // 8 sec
            }
            base.OnHitNPCWithItem(item, target, hit, damageDone);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (holyFlame && proj.DamageType == DamageClass.Melee)
            {
                target.GetGlobalNPC<HolyFlameGlobalNPC>().damageMultiplier = DebuffHelper.GetTotalAdditiveDamage(Player, DamageClass.Generic, DamageClass.Melee);
                target.AddBuff(ModContent.BuffType<Buffs.HolyFlameDebuff>(), 240); // 4 sec
            }
            base.OnHitNPCWithProj(proj, target, hit, damageDone);
        }
    }
}