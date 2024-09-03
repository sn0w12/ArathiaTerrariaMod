using Terraria.ModLoader;
using Terraria;
using Terraria.ModLoader.IO;

namespace Arathia.Content
{
    public class ArathiaPlayer : ModPlayer
    {
        public bool hasUsedDevilsHeart;
        public int permanentLifeBonus;
        public bool worldStoneEquipped;

        public override void Initialize()
        {
            hasUsedDevilsHeart = false;
            permanentLifeBonus = 0;
        }

        public override void ResetEffects()
        {
            Player.statLifeMax2 += permanentLifeBonus;
            worldStoneEquipped = false;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["hasUsedDevilsHeart"] = hasUsedDevilsHeart;
            tag["permanentLifeBonus"] = permanentLifeBonus;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("hasUsedDevilsHeart"))
            {
                hasUsedDevilsHeart = tag.GetBool("hasUsedDevilsHeart");
            }

            if (tag.ContainsKey("permanentLifeBonus"))
            {
                permanentLifeBonus = tag.GetInt("permanentLifeBonus");
            }
        }
    }
}
