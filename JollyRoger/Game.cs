using System;
using System.Threading;
using Architecture;
using Munq;
using SharpDX.Windows;
using SharpHelper;

namespace JollyRoger
{
    class Game : IDisposable
    {
        private World _world;
        private bool _started;

        public void Run()
        {
            var container = Configure(new IocContainer());
            container.Resolve<InputModule>().RegisterKeybindings();

            _world = container.Resolve<World>();
            _started = true;

            try
            {
                ThreadPool.QueueUserWorkItem(DrawLoop);
                RenderLoop.Run(container.Resolve<RenderForm>(), UpdateLoop);
            }
            finally
            {
                _started = false;
            }
        }

        private static IocContainer Configure(IocContainer container)
        {
            container.RegisterInstance(new RenderForm());
            container.RegisterInstance(new SharpDevice(container.Resolve<RenderForm>()));
            container.RegisterInstance(new InputModule(new InputManager(), container.Resolve<RenderForm>()));
            container.Register<Camera>(r => new Camera(container.Resolve<SharpDevice>(), container.Resolve<InputModule>()));
            container.Register<World>(
                r =>
                    new World(container.Resolve<SharpDevice>(), 
                              container.Resolve<RenderForm>(),
                              container.Resolve<HeadsUpDisplay>(), 
                              container.Resolve<Camera>()));

            return container;
        }

        private void DrawLoop(object obj)
        {
            while (_started)
            {
                var now = DateTime.Now;

                _world.Draw();

                var ms = (DateTime.Now - now).Milliseconds;
                //TODO(PRUETT): this is funky. fixit!
                Thread.Sleep(Math.Max(0, 1000 / 60 - ms));
            }
        }

        [STAThread]
        private void UpdateLoop()
        {
            var now = DateTime.Now;

            _world.Update();

            var ms = (DateTime.Now - now).Milliseconds;
            //TODO(PRUETT): this is funky. fixit!
            Thread.Sleep(Math.Max(0, 1000 / 100 - ms));
        }

        public void Dispose()
        {
        }
    }
}