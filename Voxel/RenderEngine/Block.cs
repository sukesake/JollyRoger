using Game;

namespace RenderEngine
{
    public struct Block
    {
        #region Static methods

        public static bool IsLightBlock(BlockType type)
        {
            return type == BlockType.Lava || type == BlockType.Snow;
        }

        public static bool IsSolidBlock(BlockType type)
        {
            return type != BlockType.None && type != BlockType.RedFlower && type != BlockType.LongGrass;
        }

        public static bool IsPlantBlock(BlockType type)
        {
            return type == BlockType.RedFlower;
        }

        public static bool IsGrassBlock(BlockType type)
        {
            return type == BlockType.LongGrass;
        }

        public static bool IsTransparentBlock(BlockType type)
        {
            return
                type == BlockType.None || type == BlockType.Water
                || type == BlockType.Leaves || type == BlockType.RedFlower || type == BlockType.LongGrass;
        }

        public static bool IsDiggable(BlockType type)
        {
            return type != BlockType.Water;
        }

        #endregion

        public const byte MaxSunValue = 16;
        public const byte MediumSunValue = 10;
        public const byte MinSunValue = 0;

        public static readonly Block Rock = new Block(BlockType.Rock, MediumSunValue);
        public static readonly Block None = new Block(BlockType.None, MaxSunValue);

        public Block(BlockType type, byte sun)
        {
            Type = type;
            Sun = sun;
        }

        public BlockType Type;
        public byte Sun;
    }
}