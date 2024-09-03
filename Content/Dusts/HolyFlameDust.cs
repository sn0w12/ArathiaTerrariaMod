using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Arathia.Utilities;

namespace Arathia.Content.Dusts
{
    public class HolyFlameDust : ModDust
    {
        public override string Texture => null; // Use the same texture as SolarDust

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true; // No gravity, dust will rise
            dust.noLight = false; // Emits light
            dust.scale = 1.2f; // Size of the dust

            // Apply a small initial upward velocity to simulate rising
            dust.velocity = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), -Main.rand.NextFloat(1f, 2f));

            // Set the texture frame to use the same as SolarDust
            int desiredVanillaDustTexture = DustID.GoldFlame;
            int frameX = desiredVanillaDustTexture * 10 % 1000;
            int frameY = desiredVanillaDustTexture * 10 / 1000 * 30 + Main.rand.Next(3) * 10;
            dust.frame = new Rectangle(frameX, frameY, 8, 8);
        }

        public override bool Update(Dust dust)
        {
            // Gradually move the dust upward
            dust.position += dust.velocity;

            // Make the dust slowly rise faster over time
            dust.velocity.Y -= 0.01f;
            dust.velocity *= 0.98f;
            dust.rotation += 0.05f;
            dust.scale *= 0.985f;

            Vector3 lightColor = DustHelper.CalculateLightColor(244f, 164f, 44f, dust.alpha);
            Lighting.AddLight(dust.position, lightColor.X, lightColor.Y, lightColor.Z);

            // Gradually increase the alpha value to make the dust more transparent
            float fadeSpeed = 10f * (1f - dust.scale);
            dust.alpha += (int)fadeSpeed;

            // Kill the dust when it's too small or invisible
            if (dust.scale < 0.25f || dust.alpha > 255)
            {
                dust.active = false;
            }

            return false; // Return false to prevent vanilla dust behavior
        }
    }
}
