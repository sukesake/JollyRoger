using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Architecture;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

namespace Architecture
{
    public class Level
    {
        public static Level Current
        {
            get { return TheGame.Game.LevelManager.CurLevel; }
        }

        // All Game Objects in the level
        Dictionary<string, List<GameObject>> _gameObjects = new Dictionary<string, List<GameObject>>();
        // The current instance number to be used for the next game object created
        Dictionary<string, int> _instanceList = new Dictionary<string, int>();
        // All the LevelModules and ComponentsModules in the level
        Dictionary<Type, LevelModule> _modules = new Dictionary<Type, LevelModule>();
        // The protoypes game objects are created from
        Dictionary<string, GameObject> _prototypes = new Dictionary<string, GameObject>();
        // Name of the level
        string _name;

        bool _init = false;

        public Level()
        {

        }

        public void Dispose()
        {

        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public void Init()
        {
            _init = true;
        }

        public void Update(float dt)
        {
            if(_init == false)
                Init();

            foreach (var list in _gameObjects)
            {
                list.Value.RemoveAll(go => { return go.GetRemove(); });
            }

            foreach (var item in _modules)
            {
                item.Value.BaseUpdate(dt);
            }
        }

        public T GetModule<T>() where T : LevelModule
        {
            if(_modules.ContainsKey(typeof(T)))
            {
                return _modules[typeof(T)] as T;
            }
            return null;
        }

        public void AddModule<T>(T m) where T : LevelModule
        {
            _modules.Add(typeof(T), m);
        }

        public void LoadPrototype(string name)
        {
            // Todo: serialization code for loading game objects   
        }

        public void SetPrototype(GameObject go)
        {
            _prototypes[go.Name] = go;
            go.Instance = -1;
            _instanceList.Add(go.Name, 0);
            _gameObjects.Add(go.Name, new List<GameObject>());
            go.SetActive(false);
        }

        public GameObject NewFromPrototype(string name)
        {
            if (_prototypes.ContainsKey(name) == false)
            {
                LoadPrototype(name);
            }

            GameObject go = _prototypes[name].Clone(GetNextInstanceNumber(name));
            _gameObjects[name].Add(go);
            return go;
        }

        private int GetNextInstanceNumber(string name)
        {
            return _instanceList[name]++;
        }
    }
}
