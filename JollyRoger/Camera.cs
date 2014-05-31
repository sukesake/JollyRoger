using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Windows;
using SharpHelper;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace JollyRoger
{
    public class Camera
    {
        private readonly SharpDevice _device;
        private readonly InputModule _inputModule;
        private Point _lastKnownMousePosition;
        private SharpShader _shader;
        private Buffer _buffer;
        private ShaderResourceView _texture;
        private float PositionX { get; set; }
        private float PositionY { get; set; }
        private float PositionZ { get; set; }

        private float RotationX { get; set; }
        private float RotationY { get; set; }
        private float RotationZ { get; set; }

        public Matrix ViewMatrix { get; private set; }

        public Camera(SharpDevice device, InputModule inputModule)
        {
            _device = device;
            _inputModule = inputModule;

            PositionX = 161;
            PositionY = 15;
            PositionZ = 89;
        }

        private void GetInputUpdates()
        {
            if (_inputModule.EnableWireframe())
            {
                _device.SetWireframeRasterState();
                _device.SetDefaultBlendState();
            }

            if (_inputModule.DisableWireframe())
            {
                _device.SetDefaultRasterState();
            }

            if (_inputModule.StrafeLeft())
            {
                PositionX--;
            }

            if (_inputModule.StrafeRight())
            {
                PositionX++;
            }

            if (_inputModule.MoveForward())
            {
                PositionZ++;
            }

            if (_inputModule.MoveBackward())
            {
                PositionZ--;
            }
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
            GetInputUpdates();
            var currentMousePosition = _inputModule.GetMousePosition();
            RotationY =  currentMousePosition.X;
            RotationX =  currentMousePosition.Y;
            _lastKnownMousePosition = currentMousePosition;

            //// Setup the position of the camera in the world.
            var moveVector = new Vector3(PositionX, PositionY, PositionZ);

            // Setup where the camera is looking by default.
            var lookAt = new Vector3(0, 0, 1);
            //// Set the yaw (Y axis), pitch (X axis), and roll (Z axis) rotations in radians.
            var pitch = (RotationX * 0.0174532925f)*.4f;
            var yaw = (RotationY * 0.0174532925f)*.4f;
            var roll = (RotationZ * 0.0174532925f)*.4f;

            // Create the rotation matrix from the yaw, pitch, and roll values.
            var rotationMatrix = Matrix.RotationYawPitchRoll(yaw, pitch, roll);
            
            // Transform the lookAt and up vector by the rotation matrix so the view is correctly rotated at the origin.
            lookAt = Vector3.TransformCoordinate(lookAt, rotationMatrix);
            var up = Vector3.TransformCoordinate(Vector3.UnitY, rotationMatrix);

            // Translate the rotated camera position to the location of the viewer.
            lookAt = moveVector + lookAt;

            // Finally create the view matrix from the three updated vectors.
            var view = Matrix.LookAtLH(moveVector, lookAt, up);

            const float ratio = 800f / 600f;
            var projection = Matrix.PerspectiveFovLH(3.14F / 3.0F, ratio, 1, 1000);
            ViewMatrix = view * projection;


        }
    }
}