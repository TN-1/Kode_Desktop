using System;
using System.Windows.Forms;

namespace koside.Forms
{
    public partial class Setup : Form
    {
        public Setup()
        {
            InitializeComponent();
            UpdateList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.KSPLoc.Add(folderBrowserDialog1.SelectedPath);
                UpdateList();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            if (i != -1)
            {
                Properties.Settings.Default.KSPLoc.RemoveAt(i);
                UpdateList();
            }
            else
                MessageBox.Show("You need to select an entry");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            if (i != -1)
            {
                string s = Properties.Settings.Default.KSPLoc[i];
                Properties.Settings.Default.KSPLoc.RemoveAt(i);
                Properties.Settings.Default.KSPLoc.Insert(0, s);
                UpdateList();
            }
            else
                MessageBox.Show("You need to select an entry");
        }

        private void UpdateList()
        {
            listBox1.Items.Clear();
            foreach (String s in Properties.Settings.Default.KSPLoc)
            {
                listBox1.Items.Add(s);
            }
        }

        private void Setup_Load(object sender, EventArgs e)
        {
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.SetToolTip(this.button1, "When adding a new install, Please select the Top Level KSP directory");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
