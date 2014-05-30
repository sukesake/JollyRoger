namespace RenderEngine
{
    public class PlantArray
    {
        public PlantArray(int sizeX, int sizeZ)
        {
            Plant = new byte[sizeX, sizeZ];
        }

        private byte[,] Plant;

        public byte this[int x, int z]
        {
            get
            {
                return Plant[x + 1, z + 1];
            }
            set
            {
                Plant[x + 1, z + 1] = value;
            }
        }
    }
}