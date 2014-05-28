using System;
using System.Globalization;

namespace RenderEngine
{

    public struct MouseState
    {
        internal int x;
        internal int y;
        internal MouseButtonState leftButton;
        internal MouseButtonState rightButton;
        internal MouseButtonState middleButton;
        internal MouseButtonState xb1;
        internal MouseButtonState xb2;
        internal int wheel;

        public int X
        {
            get { return this.x; }
        }

        public int Y
        {
            get { return this.y; }
        }

        public MouseButtonState LeftButton
        {
            get { return this.leftButton; }
        }

        public MouseButtonState RightButton
        {
            get { return this.rightButton; }
        }

        public MouseButtonState MiddleButton
        {
            get { return this.middleButton; }
        }

        public MouseButtonState XButton1
        {
            get { return this.xb1; }
        }

        public MouseButtonState XButton2
        {
            get { return this.xb2; }
        }

        public int ScrollWheelValue
        {
            get { return this.wheel; }
        }

        public MouseState(int x, int y, int scrollWheel, MouseButtonState leftButton, MouseButtonState middleButton,
            MouseButtonState rightButton, MouseButtonState xButton1, MouseButtonState xButton2)
        {
            this.x = x;
            this.y = y;
            this.wheel = scrollWheel;
            this.leftButton = leftButton;
            this.rightButton = rightButton;
            this.middleButton = middleButton;
            this.xb1 = xButton1;
            this.xb2 = xButton2;
        }

        public static bool operator ==(MouseState left, MouseState right)
        {
            if (left.x == right.x && left.y == right.y &&
                (left.leftButton == right.leftButton && left.rightButton == right.rightButton) &&
                (left.middleButton == right.middleButton && left.xb1 == right.xb1 && left.xb2 == right.xb2))
                return left.wheel == right.wheel;
            else
                return false;
        }

        public static bool operator !=(MouseState left, MouseState right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.leftButton.GetHashCode() ^
                   this.rightButton.GetHashCode() ^ this.middleButton.GetHashCode() ^ this.xb1.GetHashCode() ^
                   this.xb2.GetHashCode() ^ this.wheel.GetHashCode();
        }

        public override string ToString()
        {
            string str = string.Empty;
            if (this.leftButton == MouseButtonState.Pressed)
                str = str + (string.IsNullOrEmpty(str) ? "" : " ") + "Left";
            if (this.rightButton == MouseButtonState.Pressed)
                str = str + (string.IsNullOrEmpty(str) ? "" : " ") + "Right";
            if (this.middleButton == MouseButtonState.Pressed)
                str = str + (string.IsNullOrEmpty(str) ? "" : " ") + "Middle";
            if (this.xb1 == MouseButtonState.Pressed)
                str = str + (string.IsNullOrEmpty(str) ? "" : " ") + "XButton1";
            if (this.xb2 == MouseButtonState.Pressed)
                str = str + (string.IsNullOrEmpty(str) ? "" : " ") + "XButton2";
            if (string.IsNullOrEmpty(str))
                str = "None";
            return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{{X:{0} Y:{1} Buttons:{2} Wheel:{3}}}",
                (object) this.x, (object) this.y, (object) str, (object) this.wheel);
        }

        public override bool Equals(object obj)
        {
            if (obj is MouseState)
                return this == (MouseState) obj;
            else
                return false;
        }
    }

}