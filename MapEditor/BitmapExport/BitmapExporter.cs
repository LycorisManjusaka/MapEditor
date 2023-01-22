using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace MapEditor.BitmapExport
{
  public class BitmapExporter
    {
        private MapView.TimeContent copiedArea;


        public BitmapExporter(MapView.TimeContent copiedArea)
        {
            this.copiedArea = copiedArea;
        }

        public Image FromTiles()
        {
            var tiles = copiedArea.StoredTiles;

            var points = new List<Point>();
            var colors = new List<Color>();

            foreach (var timeTile in tiles)
            {
                var tile = timeTile.Tile;
                var location = tile.Location;

                int x = (location.X + (BitmapCommon.PointCount - location.Y)) / 2;
                int y = (location.X + location.Y) / 2;

                points.Add(new Point(x, y));
                colors.Add(tile.col);
            }

            var pointMin = new Point(int.MaxValue, int.MaxValue);
            var pointMax = new Point(int.MinValue, int.MinValue);

            foreach (var point in points)
            {
                if (point.X < pointMin.X)
                    pointMin.X = point.X;
                if (point.Y < pointMin.Y)
                    pointMin.Y = point.Y;
                if (point.X > pointMax.X)
                    pointMax.X = point.X;
                if (point.Y > pointMax.Y)
                    pointMax.Y = point.Y;
            }

            var size = new Size(pointMax.X - pointMin.X + 1, pointMax.Y - pointMin.Y + 1);
            var bmp = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
            var gg = Graphics.FromImage(bmp);
            var rect = new Rectangle(new Point(), size);
            gg.FillRectangle(Brushes.Black, rect);

            for (int i = 0; i < points.Count; ++i)
            {
                var point = points[i];
                bmp.SetPixel(point.X - pointMin.X, point.Y - pointMin.Y, colors[i]);
            }

            return bmp;
        }

        public Image FromWalls()
        {
            var walls = copiedArea.StoredWalls;

            var points = new List<Point>();
            var colors = new List<Color>();

            foreach (var timeWall in walls)
            {
                var wall = timeWall.Wall;
                var location = wall.Location;
                var id = wall.matId;
                int x = (location.X + (BitmapCommon.PointCount - location.Y)) / 2;
                int y = (location.X + location.Y) / 2;

                points.Add(new Point(x, y));
                colors.Add(BitmapCommon.WallColors[id]);
            }

            var pointMin = new Point(int.MaxValue, int.MaxValue);
            var pointMax = new Point(int.MinValue, int.MinValue);

            foreach (var point in points)
            {
                if (point.X < pointMin.X)
                    pointMin.X = point.X;
                if (point.Y < pointMin.Y)
                    pointMin.Y = point.Y;
                if (point.X > pointMax.X)
                    pointMax.X = point.X;
                if (point.Y > pointMax.Y)
                    pointMax.Y = point.Y;
            }

            var size = new Size(pointMax.X - pointMin.X + 1, pointMax.Y - pointMin.Y + 1);
            var bmp = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
            var gg = Graphics.FromImage(bmp);
            var rect = new Rectangle(new Point(), size);
            gg.FillRectangle(Brushes.Black, rect);

            for (int i = 0; i < points.Count; ++i)
            {
                var point = points[i];
                bmp.SetPixel(point.X - pointMin.X, point.Y - pointMin.Y, colors[i]);
            }

            return bmp;
        }
    }
}
