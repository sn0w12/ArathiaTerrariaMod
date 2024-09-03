using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Arathia.Content.Items.Materials
{
    public class TsukiOre : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 12; // The width of the item texture in pixels
            Item.height = 12; // The height of the item texture in pixels
            Item.maxStack = 9999; // The maximum stack size
            Item.value = Item.sellPrice(silver: 10); // The value of the ore when sold
            Item.rare = ItemRarityID.Orange; // The rarity of the item
            Item.useStyle = ItemUseStyleID.Swing; // Use style for the item
            Item.useTime = 10; // The time it takes to use the item
            Item.useAnimation = 15; // The animation time for using the item
            Item.autoReuse = true; // If true, the item will auto-reuse when held down
            Item.useTurn = true; // Whether the player can turn while using the item
            Item.consumable = true; // This makes the item consumable (it will be used up)
            Item.createTile = ModContent.TileType<Tiles.TsukiOreTile>(); // The tile this item will place
        }
    }
}
