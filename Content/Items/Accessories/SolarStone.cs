using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System;

namespace Arathia.Content.Items.Accessories
{
    public class SolarStone : ModItem
    {
        // Declaring the buff values for easy adjustment
        public static readonly float MeleeDamageBonus = 15;
        public static readonly int DefenseBonus = 7;
        public static readonly float MovementSpeedBonus = 10;

        // Tooltip binding using WithFormatArgs for dynamic values
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeDamageBonus, DefenseBonus, MovementSpeedBonus);

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = player.GetModPlayer<ArathiaPlayer>();
            if (!modPlayer.worldStoneEquipped)
            {
                player.GetDamage(DamageClass.Generic) += MeleeDamageBonus / 100f;
                player.statDefense += DefenseBonus;
                player.GetModPlayer<SolarStoneAccessoryPlayer>().exampleStatBonusAccessory = true;
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddIngredient(ItemID.FragmentSolar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }

    public class SolarStoneAccessoryPlayer : ModPlayer
    {
        public bool exampleStatBonusAccessory = false;

        public override void ResetEffects()
        {
            exampleStatBonusAccessory = false;
        }

        public override void PostUpdateRunSpeeds()
        {
            var modPlayer = Player.GetModPlayer<ArathiaPlayer>();
            // We only want our additional changes to apply if equipped and not on a mount.
            if (Player.mount.Active || !exampleStatBonusAccessory || !modPlayer.worldStoneEquipped)
            {
                return;
            }

            float movementSpeedBonus = SolarStone.MovementSpeedBonus;
            int numberOfDigits = movementSpeedBonus.ToString().Length;
            // Calculate the factor to convert movementSpeedBonus to a decimal < 1
            float factor = (float)Math.Pow(10, numberOfDigits);
            // Convert to 1.x format
            float convertedValue = 1 + (movementSpeedBonus / factor);

            // The following modifications are similar to Shadow Armor set bonus
            Player.accRunSpeed *= convertedValue;
        }
    }
}
