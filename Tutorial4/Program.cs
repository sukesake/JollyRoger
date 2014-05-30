using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using SharpHelper;

using Buffer11 = SharpDX.Direct3D11.Buffer;

namespace Tutorial4
{
    static class Program
    {
        private static float _lookX = 0f;
        private static float _lookY = 10f;
        private static float _lookZ = -40f;
        private static int _worldSize = 128;
        private static int _worldHeight = 128;
        private static int _groundLevel = 8;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!SharpDevice.IsDirectX11Supported())
            {
                System.Windows.Forms.MessageBox.Show("DirectX11 Not Supported");
                return;
            }

            var pointCloud = GeneratePointCloud(_worldSize, _worldHeight, _worldSize);

            int voxelCount = _worldSize*+_worldHeight*_worldSize;

            //render form
            RenderForm form = new RenderForm();
            form.Text = "Tutorial 6: Rasterizer & Alphablending";
            //frame rate counter
            SharpFPS fpsCounter = new SharpFPS();

            var perlineNoise = PerlineNoiseGenerator.Create(PerlineNoiseGenerator.GenerateWhiteNoise(_worldSize, _worldSize), 4, 5f);

            pointCloud = SetBlockTypes(pointCloud, perlineNoise);
            pointCloud = SetBlockStates(pointCloud);

            using (SharpDevice device = new SharpDevice(form))
            {
                //load font
                SharpBatch font = new SharpBatch(device, "textfont.dds");

               
                //init shader
                SharpShader shader = new SharpShader(device, "../../HLSL.txt",
                    new SharpShaderDescription() { VertexShaderFunction = "VS", PixelShaderFunction = "PS" },
                    new InputElement[] {  
                        new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32_Float, 12, 0)
                    });

                //init constant buffer
                Buffer11 buffer = shader.CreateBuffer<Matrix>();

                //load Shader Resouce View from file
                //it contains texture for using inside shaders
                ShaderResourceView texture = ShaderResourceView.FromFile(device.Device, "../../texture.dds");

                //init frame rate counter
                fpsCounter.Reset();
                SharpMesh mesh = new Voxel(device).Mesh;
                //keyboard event
                //change depth and rasterizer state
                form.KeyDown += (sender, e) =>
                {
                    switch (e.KeyCode)
                    {
                        case Keys.W:
                            device.SetWireframeRasterState();
                            device.SetDefaultBlendState();
                            break;
                        case Keys.S:
                            device.SetDefaultRasterState();
                            break;
                        case Keys.D1:
                            device.SetDefaultBlendState();
                            break;
                        case Keys.D2:
                            device.SetBlend(BlendOperation.Add, BlendOption.InverseSourceAlpha, BlendOption.SourceAlpha);
                            break;
                        case Keys.D3:
                            device.SetBlend(BlendOperation.Add, BlendOption.SourceAlpha, BlendOption.InverseSourceAlpha);
                            break;
                        case Keys.D4:
                            device.SetBlend(BlendOperation.Add, BlendOption.SourceColor, BlendOption.InverseSourceColor);
                            break;
                        case Keys.D5:
                            device.SetBlend(BlendOperation.Add, BlendOption.SourceColor, BlendOption.DestinationColor);
                            break;
                        case Keys.Left:
                            _lookX--;
                            break;
                        case Keys.Right:
                            _lookX++;
                            break;
                        case Keys.Up:
                            _lookZ++;
                            break;
                        case Keys.Down:
                            _lookZ--;
                            break;
                        case Keys.PageUp:
                            _lookY++;
                            break;
                        case Keys.PageDown:
                            _lookY--;
                            break;
                        case Keys.Add:
                            _groundLevel--;
                            pointCloud = SetBlockTypes(pointCloud,perlineNoise);
                            pointCloud = SetBlockStates(pointCloud);
                            break;
                        case Keys.Subtract:
                            _groundLevel++;
                            pointCloud = SetBlockTypes(pointCloud,perlineNoise);
                            pointCloud = SetBlockStates(pointCloud);
                            break;
                    }
                };

                //main loop
                RenderLoop.Run(form, () =>
                {
                    //Resizing
                    if (device.MustResize)
                    {
                        device.Resize();
                        font.Resize();
                    }


                    //apply state
                    device.UpdateAllStates();

                    //clear color
                    device.Clear(Color.CornflowerBlue);

                    //apply shader
                    shader.Apply();

                    //apply constant buffer to shader
                    device.DeviceContext.VertexShader.SetConstantBuffer(0, buffer);

                    //set texture
                    device.DeviceContext.PixelShader.SetShaderResource(0, texture);

                    //set transformation matrix
                    float ratio = (float)form.ClientRectangle.Width / (float)form.ClientRectangle.Height;

                    //projection matrix
                    Matrix projection = Matrix.PerspectiveFovLH(3.14F / 3.0F, ratio, 1, 1000);

                    //view matrix (camera)
                    Matrix view = Matrix.LookAtLH(new Vector3(_lookX, _lookY, _lookZ), new Vector3(_worldSize, 2, _worldSize), Vector3.UnitY);
                    Matrix world = Matrix.Translation(0, 0, 0);

                    voxelCount = 0;

                    for (int x = 0; x < _worldSize; x++)
                    {
                        for (int y = 0; y < _worldHeight; y++)
                        {
                            for (int z = 0; z < _worldSize; z++)
                            {
                                //world matrix


                                if (pointCloud[x][y][z].IsActive)
                                {
                                    
                                 
                                    voxelCount++;
                                    var translation = new Vector3(x * 2f, y * 2f, z * 2f);
                                    world = Matrix.Translation(translation);
                                    Matrix worldViewProjection = world * view * projection;

                                    //set world view projection matrix inside constant buffer
                                    device.UpdateData<Matrix>(buffer, worldViewProjection);

                                    //draw mesh

                                    mesh.Draw();
                                }
                            }
                        }
                    }

                    //begin drawing text
                    font.Begin();

                    //draw string
                    fpsCounter.Update();
                    font.DrawString("FPS: " + fpsCounter.FPS, 0, 0, Color.White);
                    font.DrawString("Press W for wireframe, S for solid", 0, 30, Color.White);
                    font.DrawString("Press From 1 to 5 for Alphablending", 0, 60, Color.White);
                    font.DrawString("Active Voxel Count: " + voxelCount, 0, 90, Color.White);
                    font.DrawString("Press + or - to change the terrain base height", 0, 120, Color.White);
                    font.DrawString("Use arrow keys, pageUp, or pageDown, to look around", 0, 150, Color.White);
                    //flush text to view
                    font.End();
                    //present
                    device.Present();
                });


                //release resource
                font.Dispose();
              //  mesh.Dispose();
                buffer.Dispose();
                texture.Dispose();
            }
        }

        private static Block[][][] SetBlockTypes(Block[][][] pointCloud, float[][] perlineNoise)
        {
            
            for (int x = 0; x < _worldSize; x++)
            {
                for (int y = 0; y < _worldHeight; y++)
                {
                    for (int z = 0; z < _worldSize; z++)
                    {
                        //reset to false for idempotency
                        pointCloud[x][y][z].BlockType = BlockType.Empty;
                        //set cube to active if it is below the heightmap
                        var heightLimit = perlineNoise[x][z] * 20 - _groundLevel;

                        if ( y <= heightLimit)
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

            for (int x = 0; x < _worldSize; x++)
            {
                for (int y = 0; y < _worldHeight; y++)
                {
                    for (int z = 0; z < _worldSize; z++)
                    {
                        //reset to false for idempotency
                        pointCloud[x][y][z].IsActive = false;
                        //set cube to active if it is below the heightmap
                        if (pointCloud[x][y][z].BlockType != BlockType.Empty)
                        {
                            //if it is part of the edge of the chunk we want it to be active
                            if (BlockExtension.IsChunkBorder(x, y, z, _worldSize, _worldHeight))
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

        private static Block[][][] GeneratePointCloud(int x, int y, int z)
        {
            var pointCloud = new Block[x][][];

            for (int i = 0; i < pointCloud.Length; i++)
            {
                pointCloud[i] = new Block[y][];
            }

            for (int i = 0; i < pointCloud.Length; i++)
            {
                for (int j = 0; j < pointCloud[i].Length; j++)
                {
                    pointCloud[i][j] = new Block[z];
                }               
            }

            for (int i = 0; i < pointCloud.Length; i++)
            {
                for (int j = 0; j < pointCloud[i].Length; j++)
                {
                    for (int k = 0; k < pointCloud[i][j].Length; k++)
                    {
                        pointCloud[i][j][k] = new Block
                        {
                            IsActive = false,
                            BlockType = BlockType.Empty
                        };
                    }
                }
            }
            return pointCloud;
        }
    }
}
