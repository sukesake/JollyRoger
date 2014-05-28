using System;
using System.Drawing;
using System.Threading;

namespace RenderEngine
{
    public class ChunkManager : IDisposable
    {
  

        public readonly static System.Drawing.Drawing2D.Matrix EmptyMatrix = new System.Drawing.Drawing2D.Matrix();

        public readonly static byte BUILD_RANGE = Convert.ToByte(30);
        private readonly static byte LIGHT_RANGE = Convert.ToByte(BUILD_RANGE + 1);
        private readonly static byte GENERATE_RANGE_LOW = Convert.ToByte(LIGHT_RANGE + 1);
        public readonly static byte GENERATE_RANGE_HIGH = GENERATE_RANGE_LOW;
        public readonly static float GENERATE_RANGE_HIGH_SQUARED = GENERATE_RANGE_HIGH * GENERATE_RANGE_HIGH;
        public static readonly int SKY_DOME_Diametr = (GENERATE_RANGE_HIGH + 5) * 16;

        public readonly static int FOGNEAR = BUILD_RANGE * 16;
        public readonly static float FOGFAR = (GENERATE_RANGE_LOW + 1) * 16;

 



        private readonly ChunkCollection Chunks = new ChunkCollection(GENERATE_RANGE_HIGH * 2);

        private ChunkBuilder Builder;

        private Camera Camera;



        public bool IsRuning;

        public Vector3I RenderPositionIndex;

        public Chunk[] Values
        {
            get { return Chunks.Values; }
        }


        public void Initialize(Camera camera, ChunkBuilder builder)
        {
            Camera = camera;

            Builder = builder;

            DateTime now = DateTime.Now;

            BuildAll();

            Console.WriteLine((DateTime.Now - now).Seconds);

            IsRuning = true;

            ThreadPool.QueueUserWorkItem(GenerateThread);
        }


        private void GenerateThread(object sender)
        {
            while (IsRuning)
            {
                var currentChunkIndex = Camera.PositionIndex;

                if (RenderPositionIndex == currentChunkIndex)
                {
                    continue;
                }

                uint xMin = currentChunkIndex.X - GENERATE_RANGE_HIGH;
                uint xLMin = RenderPositionIndex.X - GENERATE_RANGE_HIGH;

                uint zMin = currentChunkIndex.Z - GENERATE_RANGE_HIGH;
                uint zLMin = RenderPositionIndex.Z - GENERATE_RANGE_HIGH;

                uint xMax = currentChunkIndex.X + GENERATE_RANGE_HIGH;
                uint zMax = currentChunkIndex.Z + GENERATE_RANGE_HIGH;

                int size = GENERATE_RANGE_HIGH * 2;

                Rectangle lastRect =
                    new Rectangle((int)xLMin, (int)zLMin,
                                                 size,
                                                 size);

                Rectangle сurrentRect =
                    new Rectangle((int)xMin, (int)zMin,
                                                 size,
                                                 size);

                Region buildRegion = new Region(lastRect);

                buildRegion.Complement(сurrentRect);

                RectangleF[] rects = buildRegion.GetRegionScans(EmptyMatrix);

                for (int i = 0; i < rects.Length; i++)
                {
                    RectangleF rect = rects[i];

                    for (uint x = (uint)rect.Left; x < rect.Right + 1; x++)
                    {
                        for (uint z = (uint)rect.Top; z < rect.Bottom + 1; z++)
                        {
                            Chunk chunk;

                            if (AssingChunk(new VectorIndex(x, z), out chunk))
                            {
                                Builder.Build(chunk);
                            }
                        }
                    }
                }

                RenderPositionIndex = currentChunkIndex;

                if (currentChunkIndex == Camera.PositionIndex)
                {
                    Thread.Sleep(100);
                }
            }
        }



        private void BuildAll()
        {
            Vector3I currentChunkIndex = Camera.PositionIndex;

            if (RenderPositionIndex == currentChunkIndex)
            {
                return;
            }

            uint xMin = currentChunkIndex.X - GENERATE_RANGE_HIGH;
            uint xMax = currentChunkIndex.X + GENERATE_RANGE_HIGH;

            uint zMin = currentChunkIndex.Z - GENERATE_RANGE_HIGH;
            uint zMax = currentChunkIndex.Z + GENERATE_RANGE_HIGH;

            for (uint x = xMin; x < xMax; x++)
            {
                for (uint z = zMin; z < zMax; z++)
                {
                    Chunk chunk;

                    if (AssingChunk(new VectorIndex(x, z), out chunk))
                    {
                        Builder.Build(chunk);
                    }
                }
            }

            RenderPositionIndex = currentChunkIndex;
        }

        private bool AssingChunk(VectorIndex index, out Chunk assingedChunk)
        {
            assingedChunk = Chunks[index.X, index.Z];

            if (assingedChunk != null)
            {
                return
                    false;
            }

            assingedChunk = Chunks.GetOnPosition(index.X, index.Z);

            if (assingedChunk != null)
            {
                assingedChunk.ReinitializeBlock(index);
            }
            else
            {
                assingedChunk = Chunks.Add(Builder.CreateChunk(index));
            }

            return
                true;
        }



        public void Dispose()
        {
            IsRuning = false;
        }

    }
}