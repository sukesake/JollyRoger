using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Architecture
{
    public class TheGame
    {
        static TheGame _game;
        public static TheGame Game
        {
            get { return TheGame._game; }
        }

        LevelManager _manager = new LevelManager();
        public LevelManager LevelManager
        {
            get { return _manager; }
        }

        Timer _timer = new Timer(60);
        Dictionary<Type, GameModule> _modules = new Dictionary<Type, GameModule>(); 
        bool _end = false;

        public void AddGameModule<T>(T mod) where T : GameModule
        {
            _modules.Add(typeof(T), mod);
        }

        public T GetGameModule<T>() where T : GameModule
        {
            if (_modules.ContainsKey(typeof(T)))
            {
                return _modules[typeof(T)] as T;
            }
            return null;
        }

        public static void Start()
        {
            _game = new TheGame();
        }

        public static void Run()
        {
            _game.RunImpl();
        }

        private void RunImpl()
        {
            while (!_end)
            {
                _timer.Update();
                _manager.Update(_timer.Dt);

                foreach (var item in _modules)
                {
                    item.Value.BaseUpdate(_timer.Dt);
                }
            }
        }
    }
}
