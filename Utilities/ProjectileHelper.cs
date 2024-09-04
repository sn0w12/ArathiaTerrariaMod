using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Arathia.Utilities
{
    public static class ProjectileHelper
    {
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
    }
}