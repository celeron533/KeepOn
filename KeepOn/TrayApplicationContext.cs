using KeepOn.Properties;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static KeepOn.Native;

namespace KeepOn
{
    public class TrayApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private AboutBox aboutBox = new AboutBox();

        public TrayApplicationContext()
        {
            // Prevent system auto lock or shut down the monitor
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS |
                                    EXECUTION_STATE.ES_DISPLAY_REQUIRED |
                                    EXECUTION_STATE.ES_SYSTEM_REQUIRED);

            // Resume everything when exit the application
            Application.ApplicationExit += (sender, e) =>
            {
                SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            };

            // Initialize tray icon
            trayIcon = new NotifyIcon()
            {
                Icon = Resources.AppIcon,
                Text = "'KeepOn' is running",
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("About", About),
                    new MenuItem("Exit", Exit)
            }),
                Visible = true
            };

            trayIcon.BalloonTipText = trayIcon.Text;
            trayIcon.ShowBalloonTip(3000);
        }


        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user move mouse over it
            trayIcon.Visible = false;
            Application.Exit();
        }


        void About(object sender, EventArgs e)
        {
            if (!aboutBox.Visible)
                aboutBox.ShowDialog();
        }
    }
}
