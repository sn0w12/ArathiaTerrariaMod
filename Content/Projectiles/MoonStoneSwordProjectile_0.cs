using Terraria.ModLoader;

namespace Arathia.Content.Projectiles
{
    public class MoonStoneSwordProjectile_0 : BaseMoonStoneSwordProjectile
    {
        public override string Texture => "Arathia/Content/Items/Weapons/MoonStoneSword_0";
        public override void SetDefaults()
        {
            base.SetDefaults();
            projectileType = -1;
        }
    }
}