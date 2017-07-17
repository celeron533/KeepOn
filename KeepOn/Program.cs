using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace KeepOn
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool hasUnknowArgs = false;
            foreach (string arg in args)
            {
                try
                {
                    // /lang=zh_CN
                    if (arg.StartsWith("/lang"))
                    {
                        I18N.overrideLanguage = arg.Split('=')[1].Trim();
                    }
                    else
                        hasUnknowArgs = true;
                }
                catch
                {
                    hasUnknowArgs = true;
                }
            }
            if (hasUnknowArgs)
                MessageBox.Show(
@"Set UI language:
  /lang=zh_CN
  /lang=en_US"
,
            "Available arguments");

            I18N.Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool createdNew;
            // To prevent the program to be started twice
            Mutex appMutex = new Mutex(true, Application.ProductName, out createdNew);
            if (createdNew)
            {
                Application.Run(new TrayApplicationContext());
                appMutex.ReleaseMutex();
            }
            else
            {
                string msg = string.Format(I18N.GetString("app.dupInstanceMsg"), Application.ProductName);
                MessageBox.Show(msg, Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
