using Arathia.Content.Dusts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Arathia.Content.Tiles
{
    public class TsukiOreTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Ore[Type] = true;
            Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
            Main.tileOreFinderPriority[Type] = 600; // Metal Detector value, see https://terraria.wiki.gg/wiki/Metal_Detector
            Main.tileShine2[Type] = true; // Modifies the draw color slightly.
            Main.tileShine[Type] = 975; // How often tiny dust appear off this tile. Larger is less frequently
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(152, 171, 198), name);

            //DustType = ModContent.DustType<TsukiDust>();
            DustType = DustID.Ice;
            HitSound = SoundID.Tink;
            MineResist = 4f;
            MinPick = 150;
        }
    }

    public class TsukiOreSystem : ModSystem
    {
        public static LocalizedText TsukiOrePassMessage { get; private set; }
        public static LocalizedText BlessedWithTsukiOreMessage { get; private set; }

        public override void SetStaticDefaults()
        {
            TsukiOrePassMessage = Mod.GetLocalization($"WorldGen.{nameof(TsukiOrePassMessage)}");
        }

        // World generation is explained more in https://github.com/tModLoader/tModLoader/wiki/World-Generation
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            // Most vanilla ores are generated in a step called "Shinies", so for maximum compatibility, we will also do this.
            // find out which step "Shinies" is.
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));

            if (ShiniesIndex != -1)
            {
                tasks.Insert(ShiniesIndex + 1, new TsukiOrePass("Tsuki Mod Ores", 237.4298f));
            }
        }
    }

    public class TsukiOrePass : GenPass
    {
        public TsukiOrePass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = TsukiOreSystem.TsukiOrePassMessage.Value;

            for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 0.01); k++)
            {
                // Randomly choose a coordinate in the world
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY);

                // Check if the tile is in the Snow biome and within the Underground or Cavern layer
                if (IsInSnowBiome(x, y) && IsInUndergroundOrCavern(y))
                {
                    WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 6), ModContent.TileType<TsukiOreTile>());
                }
            }
        }

        private static bool IsInSnowBiome(int x, int y)
        {
            return Main.tile[x, y].WallType == WallID.SnowWallUnsafe || Main.tile[x, y].WallType == WallID.SnowWallUnsafe || Main.tile[x, y].TileType == TileID.SnowBlock;
        }

        private static bool IsInUndergroundOrCavern(int y)
        {
            return y > Main.worldSurface && y < Main.rockLayer;
        }
    }
}