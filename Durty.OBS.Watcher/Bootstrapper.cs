using System;
using System.Collections.Generic;
using System.IO;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Handlers;
using Durty.OBS.Watcher.Loggers;
using Durty.OBS.Watcher.Models;
using Durty.OBS.Watcher.Repositories;
using Durty.OBS.Watcher.Services;
using Ninject;
using OBS.WebSocket.NET;

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
                Settings settings = context.Kernel.Get<SettingsRepository>().Get();
                ILogger logger = context.Kernel.Get<ILogger>();
                return new ObsManager(logger, settings.ObsWebSocketsIp, settings.ObsWebSocketsPort, settings.ObsWebSocketsAuthPassword);
            }).InSingletonScope();
            _kernel.Bind<ActiveWindowWatcher>().ToMethod(context => new ActiveWindowWatcher(context.Kernel.Get<SettingsRepository>().Get().WindowWatcherPollingDelay)).InSingletonScope();
            _kernel.Bind<ILogger>().To<ConsoleLogger>().InSingletonScope();
            _kernel.Bind<ObsWebSocketApi>().ToMethod(context => _kernel.Get<ObsManager>().Obs.Api);
            _kernel.Bind<WindowMatchService>().ToSelf();

            #region Repositories

            //TODO: Outsource this
            string configBaseFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configs");
            if (!Directory.Exists(configBaseFolderPath))
            {
                Directory.CreateDirectory(configBaseFolderPath);
            }

            _kernel.Bind<FocusedWindowSceneSwitchActionRepository>().ToMethod(context => new FocusedWindowSceneSwitchActionRepository(configBaseFolderPath, context.Kernel.Get<ILogger>())).InSingletonScope();
            _kernel.Bind<FocusedWindowSourceVisibilityActionRepository>().ToMethod(context => new FocusedWindowSourceVisibilityActionRepository(configBaseFolderPath, context.Kernel.Get<ILogger>())).InSingletonScope();
            _kernel.Bind<CaptureFullWindowActionRepository>().ToMethod(context => new CaptureFullWindowActionRepository(configBaseFolderPath, context.Kernel.Get<ILogger>())).InSingletonScope();
            _kernel.Bind<SettingsRepository>().ToMethod(context => new SettingsRepository(configBaseFolderPath, context.Kernel.Get<ILogger>())).InSingletonScope();

            #endregion
            
            #region Handlers

            _kernel.Bind<IHandler>().To<FocusedWindowSourceVisibilityHandler>().InSingletonScope();
            _kernel.Bind<IHandler>().To<FocusedWindowSceneSwitchHandler>().InSingletonScope();
            _kernel.Bind<IHandler>().To<FocusedWindowChangedDebugHandler>().InSingletonScope();
            _kernel.Bind<IHandler>().To<FullCaptureWindowFocusedChangedHandler>().InSingletonScope();

            #endregion
        }

        public void Run()
        {
            ObsManager obsManager = _kernel.Get<ObsManager>();
            Settings settings = _kernel.Get<SettingsRepository>().Get();
            ILogger logger = _kernel.Get<ILogger>();

            logger.Write(LogLevel.Info, $"Trying to connect to WebSockets {settings.ObsWebSocketsIp}:{settings.ObsWebSocketsPort} ...");
            if (!obsManager.Connect())
            {
                logger.Write(LogLevel.Error, "Failed to connect.");
                logger.Write(LogLevel.Info, "Press any key to exit.");
                Console.ReadLine();
                return;
            }

            _kernel.Get<ActiveWindowWatcher>().Start();

            List<IHandler> handlers = _kernel.Get<List<IHandler>>();
            if (settings.DebugMode)
            {
                foreach (var handler in handlers)
                {
                    logger.Write(LogLevel.Debug, $"Started {handler.GetType().Name}");
                }
            }

            logger.Write(LogLevel.Info, "Successfully connected.");
        }

        public void Stop()
        {
            _kernel.Get<ActiveWindowWatcher>().Stop();
        }
    }
}
