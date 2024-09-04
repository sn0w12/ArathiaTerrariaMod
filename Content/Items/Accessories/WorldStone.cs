using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Arathia.Utilities;

namespace Arathia.Content.Items.Accessories
{
    public class WorldStone : ModItem
    {
        // Declaring the buff values for easy adjustment
        public static readonly float AllDamageBonus = 0.25f; // 25% All Damage
        public static readonly int DefenseBonus = 15;
        public static readonly int MinionCapacityBonus = 3;
        public static readonly int SentryCapacityBonus = 1;
        public static readonly int ManaBonus = 40;
        public static readonly int AmmoConsumeReduction = 20;
        public static readonly float MovementSpeedBonus = 20; // 20% Movement Speed

        // Tooltip binding using WithFormatArgs for dynamic values
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AllDamageBonus * 100, DefenseBonus, MinionCapacityBonus, SentryCapacityBonus, ManaBonus, AmmoConsumeReduction, MovementSpeedBonus);

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(platinum: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = player.GetModPlayer<ArathiaPlayer>();
            modPlayer.worldStoneEquipped = true;

            // Apply the buffs to the player
            player.GetDamage(DamageClass.Generic) += AllDamageBonus; // All Damage +25%
            player.statDefense += DefenseBonus; // Defense +15
            player.maxMinions += MinionCapacityBonus; // Minion Capacity +3
            player.maxTurrets += SentryCapacityBonus; // Sentry Capacity +1
            player.statManaMax2 += ManaBonus; // Maximum Mana +40
            player.ammoCost80 = true;
            player.GetModPlayer<WorldStonePlayer>().worldStone = true;

            if (!hideVisual && Main.rand.NextBool(10))
            {
                SpawnDust(player);
            }
        }

        public override void AddRecipes()
        {
            // Optional: Add a recipe for the item
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddIngredient(ItemID.FragmentSolar, 5);
            recipe.AddIngredient(ItemID.FragmentNebula, 5);
            recipe.AddIngredient(ItemID.FragmentVortex, 5);
            recipe.AddIngredient(ItemID.FragmentStardust, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }

        private void SpawnDust(Player player)
        {
            List<Color> dustColors =
            [
                Color.Black,
                Color.Purple,
                Color.Pink,
                Color.BlueViolet,
            ];

            // Number of dust particles to spawn per update
            int dustAmount = Main.rand.Next(2, 6);
            float speed = 2f;

            for (int i = 0; i < dustAmount; i++)
            {
                // Generate a random position around the player
                Vector2 randomOffset = Main.rand.NextVector2Circular(16f, 16f);
                Vector2 dustPosition = player.Center + randomOffset;

                // Generate a random direction for the dust to shoot out
                Vector2 randomDirection = Main.rand.NextVector2Unit();

                // Set the dust velocity to the random direction multiplied by the desired speed
                Vector2 dustVelocity = randomDirection * speed;

                Color chosenColor = dustColors[Main.rand.Next(dustColors.Count)];

                // Create the dust at the player's position with the calculated velocity
                Dust dust = Dust.NewDustPerfect(dustPosition, DustID.TintableDustLighted, dustVelocity, 0, chosenColor);
                dust.noGravity = true;
                dust.scale = 1.5f;
                dust.fadeIn = 0.75f;
            }
        }
    }

    public class WorldStonePlayer : ModPlayer
    {
        public bool worldStone = false;

        public override void ResetEffects()
        {
            worldStone = false;
        }

        public override void PostUpdateRunSpeeds()
        {
            // We only want our additional changes to apply if equipped and not on a mount.
            if (Player.mount.Active || !worldStone)
            {
                return;
            }

            float movementSpeedBonus = SolarStone.MovementSpeedBonus;

            // The following modifications are similar to Shadow Armor set bonus
            Player.accRunSpeed *= StatHelper.ConvertToOneXFormat(movementSpeedBonus);
        }
    }
}
