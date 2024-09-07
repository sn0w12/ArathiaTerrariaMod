using Arathia.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Arathia.Content.Dusts;
using System;
using Arathia.Content.Items.Materials;
using Terraria.DataStructures;
using Arathia.Utilities;

namespace Arathia.Content.Items.Weapons
{
    public class MoonSword : ModItem
    {
        static int randomRotation = 45;
        public override void SetDefaults()
        {
            int useTime = 35;

            Item.width = 68;
            Item.height = 68;

            Item.useStyle = ItemUseStyleID.Swing; // The useStyle of the Item.
            Item.useTime = useTime; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
            Item.useAnimation = useTime; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
            Item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button.

            Item.DamageType = DamageClass.Melee; // Whether your item is part of the melee class.
            Item.damage = 70; // The damage your item deals.
            Item.knockBack = 4; // The force of knockback of the weapon. Maximum is 20
            Item.shoot = ModContent.ProjectileType<SmallMoonProjectile>();
            Item.shootSpeed = 10;

            Item.value = Item.buyPrice(gold: 1); // The value of the weapon in copper coins.
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item1; // The sound when the weapon is being used.
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 direction = Main.MouseWorld - player.Center;
            direction.Normalize();
            float baseAngle = (float)Math.Atan2(direction.Y, direction.X);

            // Define the total cone spread and number of projectiles
            float coneSpread = MathHelper.ToRadians(45f); // 45-degree cone
            int numProjectiles = 4; // Number of projectiles

            // Shoot multiple projectiles in the cone
            for (int i = 0; i < numProjectiles; i++)
            {
                // Calculate the angle for each projectile, evenly spaced within the cone
                float angleOffset = MathHelper.Lerp(-coneSpread / 2, coneSpread / 2, i / (float)(numProjectiles - 1));
                float currentAngle = baseAngle + angleOffset;

                // Calculate velocity for each projectile based on the angle
                Vector2 shootDirection = new Vector2((float)Math.Cos(currentAngle), (float)Math.Sin(currentAngle)) * velocity.Length();
                int projectileId = ProjectileHelper.ShootProjectile(player.GetSource_FromThis(), position, damage, knockback, type, shootDirection.Length(), MathHelper.ToDegrees(currentAngle));
                Main.projectile[projectileId].rotation = MathHelper.ToRadians(Main.rand.Next(-randomRotation, randomRotation + 1));
            }

            return false; // Prevent the vanilla projectile from being shot
        }


        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<TsukiBar>(15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}