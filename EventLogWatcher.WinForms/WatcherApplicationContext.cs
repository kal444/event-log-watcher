using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EventLogWatcher.WinForms
{
    class WatcherApplicationContext : ApplicationContext
    {
        private const string AppName = "EventLogWatcher";
        private const string TrayAppName = AppName + " Tray App";
        private const String StatusMessageTemplate = "{0} triggered on {1}";

        private IContainer _components;
        private NotifyIcon _watcherNotifyIcon;
        private ContextMenu _watcherNotifyIconContextMenu;
        private MenuItem _exitContextMenuItem;

        private string _lastStatusMessage = "No events triggered";
        private readonly Watcher _watcher;

        public WatcherApplicationContext()
        {
            InitializeContext();

            _watcher = new Watcher()
                           {
                               EventTriggered = WatcherTriggered
                           };
            _watcher.Start();
        }

        private void InitializeContext()
        {
            _components = new Container();

            _exitContextMenuItem = new MenuItem
                                       {
                                           Index = 1,
                                           Text = "&Exit"
                                       };
            _exitContextMenuItem.Click += ExitContextMenuItemClick;

            _watcherNotifyIconContextMenu = new ContextMenu();
            _watcherNotifyIconContextMenu.MenuItems.Add(_exitContextMenuItem);

            _watcherNotifyIcon = new NotifyIcon(_components)
                                    {
                                        Icon = new Icon(typeof(WatcherApplicationContext), "Resources.watcher.ico"),
                                        ContextMenu = _watcherNotifyIconContextMenu,
                                        Text = TrayAppName,
                                        Visible = true
                                    };
            _watcherNotifyIcon.Click += WatcherNotifyIconClick;
        }

        private void WatcherTriggered(string statusMessage)
        {
            _lastStatusMessage = String.Format(StatusMessageTemplate, statusMessage, DateTime.Now);
            ShowBalloon();
        }

        private void WatcherNotifyIconClick(object sender, EventArgs e)
        {
            ShowBalloon();
        }

        private void ExitContextMenuItemClick(object sender, EventArgs e)
        {
            _watcher.Stop();
            ExitThread();
        }

        private void ShowBalloon()
        {
            _watcherNotifyIcon.BalloonTipTitle = AppName;
            _watcherNotifyIcon.BalloonTipText = GetLastStatusMessage();

            _watcherNotifyIcon.ShowBalloonTip(1000);
        }

        private string GetLastStatusMessage()
        {
            return "Last status: " + _lastStatusMessage;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                {
                    _components.Dispose();
                }
            }
        }

    }
}
