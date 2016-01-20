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

namespace koside
{
    public partial class Form1 : Form
    {
        string file_name;
        Color BackColorVar;
        Color ForeColorVar;

        int lastCaretPos = 0;

        public Form1()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            scintilla.Margins[0].Width = 16;

            if (Properties.Settings.Default.DarkMode == true)
            {
                BackColorVar = Color.Gray;
                ForeColorVar = Color.White;
            }
            else
            {
                BackColorVar = Color.White;
                ForeColorVar = Color.Black;
            }

            var perUserAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var thispath = Path.Combine(perUserAppData, @"kode\kodecache.txt");
            if (File.Exists(thispath))
            {
                System.IO.StreamReader objReader;
                objReader = new System.IO.StreamReader(thispath);
                scintilla.Text = objReader.ReadToEnd();
                objReader.Close();
                System.IO.File.WriteAllText(thispath, "");
            }

            menuStrip1.BackColor = BackColorVar;
            menuStrip1.ForeColor = ForeColorVar;
            statusStrip1.BackColor = BackColorVar;
            statusStrip1.ForeColor = ForeColorVar;

            

            // Configuring the default style with properties
            // we have common to every lexer style saves time.
            scintilla.StyleResetDefault();
            scintilla.Styles[Style.Default].Font = "Menlo";
            scintilla.Styles[Style.Default].Size = 10;
            scintilla.Styles[Style.Default].BackColor = BackColorVar;
            scintilla.Styles[Style.Default].ForeColor = ForeColorVar;
            scintilla.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            scintilla.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
            scintilla.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            scintilla.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
            scintilla.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
            scintilla.Styles[Style.Cpp.Word2].ForeColor = Color.Magenta;
            scintilla.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
            scintilla.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
            scintilla.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
            scintilla.IndentationGuides = IndentView.LookBoth;
            scintilla.Styles[Style.BraceLight].BackColor = Color.LightGray;
            scintilla.Styles[Style.BraceLight].ForeColor = Color.BlueViolet;
            scintilla.Styles[Style.BraceBad].ForeColor = Color.Red;
            scintilla.Styles[Style.LineNumber].BackColor = BackColorVar;
            scintilla.Lexer = Lexer.Cpp;

            // Set the keywords. 0 is functions, 1 is variables
            scintilla.SetKeywords(0, "ADD ALL AT BATCH BREAK CLEARSCREEN COMPILE COPY DECLARE DELETE DEPLOY DO DO EDIT ELSE FILE FOR FROM FROM FUNCTION GLOBAL IF IN LIST LOCAL LOCK LOG OFF ON ONCE PARAMETER PRESERVE PRINT REBOOT REMOVE RENAME RUN SET SHUTDOWN STAGE STEP SWITCH THEN TO TOGGLE UNLOCK UNSET UNTIL VOLUME WAIT WHEN");
            scintilla.SetKeywords(1, "HEADING PROGRADE RETROGRADE FACING MAXTHRUST VELOCITY GEOPOSITION LATITUDE LONGITUDE UP NORTH BODY ANGULARMOMENTUM ANGULARVEL ANGULARVELOCITY COMMRANGE MASS VERTICALSPEED GROUNDSPEED AIRESPEED VESSELNAME ALTITUDE APOAPSIS PERIAPSIS SENSORS SRFPROGRADE SRFREROGRADE OBT STATUS SHIPNAME");
        }

        private int maxLineNumberCharLength;
        private void scintilla_TextChanged(object sender, EventArgs e)
        {
            // Did the number of characters in the line number display change?
            // i.e. nnn VS nn, or nnnn VS nn, etc...
            var maxLineNumberCharLength = scintilla.Lines.Count.ToString().Length;
            if (maxLineNumberCharLength == this.maxLineNumberCharLength)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 2;
            scintilla.Margins[0].Width = scintilla.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
            this.maxLineNumberCharLength = maxLineNumberCharLength;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open a file
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Script";
            theDialog.Filter = "kOS Scripts|*.ks";
            theDialog.InitialDirectory = Properties.Settings.Default.KSPLoc + @"\Ships\Script\";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                file_name = theDialog.FileName.ToString();

                System.IO.StreamReader objReader;
                objReader = new System.IO.StreamReader(file_name);

                scintilla.Text = objReader.ReadToEnd();
                objReader.Close();

                this.Text = file_name + " - Kode";

             }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Quit.
            this.Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //New file. Eventually add a check to see if changed since last save. Eventually.
            if (scintilla.TextLength != 0)
            {
                DialogResult result1 = MessageBox.Show("You have text in the editor, Are you sure you want to clear?",
                "Are you sure?",
                MessageBoxButtons.YesNo);

                if (result1 == DialogResult.Yes) { scintilla.ResetText(); }
            }
            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Save to same file as opened from
            System.IO.File.WriteAllText(file_name, scintilla.Text);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Save as. Save to new file
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "kOS Script Files|*.ks";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.InitialDirectory = Properties.Settings.Default.KSPLoc + @"\Ships\Script\";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName.ToString(), scintilla.Text);
            }

        }

        private void scintilla_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            //Line number check
            toolStripStatusLabel1.Text = "Line: " + (scintilla.CurrentLine + 1).ToString();

            //Brace highlighting
            // Has the caret changed position?
            var caretPos = scintilla.CurrentPosition;
            if (lastCaretPos != caretPos)
            {
                lastCaretPos = caretPos;
                var bracePos1 = -1;
                var bracePos2 = -1;

                // Is there a brace to the left or right?
                if (caretPos > 0 && IsBrace(scintilla.GetCharAt(caretPos - 1)))
                    bracePos1 = (caretPos - 1);
                else if (IsBrace(scintilla.GetCharAt(caretPos)))
                    bracePos1 = caretPos;

                if (bracePos1 >= 0)
                {
                    // Find the matching brace
                    bracePos2 = scintilla.BraceMatch(bracePos1);
                    if (bracePos2 == Scintilla.InvalidPosition)
                    {
                        scintilla.BraceBadLight(bracePos1);
                        scintilla.HighlightGuide = 0;
                    }
                    else
                    {
                        scintilla.BraceHighlight(bracePos1, bracePos2);
                        scintilla.HighlightGuide = scintilla.GetColumn(bracePos1);
                    }
                }
                else
                {
                    // Turn off brace matching
                    scintilla.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition);
                    scintilla.HighlightGuide = 0;
                }
            }

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scintilla.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scintilla.Paste();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scintilla.Cut();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scintilla.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scintilla.Redo();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Find and highlight
            Form4 find = new Form4();
            var result = find.ShowDialog();
            if (result == DialogResult.OK)
            {
                string text = find.name;
                HighlightWord(text);
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string test = Properties.Settings.Default.DarkMode.ToString();
            Form2 settings = new Form2();
            settings.ShowDialog();
            if(Properties.Settings.Default.DarkMode.ToString() != test)
            {
                if(Properties.Settings.Default.DarkMode == true)
                {
                    MessageBox.Show("Dark mode");
                    BackColorVar = Color.Gray;
                    ForeColorVar = Color.White;
                    DialogResult result = MessageBox.Show("We need to restart Kode. Dont worry, You wont lose your work");
                    if(result == DialogResult.OK)
                    {
                        var perUserAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        var path = Path.Combine(perUserAppData, @"kode");
                        System.IO.Directory.CreateDirectory(path);
                        path += @"\kodecache.txt";
                        System.IO.File.WriteAllText(path , scintilla.Text);
                        Application.Restart();
                    }
                }else
                {
                    MessageBox.Show("Light Mode");
                    BackColorVar = Color.White;
                    ForeColorVar = Color.Black;
                    DialogResult result = MessageBox.Show("We need to restart Kode. Dont worry, You wont lose your work");
                    if (result == DialogResult.OK)
                    {
                        var perUserAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        var path = Path.Combine(perUserAppData, @"kode");
                        System.IO.Directory.CreateDirectory(path);
                        path += @"\kodecache.txt";
                        System.IO.File.WriteAllText(path , scintilla.Text);
                        Application.Restart();
                    }
                }
            }
        }

        private void exportToKSPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Export to KSP scripts folder.
            Form3 formname = new Form3();
            var result = formname.ShowDialog();
            if (result == DialogResult.OK)
            {
                string file = formname.name;
                string location = Properties.Settings.Default.KSPLoc + @"\Ships\Script\" + file;
                System.IO.File.WriteAllText(location, scintilla.Text);
            }
        }

        private void HighlightWord(string text)
        {
            // Indicators 0-7 could be in use by a lexer
            // so we'll use indicator 8 to highlight words.
            const int NUM = 8;

            // Remove all uses of our indicator
            scintilla.IndicatorCurrent = NUM;
            scintilla.IndicatorClearRange(0, scintilla.TextLength);

            // Update indicator appearance
            scintilla.Indicators[NUM].Style = IndicatorStyle.StraightBox;
            scintilla.Indicators[NUM].Under = true;
            scintilla.Indicators[NUM].ForeColor = Color.Green;
            scintilla.Indicators[NUM].OutlineAlpha = 50;
            scintilla.Indicators[NUM].Alpha = 30;

            // Search the document
            scintilla.TargetStart = 0;
            scintilla.TargetEnd = scintilla.TextLength;
            scintilla.SearchFlags = SearchFlags.None;
            while (scintilla.SearchInTarget(text) != -1)
            {
                // Mark the search results with the current indicator
                scintilla.IndicatorFillRange(scintilla.TargetStart, scintilla.TargetEnd - scintilla.TargetStart);

                // Search the remainder of the document
                scintilla.TargetStart = scintilla.TargetEnd;
                scintilla.TargetEnd = scintilla.TextLength;
            }
        }

        private void licenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form5 licence = new Form5();
            licence.Show();
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

        private void scintilla_CharAdded(object sender, CharAddedEventArgs e)
        {
            //Autocompletion. Will eventually make nicer.
            // Find the word start
            var currentPos = scintilla.CurrentPosition;
            var wordStartPos = scintilla.WordStartPosition(currentPos, true);

            // Display the autocompletion list
            var lenEntered = currentPos - wordStartPos;
            if (lenEntered > 0)
            {
                scintilla.AutoCShow(lenEntered, "ADD ALL AT BATCH BREAK CLEARSCREEN COMPILE COPY DECLARE DELETE DEPLOY DO DO EDIT ELSE FILE FOR FROM FROM FUNCTION GLOBAL IF IN LIST LOCAL LOCK LOG OFF ON ONCE PARAMETER PRESERVE PRINT REBOOT REMOVE RENAME RUN SET SHUTDOWN STAGE STEP SWITCH THEN TO TOGGLE UNLOCK UNSET UNTIL VOLUME WAIT WHEN HEADING PROGRADE RETROGRADE FACING MAXTHRUST VELOCITY GEOPOSITION LATITUDE LONGITUDE UP NORTH BODY ANGULARMOMENTUM ANGULARVEL ANGULARVELOCITY COMMRANGE MASS VERTICALSPEED GROUNDSPEED AIRESPEED VESSELNAME ALTITUDE APOAPSIS PERIAPSIS SENSORS SRFPROGRADE SRFREROGRADE OBT STATUS SHIPNAME");

            }
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hey look, Another unimplemented feature");
        }
    }
}