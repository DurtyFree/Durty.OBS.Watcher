using System;
using System.Collections.Generic;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Handlers;
using Durty.OBS.Watcher.Models;
using Durty.OBS.Watcher.Repositories;
using Ninject;

namespace Durty.OBS.Watcher
{
    public class Bootstrapper
    {
        private readonly IKernel _kernel;

        public Bootstrapper(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void DefineRules()
        {
            _kernel.Bind<ObsManager>().ToMethod(context =>
            {
                var settingsRepository = context.Kernel.Get<SettingsRepository>();
                var settings = settingsRepository.Get();
                return new ObsManager(settings.ObsWebSocketsIp, settings.ObsWebSocketsPort, settings.ObsWebSocketsAuthPassword);
            }).InSingletonScope();
            _kernel.Bind<FocusedWindowChangeActionRepository>().ToSelf().InSingletonScope();
            _kernel.Bind<CaptureFullWindowActionRepository>().ToSelf().InSingletonScope();
            _kernel.Bind<SettingsRepository>().ToSelf().InSingletonScope();
            _kernel.Bind<ActiveWindowWatcher>().ToMethod(context => new ActiveWindowWatcher(100)).InSingletonScope();
            _kernel.Bind<IHandler>().To<FocusedWindowChangedHandler>().InSingletonScope();
            _kernel.Bind<IHandler>().To<FocusedWindowChangedDebugHandler>().InSingletonScope();
        }

        public void Run()
        {
            ObsManager obsManager = _kernel.Get<ObsManager>();
            Settings settings = _kernel.Get<SettingsRepository>().Get();

            Console.WriteLine($"Trying to connect to WebSockets {settings.ObsWebSocketsIp}:{settings.ObsWebSocketsPort} ...");
            if (!obsManager.Connect())
            {
                Console.WriteLine("Failed to connect.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
                return;
            }

            _kernel.Get<ActiveWindowWatcher>().Start();

            List<IHandler> handlers = _kernel.Get<List<IHandler>>();
            if (settings.DebugMode)
            {
                foreach (var handler in handlers)
                {
                    Console.WriteLine($"[DEBUG] Started {handler.GetType().Name}");
                }
            }

            Console.WriteLine("Successfully connected.");
        }

        public void Stop()
        {
            _kernel.Get<ActiveWindowWatcher>().Stop();
        }
    }
}
