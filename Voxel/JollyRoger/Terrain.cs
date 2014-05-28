using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpHelper;

namespace JollyRoger
{
    public class Terrain
    {
        private readonly SharpDevice _device;
        private readonly Block[][][] _pointCloud;
        private SharpMesh _mesh;
        private SharpShader _shader;
        private Buffer _buffer;
        private ShaderResourceView _texture;
        private int _terrainWidth;
        private int _terrainHeight;
        public int VoxelCount { get; private set; }

        public Terrain(SharpDevice device, float[][] terrainIsoMesh, Block[][][] pointCloud)
        {
            _device = device;
            _pointCloud = pointCloud;
            _terrainWidth = pointCloud.Length;
            _terrainHeight = pointCloud[0].Length;
            _pointCloud = SetBlockTypes(_pointCloud, terrainIsoMesh);
            _pointCloud = SetBlockStates(_pointCloud);
            _mesh = new Voxel(_device).Mesh;

            SetUpShader();
        }

        private void SetUpShader()
        {
            _shader = new SharpShader(_device, "../../HLSL.txt",
                new SharpShaderDescription() { VertexShaderFunction = "VS", PixelShaderFunction = "PS" },
                new InputElement[] {  
                    new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float, 12, 0)
                });

            _buffer = _shader.CreateBuffer<Matrix>();

            _texture = ShaderResourceView.FromFile(_device.Device, "../../texture.dds");
        }

        public void Draw(Vector3 cameraLocation)
        {
            //apply shader
            _shader.Apply();

            //apply constant buffer to shader
            _device.DeviceContext.VertexShader.SetConstantBuffer(0, _buffer);

            //set texture
            _device.DeviceContext.PixelShader.SetShaderResource(0, _texture);

            VoxelCount = 0;
            float ratio = 800f / 600f;
            Matrix projection = Matrix.PerspectiveFovLH(3.14F / 3.0F, ratio, 1, 1000);
            Matrix world = Matrix.Translation(0, 0, 0);
            Matrix view = Matrix.LookAtLH(cameraLocation, new Vector3(0, 0, 0), Vector3.UnitY);

            for (int x = 0; x < _terrainWidth; x++)
            {
                for (int y = 0; y < _terrainHeight; y++)
                {
                    for (int z = 0; z < _terrainWidth; z++)
                    {
                        if (_pointCloud[x][y][z].IsActive)
                        {
                            VoxelCount++;
                            var translation = new Vector3(x * 2f, y * 2f, z * 2f);
                            world = Matrix.Translation(translation);
                            Matrix worldViewProjection = world * view * projection;

                            //set world view projection matrix inside constant buffer
                            _device.UpdateData<Matrix>(_buffer, worldViewProjection);

                            //draw mesh

                            _mesh.Draw();
                        }
                    }
                }
            }
        }

        public void Update()
        {
            
        }

        private static Block[][][] SetBlockTypes(Block[][][] pointCloud, float[][] terrainIsoMesh)
        {

            for (int x = 0; x < pointCloud.Length; x++)
            {
                for (int y = 0; y < pointCloud[0].Length; y++)
                {
                    for (int z = 0; z < pointCloud.Length; z++)
                    {
                        //reset to false for idempotency
                        pointCloud[x][y][z].BlockType = BlockType.Empty;
                        //set cube to active if it is below the heightmap
                        var heightLimit = terrainIsoMesh[x][z] * 20 - 8;

                        if (y <= heightLimit)
                        {
                            pointCloud[x][y][z].BlockType = BlockType.Rock;
                        }
                    }
                }
            }
            return pointCloud;
        }

        private static Block[][][] SetBlockStates(Block[][][] pointCloud)
        {

            for (int x = 0; x < pointCloud.Length; x++)
            {
                for (int y = 0; y < pointCloud[0].Length; y++)
                {
                    for (int z = 0; z < pointCloud.Length; z++)
                    {
                        //reset to false for idempotency
                        pointCloud[x][y][z].IsActive = false;
                        //set cube to active if it is below the heightmap
                        if (pointCloud[x][y][z].BlockType != BlockType.Empty)
                        {
                            //if it is part of the edge of the chunk we want it to be active
                            if (BlockExtension.IsChunkBorder(x, y, z, pointCloud.Length, pointCloud[0].Length))
                            {
                                pointCloud[x][y][z].IsActive = true;
                            }
                                //or if it is bordering an empty block we want it to be active. this means it is exposed to the air
                            else if (pointCloud[x][y + 1][z].IsEmpty() || //above
                                     pointCloud[x][y - 1][z].IsEmpty() || //below
                                     pointCloud[x + 1][y][z].IsEmpty() || //east
                                     pointCloud[x - 1][y][z].IsEmpty() || //west
                                     pointCloud[x][y][z + 1].IsEmpty() || //north
                                     pointCloud[x][y][z - 1].IsEmpty())    //south
                            {
                                pointCloud[x][y][z].IsActive = true;
                            }
                        }
                    }
                }
            }
            return pointCloud;
        }
    }
}