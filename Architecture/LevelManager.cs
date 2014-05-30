using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Architecture
{
    public class LevelManager
    {
        Level _curLevel, _nextLevel;
        bool _endCurLevel = false;

        public Level CurLevel
        {
            get { return _curLevel; }
            set { _curLevel = value; }
        }

        public Level NextLevel
        {
            get { return _nextLevel; }
            set { _nextLevel = value; }
        }

        public void Update(float dt)
        {
            _curLevel.Update(dt);

            if (_endCurLevel == true)
            {
                _curLevel.Dispose();
                _curLevel = _nextLevel;
                _nextLevel = null;
            }
        }

        public void EndCurrentLevel()
        {
            _endCurLevel = true;
        }
    }
}
