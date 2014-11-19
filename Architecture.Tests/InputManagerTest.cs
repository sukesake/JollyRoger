using System;
using FluentAssertions;
using NUnit.Framework;
using System.Windows.Input;
using SharpDX.XInput;

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

            inputManager.RegisterNameToInputAction("Action", InputAction.Keys[Key.A]);

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
            inputManager.RegisterNameToInputAction("SomeAction", InputAction.Keys[Key.D1]);

            Action registerSecondAction =
                () => inputManager.RegisterNameToInputAction("anotherActionUsingTheSameKey", InputAction.Keys[Key.D1]);

            //TODO(PRUETT): im not sure argumentExcption is the intended behavior here. Robert is this the desired behavior? in my opinion this should 
            // behave similar to IOC containers. the last registration in wins. dictionaries should support this AddOrUpdate style as well
            // (Robert): I'm not a big fan of silent overrides.  I prefer big failures, or change the name of the function to reflect the potential override.
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

            inputManager.RegisterNameToInputAction("Action", InputAction.Keys[Key.N]);

            //TODO(PRUETT): these assertions might not totally adress the concern here. but in my use case in not calling inputManager.Update
            inputManager.IsPressed("Action").Should().BeFalse();
    
            inputManager.IsPressed("Action").Should().BeTrue();
 
            inputManager.IsPressed("Action").Should().BeFalse();
        }

        [Test]
        public void when_update_has_a_gamepad_button_pressed_the_first_time_it_fires_the_triggered_event()
        {
            bool pressed = false;
            var inputManager = new InputManager(
                getGamePadButtonPressOverride: (flag, playerIndex) => pressed,
                getGamePadValueOverride: (key, playerIndex) => 0,
                getKeyPressStateOverride: (key) => false);

            inputManager.RegisterNameToInputAction("Action", InputAction.GamePad_A);

            int triggeredCallCounter = 0;
            inputManager.AddOnTriggerCallback("Action", (index, name, value) => ++triggeredCallCounter);

            inputManager.Update(1);
            triggeredCallCounter.Should().Be(0);

            pressed = true;
            inputManager.Update(1);
            triggeredCallCounter.Should().Be(1);

            pressed = false;
            inputManager.Update(1);
            triggeredCallCounter.Should().Be(1);
        }

        [Test]
        public void two_players_can_register_different_gamepad_buttons_for_the_same_action_name_and_get_the_correct_IsPressed_values()
        {
            int player1PressCount = 0;
            int player2PressCount = 0;
            var pressOverride = new InputAction.GamePadButtonPressDelegate((flag, playerIndex) =>
            {
                if (playerIndex == 0 && flag == GamepadButtonFlags.A)
                {
                    player1PressCount++;
                    return true;
                }
                else if (playerIndex == 1 && flag == GamepadButtonFlags.B)
                {
                    player2PressCount++;
                    return true;
                }
                return false;
            });

            var inputManager = new InputManager(
                pressOverride,
                getGamePadValueOverride: (key, playerIndex) => 0,
                getKeyPressStateOverride: (key) => false);

            inputManager.RegisterNameToInputAction("Action", InputAction.GamePad_A, 0);
            inputManager.RegisterNameToInputAction("Action", InputAction.GamePad_B, 1);

            inputManager.IsPressed("Action", 0).Should().BeTrue();
            inputManager.IsPressed("Action", 1).Should().BeTrue();

            player1PressCount.Should().Be(1);
            player2PressCount.Should().Be(1);
        }

        //TODO(PRUETT): what you can do here is write the basic tests and have them throw not implemented. this reminds you a lot better than comments.

        //Todo: Two players can register the same action name to different keyboard keys and get the correct IsPressed values
        //Todo: Two players can register for different OnTriggered gamepad buttons and each get called once
        //Todo: OnTriggered works properly for Press value events (such as thumb sticks or triggers)
        //Todo: Registering two action names for the same player's input action results in last input action being used
        //Todo: Two players register same action name for different keys, one OnTriggered event registers for that action and receives callbacks for both players

        [Test]
        public void triggers_can_be_unregistered()
        {
            throw new NotImplementedException();
        }
    }
}
