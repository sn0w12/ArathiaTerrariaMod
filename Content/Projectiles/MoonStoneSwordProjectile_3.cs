using Terraria.ModLoader;

namespace Arathia.Content.Projectiles
{
    public class MoonStoneSwordProjectile_3 : BaseMoonStoneSwordProjectile
    {
        public override string Texture => "Arathia/Content/Items/Weapons/MoonStoneSword_3";
        public override void SetDefaults()
        {
            base.SetDefaults();
            projectileType = ModContent.ProjectileType<MoonStoneProjectile2>();
            projectileVelocityMultiplier = 1.5f;
        }
    }
}