using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RenderEngine;
using SharpDX;
using Device = SharpDX.Direct3D10.Device;

namespace Game
{
    public class World
    {
        public Device Device { get; private set; }


        private readonly WorldRenderer _worldRenderer;
        private readonly Camera _camera;
        private readonly ChunkManager _chunkManager;

        public World(WorldRenderer worldRenderer, ChunkBuilder chunkbuilder)
        {
   
            if (worldRenderer == null)
            {
                throw new ArgumentNullException("worldRenderer");
            }



            _worldRenderer = worldRenderer;
            _camera = new Camera();
            _camera.InitializeCamera(worldRenderer.WindowWidth, worldRenderer.WindowHeight, new Keyboard(), new Mouse(worldRenderer.WindowHandle));
            _chunkManager = new ChunkManager();
            _chunkManager.Initialize(_camera, chunkbuilder);
            Device = _worldRenderer.Device;
        }

        public void Draw()
        {
            _worldRenderer.BeginDraw();

            _worldRenderer.DrawTerrain(_camera, _chunkManager.Values);

    
            _worldRenderer.EndDraw();
        }

        public void Update()
        {
            _camera.Update();
        }
    }
}
