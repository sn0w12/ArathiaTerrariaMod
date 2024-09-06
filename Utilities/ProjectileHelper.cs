using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Arathia.Utilities
{
    public static class ProjectileHelper
    {
        /// <summary>
        /// Checks if the specified NPC is a valid target for the projectile.
        /// The target must be active, hostile, can take damage, and must not have solid tiles blocking the line of sight.
        /// </summary>
        public static bool IsValidTarget(Projectile projectile, NPC target)
        {
            // This method checks that the NPC is:
            // 1. active (alive)
            // 2. chaseable (e.g. not a cultist archer)
            // 3. max life bigger than 5 (e.g. not a critter)
            // 4. can take damage (e.g. moonlord core after all it's parts are downed)
            // 5. hostile (!friendly)
            // 6. not immortal (e.g. not a target dummy)
            // 7. doesn't have solid tiles blocking a line of sight between the projectile and NPC
            return target.CanBeChasedBy() && Collision.CanHit(projectile.Center, 1, 1, target.position, target.width, target.height);
        }

        /// <summary>
        /// Finds the closest valid NPC to the projectile within the specified maximum detection distance.
        /// </summary>
        /// <returns>The closest NPC if found, otherwise returns null.</returns>
        public static NPC FindClosestNPC(Projectile projectile, float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs
            foreach (var target in Main.ActiveNPCs)
            {
                // Check if NPC able to be targeted.
                if (IsValidTarget(projectile, target))
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, projectile.Center);

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

        public static NPC FindClosestNPCPreferBoss(Projectile projectile, float maxDetectDistance)
        {
            NPC closestNPC = null;
            NPC closestBoss = null;

            // Using squared values in distance checks to avoid expensive square root calculations.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;
            float sqrMaxBossDetectDistance = sqrMaxDetectDistance; // Separate distance for boss detection.

            // Loop through all NPCs
            foreach (var target in Main.ActiveNPCs)
            {
                // Check if NPC can be targeted.
                if (IsValidTarget(projectile, target))
                {
                    // Calculate squared distance between projectile and target
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, projectile.Center);

                    // If this NPC is a boss, prefer it over other NPCs
                    if (target.boss)
                    {
                        if (sqrDistanceToTarget < sqrMaxBossDetectDistance)
                        {
                            sqrMaxBossDetectDistance = sqrDistanceToTarget;
                            closestBoss = target;
                        }
                    }
                    else
                    {
                        // Check if it is within the radius and closer than the current closest NPC
                        if (sqrDistanceToTarget < sqrMaxDetectDistance)
                        {
                            sqrMaxDetectDistance = sqrDistanceToTarget;
                            closestNPC = target;
                        }
                    }
                }
            }

            // Prefer the closest boss if found, otherwise return the closest regular NPC
            return closestBoss ?? closestNPC;
        }

        /// <summary>
        /// Finds the closest valid NPC target for homing purposes.
        /// If the current homing target becomes invalid, it returns null.
        /// </summary>
        public static NPC FindValidTarget(Projectile projectile, float maxDetectDistance)
        {
            NPC HomingTarget = FindClosestNPC(projectile, maxDetectDistance);

            // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
            if (HomingTarget != null && !IsValidTarget(projectile, HomingTarget))
            {
                HomingTarget = null;
            }

            return HomingTarget;
        }

        /// <summary>
        /// Finds the closest valid NPC or boss target for homing purposes, preferring bosses if available.
        /// If the current homing target becomes invalid, it returns null.
        /// </summary>
        public static NPC FindValidTargetPreferBoss(Projectile projectile, float maxDetectDistance)
        {
            NPC HomingTarget = FindClosestNPCPreferBoss(projectile, maxDetectDistance);

            // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
            if (HomingTarget != null && !IsValidTarget(projectile, HomingTarget))
            {
                HomingTarget = null;
            }

            return HomingTarget;
        }

        /// <summary>
        /// Draws a trail behind the projectile using a texture, with each trail segment fading out and scaling down over time.
        /// </summary>
        public static void DrawImageTrail(Projectile projectile, Texture2D texture, Color lightColor, int drawEvery = 1)
        {
            // Ensure we don't try to access more old positions than available
            int maxTrailLength = projectile.oldPos.Length;

            // Get the origin of the texture (center of the texture)
            Vector2 origin = texture.Size() / 2f;

            // Loop to draw each trail segment
            for (int i = 0; i < maxTrailLength; i += drawEvery)
            {
                // Calculate the position for this trail segment
                Vector2 trailPosition = projectile.oldPos[i] - Main.screenPosition + origin;

                // Calculate the opacity for this segment (fades out towards the end of the trail)
                float opacity = 0.5f * (1f - (float)i / maxTrailLength);
                float scale = projectile.scale * (1f - 0.1f * ((float)i / maxTrailLength));

                // Draw the projectile's trail segment with the calculated opacity
                Main.spriteBatch.Draw(
                    texture,
                    trailPosition,
                    null,
                    lightColor * opacity,
                    projectile.rotation,
                    origin,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
        }

        /// <summary>
        /// Creates a dust trail behind the projectile using the specified dust ID and optional velocity multiplier and dust scale.
        /// </summary>
        public static void DrawDustTrail(Projectile projectile, Texture2D texture, int dustId, float velocityMultiplier = 1f, float dustScale = 0f)
        {
            Vector2 firstTrailPosition = projectile.oldPos[0] + texture.Size() / 2f;
            float randomHeightOffset = Main.rand.NextFloat(-texture.Height / 2f, texture.Height / 2f);

            // Create a vector representing the offset from the center of the projectile
            Vector2 offset = new Vector2(-Math.Abs(randomHeightOffset) * 0.5f, randomHeightOffset);
            // Rotate the offset vector by the projectile's rotation
            Vector2 rotatedOffset = offset.RotatedBy(projectile.rotation);
            firstTrailPosition += rotatedOffset;

            // Adjust the dust velocity to shoot backward
            Vector2 dustVelocity = -(projectile.velocity * velocityMultiplier);

            // Create the dust
            Dust dust = Dust.NewDustPerfect(firstTrailPosition, dustId, dustVelocity);
            dust.noGravity = true;
            dust.fadeIn = 0.75f;
            if (dustScale > 0)
            {
                dust.scale = dustScale;
            }
        }

        /// <summary>
        /// Creates an explosion at the projectile's position, spawning visual effects (dust) and applying damage to nearby NPCs within the explosion radius.
        /// </summary>
        public static void CreateExplosion(Projectile projectile, int dustId, SoundStyle sound, DamageClass damageType, float explosionRadius = 100f, float speed = 7.5f, float dustScaleMultiplier = 1.5f)
        {
            // Spawn visual effects (dust, sound, etc.)
            if (dustId != 0)
            {
                DustHelper.SpawnCircleDust(projectile.Center, dustId, (int)explosionRadius * 2, (int)explosionRadius / 10, speed, dustScaleMultiplier);
            }
            SoundEngine.PlaySound(sound, projectile.position);

            // Damage all nearby NPCs within the explosion radius
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (IsValidTarget(projectile, npc))
                {
                    // Calculate the distance from the projectile to the NPC
                    float distance = Vector2.Distance(projectile.Center, npc.Center);

                    // If the NPC is within the explosion radius, apply damage
                    if (distance <= explosionRadius)
                    {
                        // Create a HitInfo object to specify the damage details
                        NPC.HitInfo hitInfo = new()
                        {
                            Damage = projectile.damage,
                            DamageType = damageType,
                            Knockback = 5f,
                            HitDirection = (projectile.Center.X < npc.Center.X) ? 1 : -1,
                        };

                        // Apply the damage to the NPC
                        npc.StrikeNPC(hitInfo);
                    }
                }
            }
        }
    }
}