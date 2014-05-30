
namespace JollyRoger
{
    public static class BlockExtension
    {
        public static bool IsNotEmpty(this Block block)
        {
            return block.BlockType != BlockType.Empty;
        }

        public static bool IsEmpty(this Block block)
        {
            return block.BlockType == BlockType.Empty;
        }

        public static bool IsChunkBorder(int x, int y, int z, int chunkWidth, int chunkHeight)
        {
            return x == 0 || y == 0 || z == 0 || x == chunkWidth - 1 || y == chunkHeight - 1 || z == chunkWidth - 1;
        }
    }
}
