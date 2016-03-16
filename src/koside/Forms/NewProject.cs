using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace koside.Forms
{
    public partial class NewProject : Form
    {
        public string ksproj;
        public NewProject()
        {
            InitializeComponent();
            textBox2.Text = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Kode Projects");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ksproj = Classes.KsprojTools.ProjectFile(textBox1.Text, textBox3.Text, textBox2.Text, checkBox1.Checked, null, null, -1, null, null);
        }
    }
}