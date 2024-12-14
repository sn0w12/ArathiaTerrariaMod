using Arathia.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Arathia.Content.Items.Materials;
using Terraria.DataStructures;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Arathia.Content.Items.Weapons
{
    public class MoonStoneSword : ModItem
    {
        public Texture2D[] stageTextures;
        public const int MAX_STAGE = 4;
        public const int STAGE_DECAY_TIME = 300;
        public int currentStage = 0;
        public int hitTimer;
        public Vector3[] colors = [
                new Vector3(0f, 0f, 0f),
                new Vector3(0.18f, 0.18f, 0.25f),
                new Vector3(0.36f, 0.37f, 0.50f),
                new Vector3(0.54f, 0.55f, 0.75f),
                new Vector3(0.73f, 0.74f, 0.99f)
            ];
        public float[] speedMultipliers = [1f, 0.975f, 0.95f, 0.9f, 0.8f];
        private float[] damageMultipliers = [1f, 1.25f, 1.5f, 1.75f, 2f];
        private int[] projectileTypes =
            [
                ModContent.ProjectileType<MoonStoneSwordProjectile_0>(),
                ModContent.ProjectileType<MoonStoneSwordProjectile_1>(),
                ModContent.ProjectileType<MoonStoneSwordProjectile_2>(),
                ModContent.ProjectileType<MoonStoneSwordProjectile_3>(),
                ModContent.ProjectileType<MoonStoneSwordProjectile_4>(),
            ];

        public int attackType = 0;
		public int comboExpireTimer = 0;

        private void LoadTextures()
        {
            if (Main.netMode == NetmodeID.Server) return;

            try
            {
                stageTextures = new Texture2D[MAX_STAGE + 1];
                for (int i = 0; i <= MAX_STAGE; i++)
                {
                    string texturePath = $"{Texture}_{i}";
                    stageTextures[i] = ModContent.Request<Texture2D>(texturePath).Value;
                }
            }
            catch (Exception e)
            {
                Main.NewText($"Failed to load textures: {e.Message}");
            }
        }

        public override void SetDefaults()
        {
            int useTime = 35;

            Item.width = 128;
            Item.height = 128;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = useTime;
            Item.useAnimation = useTime;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 235;
            Item.knockBack = 8;
            Item.shoot = ModContent.ProjectileType<BaseMoonStoneSwordProjectile>();
            Item.shootSpeed = 10;

            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item1;

            Item.noMelee = true;
			Item.noUseGraphic = true;

            LoadTextures();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            damage = (int)(damage * damageMultipliers[currentStage]);

            int projectileType = projectileTypes[currentStage];
            int proj = Projectile.NewProjectile(source, position, velocity, projectileType, damage, knockback, Main.myPlayer, attackType);
            if (Main.projectile[proj].ModProjectile is BaseMoonStoneSwordProjectile swordProj) {
                swordProj.Initialize();
            }
			attackType = (attackType + 1) % 2;
			comboExpireTimer = 0;
			return false;
		}

        public override void UpdateInventory(Player player)
        {
            if (hitTimer > 0)
                hitTimer--;
            else if (currentStage > 0 && Main.GameUpdateCount % 10 == 0) {
                currentStage--;
            }

            if (comboExpireTimer++ >= 120)
				attackType = 0;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (currentStage < MAX_STAGE)
                currentStage++;
            hitTimer = STAGE_DECAY_TIME;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "StageInfo", $"Current Stage: {currentStage + 1}/5"));
        }

        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<TsukiBar>(15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = stageTextures[currentStage];
            Rectangle frame = texture.Frame();
            Vector2 origin = frame.Size() / 2f;
            Vector2 position = Item.Center - Main.screenPosition;

            spriteBatch.Draw(
                texture,
                position,
                frame,
                lightColor,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0
            );

            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (stageTextures != null)
            {
                Texture2D texture = stageTextures[currentStage];
                spriteBatch.Draw(texture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }
    }
}