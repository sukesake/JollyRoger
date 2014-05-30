using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Architecture
{
    // Base class for modules whose functionality is 
    // tied to the lifetime of the level
    public abstract class LevelModule : Module
    {
        private Level _owner;

        public LevelModule(Level owner)
        {
            _owner = owner;
        }

        public override void DerivedUpdate(float dt)
        {
            Update(dt);
        }

        public abstract void Update(float dt);

        public static T Get<T>() where T : LevelModule
        {
            return Level.Current.GetModule<T>();
        }
    }
}
