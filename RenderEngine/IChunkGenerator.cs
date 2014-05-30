namespace RenderEngine
{
    public interface IChunkGenerator
    {
        void GenerateGroundHeight(Chunk chunk, int blockXInChunk, int blockZInChunk, uint worldX, uint worldZ);
        BlockType GetBlockType(VectorIndex position, byte higerPlant, byte lowerPlant, int x, int y, int z, bool isSunlite);
        void Generate(Chunk chunk);
    }
}