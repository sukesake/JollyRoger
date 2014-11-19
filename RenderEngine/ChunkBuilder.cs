using Game;
using SharpDX;
using SharpDX.Direct3D10;
using Buffer = SharpDX.Direct3D10.Buffer;
using Color = System.Drawing.Color;

namespace RenderEngine
{
    public class ChunkBuilder
    {
        private BiomeBuilder Biome;

        private IChunkGenerator ChunkGenerator;

        private Block blockTopNW, blockTopN, blockTopNE,
                    blockTopW, blockTopM, blockTopE,
                    blockTopSW, blockTopS, blockTopSE;

        private Block blockMidNW, blockMidN, blockMidNE,
                blockMidW, blockMidM, blockMidE,
                blockMidSW, blockMidS, blockMidSE;

        private Block blockBotNW, blockBotN, blockBotNE,
                blockBotW, blockBotM, blockBotE,
                blockBotSW, blockBotS, blockBotSE;

        private BlockArray BlockArray;

        private DataStream VertexStream;
        private DataStream IndexStream;
        private Device Device;

        public ChunkBuilder(Device device)
        {
            Device = device;
            Biome = new BiomeBuilder();
            BlockArray = new BlockArray();
        }

        public void Build(Chunk chunk)
        {
            if (chunk.IsReady)
            {
                return;
            }

            BeforeChunkBuild(chunk);

            ChunkGenerator = Biome.GetBiome(chunk.Index);

            BlockArray.Flush();

            for (int x = -1; x < Chunk.Size.X + 1; x++)
            {
                for (int z = -1; z < Chunk.Size.Z + 1; z++)
                {
                    if (!chunk.IsGenerated)
                    {
                        uint worldX = (uint)(chunk.Position.X + x);// + World.SEED);
                        uint worldZ = (uint)(chunk.Position.Z + z);

                        ChunkGenerator.GenerateGroundHeight(chunk, x, z, worldX, worldZ);
                    }

                    GenerateY(chunk, x, z);
                }
            }

            chunk.IsGenerated = true;

            for (int x = 0; x < Chunk.Size.X; x++)
            {
                for (int z = 0; z < Chunk.Size.Z; z++)
                {
                    byte upperGroundHeight = chunk.UpperPlant[x, z];
                    byte lowerGroundHeight = chunk.LowerPlant[x, z];

                    for (byte y = lowerGroundHeight; y <= upperGroundHeight; y++)
                    {
                        Block block = BlockArray[x, y, z];

                        if (block.Type != BlockType.None)
                        {
                            if (Block.IsPlantBlock(block.Type))
                            {
                                // BuildPlantVertexList(blockType, chunk, new Vector3i(x, y, z));
                            }
                            else if (Block.IsGrassBlock(block.Type))
                            {
                                //BuildGrassVertexList(blockType, chunk, new Vector3i(x, y, z));
                            }
                            else
                            {
                                BuildBlockVertexList(block.Type, chunk, new Vector3I((uint)x, y, (uint)z));
                            }
                        }
                    }
                }
            }

            AfterChunkBuild(chunk);
        }

        private void GenerateY(Chunk chunk, int x, int z)
        {
            byte upperGroundHeight = (byte)(chunk.UpperPlant[x, z]);

            byte lowerGroundHeight = chunk.LowerPlant[x, z];

            bool isSunlite = true;

            byte yFrom = (byte)(chunk.UpperGroundHeight + 3);

            if (yFrom > Chunk.Size.Y)
            {
                yFrom = Chunk.Size.Y;
            }

            for (byte y = yFrom; y >= lowerGroundHeight; y--)
            {
                BlockType type = ChunkGenerator.GetBlockType(chunk.Position,
                                                             upperGroundHeight, lowerGroundHeight, x, y, z, isSunlite);

                if (isSunlite)
                {
                    BlockArray[x, y, z] = new Block(type, Block.MaxSunValue);
                }
                else
                {
                    BlockArray[x, y, z] = new Block(type, Block.MediumSunValue);
                }

                if (Block.IsTransparentBlock(type) == false)
                {
                    isSunlite = false;
                }
            }
        }

        public Chunk CreateChunk(VectorIndex index)
        {
            return new Chunk(index);
        }

        protected void BeforeChunkBuild(Chunk chunk)
        {
            if (VertexStream == null)
            {
                VertexStream = new DataStream(24 * Chunk.BlockCount * BlockVertex.BlockVertexSize, true, true);
                IndexStream = new DataStream(36 * Chunk.BlockCount * sizeof(int), true, true);
            }
            else
            {
                VertexStream.Position = 0;
                IndexStream.Position = 0;
            }
        }

        protected void AddVertex(Chunk chunk, BlockFaceDirection faceDir, BlockType blockType, Vector3I blockPosition, Vector3I chunkRelativePosition, Vector3 vertexAdd, Vector3 normal, Vector2 uv1, byte sunLight, Color localLight)
        {


            VertexStream.Write(new Vector4(chunkRelativePosition.X + vertexAdd.X,
                                           chunkRelativePosition.Y + vertexAdd.Y,
                                           chunkRelativePosition.Z + vertexAdd.Z,
                                           sunLight));

            VertexStream.Write(new Vector3(normal.X, normal.Y, normal.Z));

            VertexStream.WriteRange(new float[] { uv1.X, uv1.Y });
        }

        protected void AddIndex(Chunk chunk, BlockType blockType, short i1, short i2, short i3, short i4, short i5, short i6)
        {
            int vertexCount = (int)(VertexStream.Position / BlockVertex.BlockVertexSize) - 4;

            IndexStream.Write((int)(vertexCount + i1));
            IndexStream.Write((int)(vertexCount + i2));
            IndexStream.Write((int)(vertexCount + i3));
            IndexStream.Write((int)(vertexCount + i4));
            IndexStream.Write((int)(vertexCount + i5));
            IndexStream.Write((int)(vertexCount + i6));
        }

        protected void AfterChunkBuild(Chunk chunk)
        {
            // Before create buffer

            int vertexSizeInBytes = (int)VertexStream.Position;
            int indexSizeInBytes = (int)IndexStream.Position;

            int vertexCount = (int)(VertexStream.Position / BlockVertex.BlockVertexSize);
            int indexCount = (int)IndexStream.Position / sizeof(int);

            VertexStream.Position = 0;
            IndexStream.Position = 0;

            if (chunk.VertexBuffer != null)
            {
                chunk.VertexBuffer.Dispose();
                chunk.VertexBuffer = null;
            }

            chunk.VertexBuffer = new Buffer(Device, VertexStream,
                                                   new BufferDescription()
                                                   {
                                                       BindFlags = BindFlags.VertexBuffer,
                                                       CpuAccessFlags = CpuAccessFlags.None,
                                                       OptionFlags = ResourceOptionFlags.None,
                                                       SizeInBytes = vertexSizeInBytes,
                                                       Usage = ResourceUsage.Default
                                                   });

            if (chunk.IndexBuffer != null)
            {
                chunk.IndexBuffer.Dispose();
                chunk.IndexBuffer = null;
            }

            chunk.IndexBuffer = new Buffer(Device, IndexStream, new BufferDescription()
            {
                BindFlags = BindFlags.IndexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = indexSizeInBytes,
                Usage = ResourceUsage.Default
            });

            chunk.VertexCount = vertexCount;
            chunk.IndexCount = indexCount;
            chunk.PrimitivesCount = indexCount / 3;

            chunk.IsReady = true;
        }

        private void BuildBlockVertexList(BlockType blockType, Chunk chunk, Vector3I chunkRelativePosition)
        {
            Vector3I blockPosition = chunk.Position + chunkRelativePosition;

            int X = (int)chunkRelativePosition.X;
            int Y = (int)chunkRelativePosition.Y;
            int Z = (int)chunkRelativePosition.Z;

            blockTopNW = BlockArray[X - 1, Y + 1, Z + 1];
            blockTopN = BlockArray[X, Y + 1, Z + 1];
            blockTopNE = BlockArray[X + 1, Y + 1, Z + 1];
            blockTopW = BlockArray[X - 1, Y + 1, Z];
            blockTopM = BlockArray[X, Y + 1, Z];
            blockTopE = BlockArray[X + 1, Y + 1, Z];
            blockTopSW = BlockArray[X - 1, Y + 1, Z - 1];
            blockTopS = BlockArray[X, Y + 1, Z - 1];
            blockTopSE = BlockArray[X + 1, Y + 1, Z - 1];

            blockMidNW = BlockArray[X - 1, Y, Z + 1];
            blockMidN = BlockArray[X, Y, Z + 1];
            blockMidNE = BlockArray[X + 1, Y, Z + 1];
            blockMidW = BlockArray[X - 1, Y, Z];
            //blockMidM = BlockArray[X, Y, Z];
            blockMidE = BlockArray[X + 1, Y, Z];
            blockMidSW = BlockArray[X - 1, Y, Z - 1];
            blockMidS = BlockArray[X, Y, Z - 1];
            blockMidSE = BlockArray[X + 1, Y, Z - 1];

            //blockBotNW = BlockArray[X - 1, Y - 1, Z + 1];
            //blockBotN = BlockArray[X, Y - 1, Z + 1];
            //blockBotNE = BlockArray[X + 1, Y - 1, Z + 1];
            //blockBotW = BlockArray[X - 1, Y - 1, Z];
            //blockBotM = BlockArray[X, Y - 1, Z];
            //blockBotE = BlockArray[X + 1, Y - 1, Z];
            //blockBotSW = BlockArray[X - 1, Y - 1, Z - 1];
            //blockBotS = BlockArray[X, Y - 1, Z - 1];
            //blockBotSE = BlockArray[X + 1, Y - 1, Z - 1];

            byte sunTR, sunTL, sunBR, sunBL;

            Color localTR = Color.White;
            Color localTL = Color.White;
            Color localBR = Color.White;
            Color localBL = Color.White;

            //YIncreasing
            if (Block.IsTransparentBlock(blockTopM.Type))// && !(blockType == blockTopM.Type))
            {
                if (Block.IsTransparentBlock(blockTopNW.Type) == false
                    || Block.IsTransparentBlock(blockTopN.Type) == false
                    || Block.IsTransparentBlock(blockTopNE.Type) == false
                    || Block.IsTransparentBlock(blockTopW.Type) == false
                    || Block.IsTransparentBlock(blockTopM.Type) == false
                    || Block.IsTransparentBlock(blockTopE.Type) == false
                    || Block.IsTransparentBlock(blockTopSW.Type) == false
                    || Block.IsTransparentBlock(blockTopS.Type) == false
                    || Block.IsTransparentBlock(blockTopSE.Type) == false)
                {

                    sunTL = (byte)14;// ((blockTopNW.Sun + blockTopN.Sun + blockTopW.Sun + blockTopM.Sun) / 4);
                    sunTR = (byte)14;// ((blockTopNE.Sun + blockTopN.Sun + blockTopE.Sun + blockTopM.Sun) / 4);
                    sunBL = (byte)14;// ((blockTopSW.Sun + blockTopS.Sun + blockTopW.Sun + blockTopM.Sun) / 4);
                    sunBR = (byte)14;// ((blockTopSE.Sun + blockTopS.Sun + blockTopE.Sun + blockTopM.Sun) / 4);
                }
                else
                {
                    sunTL = (byte)16;// ((blockTopNW.Sun + blockTopN.Sun + blockTopW.Sun + blockTopM.Sun) / 4);
                    sunTR = (byte)16;// ((blockTopNE.Sun + blockTopN.Sun + blockTopE.Sun + blockTopM.Sun) / 4);
                    sunBL = (byte)16;// ((blockTopSW.Sun + blockTopS.Sun + blockTopW.Sun + blockTopM.Sun) / 4);
                    sunBR = (byte)16;// ((blockTopSE.Sun + blockTopS.Sun + blockTopE.Sun + blockTopM.Sun) / 4);
                }

                BuildFaceVertices(chunk, blockPosition, chunkRelativePosition, BlockFaceDirection.YIncreasing, blockType,
                                  sunTL, sunTR, sunBL, sunBR, localTL, localTR, localBL, localBR);
            }

            //if (Block.IsTransparentBlock(blockBotM.Type))// && !(blockType == blockBotM.Type))
            //{
            //    sunBL = (byte)((blockBotSW.Sun + blockBotS.Sun + blockBotM.Sun + blockTopW.Sun) / 4);
            //    sunBR = (byte)((blockBotSE.Sun + blockBotS.Sun + blockBotM.Sun + blockTopE.Sun) / 4);
            //    sunTL = (byte)((blockBotNW.Sun + blockBotN.Sun + blockBotM.Sun + blockTopW.Sun) / 4);
            //    sunTR = (byte)((blockBotNE.Sun + blockBotN.Sun + blockBotM.Sun + blockTopE.Sun) / 4);

            //    BuildFaceVertices(chunk, blockPosition, chunkRelativePosition, BlockFaceDirection.YDecreasing, blockType,
            //                      sunTL, sunTR, sunBL, sunBR, localTL, localTR, localBL, localBR);
            //}

            // XDecreasing
            if (Block.IsTransparentBlock(blockMidW.Type))// && !(blockType == blockMidW.Type))
            {
                sunTL = 10;// (byte)((blockTopNW.Sun + blockTopW.Sun + blockMidNW.Sun + blockMidW.Sun) / 4);
                sunTR = 10;// (byte)((blockTopSW.Sun + blockTopW.Sun + blockMidSW.Sun + blockMidW.Sun) / 4);
                sunBL = 10;// (byte)((blockBotNW.Sun + blockBotW.Sun + blockMidNW.Sun + blockMidW.Sun) / 4);
                sunBR = 10;// (byte)((blockBotSW.Sun + blockBotW.Sun + blockMidSW.Sun + blockMidW.Sun) / 4);

                BuildFaceVertices(chunk, blockPosition, chunkRelativePosition, BlockFaceDirection.XDecreasing, blockType,
                                  sunTL, sunTR, sunBL, sunBR, localTL, localTR, localBL, localBR);
            }
            if (Block.IsTransparentBlock(blockMidE.Type))// && !(blockType == blockMidE.Type))
            {
                sunTL = 10;// (byte)((blockTopSE.Sun + blockTopE.Sun + blockMidSE.Sun + blockMidE.Sun) / 4);
                sunTR = 10;// (byte)((blockTopNE.Sun + blockTopE.Sun + blockMidNE.Sun + blockMidE.Sun) / 4);
                sunBL = 10;// (byte)((blockBotSE.Sun + blockBotE.Sun + blockMidSE.Sun + blockMidE.Sun) / 4);
                sunBR = 10;// (byte)((blockBotNE.Sun + blockBotE.Sun + blockMidNE.Sun + blockMidE.Sun) / 4);

                BuildFaceVertices(chunk, blockPosition, chunkRelativePosition, BlockFaceDirection.XIncreasing, blockType,
                                  sunTL, sunTR, sunBL, sunBR, localTL, localTR, localBL, localBR);
            }


            if (Block.IsTransparentBlock(blockMidS.Type))// && !(blockType == blockMidS.Type))
            {
                sunTL = 10;// (byte)((blockTopSW.Sun + blockTopS.Sun + blockMidSW.Sun + blockMidS.Sun) / 4);
                sunTR = 10;// (byte)((blockTopSE.Sun + blockTopS.Sun + blockMidSE.Sun + blockMidS.Sun) / 4);
                sunBL = 10;// (byte)((blockBotSW.Sun + blockBotS.Sun + blockMidSW.Sun + blockMidS.Sun) / 4);
                sunBR = 10;// (byte)((blockBotSE.Sun + blockBotS.Sun + blockMidSE.Sun + blockMidS.Sun) / 4);

                BuildFaceVertices(chunk, blockPosition, chunkRelativePosition, BlockFaceDirection.ZDecreasing, blockType,
                                  sunTL, sunTR, sunBL, sunBR, localTL, localTR, localBL, localBR);
            }
            if (Block.IsTransparentBlock(blockMidN.Type))// && !(blockType == blockMidN.Type))
            {
                sunTL = 10;// (byte)((blockTopNE.Sun + blockTopN.Sun + blockMidNE.Sun + blockMidN.Sun) / 4);
                sunTR = 10;// (byte)((blockTopNW.Sun + blockTopN.Sun + blockMidNW.Sun + blockMidN.Sun) / 4);
                sunBL = 10;// (byte)((blockBotNE.Sun + blockBotN.Sun + blockMidNE.Sun + blockMidN.Sun) / 4);
                sunBR = 10;// (byte)((blockBotNW.Sun + blockBotN.Sun + blockMidNW.Sun + blockMidN.Sun) / 4);

                BuildFaceVertices(chunk, blockPosition, chunkRelativePosition, BlockFaceDirection.ZIncreasing, blockType,
                                  sunTL, sunTR, sunBL, sunBR, localTL, localTR, localBL, localBR);
            }
        }

        private void BuildFaceVertices(Chunk chunk, Vector3I blockPosition, Vector3I chunkRelativePosition, BlockFaceDirection faceDir, BlockType blockType, byte sunLightTL, byte sunLightTR, byte sunLightBL, byte sunLightBR, Color localLightTL, Color localLightTR, Color localLightBL, Color localLightBR)
        {
            if (blockType == BlockType.Water && faceDir != BlockFaceDirection.YIncreasing)
            {
                return;
            }

            BlockTexture texture = TextureHelper.GetTexture(blockType, faceDir);

            Vector2[] UVList = TextureHelper.GetUVMapping((int)texture, faceDir);

            byte height = 1;

            switch (faceDir)
            {
                case BlockFaceDirection.XIncreasing:
                    {
                        //TR,TL,BR,BR,TL,BL
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(1, height, 1), new Vector3(1, 0, 0), UVList[0], sunLightTR, localLightTR);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(1, height, 0), new Vector3(1, 0, 0), UVList[1], sunLightTL, localLightTL);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(1, 0, 1), new Vector3(1, 0, 0), UVList[2], sunLightBR, localLightBR);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(1, 0, 0), new Vector3(1, 0, 0), UVList[5], sunLightBL, localLightBL);
                        AddIndex(chunk, blockType, 0, 1, 2, 2, 1, 3);
                    }
                    break;

                case BlockFaceDirection.XDecreasing:
                    {
                        //TR,TL,BL,TR,BL,BR
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(0, height, 0), new Vector3(-1, 0, 0), UVList[0], sunLightTR, localLightTR);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(0, height, 1), new Vector3(-1, 0, 0), UVList[1], sunLightTL, localLightTL);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(0, 0, 0), new Vector3(-1, 0, 0), UVList[5], sunLightBR, localLightBR);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(0, 0, 1), new Vector3(-1, 0, 0), UVList[2], sunLightBL, localLightBL);
                        AddIndex(chunk, blockType, 0, 1, 3, 0, 3, 2);
                    }
                    break;

                case BlockFaceDirection.YIncreasing:
                    {
                        //BL,BR,TR,BL,TR,TL
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(1, height, 1), new Vector3(0, 1, 0), UVList[4], sunLightTR, localLightTR);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(0, height, 1), new Vector3(0, 1, 0), UVList[5], sunLightTL, localLightTL);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(1, height, 0), new Vector3(0, 1, 0), UVList[1], sunLightBR, localLightBR);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(0, height, 0), new Vector3(0, 1, 0), UVList[3], sunLightBL, localLightBL);
                        AddIndex(chunk, blockType, 3, 2, 0, 3, 0, 1);
                    }
                    break;

                case BlockFaceDirection.YDecreasing:
                    {
                        //TR,BR,TL,TL,BR,BL
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(1, 0, 1), new Vector3(0, -1, 0), UVList[0], sunLightTR, localLightTR);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(0, 0, 1), new Vector3(0, -1, 0), UVList[2], sunLightTL, localLightTL);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(1, 0, 0), new Vector3(0, -1, 0), UVList[4], sunLightBR, localLightBR);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(0, 0, 0), new Vector3(0, -1, 0), UVList[5], sunLightBL, localLightBL);
                        AddIndex(chunk, blockType, 0, 2, 1, 1, 2, 3);
                    }
                    break;

                case BlockFaceDirection.ZIncreasing:
                    {
                        //TR,TL,BL,TR,BL,BR
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(0, height, 1), new Vector3(0, 0, 1), UVList[0], sunLightTR, localLightTR);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(1, height, 1), new Vector3(0, 0, 1), UVList[1], sunLightTL, localLightTL);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(0, 0, 1), new Vector3(0, 0, 1), UVList[5], sunLightBR, localLightBR);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(1, 0, 1), new Vector3(0, 0, 1), UVList[2], sunLightBL, localLightBL);
                        AddIndex(chunk, blockType, 0, 1, 3, 0, 3, 2);
                    }
                    break;

                case BlockFaceDirection.ZDecreasing:
                    {
                        //TR,TL,BR,BR,TL,BL
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(1, height, 0), new Vector3(0, 0, -1), UVList[0], sunLightTR, localLightTR);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(0, height, 0), new Vector3(0, 0, -1), UVList[1], sunLightTL, localLightTL);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(1, 0, 0), new Vector3(0, 0, -1), UVList[2], sunLightBR, localLightBR);
                        AddVertex(chunk, faceDir, blockType, blockPosition, chunkRelativePosition, new Vector3(0, 0, 0), new Vector3(0, 0, -1), UVList[5], sunLightBL, localLightBL);
                        AddIndex(chunk, blockType, 0, 1, 2, 2, 1, 3);
                    }
                    break;
            }
        }

    }
}
