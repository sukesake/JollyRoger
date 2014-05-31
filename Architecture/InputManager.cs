using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Architecture
{
    public class InputManager : GameModule
    {
        public delegate float InputValueDelegate(int playerIndex);

        public delegate void InputEventCallbackDelegate(int playerIndex, string actionName, float value);

        readonly Dictionary<string, InputAction> _nameToAction = new Dictionary<string,InputAction>();
        readonly Dictionary<InputAction, string> _actionToName = new Dictionary<InputAction, string>();
        readonly Dictionary<InputAction, float> _prevFrameValues = new Dictionary<InputAction, float>();
        readonly Dictionary<string, InputEvent> _inputTriggerCallbacks = new Dictionary<string, InputEvent>();
        readonly List<InputAction> _allActions = new List<InputAction>();

        readonly List<Controller> _controllers = new List<Controller>();
        public InputAction InputAction { get; private set; }

        public InputManager(InputAction.GamePadButtonPressDelegate getGamePadButtonPressOverride = null, 
                            InputAction.GamePadValueDelegate getGamePadValueOverride = null,
                            KeyboardInputAction.GetKeyPressStateDelegate getKeyPressStateOverride = null)
        {
            InputAction = new InputAction(null, getGamePadButtonPressOverride, getGamePadValueOverride);
            InputAction.InitializeActions(this, getKeyPressStateOverride);

            RegisterControllers();
            RegisterGamepadInputActions();

            foreach (var pair in InputAction.Keys)
            {
                _allActions.Add(pair.Value);
            }
        }

        private void RegisterControllers()
        {
            _controllers.Add(new Controller(UserIndex.One));
            _controllers.Add(new Controller(UserIndex.Two));
            _controllers.Add(new Controller(UserIndex.Three));
            _controllers.Add(new Controller(UserIndex.Four));
        }

        private void RegisterGamepadInputActions()
        {
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

        public bool IsPressed(string actionName, int playerIndex = 0)
        {
            if (_nameToAction.ContainsKey(actionName))
            {
                var action = _nameToAction[actionName];
                return action.IsPressed(playerIndex);
            }
            // Throw maybe instead? no. using exceptions for control flow is bad juju
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
