using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RenderEngine
{
    public class Keyboard
    {
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        public bool IsKeyPressed(Keys key)
        {
            bool keyPressed = false;
            short result = GetKeyState((int)key);

            switch (result)
            {
                case 0:
                    // Not pressed and not toggled on.
                    keyPressed = false;
                    break;

                case 1:
                    // Not pressed, but toggled on
                    keyPressed = false;
                    break;

                default:
                    // Pressed (and may be toggled on)
                    keyPressed = true;
                    break;
            }

            return keyPressed;
        }
    }
}