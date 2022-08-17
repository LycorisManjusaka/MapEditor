using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapEditor.newgui
{
    public partial class PatternSelectDialog : Form
    {
        public PatternSelectDialog()
        {
            InitializeComponent();
        }

        private void tbPatternString_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) 
                && e.KeyChar != 'N' && e.KeyChar != 'n'
                && e.KeyChar != 'W' && e.KeyChar != 'w' 
                && e.KeyChar != 'S' && e.KeyChar != 's' 
                && e.KeyChar != 'E' && e.KeyChar != 'e')
            {
                e.Handled = true;
            }
        }

        private void tbPatternString_TextChanged(object sender, EventArgs e)
        {
            int selectionStart = tbPatternString.SelectionStart;
            int selectionLength = tbPatternString.SelectionLength;
            tbPatternString.Text = tbPatternString.Text.ToUpper();
            tbPatternString.SelectionStart = selectionStart;
            tbPatternString.SelectionLength = selectionLength;
            pbPreview.Update();
        }


        private void pbPreview_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
        }
    }
}
