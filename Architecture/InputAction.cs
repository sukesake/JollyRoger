using System;
using System.Collections.Generic;
using System.Windows.Input;
using SharpDX.XInput;

namespace Architecture
{
    public class InputAction
    {
        public enum GamePadValueActions
        {
            LeftThumbX,
            LeftThumbY,
            LeftTrigger,
            RightThumbX,
            RightThumbY,
            RightTrigger
        }

        private InputManager.InputValueDelegate GetInputDelegate;


        public delegate bool GamePadButtonPressDelegate(GamepadButtonFlags flag, int playerIndex);
        private readonly GamePadButtonPressDelegate _getGamePadButtonPress;

        public delegate float GamePadValueDelegate(GamePadValueActions key, int playerIndex);
        private readonly GamePadValueDelegate _getGamePadValue;

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

        private GamePadButtonPressDelegate GetGamePadButtonPressDelegate(GamePadButtonPressDelegate getGamePadButtonPress)
        {
            return getGamePadButtonPress ?? ((flag, playerIndex) =>
            {
                var buttonState = (int)_inputManager.GetController(playerIndex).GetState().Gamepad.Buttons;
                return (buttonState & (int)flag) > 0;
            });
        }

        public virtual bool IsPressed(int playerIndex = 0)
        {
            return GetInputDelegate(playerIndex) > 0;
        }

        public virtual float GetPressValue( int playerIndex = 0)
        {
            return GetInputDelegate(playerIndex);
        }

        private InputManager _inputManager;
        public  static InputAction GamePad_A;
        public  static InputAction GamePad_B;
        public  static InputAction GamePad_Back;
        public  static InputAction GamePad_DPadDown;
        public  static InputAction GamePad_DPadLeft;
        public  static InputAction GamePad_DPadRight;
        public  static InputAction GamePad_DPadUp;
        public  static InputAction GamePad_LeftShoulder;
        public  static InputAction GamePad_LeftThumb;
        public  static InputAction GamePad_RightShoulder;
        public  static InputAction GamePad_RightThumb;
        public  static InputAction GamePad_Start;
        public  static InputAction GamePad_X;
        public  static InputAction GamePad_Y;
        public  static InputAction GamePad_LeftThumbX;
        public  static InputAction GamePad_LeftThumbY;
        public  static InputAction GamePad_LeftTrigger;
        public  static InputAction GamePad_RightThumbX;
        public  static InputAction GamePad_RightThumbY;
        public  static InputAction GamePad_RightTrigger;
        public Dictionary<Key, InputAction> Keys = new Dictionary<Key, InputAction>();


        public void InitializeActions(InputManager inputManager, KeyboardInputAction.GetKeyPressStateDelegate getKeyPressStateOverride = null)
        {
            _inputManager = inputManager;
            GamePad_A = new InputAction((index) => _getGamePadButtonPress(GamepadButtonFlags.A, index) ? 1 : 0);
            GamePad_B = new InputAction((index) => _getGamePadButtonPress(GamepadButtonFlags.B, index) ? 1 : 0);
            GamePad_Back = new InputAction((index) => _getGamePadButtonPress(GamepadButtonFlags.Back, index) ? 1 : 0);
            GamePad_DPadDown = new InputAction((index) => _getGamePadButtonPress(GamepadButtonFlags.DPadDown, index) ? 1 : 0);
            GamePad_DPadLeft = new InputAction((index) => _getGamePadButtonPress(GamepadButtonFlags.DPadLeft, index) ? 1 : 0);
            GamePad_DPadRight = new InputAction((index) => _getGamePadButtonPress(GamepadButtonFlags.DPadRight, index) ? 1 : 0);
            GamePad_DPadUp = new InputAction((index) => _getGamePadButtonPress(GamepadButtonFlags.DPadUp, index) ? 1 : 0);
            GamePad_LeftShoulder = new InputAction((index) => _getGamePadButtonPress(GamepadButtonFlags.LeftShoulder, index) ? 1 : 0);
            GamePad_LeftThumb = new InputAction((index) => _getGamePadButtonPress(GamepadButtonFlags.LeftThumb, index) ? 1 : 0);
            GamePad_RightShoulder = new InputAction((index) => _getGamePadButtonPress(GamepadButtonFlags.RightShoulder, index) ? 1 : 0);
            GamePad_RightThumb = new InputAction((index) => _getGamePadButtonPress(GamepadButtonFlags.RightThumb, index) ? 1 : 0);
            GamePad_Start = new InputAction((index) => _getGamePadButtonPress(GamepadButtonFlags.Start, index) ? 1 : 0);
            GamePad_X = new InputAction((index) => _getGamePadButtonPress(GamepadButtonFlags.X, index) ? 1 : 0);
            GamePad_Y = new InputAction((index) => _getGamePadButtonPress(GamepadButtonFlags.Y, index) ? 1 : 0);

            GamePad_LeftThumbX = new InputAction((index) => _getGamePadValue(GamePadValueActions.LeftThumbX, index));
            GamePad_LeftThumbY = new InputAction((index) => _getGamePadValue(GamePadValueActions.LeftThumbY, index));
            GamePad_LeftTrigger = new InputAction((index) => _getGamePadValue(GamePadValueActions.LeftTrigger, index));
            GamePad_RightThumbX = new InputAction((index) => _getGamePadValue(GamePadValueActions.RightThumbX, index));
            GamePad_RightThumbY = new InputAction((index) => _getGamePadValue(GamePadValueActions.RightThumbY, index));
            GamePad_RightTrigger = new InputAction((index) => _getGamePadValue(GamePadValueActions.RightTrigger, index));

            for (var i = 0; i < (int)Key.DeadCharProcessed; i++)
            {
                Keys.Add((Key)i, new KeyboardInputAction((Key)i, getKeyPressStateOverride));
            }
        }
    }
}