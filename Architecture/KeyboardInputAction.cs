using System;
using System.Windows.Input;

namespace Architecture
{
    public class KeyboardInputAction : InputAction
    {
        public delegate bool GetKeyPressStateDelegate(Key key);

        private static GetKeyPressStateDelegate _getKeyPressState = (Key key) => { return Keyboard.IsKeyDown(key); };
        private readonly Key _key;

        public KeyboardInputAction(Key key, GetKeyPressStateDelegate getKeyPressState = null)
        {
            _key = key;

            if (getKeyPressState != null)
            {
                _getKeyPressState = getKeyPressState;
            }
        }

        [STAThread]
        public override bool IsPressed(int playerIndex = 0)
        {
            return _getKeyPressState(_key);
        }

        [STAThread]
        public override float GetPressValue(int playerIndex = 0)
        {
            return IsPressed(playerIndex) ? 1 : 0;
        }
    }
}