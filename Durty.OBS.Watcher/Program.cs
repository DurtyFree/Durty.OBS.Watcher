using System;
using System.Collections.Generic;
using System.Linq;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Handlers;
using Durty.OBS.Watcher.Repositories;
using Ninject;
using OBSWebsocketDotNet;

namespace Durty.OBS.Watcher
{
    /* TODO / IDEAS
    - On startup check all defined actions if scene & source names are valid
    - All actions should be defined in json files
    - Application configuration should be outsourced to configuration json
    - Maybe consider porting to .NET Console project, because of OBS /OBS WebSockets dependency & focused window change API

    - Focused Window Change title should support placeholders 'DurtyMapEditor%Visual Studio' to check for multiple contain or exact window title match

    - Change Scene Action on focused window changed
    -> Optional only enabled when in specific scene currently

    - Change Scene Source Render/Visibility on focused window changed
    -> Optional only enabled when in specific scene currently
    -> Only render source when specified window is focused
    -> Support changing source render toggle for all available scenes, not only current one

    - Capture Window with tooltips mode
    -> Add support for multiple full capture actions
    -> Set full display capture visibility to false when tool is closed
    -> Auto raise focused window watcher polling?
    -> Remove display capture with transform on source by adding 42 to transform size y (Obs transform extension method)
    -> Use System.Windows.Forms.Screen to retrieve working area of given display (without task bar)
    -> TODO: Better Title Match check in order to prevent false "full capture activations" -> glitches (Website names)
     */

    class Program
    {
        public static readonly IKernel Kernel = new StandardKernel();

        static void Main(string[] args)
        {
            Console.Title = "Durtys OBS Watcher Tool";

            Bootstrapper bootstrapper = new Bootstrapper(Kernel);
            bootstrapper.DefineRules();
            bootstrapper.Run();

            ILogger logger = Kernel.Get<ILogger>();

            logger.Write(LogLevel.Info, "Press any key to exit.");
            Console.ReadLine();
        }
    }
}
