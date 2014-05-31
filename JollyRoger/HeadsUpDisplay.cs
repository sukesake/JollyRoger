using SharpDX;
using SharpHelper;

namespace JollyRoger
{
    public class HeadsUpDisplay
    {
        private readonly SharpBatch _batch;
        private readonly SharpDevice _device;
        private readonly SharpFPS _fpsCounter;
        private string[] _messages = new string[0];

        public HeadsUpDisplay(SharpDevice device, SharpFPS fpsCounter)
        {
            _device = device;
            _fpsCounter = fpsCounter;
            _batch = new SharpBatch(_device, "textfont.dds");
            fpsCounter.Reset();
        }

        private void Resize()
        {
            if (_device.MustResize)
            {
                _device.Resize();
                _batch.Resize();
            }
        }

        public void Update(string[] messages)
        {
            Resize();
            _messages = messages;
        }

        public void Draw()
        {
            //updating the fpsCounter here may seem counterintuitive but we need to count based on the draw thread
            _fpsCounter.Update();
            _batch.Begin();
            _batch.DrawString("FPS: " + _fpsCounter.FPS, 0, 0, Color.White);
            _batch.DrawString("Press W for wireframe, S for solid", 0, 30, Color.White);
            _batch.DrawString("Press From 1 to 5 for Alphablending", 0, 60, Color.White);
            _batch.DrawString("Press + or - to change the terrain base height", 0, 120, Color.White);
            _batch.DrawString("Use arrow keys, pageUp, or pageDown, to look around", 0, 150, Color.White);
            var nextmessageLocation = 180;
            foreach (var message in _messages)
            {
                _batch.DrawString(message, 0, nextmessageLocation, Color.White);
                nextmessageLocation += 30;
            }
            _batch.End();
        }
    }
}