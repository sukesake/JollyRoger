using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms.VisualStyles;
using Munq;
using RenderEngine;
using SharpDX.Direct3D10;
using SharpDX.Windows;
using SharpHelper;

namespace Game
{
    class Game : IDisposable
    {
        private World _world;
        private bool _started;

        public void Run()
        {
            var container = Configure(new IocContainer());
            
            _world = container.Resolve<World>();
            _started = true;

            try
            {
                ThreadPool.QueueUserWorkItem(DrawLoop);

                RenderLoop.Run(container.Resolve<WorldRenderer>().RenderForm, UpdateLoop);
            }
            finally
            {
                _started = false;
            }
        }

        private static IocContainer Configure(IocContainer container)
        {
            container.RegisterInstance<WorldRenderer>(WorldRenderer.Create());
            container.Register<ChunkBuilder>(r => new ChunkBuilder(container.Resolve<WorldRenderer>().Device));
            container.Register<World>(r => new World(container.Resolve<WorldRenderer>(), container.Resolve<ChunkBuilder>()));

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