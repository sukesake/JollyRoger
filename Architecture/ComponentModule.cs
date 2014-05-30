using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Architecture
{
    // Base class for Modules in a level that give and own Components
    public abstract class ComponentModule : LevelModule
    {
        protected List<Component> _components = new List<Component>();

        public ComponentModule(Level owner)
            : base(owner)
        {

        }

        public override void DerivedUpdate(float dt)
        {
            base.DerivedUpdate(dt);
            _components.RemoveAll(comp => { return comp.GetRemove(); });
            Update(dt);
        }

        public void RegisterComp(Component c)
        {
            _components.Add(c);
        }

        public abstract Component GetComp(GameObject go);
    }
}
