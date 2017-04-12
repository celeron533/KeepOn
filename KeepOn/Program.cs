using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KeepOn
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

            bool createdNew;
            // To prevent the program to be started twice
            System.Threading.Mutex appMutex = new System.Threading.Mutex(true, Application.ProductName, out createdNew);
            if (createdNew)
            {
                Application.Run(new TrayApplicationContext());
                appMutex.ReleaseMutex();
            }
            else
            {
                string msg = String.Format("\"{0}\" is already running", Application.ProductName);
                MessageBox.Show(msg, Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
