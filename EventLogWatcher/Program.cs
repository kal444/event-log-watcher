namespace EventLogWatcher
{
    /// <summary>
    /// Tester program to try out the watcher function
    /// </summary>
    class Program
    {
        static void Main()
        {
            var watcher = new Watcher();

            watcher.Start();

            // wait for events
            System.Threading.Thread.Sleep(60 * 1000);

            watcher.Stop();
        }

    }
}
