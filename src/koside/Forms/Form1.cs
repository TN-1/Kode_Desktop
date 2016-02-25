﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ScintillaNET;
using System.IO;
using System.Xml;

namespace koside
{
    public partial class Form1 : Form
    {
        List<string> file_name = new List<string>();
        List<bool> ismodified = new List<bool>();
        List<bool> hasstar = new List<bool>();
        List<int> charcount = new List<int>();
        Color BackColorVar;
        Color ForeColorVar;
        string CurrentInstall;
        int lastCaretPos = 0;

        public Form1()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;

            //Setting our main colours based on light/dark mode seeting
            if (Properties.Settings.Default.DarkMode == true)
            {
                BackColorVar = Color.FromArgb(40, 40, 40);
                ForeColorVar = Color.White;
            }
            else if (Properties.Settings.Default.DarkMode == false)
            {
                BackColorVar = Color.White;
                ForeColorVar = Color.Black;
            }

            tabControl1.BackColor = BackColorVar;
            tabControl1.ForeColor = ForeColorVar;

            menuStrip1.BackColor = BackColorVar;
            menuStrip1.ForeColor = ForeColorVar;
            statusStrip1.BackColor = BackColorVar;
            statusStrip1.ForeColor = ForeColorVar;
            toolStrip1.BackColor = BackColorVar;
            toolStripComboBox1.BackColor = BackColorVar;
            toolStripComboBox1.ForeColor = ForeColorVar;
            BackColor = BackColorVar;
            ForeColor = ForeColorVar;

            toolStrip1.Renderer = new MySR();

            if (Properties.Settings.Default.KSPLoc.Count == -1)
            {
                toolStripComboBox1.Items.Add("Select an install location");
            }
            else
            {
                foreach (String install in Properties.Settings.Default.KSPLoc)
                {
                    toolStripComboBox1.Items.Add(install);
                }
                toolStripComboBox1.SelectedIndex = 0;
                CurrentInstall = Properties.Settings.Default.KSPLoc[0];
            }

            if (Properties.Settings.Default.DarkMode == true)
            {
                NewButton.Image = Properties.Resources.NewFileNight;
                SaveButton.Image = Properties.Resources.SaveNight;
                SaveAllButton.Image = Properties.Resources.SaveAllNight;
                OpenButton.Image = Properties.Resources.OpenNight;
                CutButton.Image = Properties.Resources.CutNight;
                CopyButton.Image = Properties.Resources.CopyNight;
                PasteButton.Image = Properties.Resources.PasteNight;
                UndoButton.Image = Properties.Resources.UndoNight;
                RedoButton.Image = Properties.Resources.RedoNight;
            }
            else
            {
                NewButton.Image = Properties.Resources.NewFile;
                SaveButton.Image = Properties.Resources.Save;
                SaveAllButton.Image = Properties.Resources.Saveall;
                OpenButton.Image = Properties.Resources.Open;
                CutButton.Image = Properties.Resources.Cut;
                CopyButton.Image = Properties.Resources.Copy;
                PasteButton.Image = Properties.Resources.Paste;
                UndoButton.Image = Properties.Resources.Undo;
                RedoButton.Image = Properties.Resources.Redo;
            }

            //Check to see if there is a previous session(Light-Dark mode transition)
            if (File.Exists("KodeCache.xml"))
                RestoreSession();
            else
                addTab();
        }

        private void scintilla_TextChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                // Did the number of characters in the line number display change?
                // i.e. nnn VS nn, or nnnn VS nn, etc...
                var maxLineNumberCharLength = body.Lines.Count.ToString().Length;
                // Calculate the width required to display the last line number
                // and include some padding for good measure.
                const int padding = 2;
                body.Margins[0].Width = body.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Quit.
            this.Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            New();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void scintilla_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            int i = tabControl1.SelectedIndex;
            if (ismodified[i] == true)
            {
                if (hasstar[i] == false)
                {
                    string s = tabControl1.SelectedTab.Text;
                    s = s.Remove(s.Length - 9);
                    s += " *        X";
                    tabControl1.SelectedTab.Text = s;
                    hasstar[i] = true;
                }
            }

            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                //Line number check
                toolStripStatusLabel1.Text = "Line: " + (body.CurrentLine + 1).ToString();

                //Brace highlighting
                // Has the caret changed position?
                var caretPos = body.CurrentPosition;
                if (lastCaretPos != caretPos)
                {
                    lastCaretPos = caretPos;
                    var bracePos1 = -1;
                    var bracePos2 = -1;

                    // Is there a brace to the left or right?
                    if (caretPos > 0 && IsBrace(body.GetCharAt(caretPos - 1)))
                        bracePos1 = (caretPos - 1);
                    else if (IsBrace(body.GetCharAt(caretPos)))
                        bracePos1 = caretPos;

                    if (bracePos1 >= 0)
                    {
                        // Find the matching brace
                        bracePos2 = body.BraceMatch(bracePos1);
                        if (bracePos2 == Scintilla.InvalidPosition)
                        {
                            body.BraceBadLight(bracePos1);
                            body.HighlightGuide = 0;
                        }
                        else
                        {
                            body.BraceHighlight(bracePos1, bracePos2);
                            body.HighlightGuide = body.GetColumn(bracePos1);
                        }
                    }
                    else
                    {
                        // Turn off brace matching
                        body.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition);
                        body.HighlightGuide = 0;
                    }
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Paste();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Redo();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                //Find and highlight
                Form4 find = new Form4();
                var result = find.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string text = find.name;
                    HighlightWord(text);
                }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                bool test = Properties.Settings.Default.DarkMode;
                bool othertest = Properties.Settings.Default.Uppercase;

                Form2 settings = new Form2();
                settings.ShowDialog();
                if (Properties.Settings.Default.DarkMode != test)
                {
                    if (Properties.Settings.Default.DarkMode == true)
                    {
                        BackColorVar = Color.Gray;
                        ForeColorVar = Color.White;
                        DialogResult result = MessageBox.Show("We need to restart Kode. Dont worry, You wont lose your work");
                        if (result == DialogResult.OK)
                        {
                            createXML();
                            Application.Restart();
                        }
                    }
                    else
                    {
                        BackColorVar = Color.White;
                        ForeColorVar = Color.Black;
                        DialogResult result = MessageBox.Show("We need to restart Kode. Dont worry, You wont lose your work");
                        if (result == DialogResult.OK)
                        {
                            createXML();
                            Application.Restart();
                        }
                    }
                }
                if(Properties.Settings.Default.Uppercase != othertest)
                {
                    DialogResult result = MessageBox.Show("We need to restart Kode. Dont worry, You wont lose your work");
                    if (result == DialogResult.OK)
                    {
                        createXML();
                        Application.Restart();
                    }
                }

                toolStripComboBox1.Items.Clear();
                foreach (String s in Properties.Settings.Default.KSPLoc)
                {
                    toolStripComboBox1.Items.Add(s);
                }
                toolStripComboBox1.SelectedIndex = 0;
            }
        }

        private void exportToKSPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                //Export to KSP scripts folder.
                Form3 formname = new Form3();
                var result = formname.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string file = formname.name;
                    string location = Properties.Settings.Default.KSPLoc + @"\Ships\Script\" + file;
                    System.IO.File.WriteAllText(location, body.Text);
                }
            }*/
            MessageBox.Show("Feautre inentionally disabled. Please use save as instead");
        }

        private void licenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form5 licence = new Form5();
            licence.Show();
        }

        private void scintilla_CharAdded(object sender, CharAddedEventArgs e)
        {
            int i = tabControl1.SelectedIndex;

            if (ismodified[i] == false)
                ismodified[i] = true;

            charcount[i]++;

            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                //Autocompletion. Will eventually make nicer.
                // Find the word start
                var currentPos = body.CurrentPosition;
                var wordStartPos = body.WordStartPosition(currentPos, true);

                // Display the autocompletion list
                var lenEntered = currentPos - wordStartPos;
                if (lenEntered > 0)
                {
                    if(Properties.Settings.Default.Uppercase == true)
                        body.AutoCShow(lenEntered, Keywords.FullUpper());
                    else
                        body.AutoCShow(lenEntered, Keywords.FullLower());
                }

                if (charcount[i] == 10)
                {
                    charcount[i] = 0;
                    body.EndUndoAction();
                    body.BeginUndoAction();
                }
            }
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckUpdate(false);
        }

        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            //Looping through the controls.
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
                Rectangle r = tabControl1.GetTabRect(i);
                //Getting the position of the "x" mark.
                Rectangle closeButton = new Rectangle(r.Right - 28, r.Top + 4, 12, 7);
                if (closeButton.Contains(e.Location))
                {
                    if (MessageBox.Show("Would you like to Close this Tab?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this.tabControl1.TabPages.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAll();
        }

        private void kOSDocumentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://ksp-kos.github.io/KOS_DOC/language.html");
        }

        private void reportABugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/TN-1/Kode/issues/new");
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            New();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void SaveAllButton_Click(object sender, EventArgs e)
        {
            SaveAll();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            Open();
        }

        private void CutButton_Click(object sender, EventArgs e)
        {
            Cut();
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void PasteButton_Click(object sender, EventArgs e)
        {
            Paste();
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void RedoButton_Click(object sender, EventArgs e)
        {
            Redo();
        }

        private void wholeScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = tabControl1.SelectedIndex;

            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                //Save to same file as opened from
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                string s = body.Text;
                s = "    " + s;
                s = s.Replace(System.Environment.NewLine, "  \r\n    ");
                Clipboard.SetText(s);
                MessageBox.Show("Scipt is now in your clipboard");
            }
        }

        private void selectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = tabControl1.SelectedIndex;

            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                //Save to same file as opened from
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                string s = body.SelectedText;
                s = "    " + s;
                s = s.Replace(System.Environment.NewLine, "  \r\n    ");
                Clipboard.SetText(s);
                MessageBox.Show("Selection is now in your clipboard");
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentInstall != toolStripComboBox1.SelectedItem.ToString())
            {
                CurrentInstall = toolStripComboBox1.SelectedItem.ToString();
                Properties.Settings.Default.KSPLoc.Remove(CurrentInstall);
                Properties.Settings.Default.KSPLoc.Insert(0, CurrentInstall);
                toolStripComboBox1.Items.Clear();
                foreach (String s in Properties.Settings.Default.KSPLoc)
                {
                    toolStripComboBox1.Items.Add(s);
                }
                toolStripComboBox1.SelectedIndex = 0;
                Properties.Settings.Default.Save();
            }
        }

        private void minimiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = tabControl1.SelectedIndex;
            if (ismodified[i] == true)
            {
                ismodified[i] = false;
                if (hasstar[i] == true)
                {
                    string s = tabControl1.SelectedTab.Text;
                    s = s.Remove(s.Length - 11);
                    s += "        X";
                    tabControl1.SelectedTab.Text = s;
                    hasstar[i] = false;
                }
            }
            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "kOS Script Files|*.ks";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.InitialDirectory = Path.Combine(CurrentInstall, "Ships", "Script");
                saveFileDialog1.FileName = "MinimisedScript";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(saveFileDialog1.FileName.ToString(), Minimiser.Minimise(body.Text));
                    file_name[i] = Path.GetFileNameWithoutExtension(saveFileDialog1.FileName.ToString());
                    tabControl1.SelectedTab.Text = Path.GetFileNameWithoutExtension(file_name[i]) + ".ks        X";
                }
            }
        }

        private void New()
        {
            try
            {
                if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
                {
                    Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                    if (body.Text.Length != 0)
                        addTab();
                }
            }
            catch
            {
                addTab();
            }
        }

        private void Cut()
        {
            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                body.Cut();
            }
        }

        private void Copy()
        {
            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                body.Copy();
            }
        }

        private void Paste()
        {
            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                body.Paste();
            }
        }

        private void Undo()
        {
            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                body.Undo();
            }
        }

        private void Redo()
        {
            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                body.Redo();
            }
        }

        private void Save()
        {
            int i = tabControl1.SelectedIndex;
            if (file_name[i] != null)
            {
                if (ismodified[i] == true)
                {
                    ismodified[i] = false;
                    if (hasstar[i] == true)
                    {
                        string s = tabControl1.SelectedTab.Text;
                        s = s.Remove(s.Length - 11);
                        s += "        X";
                        tabControl1.SelectedTab.Text = s;
                        hasstar[i] = false;
                    }
                }
                if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
                {
                    //Save to same file as opened from
                    Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                    System.IO.File.WriteAllText(file_name[i], body.Text);
                }
            }
            else
                SaveAs();
        }

        private void SaveAs()
        {
            int i = tabControl1.SelectedIndex;
            if (ismodified[i] == true)
            {
                ismodified[i] = false;
                if (hasstar[i] == true)
                {
                    string s = tabControl1.SelectedTab.Text;
                    s = s.Remove(s.Length - 11);
                    s += "        X";
                    tabControl1.SelectedTab.Text = s;
                    hasstar[i] = false;
                }
            }
            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                //Save as. Save to new file
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "kOS Scripts|*.ks";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.InitialDirectory = Path.Combine(CurrentInstall, "Ships", "Script");
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(saveFileDialog1.FileName.ToString(), body.Text);
                    file_name[i] = Path.GetFileNameWithoutExtension(saveFileDialog1.FileName.ToString());
                    tabControl1.SelectedTab.Text = Path.GetFileNameWithoutExtension(file_name[i]) + ".ks        X";
                }

            }
        }

        private void SaveAll()
        {
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
                tabControl1.SelectedIndex = i;
                if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
                {
                    Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];

                    if (ismodified[i] == true)
                    {
                        ismodified[i] = false;
                        if (hasstar[i] == true)
                        {
                            string s = tabControl1.SelectedTab.Text;
                            s = s.Remove(s.Length - 11);
                            s += "        X";
                            tabControl1.SelectedTab.Text = s;
                            hasstar[i] = false;
                        }

                        if (file_name[i] != null)
                        {
                            Save();
                        }
                        else
                        {
                            SaveAs();
                        }
                    }
                }
            }
        }

        private void Open()
        {
            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];

                if (tabControl1.TabCount == 1 && body.TextLength == 0)
                {
                    //Open a file
                    OpenFileDialog theDialog = new OpenFileDialog();
                    theDialog.Title = "Open Script";
                    theDialog.Filter = "kOS Scripts|*.ks";
                    theDialog.InitialDirectory = Path.Combine(CurrentInstall, "Ships", "Script");
                    if (theDialog.ShowDialog() == DialogResult.OK)
                    {
                        int i = tabControl1.TabCount - 1;
                        file_name[i] = theDialog.FileName.ToString();

                        StreamReader objReader;
                        objReader = new StreamReader(file_name[i]);

                        body.Text = objReader.ReadToEnd();
                        objReader.Close();

                        this.Text = file_name[i] + " - Kode";
                        tabControl1.SelectedTab.Text = Path.GetFileNameWithoutExtension(file_name[i]) + ".ks        X";
                    }
                }
                else
                {
                    addTab();
                    if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
                    {
                        Scintilla bodyi = (Scintilla)tabControl1.SelectedTab.Controls["body"];

                        //Open a file
                        OpenFileDialog theDialogi = new OpenFileDialog();
                        theDialogi.Title = "Open Script";
                        theDialogi.Filter = "kOS Scripts|*.ks";
                        theDialogi.InitialDirectory = Path.Combine(CurrentInstall, "Ships", "Script");
                        if (theDialogi.ShowDialog() == DialogResult.OK)
                        {
                            int i = tabControl1.TabCount - 1;
                            file_name[i] = theDialogi.FileName.ToString();

                            System.IO.StreamReader objReader;
                            objReader = new System.IO.StreamReader(file_name[i]);

                            bodyi.Text = objReader.ReadToEnd();
                            objReader.Close();

                            this.Text = file_name[i] + " - Kode";
                            tabControl1.SelectedTab.Text = Path.GetFileNameWithoutExtension(file_name[i]) + ".ks        X";
                        }
                    }
                }
            }
            tabControl1.Refresh();
        }

        private void addTab()
        {

            TabPage tab = new TabPage("Untitled        X");
            Scintilla body = new Scintilla();
            file_name.Add(null);
            ismodified.Add(false);
            hasstar.Add(false);
            charcount.Add(0);

            body.Name = "body";
            body.Anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom);
            body.Margins[0].Width = 16;

            // Configuring the default style with properties
            // we have common to every lexer style saves time.
            body.StyleResetDefault();
            body.Styles[Style.Default].Font = "Manillo";
            body.Styles[Style.Default].Size = 10;
            body.Styles[Style.Default].BackColor = BackColorVar;
            body.Styles[Style.Default].ForeColor = ForeColorVar;
            body.StyleClearAll();

            if (Properties.Settings.Default.DarkMode == true)
            {
                //DARK MODE SYNTAX HIGHLIGHTING SETTING
                body.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(87, 166, 74); // Green
                body.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(87, 166, 74); // Green
                body.Styles[Style.Cpp.Word].ForeColor = Color.FromArgb(38, 139, 210); //Blue
                body.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(220, 50, 47); // Red
                body.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(220, 50, 47); // Red
                body.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(220, 50, 47); // Red
            }
            else if (Properties.Settings.Default.DarkMode == false)
            {
                //LIGHT MODE SYNTAX HIGHLIGHTING SETTINGS
                body.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
                body.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
                body.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
                body.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
                body.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
                body.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
            }

            // Configure the CPP (C#) lexer styles
            body.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
            body.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            body.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
            body.Styles[Style.Cpp.Word2].ForeColor = Color.Magenta;
            body.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
            body.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
            body.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
            body.IndentationGuides = IndentView.LookBoth;
            body.Styles[Style.BraceLight].BackColor = Color.LightGray;
            body.Styles[Style.BraceLight].ForeColor = Color.BlueViolet;
            body.Styles[Style.BraceBad].ForeColor = Color.Red;
            body.Styles[Style.LineNumber].BackColor = BackColorVar;
            body.Lexer = Lexer.Cpp;

            // Set the keywords. 0 is functions, 1 is variables
            if (Properties.Settings.Default.Uppercase == true)
            {
                body.SetKeywords(0, Keywords.Upper(0));
                body.SetKeywords(1, Keywords.Upper(1));
            }
            else if (Properties.Settings.Default.Uppercase == false)
            {
                body.SetKeywords(0, Keywords.Lower(0));
                body.SetKeywords(1, Keywords.Lower(1));
            }
            body.CaretLineBackColor = Color.White;
            body.CaretForeColor = ForeColorVar;

            body.CharAdded += new System.EventHandler<ScintillaNET.CharAddedEventArgs>(this.scintilla_CharAdded);
            body.UpdateUI += new System.EventHandler<ScintillaNET.UpdateUIEventArgs>(this.scintilla_UpdateUI);
            body.TextChanged += new System.EventHandler(this.scintilla_TextChanged);

            body.BeginUndoAction();

            tab.Controls.Add(body);
            tabControl1.TabPages.Add(tab);
            int i = tabControl1.TabCount - 1;
            tabControl1.SelectedIndex = i;
        }

        private static bool IsBrace(int c)
        {
            switch (c)
            {
                case '(':
                case ')':
                case '{':
                case '}':
                    return true;
            }

            return false;
        }

        private void HighlightWord(string text)
        {
            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
            {
                Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];

                // Indicators 0-7 could be in use by a lexer
                // so we'll use indicator 8 to highlight words.
                const int NUM = 8;

                // Remove all uses of our indicator
                body.IndicatorCurrent = NUM;
                body.IndicatorClearRange(0, body.TextLength);

                // Update indicator appearance
                body.Indicators[NUM].Style = IndicatorStyle.StraightBox;
                body.Indicators[NUM].Under = true;
                body.Indicators[NUM].ForeColor = Color.Green;
                body.Indicators[NUM].OutlineAlpha = 50;
                body.Indicators[NUM].Alpha = 30;

                // Search the document
                body.TargetStart = 0;
                body.TargetEnd = body.TextLength;
                body.SearchFlags = SearchFlags.None;
                while (body.SearchInTarget(text) != -1)
                {
                    // Mark the search results with the current indicator
                    body.IndicatorFillRange(body.TargetStart, body.TargetEnd - body.TargetStart);

                    // Search the remainder of the document
                    body.TargetStart = body.TargetEnd;
                    body.TargetEnd = body.TextLength;
                }
            }
        }

        static public void CheckUpdate(bool IsSilent)
        {
            Version newVersion = null;
            string url = "https://github.com/TN-1/Kode/releases";
            XmlTextReader reader;
            try
            {
                string xmlURL = "https://raw.githubusercontent.com/TN-1/Kode/master/resources/version.xml";
                reader = new XmlTextReader(xmlURL);
                reader.MoveToContent();
                string elementName = "";
                if ((reader.NodeType == XmlNodeType.Element) &&
                    (reader.Name == "kode"))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                            elementName = reader.Name;
                        else
                        {
                            if ((reader.NodeType == XmlNodeType.Text) &&
                                (reader.HasValue))
                            {
                                switch (elementName)
                                {
                                    case "version":
                                        newVersion = new Version(reader.Value);
                                        break;
                                    case "url":
                                        url = reader.Value;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            Version curVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            if (curVersion.CompareTo(newVersion) < 0)
            {
                if (DialogResult.Yes ==
                 MessageBox.Show("Would you like to download?", "New Version Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    System.Diagnostics.Process.Start(url);
                }
            }
            else
            {
                if (!IsSilent)
                    MessageBox.Show("Kode is up to date", "No new version");
            }
        }

        private void createXML()
        {
            XmlTextWriter writer = new XmlTextWriter("KodeCache.xml", System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("Kode_Cache");
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
                tabControl1.SelectedIndex = i;
                if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
                {
                    Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                    createNode(tabControl1.SelectedIndex.ToString(), tabControl1.SelectedTab.Text, body.Text, file_name[i], writer);
                }
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        private void createNode(string tID, string tTitle, string tText, string tFile, XmlTextWriter writer)
        {
            writer.WriteStartElement("Tab");
            writer.WriteStartElement("Tab_Index");
            writer.WriteString(tID);
            writer.WriteEndElement();
            writer.WriteStartElement("Tab_Title");
            writer.WriteString(tTitle);
            writer.WriteEndElement();
            writer.WriteStartElement("Tab_Text");
            writer.WriteString(tText);
            writer.WriteEndElement();
            writer.WriteStartElement("Tab_File");
            writer.WriteString(tFile);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void RestoreSession()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"KodeCache.xml");

            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Kode_Cache/Tab");

            List<TabRestore> tabs = new List<TabRestore>();

            foreach (XmlNode node in nodes)
            {
                TabRestore tab = new TabRestore();

                tab.index = Convert.ToInt32(node.SelectSingleNode("Tab_Index").InnerText);
                tab.title = node.SelectSingleNode("Tab_Title").InnerText;
                tab.text = node.SelectSingleNode("Tab_Text").InnerText;
                tab.filename = node.SelectSingleNode("Tab_File").InnerText;

                tabs.Add(tab);
            }

            foreach (TabRestore tab in tabs)
            {
                addTab();
                tabControl1.SelectedIndex = tab.index;
                if(tab.title != null)
                    tabControl1.SelectedTab.Text = tab.title;
                if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
                {
                    Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                    if(tab.title != null)
                        body.Text = tab.text;
                }
                if (tab.filename != null)
                    file_name[tab.index] = tab.filename;
            }

            tabControl1.SelectedIndex = 0;
            File.Delete("KodeCache.xml");
        }

    }

    class MySR : ToolStripSystemRenderer
    {
        public MySR() { }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            //base.OnRenderToolStripBorder(e);
        }
    }

    class TabRestore
    {
        public int index;
        public string title;
        public string text;
        public string filename;
    }

    class Keywords
    {
        static private string key0 = "ADD ALL AT BATCH BREAK CLEARSCREEN COMPILE COPY DECLARE DELETE DEPLOY DO DO EDIT ELSE FILE FOR FROM FROM FUNCTION GLOBAL IF IN LIST LOCAL LOCK LOG OFF ON ONCE PARAMETER PRESERVE PRINT REBOOT REMOVE RENAME RUN SET SHUTDOWN STAGE STEP SWITCH THEN TO TOGGLE UNLOCK UNSET UNTIL VOLUME WAIT WHEN";
        static private string key1 = "HEADING PROGRADE RETROGRADE FACING MAXTHRUST VELOCITY GEOPOSITION LATITUDE LONGITUDE UP NORTH BODY ANGULARMOMENTUM ANGULARVEL ANGULARVELOCITY COMMRANGE MASS VERTICALSPEED GROUNDSPEED AIRESPEED VESSELNAME ALTITUDE APOAPSIS PERIAPSIS SENSORS SRFPROGRADE SRFREROGRADE OBT STATUS SHIPNAME";

        static public string Upper(int index)
        {
            if (index == 0)
                return key0.ToUpper();
            if (index == 1)
                return key1.ToUpper();
            return null;   
        }

        static public string Lower(int index)
        {
            if (index == 0)
                return key0.ToLower();
            if (index == 1)
                return key1.ToLower();
            return null;
        }

        static public string FullUpper()
        {
            string s = key0 + " " + key1;
            return s.ToUpper();
        }

        static public string FullLower()
        {
            string s = key0 + " " + key1;
            return s.ToLower();
        }
    }
}