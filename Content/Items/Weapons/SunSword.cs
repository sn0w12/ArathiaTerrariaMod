using Arathia.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Arathia.Content.Dusts;
using System;

namespace Arathia.Content.Items.Weapons
{
	public class SunSword : ModItem
	{

		public override void SetDefaults()
		{
			int useTime = 20;

			Item.width = 50;
			Item.height = 64;

			Item.useStyle = ItemUseStyleID.Swing; // The useStyle of the Item.
			Item.useTime = useTime; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
			Item.useAnimation = useTime; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
			Item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button.

			Item.DamageType = DamageClass.Melee; // Whether your item is part of the melee class.
			Item.damage = 250; // The damage your item deals.
			Item.knockBack = 6; // The force of knockback of the weapon. Maximum is 20
			Item.crit = 6; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.
			Item.shoot = ModContent.ProjectileType<RadianceMainProjectile>();
			Item.shootSpeed = 15;

			Item.value = Item.buyPrice(gold: 1); // The value of the weapon in copper coins.
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item1; // The sound when the weapon is being used.
		}

		public override bool? UseItem(Player player)
		{
			// Set the total number of dust particles to generate
			int dustAmount = 20;
			float towardsMouseRatio = 0.75f; // 75% towards mouse, 25% random

			// Get mouse position in world coordinates
			Vector2 mousePosition = Main.MouseWorld;

			// Loop to generate dust particles
			for (int i = 0; i < dustAmount; i++)
			{
				Vector2 dustPosition = player.Center;
				Vector2 dustVelocity;
				Vector2 mouseDirection;
				Vector2 dustDirection;

				if (i < dustAmount * towardsMouseRatio)
				{
					// Direction towards the mouse
					mouseDirection = mousePosition - dustPosition;
					mouseDirection.Normalize();
					dustDirection = mouseDirection.RotatedByRandom(MathHelper.ToRadians(30));
				}
				else
				{
					// Direction away from the mouse
					mouseDirection = dustPosition - mousePosition;
					mouseDirection.Normalize();
					dustDirection = mouseDirection.RotatedByRandom(MathHelper.ToRadians(15));
				}

				// Calculate the angle between the mouse direction and the dust direction
				float angleInRadians = (float)Math.Acos(Vector2.Dot(mouseDirection, dustDirection));
				float angleInDegrees = MathHelper.ToDegrees(angleInRadians);

				// Calculate speed based on the angle, with a sharper decrease as the angle increases
				float baseSpeed = Main.rand.NextFloat(1.5f, 3f);
				float speedMultiplier = MathHelper.Lerp(1f, 0.75f, angleInDegrees / 30f);
				//Main.NewText($"Speed Multiplier: {speedMultiplier}, Angle Degrees: {angleInDegrees}");
				float speed = baseSpeed * speedMultiplier;

				// Apply the calculated speed to the dust velocity
				dustVelocity = dustDirection * speed;

				// Create the dust
				Dust dust = Dust.NewDustPerfect(dustPosition, ModContent.DustType<SolarDust>(), dustVelocity);
				dust.noGravity = true; // Make the dust not affected by gravity
				dust.scale = 1.5f; // Scale the dust
				dust.fadeIn = 0.5f; // Make the dust fade in
			}

			return true;
		}

		public override bool MeleePrefix() => true;

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}