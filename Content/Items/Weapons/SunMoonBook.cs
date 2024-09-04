using Arathia.Content.Items.Materials;
using Arathia.Content.Projectiles;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace Arathia.Content.Items.Weapons
{
    public class SunMoonBook : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToStaff(ModContent.ProjectileType<SunMoonProjectile>(), 0.2f, 90, 30);
            Item.UseSound = SoundID.Item71;
            Item.SetWeaponValues(1200, 12, 32);

            Item.SetShopValues(ItemRarityColor.Yellow8, 10000);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<TsukiBar>(15)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}