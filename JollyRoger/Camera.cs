using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Windows;
using SharpHelper;

namespace JollyRoger
{
    public class Camera
    {
        private readonly RenderForm _renderForm;
        private readonly SharpDevice _device;
        private float PositionX { get; set; }
        private float PositionY { get; set; }
        private float PositionZ { get; set; }

        private float RotationX { get; set; }
        private float RotationY { get; set; }
        private float RotationZ { get; set; }

        public Matrix ViewMatrix { get; private set; }

        public Camera(RenderForm renderForm, SharpDevice device)
        {
            _renderForm = renderForm;
            _device = device;

            RegisterInputHandler();

            PositionX = 161;
            PositionY = 15;
            PositionZ = 89;
        }

        private void RegisterInputHandler()
        {
            _renderForm.KeyDown += (sender, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                        _device.SetWireframeRasterState();
                        _device.SetDefaultBlendState();
                        break;
                    case Keys.S:
                        _device.SetDefaultRasterState();
                        break;
                    case Keys.D1:
                        _device.SetDefaultBlendState();
                        break;
                    case Keys.D2:
                        _device.SetBlend(BlendOperation.Add, BlendOption.InverseSourceAlpha, BlendOption.SourceAlpha);
                        break;
                    case Keys.D3:
                        _device.SetBlend(BlendOperation.Add, BlendOption.SourceAlpha, BlendOption.InverseSourceAlpha);
                        break;
                    case Keys.D4:
                        _device.SetBlend(BlendOperation.Add, BlendOption.SourceColor, BlendOption.InverseSourceColor);
                        break;
                    case Keys.D5:
                        _device.SetBlend(BlendOperation.Add, BlendOption.SourceColor, BlendOption.DestinationColor);
                        break;
                    case Keys.Left:
                        PositionX--;
                        break;
                    case Keys.Right:
                        PositionX++;
                        break;
                    case Keys.Up:
                        PositionZ++;
                        break;
                    case Keys.Down:
                        PositionZ--;
                        break;
                    case Keys.PageUp:
                        PositionY++;
                        break;
                    case Keys.PageDown:
                        PositionY--;
                        break;
                }
            };
        }

        public void SetPosition(float x, float y, float z)
        {
            PositionX = x;
            PositionY = y;
            PositionZ = z;
        }

        public void SetRotation(float x, float y, float z)
        {
            RotationX = x;
            RotationY = y;
            RotationZ = z;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(PositionX, PositionY, PositionZ);
        }

        public Vector3 GetRotation()
        {
            return new Vector3(RotationX, RotationY, RotationZ);
        }

        public void Update()
        {
            // Setup the position of the camera in the world.
            var position = new Vector3(PositionX, PositionY, PositionZ);

            // Setup where the camera is looking by default.
            var lookAt = new Vector3(0, 0, 1);

            // Set the yaw (Y axis), pitch (X axis), and roll (Z axis) rotations in radians.
            var pitch = RotationX*0.0174532925f;
            var yaw = RotationY*0.0174532925f;
            var roll = RotationZ*0.0174532925f;

            // Create the rotation matrix from the yaw, pitch, and roll values.
            var rotationMatrix = Matrix.RotationYawPitchRoll(yaw, pitch, roll);

            // Transform the lookAt and up vector by the rotation matrix so the view is correctly rotated at the origin.
            lookAt = Vector3.TransformCoordinate(lookAt, rotationMatrix);
            var up = Vector3.TransformCoordinate(Vector3.UnitY, rotationMatrix);

            // Translate the rotated camera position to the location of the viewer.
            lookAt = position + lookAt;

            // Finally create the view matrix from the three updated vectors.
            ViewMatrix = Matrix.LookAtLH(position, lookAt, up);
        }
    }
}