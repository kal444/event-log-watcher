using System;
using System.Windows.Forms;

namespace EventLogWatcher.WinForms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var applicationContext = new WatcherApplicationContext();
            Application.Run(applicationContext);
        }
    }
}
