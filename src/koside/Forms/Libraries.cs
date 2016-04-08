using System;
using System.Windows.Forms;
using System.IO;

namespace koside.Forms
{
    public partial class Libraries : Form
    {
        public Libraries()
        {
            InitializeComponent();
            UpdateList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Add new
            OpenFileDialog ImportLib = new OpenFileDialog();
            ImportLib.Title = "Open Library";
            ImportLib.Filter = "kOS Libraries|*.ks";
            if (ImportLib.ShowDialog() == DialogResult.OK)
            {
                string ss = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), "Libraries", Path.GetFileNameWithoutExtension(ImportLib.FileName) + ".ks");
                ss = ss.Remove(0, 6);
                string s = ImportLib.FileName;
                try
                {
                    File.Copy(s, ss);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                UpdateList();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Delete Button
            int i = listBox1.SelectedIndex;
            if (i != -1)
            {
                if(MessageBox.Show("Are you sure you want to delete?","", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string s = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), "Libraries", listBox1.SelectedItem.ToString());
                    s = s.Remove(0, 6);
                    if (File.Exists(s))
                        File.Delete(s);
                    else
                        MessageBox.Show("Diddley, This file apparently doesnt exist");
                }
                else
                {
                    return;
                }
                UpdateList();
            }
            else
                MessageBox.Show("You need to select an entry");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Ok Button
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Run Once Button
            Program.formInstance.ImportLib(listBox1.SelectedItem.ToString(), true);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Run Button
            var form = Form.ActiveForm as Form1;
            Program.formInstance.ImportLib(listBox1.SelectedItem.ToString(), false);
        }

        private void UpdateList()
        {
            listBox1.Items.Clear();

            string s = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), "Libraries");
            s = s.Remove(0, 6);
            DirectoryInfo d = new DirectoryInfo(s);

            if (Directory.Exists(s) == false)
            {
                MessageBox.Show("Oops, Libraries dir is missing. Aborting.");
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }

            foreach (var file in d.GetFiles("*.ks"))
            {
                listBox1.Items.Add(file.Name);
            }
        }
           
    }
}
