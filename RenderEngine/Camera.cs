using System;
using System.Windows.Forms;
using SharpDX;

namespace RenderEngine
{
    public class Camera
    {
        private const float MOVEMENTSPEED = 0.5f;
        private const float ROTATIONSPEED = 0.1f;

        public MatrixF View;
        public MatrixF Projection;
        public MatrixF World = MatrixF.Identity;

        public static readonly int MaxYPosition = Chunk.Size.Y + 5;

        private float _viewAngle = MathHelper.PiOver2;
        private float _nearPlane = 0.01f;
        private float _farPlane = 10000f;

        private MouseState _mouseMoveState;

        private float _leftRightRotation = 0f;
        private float _upDownRotation = 0f;

        private Vector3 _cameraFinalTarget;
        private Vector3 _lookVector;

        public Vector3 Position = new Vector3(10000, 128, 10000);
        public Vector3I PositionIndex;

        public int ViewPortWidth;
        public int ViewPortHeight;

        private Keyboard Keyboard;

        private Mouse Mouse;

        #region ctor

        public Camera()
        {
            OnPositionChanged();
        }

        #endregion

        #region Public methods

        public void InitializeCamera(int windowWidth, int windowHeight, Keyboard keyboard, Mouse mouse)
        {
            Keyboard = keyboard;

            Mouse = mouse;

            ViewPortWidth = windowWidth;
            ViewPortHeight = windowHeight;

            CalculateProjection((float)ViewPortWidth / (float)ViewPortHeight);

            CalculateView();
        }

        public void Update()
        {
            CheckKeyboard(Keyboard);

            CheckMouse(Mouse);
        }

        #endregion

        #region Protected methods

        protected void CalculateProjection(float aspectRatio)
        {
            Projection = MatrixF.CreatePerspectiveFieldOfView(_viewAngle, aspectRatio, _nearPlane, _farPlane);
        }

        protected void CalculateView()
        {
            var rotationMatrix = Matrix3x3.RotationX(_upDownRotation) *
                                     Matrix3x3.RotationY(_leftRightRotation);

            //TODO(PRUETT): this maaaay not be a LH coord system... 
            _lookVector = Vector3.Transform(Vector3.ForwardRH, rotationMatrix);

            _cameraFinalTarget = Position + _lookVector;

            Vector3 cameraRotatedUpVector = Vector3.Transform(Vector3.Up, rotationMatrix);
            View = MatrixF.CreateLookAt(Position, _cameraFinalTarget, cameraRotatedUpVector);
        }

        #endregion

        #region Private methods

        private void OnPositionChanged()
        {
            PositionIndex = new Vector3I((uint)(Position.X / Chunk.Size.X), 0, (uint)(Position.Z / Chunk.Size.Z));
        }

        private void CheckKeyboard(Keyboard keyboard)
        {
            Vector3 moveVector = Vector3.Zero;

            if (keyboard.IsKeyPressed(Keys.Escape))
            {
                Application.Exit();
            }

            if (keyboard.IsKeyPressed(Keys.W))
            {
                //TODO(PRUETT): may not be LH
                moveVector += Vector3.ForwardRH;
            }

            if (keyboard.IsKeyPressed(Keys.S))
            {
                //TODO(PRUETT): may not be LH
                moveVector += Vector3.BackwardRH;
            }

            if (keyboard.IsKeyPressed(Keys.A))
            {
                moveVector += Vector3.Left;
            }

            if (keyboard.IsKeyPressed(Keys.D))
            {
                moveVector += Vector3.Right;
            }

            MoveTo(moveVector);
        }

        private void CheckMouse(Mouse mouse)
        {
            MouseState currentMouseState = mouse.GetState();

            RotateCamera(currentMouseState.X - _mouseMoveState.X, currentMouseState.Y - _mouseMoveState.Y);

            _mouseMoveState = new MouseState(ViewPortWidth / 2, ViewPortHeight / 2,
                0, MouseButtonState.Released,
                MouseButtonState.Released, MouseButtonState.Released,
                MouseButtonState.Released, MouseButtonState.Released);

            mouse.SetPosition(_mouseMoveState.X, _mouseMoveState.Y);
            _mouseMoveState = mouse.GetState();
        }

        private void RotateCamera(float mouseDX, float mouseDY)
        {
            if (mouseDX != 0)
            {
                _leftRightRotation -= ROTATIONSPEED * (mouseDX / 50);
            }
            else if (mouseDY == 0)
            {
                return;
            }

            if (mouseDY != 0)
            {
                _upDownRotation -= ROTATIONSPEED * (mouseDY / 50);

                // Locking camera rotation vertically between +/- 180 degrees
                float newPosition = _upDownRotation - ROTATIONSPEED * (mouseDY / 50);

                if (newPosition < -1.55f)
                {
                    newPosition = -1.55f;
                }
                else if (newPosition > 1.55f)
                {
                    newPosition = 1.55f;
                }

                _upDownRotation = newPosition;
            }
            else if (mouseDX == 0)
            {
                return;
            }

            CalculateView();
        }

        private void MoveTo(Vector3 moveVector)
        {
            if (moveVector != Vector3.Zero)
            {
                Matrix3x3 rotationMatrix = Matrix3x3.RotationX(_upDownRotation) *
                                         Matrix3x3.RotationY(_leftRightRotation);

                Vector3 rotatedVector = Vector3.Transform(moveVector, rotationMatrix);

                Position += rotatedVector * MOVEMENTSPEED;

                if (Position.Y > MaxYPosition)
                {
                    Position = new Vector3(Position.X, MaxYPosition, Position.Z);
                }

                OnPositionChanged();

                CalculateView();
            }
        }

        #endregion
    }
}