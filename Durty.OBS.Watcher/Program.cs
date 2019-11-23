using System;
using Durty.OBS.Watcher.Contracts;
using Ninject;

namespace Durty.OBS.Watcher
{
    /* TODO / IDEAS
    - On startup check all defined actions if scene & source names are valid
    - TODO All actions should be defined in json files
    - TODO Application configuration should be outsourced to configuration json

    - Focused Window Change title should support placeholders 'DurtyMapEditor%Visual Studio' to check for multiple contain or exact window title match
    - Better Title Match check in order to prevent false "full capture activations" -> glitches (Website names)

    - Change Scene Action on focused window changed
    -> Optional only enabled when in specific scene currently

    - Change Scene Source Render/Visibility on focused window changed
    -> Support changing source render toggle for all available scenes, not only current one by setting explicit scene name
    -> Support multiple actions 
    -> Fix "race condition" whenever action is only enabled for certain scene, that is switched to inbefore cuz of switch action

    - Capture Window with tooltips mode
    -> Add support for multiple full capture actions
    -> Add support for window title match lists
    -> Remove display capture with transform on source by adding 42 to transform size y (Obs transform extension method)
    -> Use System.Windows.Forms.Screen to retrieve working area of given display (without task bar)
     */

    class Program
    {
        private static readonly IKernel Kernel = new StandardKernel();
        private static Bootstrapper _bootstrapper;

        static void Main(string[] args)
        {
            Console.Title = "Durtys OBS Watcher Tool";
            
            _bootstrapper = new Bootstrapper(Kernel);
            _bootstrapper.DefineRules();
            _bootstrapper.Run();

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            ILogger logger = Kernel.Get<ILogger>();

            logger.Write(LogLevel.Info, "Press any key to exit.");
            Console.ReadLine();
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            _bootstrapper.Stop();
        }
    }
}
