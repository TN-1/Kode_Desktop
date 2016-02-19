using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace koside
{
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();
            if(Properties.Settings.Default.DarkMode == false)
            {
                radioButton4.Checked = true;
            }else
            {
                radioButton3.Checked = true;
            }
            if (Properties.Settings.Default.Uppercase == true)
            {
                radioButton1.Checked = true;
            }
            else
            {
                radioButton2.Checked = true;
            }
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
    }
}
