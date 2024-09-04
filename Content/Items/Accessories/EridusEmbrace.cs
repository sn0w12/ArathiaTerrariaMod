using Arathia.Utilities;
using Arathia.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Arathia.Content.Items.Accessories
{
    public class EridusEmbrace : ModItem
    {
        public static readonly int AggroBonus = 15;
        public static readonly int ManaCostBonus = 10;

        // Insert the modifier values into the tooltip localization. More info on this approach can be found on the wiki: https://github.com/tModLoader/tModLoader/wiki/Localization#binding-values-to-localizations
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AggroBonus, ManaCostBonus);
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.accessory = true;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(gold: 10);
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.aggro -= AggroBonus;
            player.manaCost -= StatHelper.ConvertToOneXFormat(ManaCostBonus);
            player.GetModPlayer<EridusPlayer>().voidTaint = true;
        }
    }

    public class EridusPlayer : ModPlayer
    {
        public bool voidTaint;

        public override void ResetEffects()
        {
            voidTaint = false; // Reset the effect each frame
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (voidTaint)
            {
                target.AddBuff(ModContent.BuffType<Buffs.VoidTaintDebuff>(), 480); // 8 sec
            }
            base.OnHitNPCWithItem(item, target, hit, damageDone);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (voidTaint)
            {
                target.AddBuff(ModContent.BuffType<Buffs.VoidTaintDebuff>(), 480); // 8 sec
            }
            base.OnHitNPCWithProj(proj, target, hit, damageDone);
        }
    }
}
