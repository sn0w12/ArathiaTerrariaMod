using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using System;

namespace Arathia.Content.Dusts
{
    public class TsukiDust : ModDust
    {
        public override string Texture => null;
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.scale = 1.2f;

            //float speed = Main.rand.NextFloat(2f, 4f); // Random speed between 0.5 and 2
            float speed = 100f;
            float angle = Main.rand.NextFloat(MathHelper.TwoPi); // Random angle in radians
            dust.velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

            int desiredVanillaDustTexture = 176;
            int frameX = desiredVanillaDustTexture * 10 % 1000;
            int frameY = desiredVanillaDustTexture * 10 / 1000 * 30 + Main.rand.Next(3) * 10;
            dust.frame = new Rectangle(frameX, frameY, 8, 8);
        }

        public override bool Update(Dust dust)
        {
            dust.rotation += 0.05f; // Add some rotation to the dust
            dust.scale *= 0.985f; // Make the dust shrink over time
            dust.velocity *= 0.985f;

            float fadeSpeed = 10f * (1f - dust.scale);
            dust.alpha += (int)fadeSpeed;

            // Kill the dust when it's too small or invisible
            if (dust.scale < 0.25f)
            {
                dust.active = false;
            }

            return false; // Return false to prevent vanilla dust behavior
        }
    }
}
