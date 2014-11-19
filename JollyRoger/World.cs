using System;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using SharpDX.Windows;
using SharpHelper;

namespace JollyRoger
{
    public class World
    {
        private readonly SharpDevice _device;
        private readonly RenderForm _renderForm;
        private readonly HeadsUpDisplay _headsUpDisplay;
        private readonly Camera _camera;
        private Terrain _terrain;

        public World(SharpDevice device, RenderForm renderForm, HeadsUpDisplay headsUpDisplay, Camera camera)
        {
            if (device == null) throw new ArgumentNullException("device");
            if (renderForm == null) throw new ArgumentNullException("renderForm");
            if (headsUpDisplay == null) throw new ArgumentNullException("headsUpDisplay");
            if (camera == null) throw new ArgumentNullException("camera");

            _device = device;
            _renderForm = renderForm;
            _headsUpDisplay = headsUpDisplay;
            _camera = camera;

            _terrain = new Terrain(_device, PerlineNoiseGenerator.Create(PerlineNoiseGenerator.GenerateWhiteNoise(128, 128), 4, 5f),
                                   GeneratePointCloud(128, 128, 128));

           
        }

        public void Draw()
        {
            _device.UpdateAllStates();
            _device.Clear(Color.CornflowerBlue);

            _terrain.Draw(_camera.ViewMatrix);
            _headsUpDisplay.Draw();

            _device.Present();
        }

        public void Update()
        {
            _camera.Update();
            _headsUpDisplay.Update(new []
            {
                string.Format("Camera Location: {0}", _camera.GetPosition()), 
                string.Format("Camera Rotation: {0}", _camera.GetRotation())
            });
            _terrain.Update();
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
