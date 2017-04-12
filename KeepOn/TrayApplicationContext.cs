using KeepOn.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

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


        #region NativeMethods

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }

        #endregion

    }
}
