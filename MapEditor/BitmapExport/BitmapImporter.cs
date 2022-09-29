using System.Collections.Generic;
using System.Drawing;
using static MapEditor.BitmapExport.BitmapCommon;

namespace MapEditor.BitmapExport
{
    public class BitmapImporter
    {
        private readonly Bitmap bitmap;
        private const int pointCount = 252;

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

        protected void GetColorsAndLocations(
            out List<Color> colors, out List<Point> locations, out Point pointMin, 
            out Point pointMax)
        {
            var points = new List<Point>();
            colors = new List<Color>();
            var size = bitmap.Size;
            for (int i = 0; i < size.Width; ++i)
            {
                for (int j = 0; j < size.Height; ++j)
                {
                    points.Add(new Point(i, j));
                    colors.Add(bitmap.GetPixel(i, j));
                }
            }

            locations = new List<Point>();
            foreach (var point in points)
            {
                Point location = new Point
                {
                    X = point.X + point.Y - pointCount / 2,
                    Y = -point.X + point.Y + pointCount / 2
                };
                locations.Add(location);
            }

            pointMin = new Point(int.MaxValue, int.MaxValue);
            pointMax = new Point(int.MinValue, int.MinValue);
            foreach (var location in locations)
            {
                if (location.X < pointMin.X)
                    pointMin.X = location.X;
                if (location.Y < pointMin.Y)
                    pointMin.Y = location.Y;
                if (location.X > pointMax.X)
                    pointMax.X = location.X;
                if (location.Y > pointMax.Y)
                    pointMax.Y = location.Y;
            }
        }
    }
}
