using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RenderEngine;
using SharpDX;

namespace Game
{
    public class World
    {
        public WorldRenderer WorldRenderer { get; private set; }

        private readonly Camera _camera;
        private readonly ChunkManager _chunkManager;

        public World(WorldRenderer worldRenderer, ChunkBuilder chunkbuilder)
        {
            if (worldRenderer == null)
            {
                throw new ArgumentNullException("worldRenderer");
            }
            WorldRenderer = worldRenderer;
            _camera = new Camera();
            _camera.InitializeCamera(worldRenderer.WindowWidth, worldRenderer.WindowHeight, new Keyboard(), new Mouse(worldRenderer.WindowHandle));
            _chunkManager = new ChunkManager();
            _chunkManager.Initialize(_camera, chunkbuilder);

        }

        public void Draw()
        {
            WorldRenderer.BeginDraw();

            WorldRenderer.DrawTerrain(_camera, _chunkManager.Values);

            WorldRenderer.EndDraw();
        }

        public void Update()
        {
            _camera.Update();
        }
    }
}
