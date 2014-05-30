using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Architecture
{
    public abstract class GameModule : Module
    {
        public GameModule()
        {
        }

        public abstract void Update(float dt);

        public override void DerivedUpdate(float dt)
        {
            Update(dt);
        }

        public static T Get<T>() where T : GameModule
        {
            return TheGame.Game.GetGameModule<T>();
        }
    }
}
