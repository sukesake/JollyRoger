using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Architecture
{
    // Basic module class.  Inherit from ComponentModule, GameModule, or LevelModule
    public abstract class Module
    {
        protected bool _active = true;
        public string _name;

        public Module()
        {

        }

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public abstract void DerivedUpdate(float dt);

        public void BaseUpdate(float dt)
        {
            if (_active)
            {
                DerivedUpdate(dt);
            }
        }
    }
}
