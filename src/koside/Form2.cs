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
            textBox1.Text = Properties.Settings.Default.KSPLoc;
            if(Properties.Settings.Default.DarkMode == false)
            {
                comboBox1.Text = "Light Mode";
            }else
            {
                comboBox1.Text = "Dark Mode";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.KSPLoc = textBox1.Text.ToString();
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.Text == "Light Mode")
            {
                Properties.Settings.Default.DarkMode = false;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.DarkMode = true;
                Properties.Settings.Default.Save();
            }
        }
    }
}
