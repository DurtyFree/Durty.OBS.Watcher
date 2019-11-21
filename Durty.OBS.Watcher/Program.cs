using System;
using Durty.OBS.Watcher.Contracts;
using Ninject;

namespace Durty.OBS.Watcher
{
    /* TODO / IDEAS
    - On startup check all defined actions if scene & source names are valid
    - All actions should be defined in json files
    - Application configuration should be outsourced to configuration json

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
    -> Add support for window title match lists
    -> Add setting & support to additionaly include all child / spawned sub processes for the given window title match
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
