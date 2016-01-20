using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace koside
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
            if(Properties.Settings.Default.FirstRun == true)
            {
                RegistryKey OurKey = Registry.LocalMachine;
                OurKey = OurKey.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve", false);
                RegistryKey key = OurKey.OpenSubKey("Steam");
                string path = key.GetValue("InstallPath").ToString();
                DialogResult dialogResult = MessageBox.Show("Because this is the first time you have used Kode, We will see if we can find where Steam is installed. Is this correct?\n" + path, "Is this corrent?", MessageBoxButtons.YesNo);
                if(dialogResult == DialogResult.Yes)
                {
                    Properties.Settings.Default.KSPLoc = path + @"\steamapps\common\Kerbal Space Program";
                }
                else
                {
                    MessageBox.Show("Whoops. Please visit Settings to fix it :)");
                }
                //Properties.Settings.Default.FirstRun = false;
            }
            Application.Run(new Form1());
        }
    }
}
