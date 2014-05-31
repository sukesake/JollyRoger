namespace Architecture
{
    public class InputEvent
    {
        public event InputManager.InputEventCallbackDelegate Event;

        public void Fire(int playerIndex, string actionName, float value)
        {
            Event(playerIndex, actionName, value);
        }
    }
}