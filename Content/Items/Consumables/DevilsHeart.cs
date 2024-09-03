using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Arathia.Content.Items.Consumables
{
    public class DevilsHeart : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item4;
            Item.consumable = true;
            Item.rare = ItemRarityID.Expert;
            Item.maxStack = 30;
            Item.value = Item.buyPrice(platinum: 2);
            Item.expert = true;
        }

        public override bool CanUseItem(Player player)
        {
            // Ensure the item can only be used once per player by checking if they've already used it
            return !player.GetModPlayer<ArathiaPlayer>().hasUsedDevilsHeart;
        }

        public override bool? UseItem(Player player)
        {
            // Increase the player's permanent life bonus by 100
            var modPlayer = player.GetModPlayer<ArathiaPlayer>();
            modPlayer.permanentLifeBonus += 100;

            // Set the flag to true to prevent multiple uses
            modPlayer.hasUsedDevilsHeart = true;

            // Immediately heal the player for the added amount
            player.statLife += 100;
            if (player.statLife > player.statLifeMax2)
            {
                player.statLife = player.statLifeMax2;
            }

            return true;
        }

        public override void AddRecipes()
        {
            // Optional: Define a recipe for crafting the item
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LifeCrystal, 1);
            recipe.AddIngredient(ItemID.LifeFruit, 5);
            recipe.AddIngredient(ItemID.FragmentSolar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
