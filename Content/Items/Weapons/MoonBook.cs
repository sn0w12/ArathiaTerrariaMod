using Arathia.Content.Items.Materials;
using Arathia.Content.Projectiles;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace Arathia.Content.Items.Weapons
{
    public class MoonBook : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToStaff(ModContent.ProjectileType<MoonProjectile>(), 0.2f, 90, 20);
            Item.UseSound = SoundID.Item71;
            Item.SetWeaponValues(600, 6, 32);

            Item.SetShopValues(ItemRarityColor.LightRed4, 10000);
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