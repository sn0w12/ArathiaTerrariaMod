using Terraria.ModLoader;

namespace Arathia.Content.Projectiles
{
    public class MoonStoneSwordProjectile_1 : BaseMoonStoneSwordProjectile
    {
        public override string Texture => "Arathia/Content/Items/Weapons/MoonStoneSword_1";
        public override void SetDefaults()
        {
            base.SetDefaults();
            projectileType = ModContent.ProjectileType<MoonStoneProjectile>();
        }
    }
}