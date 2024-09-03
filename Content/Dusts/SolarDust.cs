using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Arathia.Utilities;

namespace Arathia.Content.Dusts
{
    public class SolarDust : ModDust
    {
        public override string Texture => null;
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true; // No gravity
            dust.noLight = false; // Emits light
            dust.scale = 1.2f; // Size of the dust

            int desiredVanillaDustTexture = DustID.GoldFlame;
            int frameX = desiredVanillaDustTexture * 10 % 1000;
            int frameY = desiredVanillaDustTexture * 10 / 1000 * 30 + Main.rand.Next(3) * 10;
            dust.frame = new Rectangle(frameX, frameY, 8, 8);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity; // Move dust
            dust.rotation += 0.05f; // Add some rotation to the dust
            dust.scale *= 0.985f; // Make the dust shrink over time
            dust.velocity *= 0.985f;

            float fadeSpeed = 10f * (1f - dust.scale); // Adjust this multiplier for more control
            dust.alpha += (int)fadeSpeed;

            Vector3 lightColor = DustHelper.CalculateLightColor(244f, 164f, 44f, dust.alpha);
            Lighting.AddLight(dust.position, lightColor.X, lightColor.Y, lightColor.Z);

            // Kill the dust when it's too small or invisible
            if (dust.scale < 0.25f || dust.alpha > 255)
            {
                dust.active = false;
            }

            return false; // Return false to prevent vanilla dust behavior
        }
    }
}
