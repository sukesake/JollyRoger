using System;
using FluentAssertions;
using NUnit.Framework;
using System.Windows.Input;

namespace Architecture.Tests
{
    [TestFixture]
    public class InputManagerTest
    {

        [Test]
        public void when_a_GamPad_is_pressed_then_IsPressed_should_be_true()
        {
            var inputManager = new InputManager(
                getGamePadButtonPressOverride : (flag, playerIndex) => true,
                getGamePadValueOverride : (key, playerIndex) => 0,
                getKeyPressStateOverride : (key) => true
            );

            inputManager.RegisterNameToInputAction("Action", InputAction.GamePad_A);

            inputManager.IsPressed("Action").Should().BeTrue();
        }

        [Test]
        public void when_a_GamPad_is_pressed_then_the_value_should_be_as_expeceted()
        {
            const float gamePadValue = 0.6f;

            var inputManager = new InputManager(
                getGamePadButtonPressOverride : (flag, playerIndex) => true,
                getGamePadValueOverride : (key, playerIndex) => gamePadValue,
                getKeyPressStateOverride : (key) => true
          );

            inputManager.RegisterNameToInputAction("Action", InputAction.GamePad_LeftTrigger);

            inputManager.GetPressValue("Action").Should().Be(gamePadValue);
        }

        [Test]
        public void when_a_Keyboard_Key_is_pressed_then_IsPressed_should_be_true()
        {
            var inputManager = new InputManager(
                getGamePadButtonPressOverride : (flag, playerIndex) => true,
                getGamePadValueOverride : (key, playerIndex) => 0,
                getKeyPressStateOverride : (key) => true
           );

            inputManager.RegisterNameToInputAction("Action", inputManager.InputAction.Keys[Key.A]);

            inputManager.IsPressed("Action").ShouldBeEquivalentTo(true);
        }

        [Test]
        public void multiple_ActionNames_cannot_be_registered_to_the_same_InputAction()
        {
            var inputManager = new InputManager(
                getGamePadButtonPressOverride : (flag, playerIndex) => true,
                getGamePadValueOverride : (key, playerIndex) => 0,
                getKeyPressStateOverride : (key) => true
        );
            inputManager.RegisterNameToInputAction("SomeAction", inputManager.InputAction.Keys[Key.D1]);

            Action registerSecondAction =
                () => inputManager.RegisterNameToInputAction("anotherActionUsingTheSameKey", inputManager.InputAction.Keys[Key.D1]);

            //TODO(PRUETT): im not sure argumentExcption is the intended behavior here. Robert is this the desired behavior? in my opinion this should 
            // behave similar to IOC containers. the last registration in wins. dictionaries should support this AddOrUpdate style as well
            registerSecondAction.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void when_update_is_called_three_times_and_a_key_is_pressed_for_only_one_then_IsPressed_is_only_true_one_time()
        {
            int updateCallCounter = 0;

            var inputManager = new InputManager(
                getGamePadButtonPressOverride : (flag, playerIndex) => true,
                getGamePadValueOverride : (key, playerIndex) => 0,
                getKeyPressStateOverride : (key) =>
                {
                    updateCallCounter++;
                    return updateCallCounter == 2;
                }
           );

            inputManager.RegisterNameToInputAction("Action", inputManager.InputAction.Keys[Key.N]);

            //TODO(PRUETT): these assertions might not totally adress the concern here. but in my use case in not calling inputManager.Update
            inputManager.IsPressed("Action").Should().BeFalse();
    
            inputManager.IsPressed("Action").Should().BeTrue();
 
            inputManager.IsPressed("Action").Should().BeFalse();
        
        }

    }
}
