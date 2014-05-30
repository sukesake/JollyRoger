using System;
using System.Collections.Generic;
using SharpDX;

namespace RenderEngine
{
    public static class TextureHelper
    {
        #region Static methods

        public static BlockTexture GetTexture(BlockType blockType)
        {
            return GetTexture(blockType, BlockFaceDirection.MAXIMUM, BlockType.None);
        }

        public static BlockTexture GetTexture(BlockType blockType, BlockFaceDirection faceDir)
        {
            return GetTexture(blockType, faceDir, BlockType.None);
        }

        public static BlockTexture GetTexture(BlockType blockType, BlockFaceDirection faceDir, BlockType blockAbove)
        {
            switch (blockType)
            {
                case BlockType.Dirt:
                    return BlockTexture.Dirt;
                case BlockType.Grass:
                    switch (faceDir)
                    {
                        case BlockFaceDirection.XIncreasing:
                        case BlockFaceDirection.XDecreasing:
                        case BlockFaceDirection.ZIncreasing:
                        case BlockFaceDirection.ZDecreasing:
                            return BlockTexture.GrassSide;
                        case BlockFaceDirection.YIncreasing:
                            return BlockTexture.GrassTop;
                        case BlockFaceDirection.YDecreasing:
                            return BlockTexture.Dirt;
                        default:
                            return BlockTexture.Rock;
                    }
                case BlockType.Lava:
                    return BlockTexture.Lava;
                case BlockType.Leaves:
                    return BlockTexture.Leaves;
                case BlockType.Rock:
                    return BlockTexture.Rock;
                case BlockType.Sand:
                    return BlockTexture.Sand;
                case BlockType.Snow:
                    return BlockTexture.Snow;
                case BlockType.Tree:
                    switch (faceDir)
                    {
                        case BlockFaceDirection.XIncreasing:
                        case BlockFaceDirection.XDecreasing:
                        case BlockFaceDirection.ZIncreasing:
                        case BlockFaceDirection.ZDecreasing:
                            return BlockTexture.TreeSide;
                        case BlockFaceDirection.YIncreasing:
                        case BlockFaceDirection.YDecreasing:
                            return BlockTexture.TreeTop;
                        default:
                            return BlockTexture.Rock;
                    }
                case BlockType.Water:
                    return BlockTexture.Water;
                case BlockType.RedFlower:
                    return BlockTexture.RedFlower;
                case BlockType.LongGrass:
                    return BlockTexture.LongGrass;
                default:
                    return BlockTexture.Rock;

            }
        }

        #endregion

        public const int TEXTUREATLASSIZE = 16;

        private static Dictionary<int, UvMap> UVMappings;

        static TextureHelper()
        {
            BuildUVMappings();
        }


        public struct UvMap
        {
            public Vector2[] GetVectors(BlockFaceDirection direction)
            {
                switch (direction)
                {
                    case BlockFaceDirection.XIncreasing:
                        return V0;
                    case BlockFaceDirection.XDecreasing:
                        return V1;
                    case BlockFaceDirection.YIncreasing:
                        return V2;
                    case BlockFaceDirection.YDecreasing:
                        return V3;
                    case BlockFaceDirection.ZIncreasing:
                        return V4;
                    case BlockFaceDirection.ZDecreasing:
                        return V5;
                    default:
                        throw new NotImplementedException();
                }
            }

            public UvMap(Vector2[] v0, Vector2[] v1, Vector2[] v2, Vector2[] v3, Vector2[] v4, Vector2[] v5)
            {
                V0 = v0;
                V1 = v1;
                V2 = v2;
                V3 = v3;
                V4 = v4;
                V5 = v5;
            }

            private Vector2[] V0;
            private Vector2[] V1;
            private Vector2[] V2;
            private Vector2[] V3;
            private Vector2[] V4;
            private Vector2[] V5;
        }

        private static void BuildUVMappings()
        {
            UVMappings = new Dictionary<int, UvMap>();

            foreach (var t in Enum.GetValues(typeof(BlockTexture)))
            {
                int i = (int)t;

                UVMappings.Add(i, new UvMap(CreateUVMapping(i, BlockFaceDirection.XIncreasing),
                                            CreateUVMapping(i, BlockFaceDirection.XDecreasing),
                                            CreateUVMapping(i, BlockFaceDirection.YIncreasing),
                                            CreateUVMapping(i, BlockFaceDirection.YDecreasing),
                                            CreateUVMapping(i, BlockFaceDirection.ZIncreasing),
                                            CreateUVMapping(i, BlockFaceDirection.ZDecreasing)));
            }
        }

        public static Vector2[] GetUVMapping(int texture, BlockFaceDirection faceDir)
        {
            return
                UVMappings[texture].GetVectors(faceDir);
        }

        #region GetUVMapping
        private static Vector2[] CreateUVMapping(int texture, BlockFaceDirection faceDir)
        {
            int textureIndex = texture;
            // Assumes a texture atlas of 8x8 textures

            int y = textureIndex / TEXTUREATLASSIZE;
            int x = textureIndex % TEXTUREATLASSIZE;

            float ofs = 1f / ((float)TEXTUREATLASSIZE);

            float yOfs = y * ofs;
            float xOfs = x * ofs;

            //ofs -= 0.01f;

            Vector2[] UVList = new Vector2[6];

            switch (faceDir)
            {
                case BlockFaceDirection.XIncreasing:
                    UVList[0] = new Vector2(xOfs, yOfs);                // 0,0
                    UVList[1] = new Vector2(xOfs + ofs, yOfs);          // 1,0
                    UVList[2] = new Vector2(xOfs, yOfs + ofs);          // 0,1
                    UVList[3] = new Vector2(xOfs, yOfs + ofs);          // 0,1
                    UVList[4] = new Vector2(xOfs + ofs, yOfs);          // 1,0
                    UVList[5] = new Vector2(xOfs + ofs, yOfs + ofs);    // 1,1
                    break;
                case BlockFaceDirection.XDecreasing:
                    UVList[0] = new Vector2(xOfs, yOfs);                // 0,0
                    UVList[1] = new Vector2(xOfs + ofs, yOfs);          // 1,0
                    UVList[2] = new Vector2(xOfs + ofs, yOfs + ofs);    // 1,1
                    UVList[3] = new Vector2(xOfs, yOfs);                // 0,0
                    UVList[4] = new Vector2(xOfs + ofs, yOfs + ofs);    // 1,1
                    UVList[5] = new Vector2(xOfs, yOfs + ofs);          // 0,1
                    break;
                case BlockFaceDirection.YIncreasing:
                    UVList[0] = new Vector2(xOfs, yOfs + ofs);          // 0,1
                    UVList[1] = new Vector2(xOfs, yOfs);                // 0,0
                    UVList[2] = new Vector2(xOfs + ofs, yOfs);          // 1,0
                    UVList[3] = new Vector2(xOfs, yOfs + ofs);          // 0,1
                    UVList[4] = new Vector2(xOfs + ofs, yOfs);          // 1,0
                    UVList[5] = new Vector2(xOfs + ofs, yOfs + ofs);    // 1,1
                    break;
                case BlockFaceDirection.YDecreasing:
                    UVList[0] = new Vector2(xOfs, yOfs);                // 0,0
                    UVList[1] = new Vector2(xOfs + ofs, yOfs);          // 1,0
                    UVList[2] = new Vector2(xOfs, yOfs + ofs);          // 0,1
                    UVList[3] = new Vector2(xOfs, yOfs + ofs);          // 0,1
                    UVList[4] = new Vector2(xOfs + ofs, yOfs);          // 1,0
                    UVList[5] = new Vector2(xOfs + ofs, yOfs + ofs);    // 1,1
                    break;
                case BlockFaceDirection.ZIncreasing:
                    UVList[0] = new Vector2(xOfs, yOfs);                // 0,0
                    UVList[1] = new Vector2(xOfs + ofs, yOfs);          // 1,0
                    UVList[2] = new Vector2(xOfs + ofs, yOfs + ofs);    // 1,1
                    UVList[3] = new Vector2(xOfs, yOfs);                // 0,0
                    UVList[4] = new Vector2(xOfs + ofs, yOfs + ofs);    // 1,1
                    UVList[5] = new Vector2(xOfs, yOfs + ofs);          // 0,1
                    break;
                case BlockFaceDirection.ZDecreasing:
                    UVList[0] = new Vector2(xOfs, yOfs);                // 0,0
                    UVList[1] = new Vector2(xOfs + ofs, yOfs);          // 1,0
                    UVList[2] = new Vector2(xOfs, yOfs + ofs);          // 0,1
                    UVList[3] = new Vector2(xOfs, yOfs + ofs);          // 0,1
                    UVList[4] = new Vector2(xOfs + ofs, yOfs);          // 1,0
                    UVList[5] = new Vector2(xOfs + ofs, yOfs + ofs);    // 1,1
                    break;
            }
            return UVList;
        }
        #endregion

    }
}