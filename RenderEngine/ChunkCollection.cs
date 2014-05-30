namespace RenderEngine
{
    public class ChunkCollection
    {
        private int Size;

        public ChunkCollection(int size)
        {
            Size = size;

            Values = new Chunk[Size * Size];
        }

        public Chunk[] Values;

        public virtual Chunk Add(Chunk chunk)
        {
            this[chunk.Index.X, chunk.Index.Z] = chunk;

            return
                chunk;
        }

        public Chunk GetOnPosition(uint xPos, uint zPos)
        {
            return Values[xPos % Size * Size + zPos % Size];
        }

        public virtual Chunk this[uint xPos, uint zPos]
        {
            get
            {
                Chunk result = Values[xPos % Size * Size + zPos % Size];

                if (result != null)
                {
                    if (result.Index.X != xPos
                        || result.Index.Z != zPos)
                    {
                        result = null;
                    }
                }

                return
                    result;
            }
            private set
            {
                long pos = xPos % Size * Size + zPos % Size;

                Chunk lastValue = Values[pos];

                if (value != lastValue)
                {
                    if (lastValue != null)
                    {
                        lastValue.Dispose();
                    }

                    Values[pos] = value;
                }
            }
        }

        public void Remove(uint xPos, uint zPos)
        {
            this[xPos, zPos] = null;
        }
    }
}