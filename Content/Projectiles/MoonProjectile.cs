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
	public class MoonProjectile : ModProjectile
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
		private float speedMultiplier = 0.1f;
		private float rotationMultiplier = 0.1f;

		public ref float DelayTimer => ref Projectile.ai[1];

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
			Projectile.friendly = true;
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
			float maxDetectRadius = 600f; // The maximum radius at which a projectile can detect a target
			float accelerationRate = 1.05f;

			// A short delay to homing behavior after being fired
			if (DelayTimer < 10)
			{
				DelayTimer += 1;
				return;
			}

			// Speed up the projectile slowly at first, then quickly
			if (speedMultiplier < 12f)
			{
				speedMultiplier *= accelerationRate;
				if (rotationMultiplier < 5f)
				{
					rotationMultiplier *= 1 + (accelerationRate - 1) * 0.75f;
				}
			}

			Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speedMultiplier;

			// First, we find a homing target if we don't have one
			if (HomingTarget == null)
			{
				HomingTarget = ProjectileHelper.FindValidTarget(Projectile, maxDetectRadius);
			}

			// If we don't have a target, don't adjust trajectory
			if (HomingTarget == null)
				return;

			// Extra rotation multiplier if target found
			if (rotationMultiplier < 5f)
			{
				rotationMultiplier *= 1 + (accelerationRate - 1) * 0.5f;
			}

			float length = Projectile.velocity.Length();
			float targetAngle = Projectile.AngleTo(HomingTarget.Center);
			Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(5 * (rotationMultiplier / 15))).ToRotationVector2() * length;
		}

		public override void OnKill(int timeLeft)
		{
			// Spawn visual effects (dust, sound, etc.)
			DustHelper.SpawnCircleDust(Projectile.Center, DustID.Stone, 200, 10, 15f, 1.5f);
			SoundEngine.PlaySound(SoundID.Item62, Projectile.position);

			float explosionRadius = 300f;
			// Damage all nearby NPCs within the explosion radius
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (ProjectileHelper.IsValidTarget(Projectile, npc))
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
						};

						// Apply the damage to the NPC
						npc.StrikeNPC(hitInfo);
					}
				}
			}
		}
	}
}