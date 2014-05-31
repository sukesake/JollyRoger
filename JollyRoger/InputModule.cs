using System.Windows.Forms;
using System.Windows.Input;
using Architecture;
using SharpDX;

namespace JollyRoger
{
    public class InputModule
    {
        private readonly InputManager _inputManager;

        public InputModule(InputManager inputManager)
        {
            _inputManager = inputManager;
        }

        public Point GetMousePosition()
        {
            var position = Cursor.Position;
            return new Point(position.X, position.Y);
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
            _inputManager.RegisterNameToInputAction("MoveForward", _inputManager.InputAction.Keys[Key.W]);
            _inputManager.RegisterNameToInputAction("MoveBackward", _inputManager.InputAction.Keys[Key.S]);
            _inputManager.RegisterNameToInputAction("StrafeLeft", _inputManager.InputAction.Keys[Key.A]);
            _inputManager.RegisterNameToInputAction("StrafeRight", _inputManager.InputAction.Keys[Key.D]);
            _inputManager.RegisterNameToInputAction("EnableWireframe", _inputManager.InputAction.Keys[Key.D1]);
            _inputManager.RegisterNameToInputAction("DisableWireframe", _inputManager.InputAction.Keys[Key.D2]);
        }
    }
}