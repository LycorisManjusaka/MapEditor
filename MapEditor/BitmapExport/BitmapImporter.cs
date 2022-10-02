using System.Drawing;
using static MapEditor.BitmapExport.BitmapCommon;

namespace MapEditor.BitmapExport
{
    public class BitmapImporter
    {
        protected readonly Bitmap bitmap;

        public const BaseDir South = BaseDir.South;
        public const BaseDir North = BaseDir.North;
        public const BaseDir East = BaseDir.East;
        public const BaseDir West = BaseDir.West;
        public const BaseDir SE = BaseDir.SE;
        public const BaseDir NW = BaseDir.NW;
        public const BaseDir NE = BaseDir.NE;
        public const BaseDir SW = BaseDir.SW;

        public BitmapImporter(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

    }
}
