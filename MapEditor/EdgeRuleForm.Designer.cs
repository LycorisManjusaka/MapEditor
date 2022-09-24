
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
            this.tbText = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chbRuleMakingMode = new System.Windows.Forms.CheckBox();
            this.bSaveAs = new System.Windows.Forms.Button();
            this.cmbPattern = new System.Windows.Forms.ComboBox();
            this.tbRulePattern = new System.Windows.Forms.Label();
            this.tcMain.SuspendLayout();
            this.tbTilesEdges.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tbTilesEdges);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(320, 580);
            this.tcMain.TabIndex = 1;
            // 
            // tbTilesEdges
            // 
            this.tbTilesEdges.Controls.Add(this.tbText);
            this.tbTilesEdges.Controls.Add(this.panel1);
            this.tbTilesEdges.Location = new System.Drawing.Point(4, 22);
            this.tbTilesEdges.Name = "tbTilesEdges";
            this.tbTilesEdges.Padding = new System.Windows.Forms.Padding(3);
            this.tbTilesEdges.Size = new System.Drawing.Size(312, 554);
            this.tbTilesEdges.TabIndex = 0;
            this.tbTilesEdges.Text = "Tiles -> Edges";
            this.tbTilesEdges.UseVisualStyleBackColor = true;
            // 
            // tbText
            // 
            this.tbText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbText.Location = new System.Drawing.Point(3, 71);
            this.tbText.Name = "tbText";
            this.tbText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.tbText.Size = new System.Drawing.Size(306, 480);
            this.tbText.TabIndex = 2;
            this.tbText.Text = "";
            this.tbText.WordWrap = false;
            this.tbText.TextChanged += new System.EventHandler(this.tbText_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chbRuleMakingMode);
            this.panel1.Controls.Add(this.bSaveAs);
            this.panel1.Controls.Add(this.cmbPattern);
            this.panel1.Controls.Add(this.tbRulePattern);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(306, 68);
            this.panel1.TabIndex = 1;
            // 
            // chbRuleMakingMode
            // 
            this.chbRuleMakingMode.AutoSize = true;
            this.chbRuleMakingMode.Location = new System.Drawing.Point(8, 41);
            this.chbRuleMakingMode.Name = "chbRuleMakingMode";
            this.chbRuleMakingMode.Size = new System.Drawing.Size(116, 17);
            this.chbRuleMakingMode.TabIndex = 3;
            this.chbRuleMakingMode.Text = "Rule Making Mode";
            this.chbRuleMakingMode.UseVisualStyleBackColor = true;
            // 
            // bSaveAs
            // 
            this.bSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bSaveAs.Location = new System.Drawing.Point(226, 37);
            this.bSaveAs.Name = "bSaveAs";
            this.bSaveAs.Size = new System.Drawing.Size(75, 23);
            this.bSaveAs.TabIndex = 2;
            this.bSaveAs.Text = "Save as...";
            this.bSaveAs.UseVisualStyleBackColor = true;
            this.bSaveAs.Click += new System.EventHandler(this.bSaveAs_Click);
            // 
            // cmbPattern
            // 
            this.cmbPattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPattern.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPattern.FormattingEnabled = true;
            this.cmbPattern.Location = new System.Drawing.Point(79, 10);
            this.cmbPattern.Name = "cmbPattern";
            this.cmbPattern.Size = new System.Drawing.Size(222, 21);
            this.cmbPattern.TabIndex = 1;
            this.cmbPattern.SelectedIndexChanged += new System.EventHandler(this.cmbPattern_SelectedIndexChanged);
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
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tbTilesEdges;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chbRuleMakingMode;
        private System.Windows.Forms.Button bSaveAs;
        private System.Windows.Forms.ComboBox cmbPattern;
        private System.Windows.Forms.Label tbRulePattern;
        private System.Windows.Forms.RichTextBox tbText;
    }
}