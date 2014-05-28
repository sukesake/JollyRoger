using System;
using System.Configuration;
using RenderEngine;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using SharpDX.Windows;
using Device = SharpDX.Direct3D10.Device;
using DriverType = SharpDX.Direct3D10.DriverType;
using Resource = SharpDX.Direct3D10.Resource;

namespace Game
{
    public class WorldRenderer 
    {
        public static WorldRenderer Create()
        {
            var form = new RenderForm();

            form.Width = Convert.ToInt32(ConfigurationManager.AppSettings["width"]);
            form.Height = Convert.ToInt32(ConfigurationManager.AppSettings["height"]);

            var worldRenderer = new WorldRenderer(form);

            worldRenderer.Initialize();

            return worldRenderer;
        }

        private WorldRenderer(RenderForm renderForm)
        {
            RenderForm = renderForm;
        }


        private SwapChain swapChain;
        private RenderTargetView renderView;
        private Effect effect;
        private EffectPass pass;
        private InputLayout layout;
        private EffectMatrixVariable wvpVariable = null;
        private EffectMatrixVariable worldVariable = null;
        private EffectMatrixVariable viewVariable = null;
        private EffectMatrixVariable projectionVariable = null;
        private EffectVectorVariable indexVariable = null;
        private EffectVectorVariable cameraPositionVariable = null;
        private EffectShaderResourceVariable textureVariable = null;
        private EffectVectorVariable sunPositionVariable = null;
        private EffectScalarVariable timeOfDayVariable = null;

        private DepthStencilState DepthState;

        private DepthStencilView depthStencilView;




        public  IntPtr WindowHandle
        {
            get { return RenderForm.Handle; }
        }

        public  int WindowWidth
        {
            get { return RenderForm.ClientSize.Width; }
        }

        public  int WindowHeight
        {
            get { return RenderForm.ClientSize.Height; }
        }

        public RenderForm RenderForm;

        public Device Device;

        public  void BeginDraw()
        {
            Device.ClearRenderTargetView(renderView, Color.CornflowerBlue);
            Device.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1, 0);        
        }

        public  void DrawTerrain(Camera camera, Chunk[] chunks)
        {
            wvpVariable.SetMatrix(camera.View * camera.Projection);

            worldVariable.SetMatrix(camera.World);
            viewVariable.SetMatrix(camera.View);
            projectionVariable.SetMatrix(camera.Projection);

            cameraPositionVariable.Set(camera.Position);

            BoundingFrustumF viewFrustum = new BoundingFrustumF(camera.View * camera.Projection);

            int d = 0;

            for (int i = 0; i < chunks.Length; i++)
            {
                Chunk chunk = chunks[i];

                if (chunk == null || chunk.IsReady == false)
                {
                    continue;
                }

                // Проверяем расстояние (для отрисовки чанков по окружности)
                if (Vector3.DistanceSquared(new Vector3(camera.PositionIndex.X, 0, camera.PositionIndex.Z), new Vector3(chunk.Index.X, 0, chunk.Index.Z)) 
                    > ChunkManager.GENERATE_RANGE_HIGH_SQUARED)
                {
                    continue;
                }

                if (viewFrustum.FastIntersects(chunk.BoundingBox))
                {
                    indexVariable.Set(chunk.Position3);

                    chunk.DrawChunk(Device, pass);
                }
            }
        }

        public  void EndDraw()
        {
            swapChain.Present(0, PresentFlags.None);
        }

        private void Initialize()
        {
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription =
                    new ModeDescription(WindowWidth, WindowHeight,
                        new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = WindowHandle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out Device, out swapChain);

            //Stops Alt+enter from causing fullscreen skrewiness.
            Factory factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(WindowHandle, WindowAssociationFlags.IgnoreAll);

            renderView = new RenderTargetView(Device, Resource.FromSwapChain<Texture2D>(swapChain, 0));
            
            //CreateDepthBuffer(RenderForm.Width, RenderForm.Height);

            

            depthStencilView = new DepthStencilView(Device, new Texture2D(Device, new Texture2DDescription()
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = WindowWidth,
                Height = WindowHeight,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            }));


            var effectByteCode = ShaderBytecode.CompileFromFile("Content/Shaders/triangle.fx", "fx_4_0", ShaderFlags.EnableBackwardsCompatibility, EffectFlags.None);
            effect = new Effect(Device, effectByteCode);

            var technique = effect.GetTechniqueByIndex(0);
            pass = technique.GetPassByIndex(0);

            wvpVariable = effect.GetVariableByName("WVP").AsMatrix();
            worldVariable = effect.GetVariableByName("World").AsMatrix();
            viewVariable = effect.GetVariableByName("View").AsMatrix();
            projectionVariable = effect.GetVariableByName("Projection").AsMatrix();
            indexVariable = effect.GetVariableByName("Index").AsVector();
            cameraPositionVariable = effect.GetVariableByName("CameraPosition").AsVector();
            textureVariable = effect.GetVariableByName("AtlasTexture").AsShaderResource();
            sunPositionVariable = effect.GetVariableByName("g_LightDir").AsVector();
            timeOfDayVariable = effect.GetVariableByName("timeOfDay").AsScalar();

            effect.GetVariableByName("HorizonColor").AsVector().Set(Color.White);
            effect.GetVariableByName("NightColor").AsVector().Set(Color.Black);
            effect.GetVariableByName("MorningTint").AsVector().Set(Color.Gold);
            effect.GetVariableByName("EveningTint").AsVector().Set(Color.Red);
            effect.GetVariableByName("SunColor").AsVector().Set(Color.White);


            ShaderResourceView resource = new ShaderResourceView(Device, 
                Texture2D.FromFile<Texture2D>(Device, @"Content\Textures\terrain.png"));

            textureVariable.SetResource(resource);

            layout = new InputLayout(Device, pass.Description.Signature, BlockVertex.InputElements);

            Device.InputAssembler.InputLayout = layout;
            Device.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            Device.Rasterizer.SetViewports(new Viewport(0, 0, WindowWidth, WindowHeight, 0.0f, 1.0f));

            Device.OutputMerger.SetTargets(depthStencilView, renderView);
        }

        private void CreateDepthBuffer(int width, int height)
        {
            Texture2D texDesc = new Texture2D(Device, new Texture2DDescription()
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = width,
                Height = height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            depthStencilView = new DepthStencilView(Device, texDesc);

           

            DepthStencilStateDescription stencilStateDesc = new DepthStencilStateDescription();
            stencilStateDesc.IsDepthEnabled = true;
            stencilStateDesc.IsStencilEnabled = true;
            stencilStateDesc.DepthWriteMask = DepthWriteMask.All;
            stencilStateDesc.DepthComparison = Comparison.Less;
            stencilStateDesc.StencilReadMask = 0xFF;
            stencilStateDesc.StencilWriteMask = 0xFF;

            DepthStencilOperationDescription dsod = new DepthStencilOperationDescription();

            dsod.FailOperation = StencilOperation.Keep;
            dsod.DepthFailOperation = StencilOperation.Increment;
            dsod.PassOperation = StencilOperation.Keep;
            dsod.Comparison = Comparison.Always;
            stencilStateDesc.FrontFace = dsod;
            dsod.DepthFailOperation = StencilOperation.Decrement;
            stencilStateDesc.BackFace = dsod;

            DepthState = new DepthStencilState(Device, stencilStateDesc);

            RasterizerStateDescription rsd = new RasterizerStateDescription();
            rsd.FillMode = FillMode.Solid;
            rsd.CullMode = CullMode.Back;
            rsd.IsMultisampleEnabled = true;
            rsd.IsAntialiasedLineEnabled = false;
            rsd.IsDepthClipEnabled = false;
            rsd.IsScissorEnabled = false;

            Device.Rasterizer.State = new RasterizerState(Device, rsd);

        }

 
    }
}