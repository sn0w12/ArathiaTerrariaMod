using Terraria.ModLoader;

namespace Arathia.Content.Projectiles
{
    public class MoonStoneSwordProjectile_2 : BaseMoonStoneSwordProjectile
    {
        public override string Texture => "Arathia/Content/Items/Weapons/MoonStoneSword_2";
        public override void SetDefaults()
        {
            base.SetDefaults();
            projectileType = ModContent.ProjectileType<MoonStoneProjectile2>();
            projectileVelocityMultiplier = 1.5f;
        }
    }
}