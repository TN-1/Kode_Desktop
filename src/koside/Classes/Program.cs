using System;
using System.Windows.Forms;

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
                FirstRun.Entry();
            if(Properties.Settings.Default.OS == "Windows")
                Form1.CheckUpdate(true);
            Application.Run(new Form1());
        }
    }
}
