namespace RenderEngine
{
    public class BiomeBuilder
    {
        private IChunkGenerator SimpleGenerator = new SimpleTerrain();

        public IChunkGenerator GetBiome(VectorIndex index)
        {
            return SimpleGenerator;
        }
    }
}
