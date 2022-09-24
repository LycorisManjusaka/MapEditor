using MapEditor.BitmapExport;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static NoxShared.ThingDb;

namespace MapEditor
{
    public partial class EdgeRuleForm : Form
    {
        private Dictionary<TileId, Dictionary<TileId, EdgeId>> edgeRules 
            = new Dictionary<TileId, Dictionary<TileId, EdgeId>>();
        private bool isChanging = false;

        public Dictionary<TileId, Dictionary<TileId, EdgeId>> Rules
        {
            get { return edgeRules; }
            set
            {
                edgeRules = value;
                RulesToText();
            }
        }

        private void RulesToText()
        {
            isChanging = true;
            var lines = Analizer.RulesToLines(edgeRules);
            tbText.Lines = lines.ToArray();
            isChanging = false;
        }

        public bool RuleMakingMode => chbRuleMakingMode.Checked;
        private string RulesDir => Path.GetDirectoryName(Application.ExecutablePath) + "\\Rules";
        public EdgeRuleForm()
        {
            InitializeComponent();

            ReloadFiles();
            cmbPattern.SelectedIndex = cmbPattern.Items.IndexOf("Common");
            tbText_TextChanged(this, new EventArgs());
        }

        private void EdgeRuleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void ReloadFiles()
        {
            cmbPattern.Items.Clear();
            var dir = Path.GetDirectoryName(Application.ExecutablePath)  + "\\Rules";
            var files = Directory.GetFiles(dir);
            foreach(var file in files)
            {
                var ext = Path.GetExtension(file);
                if (ext != ".rul")
                    continue;
                cmbPattern.Items.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        private void EdgeRuleForm_Load(object sender, EventArgs e)
        {

        }

        private void cmbPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            var path = RulesDir 
                + "\\" + cmbPattern.Items[cmbPattern.SelectedIndex].ToString() + ".rul";
            tbText.Lines = File.ReadAllLines(path);
        }


        private void tbText_TextChanged(object sender, EventArgs e)
        {
            if (isChanging)
                return;
            isChanging = true;
            edgeRules.Clear();

            SendMessage(tbText.Handle, WM_SETREDRAW, false, 0);

            int selectionStartBuff = tbText.SelectionStart;
            int selectionLengthBuff = tbText.SelectionLength;

            //tbText.Hide();

            var lines = tbText.Lines;
            int index = 0;
            foreach (var line in lines)
            {
                var words = line.Split(new char[] { '\t', ' ' });
                bool ok = true;
                try
                {
                    var neighTileId = (TileId)Enum.Parse(typeof(TileId), words[0]);
                    var tileId = (TileId)Enum.Parse(typeof(TileId), words[1]);
                    var edgeId = (EdgeId)Enum.Parse(typeof(EdgeId), words[2]);

                    if (!edgeRules.ContainsKey(neighTileId))
                        edgeRules[neighTileId] = new Dictionary<TileId, EdgeId>();

                    if (!edgeRules[neighTileId].ContainsKey(tileId))
                        edgeRules[neighTileId][tileId] = edgeId;
                }
                catch (Exception)
                {
                    ok = false;
                }


                int position = tbText.GetFirstCharIndexFromLine(index);
                if (position < 0)
                {
                    // lineNumber is too big
                    tbText.Select(tbText.Text.Length, 0);
                }
                else
                {
                    int lineEnd = tbText.Text.IndexOf("\n", position);
                    if (lineEnd < 0)
                    {
                        lineEnd = tbText.Text.Length;
                    }

                    tbText.Select(position, lineEnd - position);
                }
                tbText.SelectionColor 
                    = Color.FromKnownColor(ok ? KnownColor.Black : KnownColor.Red);

                ++index;
            }

           // tbText.Show();

            tbText.SelectionStart = selectionStartBuff;
            tbText.SelectionLength = selectionLengthBuff;

            // Do your thingies here
            SendMessage(tbText.Handle, WM_SETREDRAW, true, 0);

            Refresh();
            isChanging = false;
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        private const int WM_SETREDRAW = 11;

        private void bSaveAs_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Edge rules (*.rul)|*.rul",
                FilterIndex = 0,
                InitialDirectory = RulesDir
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Analizer.RulesToFile(edgeRules, dialog.FileName);
                ReloadFiles();
                cmbPattern.SelectedIndex 
                    = cmbPattern.Items.IndexOf(Path.GetFileNameWithoutExtension(dialog.FileName));
                tbText_TextChanged(this, new EventArgs());
            }
        }
    }
}
