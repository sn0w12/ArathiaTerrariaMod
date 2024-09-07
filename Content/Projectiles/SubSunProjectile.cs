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
	public class SubSunProjectile : ModProjectile
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
		private float rotationMultiplier = 1f;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 100;
			Projectile.height = 100;

			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.light = 0.3f;
			Projectile.timeLeft = 240;
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
			float maxDetectRadius = 1000f; // The maximum radius at which a projectile can detect a target
			float accelerationRate = 1.025f;

			// A short delay to homing behavior after being fired
			if (DelayTimer < 10)
			{
				DelayTimer += 1;
				if (DelayTimer < 5)
					return;
			}
			else
			{
				Projectile.friendly = true;
			}

			// First, we find a homing target if we don't have one
			HomingTarget = ProjectileHelper.FindValidTargetPreferBoss(Projectile, maxDetectRadius, HomingTarget);

			// If we don't have a target, don't adjust trajectory
			if (HomingTarget == null)
				return;

			if (rotationMultiplier < 1.5f)
			{
				rotationMultiplier *= accelerationRate;
			}

			float length = Projectile.velocity.Length();
			float targetAngle = Projectile.AngleTo(HomingTarget.Center);
			Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(4 * rotationMultiplier)).ToRotationVector2() * length;
		}

		public override void OnKill(int timeLeft)
		{
			ProjectileHelper.CreateExplosion(Projectile, ModContent.DustType<SolarDust>(), SoundID.Item62, DamageClass.Magic, 300f, 15f);
		}
	}
}