using System;
using System.Windows.Forms;

namespace koside
{
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();

            if(Properties.Settings.Default.DarkMode == false)
                radioButton4.Checked = true;
            else
                radioButton3.Checked = true;
            if (Properties.Settings.Default.Uppercase == true)
                radioButton1.Checked = true;
            else
                radioButton2.Checked = true;
            if (Properties.Settings.Default.mode == false)
                radioButton5.Checked = true;
            else
                radioButton6.Checked = true;

            numericUpDown1.Value = Properties.Settings.Default.tabsize;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
                Properties.Settings.Default.Uppercase = true;
            else if (radioButton2.Checked == true)
                Properties.Settings.Default.Uppercase = false;
            else
                Properties.Settings.Default.Uppercase = false;

            if (radioButton3.Checked == true)
                Properties.Settings.Default.DarkMode = true;
            else if (radioButton4.Checked == true)
                Properties.Settings.Default.DarkMode = false;
            else
                Properties.Settings.Default.DarkMode = false;

            if (radioButton5.Checked == true)
                Properties.Settings.Default.mode = false;
            else if (radioButton6.Checked == true)
                Properties.Settings.Default.mode = true;
            else
                Properties.Settings.Default.mode = true;

            Properties.Settings.Default.tabsize = Convert.ToInt32(numericUpDown1.Value);
                
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Forms.Setup setup = new Forms.Setup();
            setup.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Forms.Libraries lib = new Forms.Libraries();
            lib.ShowDialog();
        }
    }
}
