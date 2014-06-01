using System;
using System.Collections.Generic;
using SharpDX.XInput;

namespace Architecture
{
    public class InputManager : GameModule
    {
        public static readonly int MaxPlayers = 4;

        public delegate void InputEventCallbackDelegate(int playerIndex, string actionName, float value);

        public delegate float InputValueDelegate(int playerIndex);


        private readonly Dictionary<InputAction, string>[] _actionToName = new Dictionary<InputAction, string>[MaxPlayers];
        private readonly List<InputAction> _allActions = new List<InputAction>();

        private readonly Controller[] _controllers = new Controller[MaxPlayers];
        private readonly Dictionary<string, InputEvent>[] _inputTriggerCallbacks = new Dictionary<string, InputEvent>[MaxPlayers];
        private readonly Dictionary<string, InputAction>[] _nameToAction = new Dictionary<string, InputAction>[MaxPlayers];
        private readonly Dictionary<InputAction, float>[] _prevFrameValues = new Dictionary<InputAction, float>[MaxPlayers];

        public InputManager(InputAction.GamePadButtonPressDelegate getGamePadButtonPressOverride = null,
            InputAction.GamePadValueDelegate getGamePadValueOverride = null,
            KeyboardInputAction.GetKeyPressStateDelegate getKeyPressStateOverride = null)
        {
            InputAction.InitializeActions(this, getKeyPressStateOverride, getGamePadButtonPressOverride, getGamePadValueOverride);

            RegisterControllers();
            RegisterGamepadInputActions();

            foreach (var pair in InputAction.Keys)
            {
                _allActions.Add(pair.Value);
            }

            for (int i = 0; i < MaxPlayers; i++)
            {
                _actionToName[i] = new Dictionary<InputAction, string>();
                _inputTriggerCallbacks[i] = new Dictionary<string, InputEvent>();
                _nameToAction[i] = new Dictionary<string, InputAction>();
                _prevFrameValues[i] = new Dictionary<InputAction, float>();
            }
        }

        private void RegisterControllers()
        {
            _controllers[0] = new Controller(UserIndex.One);
            _controllers[1] = new Controller(UserIndex.Two);
            _controllers[2] = new Controller(UserIndex.Three);
            _controllers[3] = new Controller(UserIndex.Four);
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

        public void RegisterNameToInputAction(string actionName, InputAction action, int playerIndex = 0)
        {
            //Todo: Verify valid playerIndex, how do we want to check this?  Assert?  No-op?

            //Todo: Make this override previous registration, if present, instead of throwing
            _nameToAction[playerIndex].Add(actionName, action);
            _actionToName[playerIndex].Add(action, actionName);
            _inputTriggerCallbacks[playerIndex].Add(actionName, new InputEvent());
        }

        public override void Update(float dt)
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                foreach (var pair in _actionToName[i])
                {
                    var action = pair.Key;
                    var name = pair.Value;

                    var value = action.GetPressValue();
                    if (!_prevFrameValues[i].ContainsKey(action))
                    {
                        _prevFrameValues[i].Add(action, 0);
                    }
                    var prevValue = _prevFrameValues[i][action];

                    if (Math.Abs(value - prevValue) > 0.5)
                    {
                        _inputTriggerCallbacks[i][name].Fire(i, name, value);
                    }
                }
            }
        }

        public void AddOnTriggerCallback(string actionName, InputEventCallbackDelegate callback, int playerIndex = 0)
        {
            // Todo: Verify playerIndex, and _inputTriggerCallbacks contains action
            _inputTriggerCallbacks[playerIndex][actionName].Event += callback;
        }

        public void RemoveOnTriggerCallback(string actionName, InputEventCallbackDelegate callback, int playerIndex = 0)
        {
            // Todo: Verify playerIndex, and _inputTriggerCallbacks contains action
            _inputTriggerCallbacks[playerIndex][actionName].Event -= callback;
        }

        public bool IsPressed(string actionName, int playerIndex = 0)
        {
            if (_nameToAction[playerIndex].ContainsKey(actionName))
            {
                var action = _nameToAction[playerIndex][actionName];
                return action.IsPressed(playerIndex);
            }
            return false;
        }

        public float GetPressValue(string actionName, int playerIndex = 0)
        {
            if (_nameToAction[playerIndex].ContainsKey(actionName))
            {
                var action = _nameToAction[playerIndex][actionName];
                return action.GetPressValue(playerIndex);
            }
            return 0;
        }

        public Controller GetController(int playerIndex = 0)
        {
            return _controllers[playerIndex];
        }
    }
}