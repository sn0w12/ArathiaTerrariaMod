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
	public class MoonStoneProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 36;
			Projectile.height = 32;

			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.light = 0.3f;
			Projectile.timeLeft = 240;

            Projectile.rotation = 0f;
            Projectile.aiStyle = -1;
		}

        public override void AI()
        {
            Projectile.velocity.Y += 0.2f;
            Projectile.rotation += 0.2f;
        }

		public override void OnKill(int timeLeft)
		{
			ProjectileHelper.CreateExplosion(Projectile, DustID.Stone, SoundID.Dig, DamageClass.Melee, 50f, 15f);
		}
	}
}