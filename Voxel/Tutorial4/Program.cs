﻿using System;
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

            //render form
            RenderForm form = new RenderForm();
            form.Text = "Tutorial 6: Rasterizer & Alphablending";
            //frame rate counter
            SharpFPS fpsCounter = new SharpFPS();

            var whiteNoise = PerlineNoiseGenerator.GenerateWhiteNoise(_worldSize, _worldSize);
            var perlineNoise = PerlineNoiseGenerator.Create(whiteNoise, 4, 5f);

            using (SharpDevice device = new SharpDevice(form))
            {
                //load font
                SharpBatch font = new SharpBatch(device, "textfont.dds");

                //init mesh
                int[] indices = new int[] 
                { 
                    0,1,2,0,2,3,
                    4,6,5,4,7,6,
                    8,9,10,8,10,11,
                    12,14,13,12,15,14,
                    16,18,17,16,19,18,
                    20,21,22,20,22,23
                };

                float length = 1.0f;
                float height = 1.0f;
                float width = 1.0f;

                //Vertices
                TexturedVertex[] vertices = new[] 
            {
                ////TOP
                new TexturedVertex(new Vector3(-length, height, width),new Vector2(1,1)),
                new TexturedVertex(new Vector3( length, height, width),new Vector2(0,1)),
                new TexturedVertex(new Vector3( length, height,-width),new Vector2(0,0)),
                new TexturedVertex(new Vector3(-length, height,-width),new Vector2(1,0)),
                //BOTTOM                                         
                new TexturedVertex(new Vector3(-length,-height ,width),new Vector2(1,1)),
                new TexturedVertex(new Vector3( length,-height ,width),new Vector2(0,1)),
                new TexturedVertex(new Vector3( length,-height,-width),new Vector2(0,0)),
                new TexturedVertex(new Vector3(-length,-height,-width),new Vector2(1,0)),
                //LEFT                                            
                new TexturedVertex(new Vector3(-length,-height, width),new Vector2(0,1)),
                new TexturedVertex(new Vector3(-length, height, width),new Vector2(0,0)),
                new TexturedVertex(new Vector3(-length, height,-width),new Vector2(1,0)),
                new TexturedVertex(new Vector3(-length,-height,-width),new Vector2(1,1)),
                //RIGHT                                               
                new TexturedVertex(new Vector3( length,-height, width),new Vector2(1,1)),
                new TexturedVertex(new Vector3( length, height, width),new Vector2(1,0)),
                new TexturedVertex(new Vector3( length, height,-width),new Vector2(0,0)),
                new TexturedVertex(new Vector3( length,-height,-width),new Vector2(0,1)),
                //FRONT                                        
                new TexturedVertex(new Vector3(-length, height, width),new Vector2(1,0)),
                new TexturedVertex(new Vector3( length, height, width),new Vector2(0,0)),
                new TexturedVertex(new Vector3( length,-height, width),new Vector2(0,1)),
                new TexturedVertex(new Vector3(-length,-height, width),new Vector2(1,1)),
                //BACK                                               
                new TexturedVertex(new Vector3(-length, height,-width),new Vector2(0,0)),
                new TexturedVertex(new Vector3( length, height,-width),new Vector2(1,0)),
                new TexturedVertex(new Vector3( length,-height,-width),new Vector2(1,1)),
                new TexturedVertex(new Vector3(-length,-height,-width),new Vector2(0,1))
            };


                SharpMesh mesh = SharpMesh.Create<TexturedVertex>(device, vertices, indices);

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



                    for (int x = 0; x < _worldSize; x++)
                    {
                        for (int y = 0; y < 20; y++)
                        {
                            for (int z = 0; z < _worldSize; z++)
                            {
                                //world matrix
                                var translation = new Vector3(x * 2f, y * 2f, z * 2f);
                                var heightCutoff = (perlineNoise[x][z]*20-8);

                                if (translation.Y <= heightCutoff)
                                {
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

                    //flush text to view
                    font.End();
                    //present
                    device.Present();


                });


                //release resource
                font.Dispose();
                mesh.Dispose();
                buffer.Dispose();
                texture.Dispose();
            }
        }

       
    }
}
