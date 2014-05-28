namespace RenderEngine
{
        public class SimpleTerrain : IChunkGenerator
        {
            #region Fields

            public const int WATERLEVEL = 64; //Chunk.Size.Y/2
            public const int SNOWLEVEL = 95;

            public static readonly int MINIMUMGROUNDHEIGHT = Chunk.Size.Y / 4;
            public static readonly int MINIMUMGROUNDDEPTH = (int)(Chunk.Size.Y * 0.75f);

            #endregion

            public virtual void Generate(Chunk chunk)
            {
                for (int x = -1; x < Chunk.Size.X + 1; x++)
                {
                    for (int z = -1; z < Chunk.Size.Z + 1; z++)
                    {
                        //TODO(PRUETT): SeedValue 123
                        uint worldX = (uint)(chunk.Position.X + x + 123);
                        uint worldZ = (uint)(chunk.Position.Z + z);

                        GenerateGroundHeight(chunk, x, z, worldX, worldZ);
                    }
                }

                chunk.IsGenerated = true;
            }

            public virtual BlockType GetBlockType(VectorIndex position, byte higerPlant, byte lowerPlant, int x, int y, int z,
                                                  bool isSunlite)
            {
                byte lowerGroundHeight = lowerPlant;

                if (y > lowerGroundHeight)
                {
                    return BlockType.None;
                }
                else if (y < lowerGroundHeight)
                {
                    return BlockType.Rock;
                }
                else
                {
                    return BlockType.Grass;
                }
            }

            public virtual void GenerateGroundHeight(Chunk chunk, int blockXInChunk, int blockZInChunk, uint worldX,
                                                     uint worldY)
            {
                // The lower ground level is at least this high.
                float octave1 = PerlinSimplexNoise.noise(worldX * 0.0001f, worldY * 0.0001f) * 0.5f;
                float octave2 = PerlinSimplexNoise.noise(worldX * 0.0005f, worldY * 0.0005f) * 0.25f;
                float octave3 = PerlinSimplexNoise.noise(worldX * 0.005f, worldY * 0.005f) * 0.12f;
                float octave4 = PerlinSimplexNoise.noise(worldX * 0.01f, worldY * 0.01f) * 0.12f;
                float octave5 = PerlinSimplexNoise.noise(worldX * 0.03f, worldY * 0.03f) * octave4;
                float lowerGroundHeight = octave1 + octave2 + octave3 + octave4 + octave5;

                lowerGroundHeight = lowerGroundHeight * MINIMUMGROUNDDEPTH + MINIMUMGROUNDHEIGHT;

                chunk.LowerPlant[blockXInChunk, blockZInChunk] = (byte)lowerGroundHeight;
                chunk.UpperPlant[blockXInChunk, blockZInChunk] = (byte)lowerGroundHeight;

                if (chunk.UpperGroundHeight < lowerGroundHeight)
                {
                    chunk.UpperGroundHeight = (byte)(lowerGroundHeight);
                }

                if (chunk.LowerGroundHeight > lowerGroundHeight - 1)
                {
                    chunk.LowerGroundHeight = (byte)(lowerGroundHeight - 1);
                }
            }
        }
}
