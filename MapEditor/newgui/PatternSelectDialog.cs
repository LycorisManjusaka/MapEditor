using MapEditor.MapInt;
using NoxShared;
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
            pbPreview.Refresh();
        }

        private void tbPatternString_TextChanged(object sender, EventArgs e)
        {
            int selectionStart = tbPatternString.SelectionStart;
            int selectionLength = tbPatternString.SelectionLength;
            tbPatternString.Text = tbPatternString.Text.ToUpper();
            tbPatternString.SelectionStart = selectionStart;
            tbPatternString.SelectionLength = selectionLength;
            pbPreview.Refresh();
        }


        private RectangleF PatternRectangle
        {
            get 
            {
                RectangleF current = new RectangleF(new PointF(0.0f, 0.0f), new SizeF(1.0f, 1.0f));

                int length = tbPatternString.Text.Length;
                for (int i = 0; i < length; i++)
                {
                    RectangleF part = new RectangleF();

                    char ch = tbPatternString.Text[i];
                    switch (ch)
                    {
                        case 'W':
                            part = new RectangleF(new PointF(0.0f, 0.5f), new SizeF(0.5f, 0.5f));
                            break;
                        case 'N':
                            part = new RectangleF(new PointF(0.0f, 0.0f), new SizeF(0.5f, 0.5f));
                            break;
                        case 'E':
                            part = new RectangleF(new PointF(0.5f, 0.0f), new SizeF(0.5f, 0.5f));
                            break;
                        case 'S':
                            part = new RectangleF(new PointF(0.5f, 0.5f), new SizeF(0.5f, 0.5f));
                            break;
                    }

                    current = new RectangleF(
                        new PointF(
                            current.X + part.X * current.Width, current.Y + part.Y * current.Height),
                        new SizeF(current.Width / 2.0f, current.Height / 2.0f));
                }
                return current;
            }
        }

        private void pbPreview_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
    
            g.FillRectangle(
                Brushes.White, 
                new RectangleF(
                    new PointF(0.0f, 0.0f), 
                               new SizeF(pbPreview.Size.Width, pbPreview.Size.Height)));
            var patternRectangle = PatternRectangle;

            g.FillRectangle(
                Brushes.Black,
                new RectangleF(
                    new PointF(
                        patternRectangle.X * pbPreview.Size.Width,
                        patternRectangle.Y * pbPreview.Size.Height),
                    new SizeF(
                        patternRectangle.Size.Width * pbPreview.Size.Width,
                        patternRectangle.Size.Height * pbPreview.Size.Height)));
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            var maxSize = new SizeF(5888.0f, 5888.0f);

            var patternRectangle = PatternRectangle;

            var selectionRect = new RectangleF(
                    new PointF(
                        patternRectangle.X * maxSize.Width,
                        patternRectangle.Y * maxSize.Height),
                    new SizeF(
                        patternRectangle.Size.Width * maxSize.Width,
                        patternRectangle.Size.Height * maxSize.Height));

            foreach (var item in MapInterface.TheMap.Objects)
            {
                if (item is Map.Object obj) 
                {
                    if (selectionRect.Contains(obj.Location))
                    {
                        MapInterface.SelectedObjects.Items.Add(obj);
                    }
                }
            }
        }

        private void PatternSelectDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void btnClearSelection_Click(object sender, EventArgs e)
        {
            MapInterface.SelectedObjects.Items.Clear();
        }
    }
}
