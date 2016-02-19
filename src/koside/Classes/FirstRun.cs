using System.Windows.Forms;

namespace koside
{
    class FirstRun
    {
        static public void Entry()
        {
            Properties.Settings.Default.FirstRun = false;
            Properties.Settings.Default.OS = "Windows";
            Properties.Settings.Default.KSPLoc = new System.Collections.Generic.List<string>();

            string ksp = KSPPathTools.KSPDirectory(Steam());

            if (ksp == null)
                ShowSetup();
            else
            {
                if (MessageBox.Show("We have found KSP at: " + ksp + "\r\nIs this correct?", "Steam KSP found", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Properties.Settings.Default.KSPLoc.Add(ksp);
                    if (MessageBox.Show("Would you like to add additional install locations? (Can be done in settings later)", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        ShowSetup();
                }else if (MessageBox.Show("Would you like to add the correct install now? (Can be done in settings later)", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    ShowSetup();
            }
        }

        static private string Steam()
        {
            bool linux = KSPPathTools.IsLinux();
            string steam;

            if (linux == true)
            {
                Properties.Settings.Default.OS = "Linux";
                return steam = KSPPathTools.LinuxSteam();
            }
            else
                return steam = KSPPathTools.WindowsSteam();
        }

        static void ShowSetup()
        {
            Forms.Setup setup = new Forms.Setup();
            setup.ShowDialog();
        }
    }
}
