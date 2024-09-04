using System;
using Arathia.Content.Dusts;
using Arathia.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Arathia.Content.Projectiles
{
    public class RadianceProjectile : ModProjectile
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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // how long you want the trail to be
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 46; // The width of projectile hitbox
            Projectile.height = 52; // The height of projectile hitbox

            Projectile.DamageType = DamageClass.Melee; // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false;
            Projectile.timeLeft = 4 * 60; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.penetrate = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Get the texture of the projectile
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            ProjectileHelper.DrawImageTrail(Projectile, texture, lightColor, 2);
            ProjectileHelper.DrawDustTrail(Projectile, texture, ModContent.DustType<SolarDust>(), 0.1f, 1.2f);

            // Continue with the normal drawing (to draw the main projectile itself)
            return true;
        }

        private int maxTurnRadius = 5;
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

            // A short delay to homing behavior after being fired
            if (DelayTimer < 10)
            {
                DelayTimer += 1;
                return;
            }

            // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
            if (HomingTarget != null && !ProjectileHelper.IsValidTarget(Projectile, HomingTarget))
            {
                HomingTarget = null;
            }

            // First, we find a homing target if we don't have one
            if (HomingTarget == null)
            {
                HomingTarget = ProjectileHelper.FindValidTarget(Projectile, maxDetectRadius);
                // If we don't have a target, don't adjust trajectory
                if (HomingTarget == null)
                {
                    return;
                }
            }

            // If found, we rotate the projectile velocity in the direction of the target.
            // We only rotate by 3 degrees an update to give it a smooth trajectory. Increase the rotation speed here to make tighter turns
            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(maxTurnRadius)).ToRotationVector2() * length;
        }

        // Method to set the homing target externally
        public void SetHomingTarget(NPC target)
        {
            if (ProjectileHelper.IsValidTarget(Projectile, target))
            {
                HomingTarget = target;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            maxTurnRadius += 2;
            Projectile.velocity *= 1.05f;

            if (target.life <= 0 && Projectile.penetrate < 6)
            {
                Projectile.penetrate++;
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft)
        {
            DustHelper.SpawnCircleDust(Projectile.Center, ModContent.DustType<SolarDust>(), 50, 50f);
        }
    }
}
