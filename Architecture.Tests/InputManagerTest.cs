using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Architecture;
using FluentAssertions;

namespace Architecture.Tests
{
    using NUnit.Framework;
    using SharpDX.XInput;
using System.Windows.Input;
    [TestFixture]
    public class InputManagerTest
    {
        InputManager _im = new InputManager();
        private bool _pressed;
        private float _gamePadValue;
        private GamepadButtonFlags _expectedFlag;
        private int _expectedPlayerIndex;
        private InputAction.GamePadValueActions _expectedGamePadKey;
        private Key _expectedKey;
        private bool _eventTest = false;

        private bool ButtonPressOverride(GamepadButtonFlags flag, int playerIndex)
        {
            if (!_eventTest)
            {
                flag.ShouldBeEquivalentTo(_expectedFlag);
                playerIndex.ShouldBeEquivalentTo(_expectedPlayerIndex);
            }
            return _pressed;
        }

        private float GamePadValueOverride(InputAction.GamePadValueActions key, int playerIndex)
        {
            if (!_eventTest)
            {
                key.ShouldBeEquivalentTo(_expectedGamePadKey);
                playerIndex.ShouldBeEquivalentTo(_expectedPlayerIndex);
            }
            return _gamePadValue;
        }

        private bool IsPressedKeyboardOverride(Key key)
        {
            if (!_eventTest)
            {
                key.ShouldBeEquivalentTo(_expectedKey);
            }
            return _pressed;
        }

        public InputManagerTest()
        {
            InputAction.GetGamePadButtonPressOverride = ButtonPressOverride;
            InputAction.GetGamePadValueOverride = GamePadValueOverride;
            KeyboardInputAction.IsPressedKeyboardOverride = IsPressedKeyboardOverride;
        }

        [Test]
        public void IsGamePadPressedTest()
        {
            _pressed = true;
            _expectedPlayerIndex = 0;
            _expectedFlag = GamepadButtonFlags.A;
            _im.RegisterNameToInputAction("Action1", InputAction.GamePad_A);

            _im.IsPressed("Action1").ShouldBeEquivalentTo(true);
        }

        [Test]
        public void GetPressedTest()
        {
            _gamePadValue = 0.6f;
            _expectedPlayerIndex = 0;
            _expectedGamePadKey = InputAction.GamePadValueActions.LeftTrigger;
            _im.RegisterNameToInputAction("Action2", InputAction.GamePad_LeftTrigger);

            _im.GetPressValue("Action2").ShouldBeEquivalentTo(_gamePadValue);
        }

        [Test]
        public void IsKeyboardPressedTest()
        {
            _pressed = true;
            _expectedKey = Key.A;
            _im.RegisterNameToInputAction("Action3", InputAction.Keys[Key.A]);

            _im.IsPressed("Action3").ShouldBeEquivalentTo(true);
        }

        [Test]
        public void KeyboardTriggerTest()
        {
            _eventTest = true;
            _expectedKey = Key.A;
            _im.RegisterNameToInputAction("Action4", InputAction.Keys[Key.A]);
            int timesCalled = 0;

            _im.AddOnTriggerCallback("Action4", (index, name, value) =>
            {
                index.ShouldBeEquivalentTo(0);
                name.ShouldAllBeEquivalentTo("Action4");
                value.ShouldBeEquivalentTo(1.0f);
                ++timesCalled;
            });

            _pressed = false;
            _im.Update(1);
            _pressed = true;
            _im.Update(1);
            _pressed = false;
            _im.Update(1);

            timesCalled.ShouldBeEquivalentTo(1);
        }
    }
}
