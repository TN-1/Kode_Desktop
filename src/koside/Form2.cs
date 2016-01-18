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
            textBox1.Text = Settings1.Default.KSPLoc;
            if(Settings1.Default.DarkMode == false)
            {
                comboBox1.Text = "Light Mode";
            }else
            {
                //Redundent for now but meh, One less line for later.
                comboBox1.Text = "Dark Mode";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Settings1.Default.KSPLoc = textBox1.Text.ToString();
            Settings1.Default.Save();
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
                Settings1.Default.DarkMode = false;
            }else
            {
                MessageBox.Show("Sorry, Not yet implemented :(");
                //Uncomment when implemented
                //Settings1.Default.DarkMode = true;
            }
        }
    }
}
