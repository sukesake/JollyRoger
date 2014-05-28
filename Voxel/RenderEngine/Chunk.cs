using Game;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D10.Buffer;

namespace RenderEngine
{
    public class Chunk
    {
        
        #region Constants

        public static readonly Vector3B Size = new Vector3B(16, 128, 16);

        public static readonly int BlockCount = Size.X*Size.Y*Size.Z;

        #endregion

        #region .ctor

        public Chunk(VectorIndex index)
        {
            ReinitializeBlock(index);
        }

        #endregion

        #region Properties

        public byte UpperGroundHeight = 0;

        public byte LowerGroundHeight = Size.Y;

        public int VertexCount;

        public int IndexCount;

        public int PrimitivesCount;

        public Vector3 Index3;

        public VectorIndex Index;

        public VectorIndex Position;

        public Vector3 Position3;

        public bool IsGenerated;

        public bool IsReady;

        public Buffer IndexBuffer;
        public Buffer VertexBuffer;

        public PlantArray LowerPlant = new PlantArray(Size.X + 2, Size.Z + 2);

        public PlantArray UpperPlant = new PlantArray(Size.X + 2, Size.Z + 2);

        public BoundingBoxF BoundingBox;

        #endregion

        public void ReinitializeBlock(VectorIndex index)
        {
            Index = index;

            Index3 = new Vector3(index.X, 0, index.Z);

            Position = new VectorIndex(index.X * Size.X, index.Z * Size.Z);

            Position3 = new Vector3(index.X * Size.X, 0, index.Z * Size.Z);

            IsReady = false;
            IsGenerated = false;
        }

        public void DrawChunk(SharpDX.Direct3D10.Device device, EffectPass pass)
        {
            device.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);

            device.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            device.InputAssembler.SetVertexBuffers(0,
                                                   new VertexBufferBinding(VertexBuffer,
                                                                           BlockVertex.BlockVertexSize, 0));

            pass.Apply();

            device.DrawIndexed(IndexCount, 0, 0);
        }

        public void Dispose()
        {
        }
    }
}
