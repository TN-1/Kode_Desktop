using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace koside
{ 
    static class Program
    {
        public static Form1 formInstance;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string s = null;
            if (args.Length != 0)
            {
                StringBuilder builder = new StringBuilder();

                foreach (string y in args)
                {
                    builder.Append(y);
                    builder.Append(' ');
                }
                s = builder.ToString();
                s = s.Remove(s.Length - 1);
                builder = null;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if(Properties.Settings.Default.FirstRun == true)
                FirstRun.Entry();
            if(Properties.Settings.Default.OS == "Windows")
                Form1.CheckUpdate(true);
            Application.Run(formInstance = new Form1(s));
        }
    }
}
