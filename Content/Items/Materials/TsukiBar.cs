using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Arathia.Content.Items.Materials
{
    public class TsukiBar : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 10);
            Item.rare = ItemRarityID.Orange;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.TsukiBarTile>();
            Item.useTurn = true;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<TsukiOre>(3)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
