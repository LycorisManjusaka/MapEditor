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

        private Dictionary<TileId, WallId> wallRules = new Dictionary<TileId, WallId>();

        private bool isChanging = false;

        public Dictionary<TileId, Dictionary<TileId, EdgeId>> EdgeRules
        {
            get { return edgeRules; }
            set
            {
                edgeRules = value;
                EdgeRulesToText();
            }
        }

        public Dictionary<TileId, WallId> WallRules
        {
            get { return wallRules; }
            set
            {
                wallRules = value;
                EdgeRulesToText();
            }
        }

        private void EdgeRulesToText()
        {
            isChanging = true;
            var lines = EdgeAnalizer.RulesToLines(edgeRules);
            tbEdgeText.Lines = lines.ToArray();
            isChanging = false;
        }

        public bool EdgeMakingMode => chbEdgeRuleMakingMode.Checked;
        private string RulesDir => Path.GetDirectoryName(Application.ExecutablePath) + "\\Rules";
        public EdgeRuleForm()
        {
            InitializeComponent();

            ReloadEdgeFiles();
            ReloadWallFiles();
            cmbEdgePattern.SelectedIndex = cmbEdgePattern.Items.IndexOf("Common");
            cmbWallPattern.SelectedIndex = cmbWallPattern.Items.IndexOf("Common");
            tbEdgeText_TextChanged(this, new EventArgs());
            tbWallText_TextChanged(this, new EventArgs());
        }



        private void EdgeRuleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void ReloadEdgeFiles()
        {
            cmbEdgePattern.Items.Clear();
            var dir = Path.GetDirectoryName(Application.ExecutablePath)  + "\\Rules";
            var files = Directory.GetFiles(dir);
            foreach(var file in files)
            {
                var ext = Path.GetExtension(file);
                if (ext != ".rul")
                    continue;
                cmbEdgePattern.Items.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        private void ReloadWallFiles()
        {
            cmbWallPattern.Items.Clear();
            var dir = Path.GetDirectoryName(Application.ExecutablePath) + "\\Rules";
            var files = Directory.GetFiles(dir);
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file);
                if (ext != ".wrul")
                    continue;
                cmbWallPattern.Items.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        private void EdgeRuleForm_Load(object sender, EventArgs e)
        {

        }

        private void cmbPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            var path = RulesDir 
                + "\\" + cmbEdgePattern.Items[cmbEdgePattern.SelectedIndex].ToString() + ".rul";
            tbEdgeText.Lines = File.ReadAllLines(path);
        }

        private void cmbWallPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            var path = RulesDir
                + "\\" + cmbWallPattern.Items[cmbWallPattern.SelectedIndex].ToString() + ".wrul";
            tbWallText.Lines = File.ReadAllLines(path);
        }

        private void tbEdgeText_TextChanged(object sender, EventArgs e)
        {
            if (isChanging)
                return;
            isChanging = true;
            edgeRules.Clear();

            SendMessage(tbEdgeText.Handle, WM_SETREDRAW, false, 0);

            int selectionStartBuff = tbEdgeText.SelectionStart;
            int selectionLengthBuff = tbEdgeText.SelectionLength;

            //tbText.Hide();

            var lines = tbEdgeText.Lines;
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


                int position = tbEdgeText.GetFirstCharIndexFromLine(index);
                if (position < 0)
                {
                    // lineNumber is too big
                    tbEdgeText.Select(tbEdgeText.Text.Length, 0);
                }
                else
                {
                    int lineEnd = tbEdgeText.Text.IndexOf("\n", position);
                    if (lineEnd < 0)
                    {
                        lineEnd = tbEdgeText.Text.Length;
                    }

                    tbEdgeText.Select(position, lineEnd - position);
                }
                tbEdgeText.SelectionColor 
                    = Color.FromKnownColor(ok ? KnownColor.Black : KnownColor.Red);

                ++index;
            }

           // tbText.Show();
            tbEdgeText.SelectionStart = selectionStartBuff;
            tbEdgeText.SelectionLength = selectionLengthBuff;

            // Do your thingies here
            SendMessage(tbEdgeText.Handle, WM_SETREDRAW, true, 0);

            Refresh();
            isChanging = false;
        }

        private void tbWallText_TextChanged(object sender, EventArgs e)
        {
            if (isChanging)
                return;
            isChanging = true;
            wallRules.Clear();

            SendMessage(tbWallText.Handle, WM_SETREDRAW, false, 0);

            int selectionStartBuff = tbWallText.SelectionStart;
            int selectionLengthBuff = tbWallText.SelectionLength;

            //tbText.Hide();

            var lines = tbWallText.Lines;
            int index = 0;
            foreach (var line in lines)
            {
                var words = line.Split(new char[] { '\t', ' ' });
                bool ok = true;
                try
                {
                    var tileId = (TileId)Enum.Parse(typeof(TileId), words[0]);
                    var wallId = (WallId)Enum.Parse(typeof(WallId), words[1]);
                    wallRules[tileId] = wallId;
                }
                catch (Exception)
                {
                    ok = false;
                }


                int position = tbWallText.GetFirstCharIndexFromLine(index);
                if (position < 0)
                {
                    // lineNumber is too big
                    tbWallText.Select(tbWallText.Text.Length, 0);
                }
                else
                {
                    int lineEnd = tbWallText.Text.IndexOf("\n", position);
                    if (lineEnd < 0)
                    {
                        lineEnd = tbWallText.Text.Length;
                    }

                    tbWallText.Select(position, lineEnd - position);
                }
                tbWallText.SelectionColor
                    = Color.FromKnownColor(ok ? KnownColor.Black : KnownColor.Red);

                ++index;
            }

            // tbText.Show();

            tbWallText.SelectionStart = selectionStartBuff;
            tbWallText.SelectionLength = selectionLengthBuff;

            // Do your thingies here
            SendMessage(tbWallText.Handle, WM_SETREDRAW, true, 0);

            Refresh();
            isChanging = false;
        }

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
                EdgeAnalizer.RulesToFile(edgeRules, dialog.FileName);
                ReloadEdgeFiles();

                cmbEdgePattern.SelectedIndex 
                    = cmbEdgePattern.Items
                    .IndexOf(Path.GetFileNameWithoutExtension(dialog.FileName));

                tbEdgeText_TextChanged(this, new EventArgs());
            }
        }

        private void bWallSaveAs_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Wall rules (*.wrul)|*.wrul",
                FilterIndex = 0,
                InitialDirectory = RulesDir
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                WallAnalizer.RulesToFile(wallRules, dialog.FileName);
                ReloadWallFiles();

                cmbWallPattern.SelectedIndex
                    = cmbWallPattern.Items
                    .IndexOf(Path.GetFileNameWithoutExtension(dialog.FileName));

                tbWallText_TextChanged(this, new EventArgs());
            }
        }



        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);
        private const int WM_SETREDRAW = 11;

    }
}
