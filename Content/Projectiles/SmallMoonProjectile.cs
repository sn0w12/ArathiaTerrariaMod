using Arathia.Content.Dusts;
using Arathia.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Arathia.Content.Projectiles
{
    // This Example show how to implement simple homing projectile
    // Can be tested with ExampleCustomAmmoGun
    public class SmallMoonProjectile : ModProjectile
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
        private float rotationMultiplier = 0.1f;

        public ref float DelayTimer => ref Projectile.ai[1];
        public ref float State => ref Projectile.ai[2]; // Use ai[2] for state: 0 = slow down, 1 = homing

        private const float StopThreshold = 0.1f; // The speed at which we consider the projectile to have stopped
        private const float HomingAcceleration = 0.3f; // The acceleration rate after switching to homing

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.light = 0.3f;
            Projectile.timeLeft = 240;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Get the texture of the projectile
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            ProjectileHelper.DrawImageTrail(Projectile, texture, lightColor);

            // Continue with the normal drawing (to draw the main projectile itself)
            return true;
        }

        public override void AI()
        {
            const float waitTimeBeforeHoming = 30f;

            if (State == 0) // Phase 1: Slow down until it stops
            {
                Projectile.velocity *= 0.9f; // Reduce speed gradually

                if (Projectile.velocity.Length() < StopThreshold)
                {
                    Projectile.velocity = Vector2.Zero; // Stop completely

                    // Use DelayTimer to track the waiting period before switching to State 1
                    DelayTimer++;
                    if (DelayTimer >= waitTimeBeforeHoming)
                    {
                        State = 1; // Switch to homing state after waiting
                        DelayTimer = 0; // Reset the timer
                    }
                }
            }
            else if (State == 1) // Phase 2: Homing towards a target and speeding up
            {
                float maxDetectRadius = 800f;
                HomingTarget = ProjectileHelper.FindValidTarget(Projectile, maxDetectRadius, HomingTarget);

                if (HomingTarget == null || !HomingTarget.active)
                {
                    Projectile.velocity *= 0.95f;
                    return;
                }

                float targetAngle = Projectile.AngleTo(HomingTarget.Center);

                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(5)).ToRotationVector2() * Projectile.velocity.Length();
                Projectile.velocity += Projectile.DirectionTo(HomingTarget.Center) * HomingAcceleration;
                Projectile.velocity = Vector2.Clamp(Projectile.velocity, -new Vector2(15f), new Vector2(15f));
            }
        }

        public override void OnKill(int timeLeft)
        {
            DustHelper.SpawnCircleDust(Projectile.Center, DustID.Stone, 50, 5);
        }
    }
}