using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Architecture
{
    public delegate float InputValueDelegate(int playerIndex);

    public delegate bool GetGamePadButtonPressDelegate(GamepadButtonFlags flag, int playerIndex);

    public delegate float GetGamePadValueDelegate(InputAction.GamePadValueActions key, int playerIndex);

    public delegate bool IsPressedKeyboardDelegate(Key key);

    public class KeyboardInputAction: InputAction
    {
        private readonly Key _key;

        public static IsPressedKeyboardDelegate IsPressedKeyboardOverride;

        public KeyboardInputAction(Key key)
        {
            _key = key;
        }

        public override bool IsPressed(int playerIndex = 0)
        {
            if (IsPressedKeyboardOverride != null)
            {
                return IsPressedKeyboardOverride(_key);
            }
            return Keyboard.IsKeyDown(_key);
        }

        public override float GetPressValue(int playerIndex = 0)
        {
            return IsPressed(playerIndex) ? 1 : 0;
        }
    }

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

        private InputValueDelegate GetInputDelegate;

        public static GetGamePadButtonPressDelegate GetGamePadButtonPressOverride;
        public static GetGamePadValueDelegate GetGamePadValueOverride;

        public InputAction()
        {
        }

        private InputAction(InputValueDelegate getInputDelegate)
        {
            GetInputDelegate = getInputDelegate;
        }

        public virtual bool IsPressed(int playerIndex = 0)
        {
            return GetInputDelegate(playerIndex) > 0 ? true : false;
        }

        public virtual float GetPressValue( int playerIndex = 0)
        {
            return GetInputDelegate(playerIndex);
        }

        static private bool GetGamePadButtonPress(GamepadButtonFlags flag, int playerIndex)
        {
            if (GetGamePadButtonPressOverride != null)
            {
                return GetGamePadButtonPressOverride(flag, playerIndex);
            }
            else
            {
                var buttonState = (int)_inputManager.GetController(playerIndex).GetState().Gamepad.Buttons;
                return (buttonState & (int)flag) > 0;
            }
        }

        static private float GetGamePadValue(GamePadValueActions key, int playerIndex)
        {
            if (GetGamePadValueOverride != null)
            {
                return GetGamePadValueOverride(key, playerIndex);
            }
            else
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
            }
        }

        private static InputManager _inputManager;

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

        public static Dictionary<Key, InputAction> Keys = new Dictionary<Key, InputAction>(); 

        public static void InitializeActions(InputManager im)
        {
            _inputManager = im;
            GamePad_A = new InputAction((index) => GetGamePadButtonPress(GamepadButtonFlags.A, index) ? 1 : 0);
            GamePad_B = new InputAction((index) => GetGamePadButtonPress(GamepadButtonFlags.B, index) ? 1 : 0);
            GamePad_Back = new InputAction((index) => GetGamePadButtonPress(GamepadButtonFlags.Back, index) ? 1 : 0);
            GamePad_DPadDown = new InputAction((index) => GetGamePadButtonPress(GamepadButtonFlags.DPadDown, index) ? 1 : 0);
            GamePad_DPadLeft = new InputAction((index) => GetGamePadButtonPress(GamepadButtonFlags.DPadLeft, index) ? 1 : 0);
            GamePad_DPadRight = new InputAction((index) => GetGamePadButtonPress(GamepadButtonFlags.DPadRight, index) ? 1 : 0);
            GamePad_DPadUp = new InputAction((index) => GetGamePadButtonPress(GamepadButtonFlags.DPadUp, index) ? 1 : 0);
            GamePad_LeftShoulder = new InputAction((index) => GetGamePadButtonPress(GamepadButtonFlags.LeftShoulder, index) ? 1 : 0);
            GamePad_LeftThumb = new InputAction((index) => GetGamePadButtonPress(GamepadButtonFlags.LeftThumb, index) ? 1 : 0);
            GamePad_RightShoulder = new InputAction((index) => GetGamePadButtonPress(GamepadButtonFlags.RightShoulder, index) ? 1 : 0);
            GamePad_RightThumb = new InputAction((index) => GetGamePadButtonPress(GamepadButtonFlags.RightThumb, index) ? 1 : 0);
            GamePad_Start = new InputAction((index) => GetGamePadButtonPress(GamepadButtonFlags.Start, index) ? 1 : 0);
            GamePad_X = new InputAction((index) => GetGamePadButtonPress(GamepadButtonFlags.X, index) ? 1 : 0);
            GamePad_Y = new InputAction((index) => GetGamePadButtonPress(GamepadButtonFlags.Y, index) ? 1 : 0);

            GamePad_LeftThumbX = new InputAction((index) => GetGamePadValue(GamePadValueActions.LeftThumbX, index));
            GamePad_LeftThumbY = new InputAction((index) => GetGamePadValue(GamePadValueActions.LeftThumbY, index));
            GamePad_LeftTrigger = new InputAction((index) => GetGamePadValue(GamePadValueActions.LeftTrigger, index));
            GamePad_RightThumbX = new InputAction((index) => GetGamePadValue(GamePadValueActions.RightThumbX, index));
            GamePad_RightThumbY = new InputAction((index) => GetGamePadValue(GamePadValueActions.RightThumbY, index));
            GamePad_RightTrigger = new InputAction((index) => GetGamePadValue(GamePadValueActions.RightTrigger, index));

            for (int i = 0; i < (int)Key.DeadCharProcessed; i++)
            {
                Keys.Add((Key)i, new KeyboardInputAction((Key)i));
            }
        }
    }

    public delegate void InputEventCallbackDelegate(int playerIndex, string actionName, float value);

    public class InputEvent
    {
        public event InputEventCallbackDelegate Event;

        public void Fire(int playerIndex, string actionName, float value)
        {
            Event(playerIndex, actionName, value);
        }
    }

    public class InputManager : GameModule
    {
        Dictionary<string, InputAction> _nameToAction = new Dictionary<string,InputAction>();
        Dictionary<InputAction, string> _actionToName = new Dictionary<InputAction, string>();
        Dictionary<InputAction, float> _prevFrameValues = new Dictionary<InputAction, float>();
        Dictionary<string, InputEvent> _inputTriggerCallbacks = new Dictionary<string, InputEvent>();
        readonly List<InputAction> _allActions = new List<InputAction>();

        List<Controller> _controllers = new List<Controller>();

        public InputManager()
        {
            InputAction.InitializeActions(this);

            _controllers.Add(new Controller(UserIndex.One));
            _controllers.Add(new Controller(UserIndex.Two));
            _controllers.Add(new Controller(UserIndex.Three));
            _controllers.Add(new Controller(UserIndex.Four));

            _allActions.Add(InputAction.GamePad_A);
            _allActions.Add(InputAction.GamePad_B);
            _allActions.Add(InputAction.GamePad_Back);
            _allActions.Add(InputAction.GamePad_DPadDown);
            _allActions.Add(InputAction.GamePad_DPadLeft);
            _allActions.Add(InputAction.GamePad_DPadRight);
            _allActions.Add(InputAction.GamePad_DPadUp);
            _allActions.Add(InputAction.GamePad_LeftShoulder);
            _allActions.Add(InputAction.GamePad_LeftThumb);
            _allActions.Add(InputAction.GamePad_RightShoulder);
            _allActions.Add(InputAction.GamePad_RightThumb);
            _allActions.Add(InputAction.GamePad_Start);
            _allActions.Add(InputAction.GamePad_X);
            _allActions.Add(InputAction.GamePad_Y);
            _allActions.Add(InputAction.GamePad_LeftThumbX);
            _allActions.Add(InputAction.GamePad_LeftThumbY);
            _allActions.Add(InputAction.GamePad_LeftTrigger);
            _allActions.Add(InputAction.GamePad_RightThumbX);
            _allActions.Add(InputAction.GamePad_RightThumbY);
            _allActions.Add(InputAction.GamePad_RightTrigger);

            foreach (var pair in InputAction.Keys)
            {
                _allActions.Add(pair.Value);
            }
        }

        public void RegisterNameToInputAction(string actionName, InputAction action)
        {
            _nameToAction.Add(actionName, action);
            _actionToName.Add(action, actionName);
            _inputTriggerCallbacks.Add(actionName, new InputEvent());
        }
        
        public override void Update(float dt)
        {
            foreach (var pair in _actionToName)
            {
                var action = pair.Key;
                var name = pair.Value;

                var value = action.GetPressValue();
                if(!_prevFrameValues.ContainsKey(action))
                {
                    _prevFrameValues.Add(action, 0);
                }
                var prevValue = _prevFrameValues[action];

                if (Math.Abs(value - prevValue) > 0.5)
                {
                    // Only do Player1 for now
                    _inputTriggerCallbacks[name].Fire(0, name, value);
                }
            }
        }

        public void AddOnTriggerCallback(string actionName, InputEventCallbackDelegate callback)
        {
            _inputTriggerCallbacks[actionName].Event += callback;
        }

        public void RemoveOnTriggerCallback(string actionName, InputEventCallbackDelegate callback)
        {
            _inputTriggerCallbacks[actionName].Event -= callback;
        }

        [STAThread]
        public bool IsPressed(string actionName, int playerIndex = 0)
        {
            if (_nameToAction.ContainsKey(actionName))
            {
                var action = _nameToAction[actionName];
                return action.IsPressed(playerIndex);
            }
            // Throw maybe instead?
            return false;
        }

        public float GetPressValue(string actionName, int playerIndex = 0)
        {
            if (_nameToAction.ContainsKey(actionName))
            {
                var action = _nameToAction[actionName];
                return action.GetPressValue(playerIndex);
            }
            // Throw maybe instead?
            return 0;
        }

        public Controller GetController(int playerIndex = 0)
        {
            return _controllers[playerIndex];
        }
    }
}
