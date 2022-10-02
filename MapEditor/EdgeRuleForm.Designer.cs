
namespace MapEditor
{
    partial class EdgeRuleForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tbTilesEdges = new System.Windows.Forms.TabPage();
            this.tbEdgeText = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chbEdgeRuleMakingMode = new System.Windows.Forms.CheckBox();
            this.bEdgeSaveAs = new System.Windows.Forms.Button();
            this.cmbEdgePattern = new System.Windows.Forms.ComboBox();
            this.tbRulePattern = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tbWallText = new System.Windows.Forms.RichTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chbWallRuleMakingMode = new System.Windows.Forms.CheckBox();
            this.bWallSaveAs = new System.Windows.Forms.Button();
            this.cmbWallPattern = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tcMain.SuspendLayout();
            this.tbTilesEdges.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tbTilesEdges);
            this.tcMain.Controls.Add(this.tabPage1);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(320, 580);
            this.tcMain.TabIndex = 1;
            // 
            // tbTilesEdges
            // 
            this.tbTilesEdges.Controls.Add(this.tbEdgeText);
            this.tbTilesEdges.Controls.Add(this.panel1);
            this.tbTilesEdges.Location = new System.Drawing.Point(4, 22);
            this.tbTilesEdges.Name = "tbTilesEdges";
            this.tbTilesEdges.Padding = new System.Windows.Forms.Padding(3);
            this.tbTilesEdges.Size = new System.Drawing.Size(312, 554);
            this.tbTilesEdges.TabIndex = 0;
            this.tbTilesEdges.Text = "Tiles -> Edges";
            this.tbTilesEdges.UseVisualStyleBackColor = true;
            // 
            // tbEdgeText
            // 
            this.tbEdgeText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbEdgeText.Location = new System.Drawing.Point(3, 71);
            this.tbEdgeText.Name = "tbEdgeText";
            this.tbEdgeText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.tbEdgeText.Size = new System.Drawing.Size(306, 480);
            this.tbEdgeText.TabIndex = 2;
            this.tbEdgeText.Text = "";
            this.tbEdgeText.WordWrap = false;
            this.tbEdgeText.TextChanged += new System.EventHandler(this.tbEdgeText_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chbEdgeRuleMakingMode);
            this.panel1.Controls.Add(this.bEdgeSaveAs);
            this.panel1.Controls.Add(this.cmbEdgePattern);
            this.panel1.Controls.Add(this.tbRulePattern);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(306, 68);
            this.panel1.TabIndex = 1;
            // 
            // chbEdgeRuleMakingMode
            // 
            this.chbEdgeRuleMakingMode.AutoSize = true;
            this.chbEdgeRuleMakingMode.Location = new System.Drawing.Point(8, 41);
            this.chbEdgeRuleMakingMode.Name = "chbEdgeRuleMakingMode";
            this.chbEdgeRuleMakingMode.Size = new System.Drawing.Size(116, 17);
            this.chbEdgeRuleMakingMode.TabIndex = 3;
            this.chbEdgeRuleMakingMode.Text = "Rule Making Mode";
            this.chbEdgeRuleMakingMode.UseVisualStyleBackColor = true;
            // 
            // bEdgeSaveAs
            // 
            this.bEdgeSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bEdgeSaveAs.Location = new System.Drawing.Point(226, 37);
            this.bEdgeSaveAs.Name = "bEdgeSaveAs";
            this.bEdgeSaveAs.Size = new System.Drawing.Size(75, 23);
            this.bEdgeSaveAs.TabIndex = 2;
            this.bEdgeSaveAs.Text = "Save as...";
            this.bEdgeSaveAs.UseVisualStyleBackColor = true;
            this.bEdgeSaveAs.Click += new System.EventHandler(this.bSaveAs_Click);
            // 
            // cmbEdgePattern
            // 
            this.cmbEdgePattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbEdgePattern.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEdgePattern.FormattingEnabled = true;
            this.cmbEdgePattern.Location = new System.Drawing.Point(79, 10);
            this.cmbEdgePattern.Name = "cmbEdgePattern";
            this.cmbEdgePattern.Size = new System.Drawing.Size(222, 21);
            this.cmbEdgePattern.TabIndex = 1;
            this.cmbEdgePattern.SelectedIndexChanged += new System.EventHandler(this.cmbPattern_SelectedIndexChanged);
            // 
            // tbRulePattern
            // 
            this.tbRulePattern.AutoSize = true;
            this.tbRulePattern.Location = new System.Drawing.Point(5, 13);
            this.tbRulePattern.Name = "tbRulePattern";
            this.tbRulePattern.Size = new System.Drawing.Size(68, 13);
            this.tbRulePattern.TabIndex = 0;
            this.tbRulePattern.Text = "Rule pattern:";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbWallText);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(312, 554);
            this.tabPage1.TabIndex = 1;
            this.tabPage1.Text = "Tiles -> Walls";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tbWallText
            // 
            this.tbWallText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbWallText.Location = new System.Drawing.Point(3, 71);
            this.tbWallText.Name = "tbWallText";
            this.tbWallText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.tbWallText.Size = new System.Drawing.Size(306, 480);
            this.tbWallText.TabIndex = 4;
            this.tbWallText.Text = "";
            this.tbWallText.WordWrap = false;
            this.tbWallText.TextChanged += new System.EventHandler(this.tbWallText_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.chbWallRuleMakingMode);
            this.panel2.Controls.Add(this.bWallSaveAs);
            this.panel2.Controls.Add(this.cmbWallPattern);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(306, 68);
            this.panel2.TabIndex = 3;
            // 
            // chbWallRuleMakingMode
            // 
            this.chbWallRuleMakingMode.AutoSize = true;
            this.chbWallRuleMakingMode.Location = new System.Drawing.Point(8, 41);
            this.chbWallRuleMakingMode.Name = "chbWallRuleMakingMode";
            this.chbWallRuleMakingMode.Size = new System.Drawing.Size(116, 17);
            this.chbWallRuleMakingMode.TabIndex = 3;
            this.chbWallRuleMakingMode.Text = "Rule Making Mode";
            this.chbWallRuleMakingMode.UseVisualStyleBackColor = true;
            // 
            // bWallSaveAs
            // 
            this.bWallSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bWallSaveAs.Location = new System.Drawing.Point(226, 37);
            this.bWallSaveAs.Name = "bWallSaveAs";
            this.bWallSaveAs.Size = new System.Drawing.Size(75, 23);
            this.bWallSaveAs.TabIndex = 2;
            this.bWallSaveAs.Text = "Save as...";
            this.bWallSaveAs.UseVisualStyleBackColor = true;
            this.bWallSaveAs.Click += new System.EventHandler(this.bWallSaveAs_Click);
            // 
            // cmbWallPattern
            // 
            this.cmbWallPattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbWallPattern.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWallPattern.FormattingEnabled = true;
            this.cmbWallPattern.Location = new System.Drawing.Point(79, 10);
            this.cmbWallPattern.Name = "cmbWallPattern";
            this.cmbWallPattern.Size = new System.Drawing.Size(222, 21);
            this.cmbWallPattern.TabIndex = 1;
            this.cmbWallPattern.SelectedIndexChanged += new System.EventHandler(this.cmbWallPattern_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Rule pattern:";
            // 
            // EdgeRuleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 580);
            this.Controls.Add(this.tcMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "EdgeRuleForm";
            this.Text = "Image Pasting Rules";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EdgeRuleForm_FormClosing);
            this.Load += new System.EventHandler(this.EdgeRuleForm_Load);
            this.tcMain.ResumeLayout(false);
            this.tbTilesEdges.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tbTilesEdges;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chbEdgeRuleMakingMode;
        private System.Windows.Forms.Button bEdgeSaveAs;
        private System.Windows.Forms.ComboBox cmbEdgePattern;
        private System.Windows.Forms.Label tbRulePattern;
        private System.Windows.Forms.RichTextBox tbEdgeText;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RichTextBox tbWallText;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox chbWallRuleMakingMode;
        private System.Windows.Forms.Button bWallSaveAs;
        private System.Windows.Forms.ComboBox cmbWallPattern;
        private System.Windows.Forms.Label label1;
    }
}