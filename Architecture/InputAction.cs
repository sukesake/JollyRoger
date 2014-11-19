using System.Collections.Generic;
using System.Windows.Input;
using SharpDX.XInput;

namespace Architecture
{
    public class InputAction
    {
        public delegate bool GamePadButtonPressDelegate(GamepadButtonFlags flag, int playerIndex);

        public delegate float GamePadValueDelegate(GamePadValueActions key, int playerIndex);

        public enum GamePadValueActions
        {
            LeftThumbX,
            LeftThumbY,
            LeftTrigger,
            RightThumbX,
            RightThumbY,
            RightTrigger
        }

        public static InputAction GamePad_A;
        public static InputAction GamePad_B;
        public static InputAction GamePad_Back;
        public static InputAction GamePad_DPadDown;
        public static InputAction GamePad_DPadLeft;
        public static InputAction GamePad_DPadRight;
        public static InputAction GamePad_DPadUp;
        public static InputAction GamePad_LeftShoulder;
        public static InputAction GamePad_LeftThumb;
        public static InputAction GamePad_RightShoulder;
        public static InputAction GamePad_RightThumb;
        public static InputAction GamePad_Start;
        public static InputAction GamePad_X;
        public static InputAction GamePad_Y;
        public static InputAction GamePad_LeftThumbX;
        public static InputAction GamePad_LeftThumbY;
        public static InputAction GamePad_LeftTrigger;
        public static InputAction GamePad_RightThumbX;
        public static InputAction GamePad_RightThumbY;
        public static InputAction GamePad_RightTrigger;
        private readonly InputManager.InputValueDelegate GetInputDelegate;
        private readonly GamePadButtonPressDelegate _getGamePadButtonPress;
        private readonly GamePadValueDelegate _getGamePadValue;
        public static Dictionary<Key, InputAction> Keys = new Dictionary<Key, InputAction>();
        private static InputManager _inputManager;

        public InputAction(InputManager.InputValueDelegate getInputDelegate = null,
            GamePadButtonPressDelegate getGamePadButtonPress = null,
            GamePadValueDelegate getGamePadValue = null)
        {
            _getGamePadButtonPress = GetGamePadButtonPressDelegate(getGamePadButtonPress);
            _getGamePadValue = GetGamePadValueDelegate(getGamePadValue);

            if (getInputDelegate != null)
            {
                GetInputDelegate = getInputDelegate;
            }
        }

        private GamePadValueDelegate GetGamePadValueDelegate(GamePadValueDelegate getGamePadValue)
        {
            return getGamePadValue ?? ((key, playerIndex) =>
            {
                var gamePad = _inputManager.GetController(playerIndex).GetState().Gamepad;
                switch (key)
                {
                        // Todo: Check against deadzone and trigger threshold values
                    case GamePadValueActions.LeftThumbX:
                        return gamePad.LeftThumbX;
                    case GamePadValueActions.LeftThumbY:
                        return gamePad.LeftThumbY;
                    case GamePadValueActions.LeftTrigger:
                        return gamePad.LeftTrigger;
                    case GamePadValueActions.RightThumbX:
                        return gamePad.RightThumbX;
                    case GamePadValueActions.RightThumbY:
                        return gamePad.RightThumbY;
                    case GamePadValueActions.RightTrigger:
                        return gamePad.RightTrigger;
                    default:
                        return 0;
                }
            });
        }

        private GamePadButtonPressDelegate GetGamePadButtonPressDelegate(
            GamePadButtonPressDelegate getGamePadButtonPress)
        {
            return getGamePadButtonPress ?? ((flag, playerIndex) =>
            {
                var buttonState = (int) _inputManager.GetController(playerIndex).GetState().Gamepad.Buttons;
                return (buttonState & (int) flag) > 0;
            });
        }

        public virtual bool IsPressed(int playerIndex = 0)
        {
            return GetInputDelegate(playerIndex) > 0;
        }

        public virtual float GetPressValue(int playerIndex = 0)
        {
            return GetInputDelegate(playerIndex);
        }


        public static void InitializeActions(InputManager inputManager,
            KeyboardInputAction.GetKeyPressStateDelegate getKeyPressStateOverride,
            GamePadButtonPressDelegate getGamePadButtonPressOverride,
            GamePadValueDelegate getGamePadValueOverride
            )
        {
            _inputManager = inputManager;
            GamePad_A = new InputAction(index => getGamePadButtonPressOverride(GamepadButtonFlags.A, index) ? 1 : 0);
            GamePad_B = new InputAction(index => getGamePadButtonPressOverride(GamepadButtonFlags.B, index) ? 1 : 0);
            GamePad_Back = new InputAction(index => getGamePadButtonPressOverride(GamepadButtonFlags.Back, index) ? 1 : 0);
            GamePad_DPadDown = new InputAction(index => getGamePadButtonPressOverride(GamepadButtonFlags.DPadDown, index) ? 1 : 0);
            GamePad_DPadLeft = new InputAction(index => getGamePadButtonPressOverride(GamepadButtonFlags.DPadLeft, index) ? 1 : 0);
            GamePad_DPadRight = new InputAction(index => getGamePadButtonPressOverride(GamepadButtonFlags.DPadRight, index) ? 1 : 0);
            GamePad_DPadUp = new InputAction(index => getGamePadButtonPressOverride(GamepadButtonFlags.DPadUp, index) ? 1 : 0);
            GamePad_LeftShoulder =
                new InputAction(index => getGamePadButtonPressOverride(GamepadButtonFlags.LeftShoulder, index) ? 1 : 0);
            GamePad_LeftThumb = new InputAction(index => getGamePadButtonPressOverride(GamepadButtonFlags.LeftThumb, index) ? 1 : 0);
            GamePad_RightShoulder =
                new InputAction(index => getGamePadButtonPressOverride(GamepadButtonFlags.RightShoulder, index) ? 1 : 0);
            GamePad_RightThumb =
                new InputAction(index => getGamePadButtonPressOverride(GamepadButtonFlags.RightThumb, index) ? 1 : 0);
            GamePad_Start = new InputAction(index => getGamePadButtonPressOverride(GamepadButtonFlags.Start, index) ? 1 : 0);
            GamePad_X = new InputAction(index => getGamePadButtonPressOverride(GamepadButtonFlags.X, index) ? 1 : 0);
            GamePad_Y = new InputAction(index => getGamePadButtonPressOverride(GamepadButtonFlags.Y, index) ? 1 : 0);
            GamePad_LeftThumbX = new InputAction(index => getGamePadValueOverride(GamePadValueActions.LeftThumbX, index));
            GamePad_LeftThumbY = new InputAction(index => getGamePadValueOverride(GamePadValueActions.LeftThumbY, index));
            GamePad_LeftTrigger = new InputAction(index => getGamePadValueOverride(GamePadValueActions.LeftTrigger, index));
            GamePad_RightThumbX = new InputAction(index => getGamePadValueOverride(GamePadValueActions.RightThumbX, index));
            GamePad_RightThumbY = new InputAction(index => getGamePadValueOverride(GamePadValueActions.RightThumbY, index));
            GamePad_RightTrigger = new InputAction(index => getGamePadValueOverride(GamePadValueActions.RightTrigger, index));

            Keys.Clear();
            
            for (var i = 0; i < (int) Key.DeadCharProcessed; i++)
            {
                Keys.Add((Key) i, new KeyboardInputAction((Key) i, getKeyPressStateOverride));
            }
        }
    }
}