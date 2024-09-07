using System;
using Arathia.Content.Dusts;
using Arathia.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace Arathia.Content.Projectiles
{
    public class RadianceMainProjectile : ModProjectile
    {
        // Store the target NPC using Projectile.ai[0]
        private NPC HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public ref float DelayTimer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // how long you want the trail to be
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 90;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Get the texture of the projectile
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            ProjectileHelper.DrawImageTrail(Projectile, texture, lightColor);

            // Continue with the normal drawing (to draw the main projectile itself)
            return true;
        }

        private int summonCooldown;
        private int totalSummonedProjectiles = 0;
        private int maxSummonedProjectiles = 3;
        private int npcHits = 0;

        // Custom AI
        public override void AI()
        {
            float r = 244f / 255f;
            float g = 164f / 255f;
            float b = 44f / 255f;

            // Add light at the projectile's position with the desired color
            Lighting.AddLight(Projectile.position, r, g, b);

            float maxDetectRadius = 400f; // The maximum radius at which a projectile can detect a target
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (totalSummonedProjectiles >= maxSummonedProjectiles || npcHits > 5)
            {
                Projectile.timeLeft = 0;
            }

            // A short delay to homing behavior after being fired
            if (DelayTimer < 10)
            {
                DelayTimer += 1;
                return;
            }

            if (summonCooldown > 0)
            {
                summonCooldown--;
            }

            // First, we find a homing target if we don't have one
            HomingTarget = ProjectileHelper.FindValidTarget(Projectile, maxDetectRadius, HomingTarget);
            // If we don't have a target, don't adjust trajectory
            if (HomingTarget == null)
            {
                return;
            }

            // Only summon a new projectile if the cooldown has expired
            if (summonCooldown <= 0)
            {
                // Summon a new RadianceProjectile
                int newProjectileId = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Projectile.velocity *= 1.25f,
                    ModContent.ProjectileType<RadianceProjectile>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Main.myPlayer
                );
                Projectile newProjectile = Main.projectile[newProjectileId];

                // Cast to RadianceProjectile and set the homing target
                if (newProjectile.ModProjectile is RadianceProjectile radianceProjectile)
                {
                    radianceProjectile.SetHomingTarget(HomingTarget);
                }

                float length = newProjectile.velocity.Length();
                float targetAngle = newProjectile.AngleTo(HomingTarget.Center);
                newProjectile.velocity = newProjectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(30)).ToRotationVector2() * length;
                newProjectile.rotation = Projectile.velocity.ToRotation();

                DustHelper.SpawnCircleDust(Projectile.Center, ModContent.DustType<SolarDust>(), 50, 25);
                SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
                // Reset the cooldown (e.g., 60 ticks = 1 second cooldown)
                summonCooldown = 30;
                Projectile.Opacity *= 0.666f;
                totalSummonedProjectiles++;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            npcHits++;
            if (npcHits % 2 == 0)
            {
                maxSummonedProjectiles--;
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft)
        {
            if (totalSummonedProjectiles > 0)
            {
                ProjectileHelper.CreateExplosion(Projectile, ModContent.DustType<SolarDust>(), SoundID.Item74, DamageClass.Generic);
            }
            else
            {
                DustHelper.SpawnCircleDust(Projectile.Center, ModContent.DustType<SolarDust>(), 50, 5, 2f, 0.5f);
            }
        }
    }
}