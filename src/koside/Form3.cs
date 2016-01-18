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
    public partial class Form3 : Form
    {
        public string name;

        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            name = textBox1.Text.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
