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
            Projectile.width = 60; // The width of projectile hitbox
            Projectile.height = 30; // The height of projectile hitbox

            Projectile.DamageType = DamageClass.Melee; // What type of damage does this projectile affect?
            Projectile.penetrate = 3;
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Get the texture of the projectile
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            // Number of trail segments (more segments = longer and smoother trail)
            int trailLength = 10;

            // Ensure we don't try to access more old positions than available
            int maxTrailLength = Math.Min(trailLength, Projectile.oldPos.Length);

            // Loop to draw each trail segment
            for (int i = 0; i < maxTrailLength; i++)
            {
                // Calculate the position for this trail segment
                Vector2 trailPosition = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;

                // Calculate the opacity for this segment (fades out towards the end of the trail)
                float opacity = 0.5f * (1f - (float)i / maxTrailLength);
                float scale = Projectile.scale * (1f - 0.15f * ((float)i / maxTrailLength));

                // Draw the projectile's trail segment with the calculated opacity
                Main.spriteBatch.Draw(
                    texture,
                    trailPosition,
                    null,
                    lightColor * opacity,
                    Projectile.rotation,
                    texture.Size() / 2f,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }

            // Continue with the normal drawing (to draw the main projectile itself)
            return true;
        }

        private int summonCooldown;
        private int totalSummonedProjectiles = 0;
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

            if (totalSummonedProjectiles > 2)
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
            if (HomingTarget == null)
            {
                HomingTarget = FindClosestNPC(maxDetectRadius);
            }

            // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

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
                Projectile.Opacity *= 0.666f;
                totalSummonedProjectiles++;
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs
            foreach (var target in Main.ActiveNPCs)
            {
                // Check if NPC able to be targeted.
                if (IsValidTarget(target))
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public bool IsValidTarget(NPC target)
        {
            // This method checks that the NPC is:
            // 1. active (alive)
            // 2. chaseable (e.g. not a cultist archer)
            // 3. max life bigger than 5 (e.g. not a critter)
            // 4. can take damage (e.g. moonlord core after all it's parts are downed)
            // 5. hostile (!friendly)
            // 6. not immortal (e.g. not a target dummy)
            // 7. doesn't have solid tiles blocking a line of sight between the projectile and NPC
            return target.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 1, 1, target.position, target.width, target.height);
        }

        public override void OnKill(int timeLeft)
        {
            // Spawn visual effects (dust, sound, etc.)
            DustHelper.SpawnCircleDust(Projectile.Center, ModContent.DustType<SolarDust>(), 200, 10, 7.5f, 1.5f);
            SoundEngine.PlaySound(SoundID.Item74, Projectile.position);

            float explosionRadius = 100f;
            // Damage all nearby NPCs within the explosion radius
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (IsValidTarget(npc))
                {
                    // Calculate the distance from the projectile to the NPC
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);

                    // If the NPC is within the explosion radius, apply damage
                    if (distance <= explosionRadius)
                    {
                        // Create a HitInfo object to specify the damage details
                        NPC.HitInfo hitInfo = new()
                        {
                            Damage = Projectile.damage,
                            Knockback = 5f,
                            HitDirection = (Projectile.Center.X < npc.Center.X) ? 1 : -1,
                            Crit = Projectile.CritChance > 0
                        };

                        // Apply the damage to the NPC
                        npc.StrikeNPC(hitInfo);
                    }
                }
            }
        }
    }
}