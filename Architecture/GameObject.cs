using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Architecture;
using System.Xml;

namespace Architecture
{
    public class GameObject 
    {
        protected bool _remove = false;
        
        protected bool _active = true;

        private string _name;
        private Dictionary<Type, Component> _components = new Dictionary<Type, Component>();

        private int _instance;
        private Level _owner;

        public bool GetActive()
        {
            return _active;
        }

        public bool GetRemove()
        {
            return _remove;
        }

        public Level Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public Dictionary<Type, Component> Components
        {
            get { return _components; }
            set { _components = value; }
        }

        public int Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public GameObject()
        {
        }

        public GameObject(string name, int inst, Level owner)
        {
            _name = name;
            _instance = inst;
            _owner = owner;
        }

        public void SetActive(bool set)
        {
            _active = set;
            foreach (var item in _components)
            {
                item.Value.SetActive(set);
            }
        }

        public void SetRemove(bool set)
        {
            _remove = set;
            foreach (var item in _components)
            {
                item.Value.SetRemove(set);
            }
        }

        public void AddComp<T>(T c) where T : Component
        {
            _components.Add(typeof(T), c);
            c.Parent = this;
        }

        public T GetComp<T>() where T : Component
        {
            if (_components.ContainsKey(typeof(T)))
            {
                return _components[typeof(T)] as T;
            }
            return null;
        }

        public GameObject Clone(int instance)
        {
            GameObject go = new GameObject(Name, instance, Owner);
            foreach (var item in _components)
            {
                Component c = item.Value.Clone(go);
            }
            return go;
        }

        public void RemoveComp<T>()
        {
            if (_components.ContainsKey(typeof(T)))
            {
                _components.Remove(typeof(T));
            }
        }
    }
}
