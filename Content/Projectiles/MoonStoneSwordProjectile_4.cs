using Terraria.ModLoader;

namespace Arathia.Content.Projectiles
{
    public class MoonStoneSwordProjectile_4 : BaseMoonStoneSwordProjectile
    {
        public override string Texture => "Arathia/Content/Items/Weapons/MoonStoneSword_4";
        public override void SetDefaults()
        {
            base.SetDefaults();
            projectileType = ModContent.ProjectileType<MoonStoneProjectile3>();
            projectileVelocityMultiplier = 1.75f;
        }
    }
}