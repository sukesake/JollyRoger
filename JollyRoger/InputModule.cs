using System.Windows.Forms;
using Architecture;
using SharpDX;
using SharpDX.DirectInput;
using SharpDX.Windows;
using Key = System.Windows.Input.Key;

namespace JollyRoger
{
    public class InputModule
    {
        private readonly InputManager _inputManager;
        private Mouse _mouse;
        private MouseState _mouseState;
        private DirectInput _directInput;

        public InputModule(InputManager inputManager, RenderForm renderForm)
        {
            _inputManager = inputManager;
            _directInput = new DirectInput();
            _mouse = new Mouse(_directInput);
            _mouse.Properties.AxisMode =  DeviceAxisMode.Relative;
            _mouse.SetCooperativeLevel(renderForm, CooperativeLevel.Foreground | CooperativeLevel.NonExclusive);

            
        }

        public Point GetMousePosition()
        {
            _mouseState = new MouseState();

             _mouse.GetCurrentState(ref _mouseState);
             _mouse.Acquire();
            return new Point(_mouseState.X, _mouseState.Y);
        }

        public bool MoveForward()
        {
            return _inputManager.IsPressed("MoveForward");
        }

        public bool MoveBackward()
        {
            return _inputManager.IsPressed("MoveBackward");
        }

        public bool StrafeLeft()
        {
            return _inputManager.IsPressed("StrafeLeft");
        }

        public bool StrafeRight()
        {
            return _inputManager.IsPressed("StrafeRight");
        }

        public bool EnableWireframe()
        {
            return _inputManager.IsPressed("EnableWireframe");
        }

        public bool DisableWireframe()
        {
            return _inputManager.IsPressed("DisableWireframe");
        }

        //TODO(PRUETT): this sort of redundant mapping can likely be done more eloquently via reflection and moved into Architecture itself
        public void RegisterKeybindings()
        {
            _inputManager.RegisterNameToInputAction("MoveForward", InputAction.Keys[Key.W]);
            _inputManager.RegisterNameToInputAction("MoveBackward", InputAction.Keys[Key.S]);
            _inputManager.RegisterNameToInputAction("StrafeLeft", InputAction.Keys[Key.A]);
            _inputManager.RegisterNameToInputAction("StrafeRight", InputAction.Keys[Key.D]);
            _inputManager.RegisterNameToInputAction("EnableWireframe", InputAction.Keys[Key.D1]);
            _inputManager.RegisterNameToInputAction("DisableWireframe", InputAction.Keys[Key.D2]);
        }
    }
}