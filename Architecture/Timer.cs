using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Architecture
{
    public class Timer
    {
        Stopwatch _timer = new Stopwatch();
        long _last = 0;
        long _dt = 0;
        long _cap = -1;

        public float Dt
        {
            get 
            {
                return _cap / 1000.0f;
            }
        }

        public Timer(float capInHz)
        {
            _cap = (long)(1 / capInHz * 1000);
            _timer.Start();
        }

        public void Update()
        {
            while (_timer.ElapsedMilliseconds - _last < _cap)
                Thread.Sleep(0);
            _dt = _timer.ElapsedMilliseconds - _last;
            _last = _timer.ElapsedMilliseconds;
        }
    }
}
