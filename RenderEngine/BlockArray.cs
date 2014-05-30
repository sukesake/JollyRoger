using System;
using RenderEngine;

namespace RenderEngine
{
    public class BlockArray
    {
        private readonly Block[, ,] Blocks;

        public BlockArray()
        {
            Blocks = new Block[Chunk.Size.X + 2, Chunk.Size.Y + 2, Chunk.Size.Z + 2];
        }

        public void Flush()
        {
            Array.Clear(Blocks, 0, Blocks.Length);
        }

        public Block this[int x, int y, int z]
        {
            get
            {
                return Blocks[x + 1, y + 1, z + 1];
            }
            set
            {
                Blocks[x + 1, y + 1, z + 1] = value;
            }
        }
    }
}