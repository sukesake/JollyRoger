using System;
using SharpDX.DirectInput;

namespace JollyRoger
{
    public class InputModule
    {
        private readonly Keyboard _keyboard;
        private readonly Mouse _mouse;

        public InputModule(Keyboard keyboard, Mouse mouse)
        {
            if (keyboard == null) throw new ArgumentNullException("keyboard");
            if (mouse == null) throw new ArgumentNullException("mouse");
            
            _keyboard = keyboard;
            _mouse = mouse;
        }

        public void GetMouseCoordinates()
        {
            //return _mouse.
        }

    }
}