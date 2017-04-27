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
        private Config config = new Config();
        private ContextMenu trayMenu;

        private MenuItem EnableItem;
        private MenuItem AboutItem;
        private MenuItem ExitItem;

        public TrayApplicationContext()
        {
            SetState(config);
            InitMenu();

            // Resume everything when exit the application
            Application.ApplicationExit += (sender, e) =>
            {
                UnsetState();
            };


            // Initialize tray icon
            trayIcon = new NotifyIcon()
            {
                Icon = Resources.AppIcon,
                Text = "'KeepOn' is running",
                ContextMenu = trayMenu,
                Visible = true
            };

            trayIcon.BalloonTipText = trayIcon.Text;
            trayIcon.ShowBalloonTip(3000);
        }

        void InitMenu()
        {
            trayMenu = new ContextMenu(new MenuItem[] {
                         EnableItem = new MenuItem("Enable", Enable_Clicked),
                         AboutItem = new MenuItem("About", About_Clicked),
                         ExitItem = new MenuItem("Exit", Exit_Clicked)
            });
            RefreshMenu();
        }

        void RefreshMenu()
        {
            EnableItem.Checked = config.isEnable;
        }

        void Enable_Clicked(object sender, EventArgs e)
        {
            config.isEnable = !config.isEnable;
            SetState(config);
            RefreshMenu();
        }

        void About_Clicked(object sender, EventArgs e)
        {
            if (!aboutBox.Visible)
                aboutBox.ShowDialog();
        }

        void Exit_Clicked(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user move mouse over it
            trayIcon.Visible = false;
            Application.Exit();
        }


        EXECUTION_STATE SetState(Config config)
        {
            if (config.isEnable)
            {
                return SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS |
                                            EXECUTION_STATE.ES_DISPLAY_REQUIRED |
                                            EXECUTION_STATE.ES_SYSTEM_REQUIRED);
            }
            else
                return UnsetState();

        }

        EXECUTION_STATE UnsetState()
        {
            return SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }
    }
}
