using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Security;

namespace Architecture
{
    public abstract class Component
    {
        protected bool _remove = false;
        protected bool _active = true;
        protected  GameObject _parent;
        protected ComponentModule _owner;

        public bool GetActive()
        {
            return _active;
        }

        public void SetActive(bool set)
        {
            _active = set;
        }

        public bool GetRemove()
        {
            return _remove;
        }

        public void SetRemove(bool set)
        {
            _remove = set;
        }

        public ComponentModule Owner
        {
            get { return _owner; }
        }

        public GameObject Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public Component()
        {
            _active = true;
            _remove = false;
        }

        public Component(ComponentModule owner, GameObject parent)
        {
            _owner = owner;
            _active = true;
            _remove = false;
            _parent = parent;
            if (parent != null)
                parent.AddComp(this);
        }

        public Component(int index, ComponentModule owner, GameObject parent)
        {
            _owner = owner;
            _active = true;
            _remove = false;
            _parent = parent;
            if(parent != null)
                parent.AddComp(this);
        }

        public void SetValues(ComponentModule owner, GameObject parent)
        {
            _owner = owner;
            _active = true;
            _remove = false;
            _parent = parent;
            if (owner != null)
            {
                owner.RegisterComp(this);
            }
        }

        public virtual void Update(float dt)
        {

        }

        public abstract Component Clone();
    }
}
