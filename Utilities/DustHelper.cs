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

        private static void SpawnDustInternal(Vector2 position, int type, int limit, float radiusX, float radiusY, float speed = 2.5f, float scaleMultiplier = 1f, bool noGravity = true)
        {
            for (int i = 0; i < limit; i++)
            {
                // Generate a random position around the projectile center
                Vector2 randomOffset = Main.rand.NextVector2Circular(radiusX, radiusY);
                Vector2 dustPosition = position + randomOffset;

                // Create a new dust at the random position
                Dust dust = Dust.NewDustPerfect(dustPosition, type);

                // Generate a random direction for the dust to shoot out
                Vector2 randomDirection = Main.rand.NextVector2Unit();

                // Set the dust velocity to the random direction multiplied by the desired speed
                if (speed > 0)
                {
                    dust.velocity = randomDirection * Main.rand.NextFloat(speed * 0.75f, speed * 1.25f);
                }
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
            // Call the generalized method with the same radius for both axes
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
            // Call the generalized method with different radii for the x and y axes
            SpawnDustInternal(position, type, limit, width / 2f, height / 2f, speed, scaleMultiplier, noGravity);
        }
    }
}
