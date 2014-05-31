using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Windows;
using SharpHelper;

namespace JollyRoger
{
    public class InputModule
    {
        public InputModule()
        {

        }

        public Point GetMousePosition()
        {
            var position = Cursor.Position;
            return new Point(position.X, position.Y);
        }

    }
}