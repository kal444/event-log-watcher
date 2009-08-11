using System;
using System.Configuration;
using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;

namespace EventLogWatcher
{
    public class Watcher
    {
        // query template for querying WMI objects
        private const string QueryTemplate =
            "SELECT * " + 
            "FROM __InstanceCreationEvent " + 
            "WHERE TargetInstance ISA 'Win32_NTLogEvent' " + 
            "AND TargetInstance.LogFile = '{0}' " + 
            "AND TargetInstance.EventType = {1} " + 
            "AND TargetInstance.EventCode = {2}";

        private readonly string _logFileName;
        private readonly string _eventType;
        private readonly string _eventCode;
        private readonly string _partialPattern;
        private readonly string _runnable;

        private readonly EventQuery _query;
        private readonly ManagementEventWatcher _watcher;

        public Watcher()
        {
            // look up configurations from the app config file
            _logFileName = ConfigurationManager.AppSettings["logFileName"];
            _eventType = ConfigurationManager.AppSettings["eventType"];
            _eventCode = ConfigurationManager.AppSettings["eventCode"];
            _partialPattern = ConfigurationManager.AppSettings["partialPattern"];
            _runnable = ConfigurationManager.AppSettings["runnable"];

            _query = new EventQuery(String.Format(QueryTemplate, _logFileName, _eventType, _eventCode));

            _watcher = CreateWatcher();
        }

        // call back event so that we can notify the caller what program was started
        public Action<string> EventTriggered { get; set; }

        public void Start()
        {
            _watcher.Start();
        }

        public void Stop()
        {
            _watcher.Stop();
        }

        private ManagementEventWatcher CreateWatcher()
        {
            var watcher = new ManagementEventWatcher(_query)
            {
                Options =
                {
                    // time out in 30 seconds
                    Timeout = new TimeSpan(0, 0, 30)
                }
            };
            // needs this to read security log even with admin running it
            watcher.Scope.Options.EnablePrivileges = true;
            watcher.EventArrived += WatcherEventArrived;

            return watcher;
        }

        private void WatcherEventArrived(object sender, EventArrivedEventArgs e)
        {
            var target = e.NewEvent;
            var evt = (ManagementBaseObject)target["TargetInstance"];
            var message = (string)evt["Message"];
            if (message != null && Regex.Match(message, _partialPattern).Success)
            {
                // sleep 250 ms
                Thread.Sleep(250);

                // run the runnable defined
                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = _runnable
                    }
                };
                proc.Start();

                if (EventTriggered != null)
                {
                    EventTriggered(_runnable);
                }
            }
        }
    }
}
