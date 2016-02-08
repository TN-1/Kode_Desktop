using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        int lastCaretPos = 0;

        public Form1()
        {
            var perUserAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var thispath = Path.Combine(perUserAppData, @"kode\kodecache.txt");

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

            addTab();
            tabControl1.BackColor = BackColorVar;
            tabControl1.ForeColor = ForeColorVar;

            //Check to see if there is a previous session(Light-Dark mode transition)
            if (File.Exists(thispath))
            {
                if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
                {
                    Scintilla body = (Scintilla)tabControl1.SelectedTab.Controls["body"];
                    System.IO.StreamReader objReader;
                    objReader = new System.IO.StreamReader(thispath);
                    body.Text = objReader.ReadToEnd();
                    objReader.Close();
                    System.IO.File.WriteAllText(thispath, "");
                }
            }

            menuStrip1.BackColor = BackColorVar;
            menuStrip1.ForeColor = ForeColorVar;
            statusStrip1.BackColor = BackColorVar;
            statusStrip1.ForeColor = ForeColorVar;
            toolStrip1.BackColor = BackColorVar;
            BackColor = BackColorVar;
            ForeColor = ForeColorVar;

            toolStrip1.Renderer = new MySR();

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

                saveFileDialog1.Filter = "kOS Script Files|*.ks";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.InitialDirectory = Properties.Settings.Default.KSPLoc + @"\Ships\Script\"; 
                
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(saveFileDialog1.FileName.ToString(), body.Text);
                }

            }
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
                string test = Properties.Settings.Default.DarkMode.ToString();

                Form2 settings = new Form2();
                settings.ShowDialog();
                if (Properties.Settings.Default.DarkMode.ToString() != test)
                {
                    if (Properties.Settings.Default.DarkMode == true)
                    {
                        BackColorVar = Color.Gray;
                        ForeColorVar = Color.White;
                        DialogResult result = MessageBox.Show("We need to restart Kode. Dont worry, You wont lose your work");
                        if (result == DialogResult.OK)
                        {
                            var perUserAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                            var path = Path.Combine(perUserAppData, @"kode");
                            System.IO.Directory.CreateDirectory(path);
                            path += @"\kodecache.txt";
                            System.IO.File.WriteAllText(path, body.Text);
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
                            var perUserAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                            var path = Path.Combine(perUserAppData, @"kode");
                            System.IO.Directory.CreateDirectory(path);
                            path += @"\kodecache.txt";
                            System.IO.File.WriteAllText(path, body.Text);
                            Application.Restart();
                        }
                    }
                }
            }
        }

        private void exportToKSPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Controls.ContainsKey("body"))
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
            }
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
                    body.AutoCShow(lenEntered, "ADD ALL AT BATCH BREAK CLEARSCREEN COMPILE COPY DECLARE DELETE DEPLOY DO DO EDIT ELSE FILE FOR FROM FROM FUNCTION GLOBAL IF IN LIST LOCAL LOCK LOG OFF ON ONCE PARAMETER PRESERVE PRINT REBOOT REMOVE RENAME RUN SET SHUTDOWN STAGE STEP SWITCH THEN TO TOGGLE UNLOCK UNSET UNTIL VOLUME WAIT WHEN HEADING PROGRADE RETROGRADE FACING MAXTHRUST VELOCITY GEOPOSITION LATITUDE LONGITUDE UP NORTH BODY ANGULARMOMENTUM ANGULARVEL ANGULARVELOCITY COMMRANGE MASS VERTICALSPEED GROUNDSPEED AIRESPEED VESSELNAME ALTITUDE APOAPSIS PERIAPSIS SENSORS SRFPROGRADE SRFREROGRADE OBT STATUS SHIPNAME");
                }

                if(charcount[i] == 10)
                {
                    charcount[i] = 0;
                    body.EndUndoAction();
                    body.BeginUndoAction();
                }
            }
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hey look, Another unimplemented feature");
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
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
                 MessageBox.Show(this, "Would you like to download?", "New Version Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                { 
                    System.Diagnostics.Process.Start(url);
                }
            }else
            {
                MessageBox.Show("Kode is up to date", "No new version");
            }
        }

        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            //Looping through the controls.
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
                Rectangle r = tabControl1.GetTabRect(i);
                //Getting the position of the "x" mark.
                Rectangle closeButton = new Rectangle(r.Right - 25, r.Top + 4, 9, 7);
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

        private void SaveAll()
        {
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
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
                            //Save to same file as opened from
                            System.IO.File.WriteAllText(file_name[i], body.Text);
                        }
                        else
                        {
                            tabControl1.SelectedIndex = i;

                            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                            saveFileDialog1.Filter = "kOS Script Files|*.ks";
                            saveFileDialog1.FilterIndex = 2;
                            saveFileDialog1.RestoreDirectory = true;
                            saveFileDialog1.InitialDirectory = Properties.Settings.Default.KSPLoc + @"\Ships\Script\";

                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                                System.IO.File.WriteAllText(saveFileDialog1.FileName.ToString(), body.Text);
                            }
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
                    theDialog.InitialDirectory = Properties.Settings.Default.KSPLoc + @"\Ships\Script\";
                    if (theDialog.ShowDialog() == DialogResult.OK)
                    {
                        int i = tabControl1.TabCount - 1;
                        file_name[i] = theDialog.FileName.ToString();

                        System.IO.StreamReader objReader;
                        objReader = new System.IO.StreamReader(file_name[i]);

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
                        theDialogi.InitialDirectory = Properties.Settings.Default.KSPLoc + @"\Ships\Script\";
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

        public void addTab()
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
            body.SetKeywords(0, "ADD ALL AT BATCH BREAK CLEARSCREEN COMPILE COPY DECLARE DELETE DEPLOY DO DO EDIT ELSE FILE FOR FROM FROM FUNCTION GLOBAL IF IN LIST LOCAL LOCK LOG OFF ON ONCE PARAMETER PRESERVE PRINT REBOOT REMOVE RENAME RUN SET SHUTDOWN STAGE STEP SWITCH THEN TO TOGGLE UNLOCK UNSET UNTIL VOLUME WAIT WHEN");
            body.SetKeywords(1, "HEADING PROGRADE RETROGRADE FACING MAXTHRUST VELOCITY GEOPOSITION LATITUDE LONGITUDE UP NORTH BODY ANGULARMOMENTUM ANGULARVEL ANGULARVELOCITY COMMRANGE MASS VERTICALSPEED GROUNDSPEED AIRESPEED VESSELNAME ALTITUDE APOAPSIS PERIAPSIS SENSORS SRFPROGRADE SRFREROGRADE OBT STATUS SHIPNAME");
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
    }

    public class MySR : ToolStripSystemRenderer
    {
        public MySR() { }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            //base.OnRenderToolStripBorder(e);
        }
    }

}