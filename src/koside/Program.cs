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
                var drives = System.IO.DriveInfo.GetDrives();
                if (drives.Where(data => data.Name == "Z:\\").Count() == 1)
                {
                    RegistryKey OurKey = Registry.CurrentUser;
                    OurKey = OurKey.OpenSubKey(@"SOFTWARE\", false);
                    RegistryKey key = OurKey.OpenSubKey("Wine");
                    if (key != null)
                    {
                        Properties.Settings.Default.KSPLoc = "This doesnt matter on Linux";
                        Properties.Settings.Default.OS = "Linux";
                    }
                    else
                    {
                        try
                        {
                            RegistryKey OurKeyi = Registry.LocalMachine;
                            OurKeyi = OurKeyi.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve", false);
                            RegistryKey keyi = OurKeyi.OpenSubKey("Steam");
                            string path = keyi.GetValue("InstallPath").ToString();
                            DialogResult dialogResult = MessageBox.Show("Because this is the first time you have used Kode, We will see if we can find where Steam is installed. Is this correct?\n" + path, "Is this corrent?", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                Properties.Settings.Default.KSPLoc = path + @"\steamapps\common\Kerbal Space Program";
                            }
                            else
                            {
                                MessageBox.Show("Whoops. Please visit Settings to fix it :)");
                            }
                            Properties.Settings.Default.FirstRun = false;
                            Properties.Settings.Default.OS = "Windows";
                        }
                        catch
                        {
                            MessageBox.Show("Hello! There seems to be a problem finding where Steam is installed. Please go to settings to manually add. Error: 1");
                            Properties.Settings.Default.FirstRun = false;
                            Properties.Settings.Default.OS = "Windows";
                        }
                    }
                }
                else
                {
                    try
                    {
                        RegistryKey OurKey = Registry.LocalMachine;
                        OurKey = OurKey.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve", false);
                        RegistryKey key = OurKey.OpenSubKey("Steam");
                        string path = key.GetValue("InstallPath").ToString();
                        DialogResult dialogResult = MessageBox.Show("Because this is the first time you have used Kode, We will see if we can find where Steam is installed. Is this correct?\n" + path, "Is this corrent?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            Properties.Settings.Default.KSPLoc = path + @"\steamapps\common\Kerbal Space Program";
                        }
                        else
                        {
                            MessageBox.Show("Whoops. Please visit Settings to fix it :)");
                        }
                        Properties.Settings.Default.FirstRun = false;
                        Properties.Settings.Default.OS = "Windows";
                    }
                    catch
                    {
                        MessageBox.Show("Hello! There seems to be a problem finding where Steam is installed. Please go to settings to manually add. Error: 1");
                        Properties.Settings.Default.FirstRun = false;
                        Properties.Settings.Default.OS = "Windows";
                    }
                }
            }
            Application.Run(new Form1());
        }
    }
}
