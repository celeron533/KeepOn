using KeepOn.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

        private Icon appIcon, grayAppIcon;

        private MenuItem EnableItem;
        private MenuItem AboutItem;
        private MenuItem ExitItem;

        public TrayApplicationContext()
        {
            // Initialize icons
            appIcon = Resources.AppIcon;
            grayAppIcon = Icon.FromHandle(MakeGrayscale(appIcon.ToBitmap()).GetHicon());

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
                Icon = appIcon,
                Text = "'KeepOn' is running",
                ContextMenu = trayMenu,
                Visible = true
            };

            trayIcon.BalloonTipText = trayIcon.Text;
            trayIcon.ShowBalloonTip(3000);

            RefreshUI();
        }

        void InitMenu()
        {
            trayMenu = new ContextMenu(new MenuItem[] {
                         EnableItem = new MenuItem("Enable", Enable_Clicked),
                         AboutItem = new MenuItem("About", About_Clicked),
                         ExitItem = new MenuItem("Exit", Exit_Clicked)
            });
        }

        void RefreshUI()
        {
            EnableItem.Checked = config.isEnable;
            trayIcon.Icon = EnableItem.Checked ? appIcon : grayAppIcon;
        }

        void Enable_Clicked(object sender, EventArgs e)
        {
            config.isEnable = !config.isEnable;
            SetState(config);
            RefreshUI();
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


        Bitmap MakeGrayscale(Bitmap original)
        {
            // Create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);
            Graphics g = Graphics.FromImage(newBitmap);

            // Create the grayscale ColorMatrix
            // https://msdn.microsoft.com/en-us/library/system.drawing.imaging.colormatrix.aspx
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
                new float[] {.3f, .3f, .3f, 0, 0},
                new float[] {.59f, .59f, .59f, 0, 0},
                new float[] {.11f, .11f, .11f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
               });

            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);

            // Draw the original image on the new image using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            g.Dispose();
            return newBitmap;
        }
    }
}
