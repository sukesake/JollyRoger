using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace RenderEngine
{
    public class Mouse
    {
        [DllImport("user32.dll")]
        public static extern int ShowCursor(bool bShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        private IntPtr WindowHandle;

        public Mouse(IntPtr windowHandle)
        {
            WindowHandle = windowHandle;

            ShowCursor(false);
        }

        public MouseState GetState()
        {
            MouseState mouseState = new MouseState();

            Point point;
            GetCursorPos(out point);

            ScreenToClient(WindowHandle, ref point);

            mouseState.x = point.X;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            mouseState.y = point.Y;

            MouseButtonState buttonState1 = (MouseButtonState) ((uint) (ushort) GetAsyncKeyState(1) >> 15);
            mouseState.leftButton = buttonState1;
            MouseButtonState buttonState2 = (MouseButtonState) ((uint) (ushort) GetAsyncKeyState(4) >> 15);
            mouseState.middleButton = buttonState2;
            MouseButtonState buttonState3 = (MouseButtonState) ((uint) (ushort) GetAsyncKeyState(2) >> 15);
            mouseState.rightButton = buttonState3;
            MouseButtonState buttonState4 = (MouseButtonState) ((uint) (ushort) GetAsyncKeyState(5) >> 15);
            mouseState.xb1 = buttonState4;
            MouseButtonState buttonState5 = (MouseButtonState) ((uint) (ushort) GetAsyncKeyState(6) >> 15);
            mouseState.xb2 = buttonState5;

            return mouseState;
        }

        public void SetPosition(int x, int y)
        {
            Point point = new Point(x, y);

            ClientToScreen(WindowHandle, ref point);
            SetCursorPos(point.X, point.Y);
        }
    }
}