using System;
using Arathia.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Arathia.Utilities
{
    public static class DustHelper
    {
        /// <summary>
        /// Calculates the light color based on the original RGB values and the dust's alpha.
        /// </summary>
        /// <param name="r">Original red component (0-255).</param>
        /// <param name="g">Original green component (0-255).</param>
        /// <param name="b">Original blue component (0-255).</param>
        /// <param name="alpha">Alpha value (0-255) where 0 is fully opaque and 255 is fully transparent.</param>
        /// <returns>A Vector3 representing the RGB values adjusted for light intensity.</returns>
        public static Vector3 CalculateLightColor(float r, float g, float b, int alpha)
        {
            // Normalize the RGB values
            float normalizedR = r / 255f;
            float normalizedG = g / 255f;
            float normalizedB = b / 255f;

            // Calculate light intensity based on alpha
            float lightIntensity = 1f - (alpha / 255f);

            // Return the adjusted RGB values as a Vector3
            return new Vector3(normalizedR * lightIntensity, normalizedG * lightIntensity, normalizedB * lightIntensity);
        }

        public static (int frameX, int frameY) GetVanillaTexture(int dustId)
        {
            int desiredVanillaDustTexture = dustId;
            int frameX = desiredVanillaDustTexture * 10 % 1000;
            int frameY = desiredVanillaDustTexture * 10 / 1000 * 30 + Main.rand.Next(3) * 10;
            return (frameX, frameY);
        }

        private static void SpawnDustInternal(Vector2 position, int type, int limit, float radiusX, float radiusY, float speed = 2.5f, float scaleMultiplier = 1f, bool noGravity = true, float angle = 0f, float coneWidth = 360f)
        {
            // Convert the cone width from degrees to radians
            float halfConeWidthRadians = MathHelper.ToRadians(coneWidth / 2f);

            for (int i = 0; i < limit; i++)
            {
                // Generate a random position around the center
                Vector2 randomOffset = Main.rand.NextVector2Circular(radiusX, radiusY);
                Vector2 dustPosition = position + randomOffset;

                // Create a new dust at the random position
                Dust dust = Dust.NewDustPerfect(dustPosition, type);

                // Generate a random angle within the specified cone
                float randomAngle = angle + Main.rand.NextFloat(-halfConeWidthRadians, halfConeWidthRadians);

                // Create a direction vector based on the random angle
                Vector2 direction = new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle));

                // Set the dust velocity in the specified cone direction, with a random speed
                if (speed > 0)
                {
                    float finalSpeed = Main.rand.NextFloat(speed * 0.75f, speed * 1.25f);
                    if (coneWidth < 360f)
                    {
                        Vector2 originalDirection = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                        // Calculate the angle between the mouse direction and the dust direction
                        float angleInRadians = (float)Math.Acos(Vector2.Dot(originalDirection, direction));

                        // Calculate speed based on the angle, with a sharper decrease as the angle increases
                        float speedMultiplier = MathHelper.Lerp(1f, 0.75f, angleInRadians);
                        //Main.NewText($"Speed Multiplier: {speedMultiplier}, Angle Degrees: {angleInDegrees}");
                        finalSpeed *= speedMultiplier;
                    }
                    dust.velocity = direction * finalSpeed;
                }

                // Set the scale and other dust properties
                dust.scale *= Main.rand.NextFloat(scaleMultiplier * 0.9f, scaleMultiplier * 1.1f);
                dust.noGravity = noGravity;
                dust.fadeIn = 0.75f;
            }
        }

        /// <summary>
        /// Spawns a specified number of dust particles in a circular pattern around a given position.
        /// </summary>
        /// <param name="position">The center position around which the dust particles will be spawned.</param>
        /// <param name="type">The type of dust to spawn.</param>
        /// <param name="limit">The number of dust particles to spawn.</param>
        /// <param name="radius">The radius of the circle in which the dust particles will be distributed. Defaults to 20f.</param>
        /// <param name="speed">The base speed of the dust particles. Defaults to 2.5f.</param>
        /// <param name="scaleMultiplier">Multiplier for the dust particle scale. Defaults to 1f.</param>
        /// <param name="noGravity">Whether the dust particles are affected by gravity. Defaults to true.</param>
        public static void SpawnCircleDust(Vector2 position, int type, int limit, float radius = 20f, float speed = 2.5f, float scaleMultiplier = 1f, bool noGravity = true)
        {
            SpawnDustInternal(position, type, limit, radius, radius, speed, scaleMultiplier, noGravity);
        }

        /// <summary>
        /// Spawns a specified number of dust particles in an elliptical pattern around a given position.
        /// </summary>
        /// <param name="position">The center position around which the dust particles will be spawned.</param>
        /// <param name="type">The type of dust to spawn.</param>
        /// <param name="limit">The number of dust particles to spawn.</param>
        /// <param name="width">The width of the ellipse in which the dust particles will be distributed. Defaults to 20f.</param>
        /// <param name="height">The height of the ellipse in which the dust particles will be distributed. Defaults to 20f.</param>
        /// <param name="speed">The base speed of the dust particles. Defaults to 2.5f.</param>
        /// <param name="scaleMultiplier">Multiplier for the dust particle scale. Defaults to 1f.</param>
        /// <param name="noGravity">Whether the dust particles are affected by gravity. Defaults to true.</param>
        public static void SpawnEllipseDust(Vector2 position, int type, int limit, float width = 20f, float height = 20f, float speed = 2.5f, float scaleMultiplier = 1f, bool noGravity = true)
        {
            SpawnDustInternal(position, type, limit, width / 2f, height / 2f, speed, scaleMultiplier, noGravity);
        }

        /// <summary>
        /// Spawns a specified number of dust particles in a cone pattern around a given position pointing in a given angle.
        /// </summary>
        public static void SpawnConeDust(Vector2 position, int type, int limit, float radius = 20f, float angle = 0f, float coneWidth = 360f, float speed = 2.5f, float scaleMultiplier = 1f, bool noGravity = true)
        {
            SpawnDustInternal(position, type, limit, radius, radius, speed, scaleMultiplier, noGravity, MathHelper.ToRadians(angle), coneWidth);
        }
    }
}
