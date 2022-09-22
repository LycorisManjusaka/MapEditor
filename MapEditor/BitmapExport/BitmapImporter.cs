using MapEditor.newgui;
using NoxShared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using static NoxShared.Map.Tile;
using static NoxShared.ThingDb;

namespace MapEditor.BitmapExport
{
    public class BitmapImporter
    {
        private readonly Bitmap bitmap;
        private const int pointCount = 252;
        private const TileId invalid = TileId.Invalid;
        private readonly Dictionary<uint, TileId> colorMap = new Dictionary<uint, TileId>();       
        
        private readonly Dictionary<TileId, Dictionary<TileId, EdgeId>> edgeRules 
            = new Dictionary<TileId, Dictionary<TileId, EdgeId>>();

        private TileId[,] typeMap;
        private MapView.TimeTile[,] tile2dMap;
        public const EdgeTile.Direction South = EdgeTile.Direction.South;
        public const EdgeTile.Direction North = EdgeTile.Direction.North;
        public const EdgeTile.Direction East = EdgeTile.Direction.East;
        public const EdgeTile.Direction West = EdgeTile.Direction.West;
        public const EdgeTile.Direction SE = EdgeTile.Direction.SE_Tip;
        public const EdgeTile.Direction NW = EdgeTile.Direction.NW_Tip;
        public const EdgeTile.Direction NE = EdgeTile.Direction.NE_Tip;
        public const EdgeTile.Direction SW = EdgeTile.Direction.SW_Tip;
        public static readonly Dictionary<EdgeTile.Direction, int[]> OffsetMap
            = new Dictionary<EdgeTile.Direction, int[]>
            {
                { South, new int[]{ -1, -1 } },
                { North, new int[]{ 1, 1 } },
                { East, new int[]{ -1, 1 } },
                { West, new int[]{ 1, -1 } },
                { SE, new int[]{ -2, 0} },
                { NW, new int[]{ 2, 0} },
                { NE, new int[]{ 0, 2} },
                { SW, new int[]{ 0, -2} }
            };

        public BitmapImporter(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            for (int i = 0; i < Map.tilecolors.Count(); ++i)
            {
                colorMap[Map.tilecolors[i]] = (TileId)i;
            }

            edgeRules = Analizer.LoadRules("E:/Dev/Nox/EdgeRules/current.rul");
        }

        public MapView.TimeContent ToTiles()
        {
            MakeTile2dMap();
            AddEdges();
            return MakeResult();
        }

        private void MakeTile2dMap()
        {
            var points = new List<Point>();
            var colors = new List<Color>();

            var size = bitmap.Size;
            for (int i = 0; i < size.Width; ++i)
            {
                for (int j = 0; j < size.Height; ++j)
                {
                    points.Add(new Point(i, j));
                    colors.Add(bitmap.GetPixel(i, j));
                }
            }

            var locations = new List<Point>();
            foreach (var point in points)
            {
                Point location = new Point
                {
                    X = point.X + point.Y - pointCount / 2,
                    Y = -point.X + point.Y + pointCount / 2
                };
                locations.Add(location);
            }

            var pointMin = new Point(int.MaxValue, int.MaxValue);
            var pointMax = new Point(int.MinValue, int.MinValue);

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
            var black = Color.FromArgb(255, 0, 0, 0);

            tile2dMap
                = new MapView.TimeTile[pointMax.X - pointMin.X + 1, pointMax.Y - pointMin.Y + 1];

            Bitmap test = new Bitmap(tile2dMap.GetLength(0), tile2dMap.GetLength(1),
                PixelFormat.Format24bppRgb);

            for (int i = 0; i < tile2dMap.GetLength(0); ++i)
            {
                for (int j = 0; j < tile2dMap.GetLength(1); ++j)
                {
                    tile2dMap[i, tile2dMap.GetLength(1) - j - 1] = null;
                    test.SetPixel(i, tile2dMap.GetLength(1) - j - 1, Color.FromKnownColor(KnownColor.Black));
                }
            }


            for (int i = 0; i < locations.Count; ++i)
            {
                var color = colors[i];
                if (color == black)
                    continue;

                uint colorInt = (uint)color.ToArgb();
                if (!colorMap.ContainsKey(colorInt))
                    continue;

                TileId graphicsId = colorMap[colorInt];
                var location = locations[i];
                var tilePoint = new Point(location.X - pointMin.X, location.Y - pointMin.Y);

                int x = tilePoint.X % pointCount;
                int y = tilePoint.Y % pointCount;
                int cols = FloorTiles[(int)graphicsId].numCols;
                int rows = FloorTiles[(int)graphicsId].numRows;

                ushort vari = (ushort)(((x + y) / 2 % cols) + ((y % rows) + 1 + cols
                    - (x + y) / 2 % cols) % rows * cols);

                MapView.TimeTile timeTile = new MapView.TimeTile
                {
                    Tile = new Map.Tile(tilePoint, graphicsId, vari)
                };

                //Tile tileDb = FloorTiles[(int)graphicsId];
                //int indextile = (int)tileDb.Variations[vari];

                timeTile.EdgeTiles = timeTile.Tile.EdgeTiles;
                tile2dMap[tilePoint.X, tile2dMap.GetLength(1) - tilePoint.Y - 1] = timeTile;

                test.SetPixel(tilePoint.X, tile2dMap.GetLength(1) - tilePoint.Y - 1, 
                    timeTile.Tile.col);
            }

            test.Save("E:/Dev/Nox/EdgeRules/test.bmp");
        }

        private void AddEdges()
        {
            typeMap = MakeTypeMap(tile2dMap);
            foreach (var firstTileRule in edgeRules)
            {
                TileId firstTile = firstTileRule.Key;
                var secondTileRules = firstTileRule.Value;
                foreach (var secondTileRule in secondTileRules)
                {
                    TileId secondTile = secondTileRule.Key;
                    EdgeId edge = secondTileRule.Value;

                    for (int i = 0; i < tile2dMap.GetLength(0); ++i)
                    {
                        for (int j = 0; j < tile2dMap.GetLength(1); ++j)
                        {
                            EdgeInnerCorners(i, j, firstTile, secondTile, edge);
                        }
                    }

                    for (int i = 0; i < tile2dMap.GetLength(0); ++i)
                    {
                        for (int j = 0; j < tile2dMap.GetLength(1); ++j)
                        {
                            EdgeTips(i, j, firstTile, secondTile, edge);
                        }
                    }

                    for (int i = 0; i < tile2dMap.GetLength(0); ++i)
                    {
                        for (int j = 0; j < tile2dMap.GetLength(1); ++j)
                        {
                            EdgeSides(i, j, firstTile, secondTile, edge);
                        }
                    }
                }
            }
        }


        private void EdgeInnerCorners(
            int i, int j, TileId firstTile, TileId secondTile, EdgeId edge)
        {
            TileId center = Id(i, j);
            if (center == invalid)
                return;

            if (EdgeInPair(firstTile, Id(i, j, NW)) == edge &&
                Id(i, j, North) == firstTile &&
                Id(i, j, West) == firstTile)         
                AddEdge(GetTile(i, j, NW), GetTile(i, j), edge, EdgeTile.Direction.NW_Sides);
            
            if (EdgeInPair(firstTile, Id(i, j, SE)) == edge &&
                Id(i, j, South) == firstTile &&
                Id(i, j, East) == firstTile)         
                AddEdge(GetTile(i, j, SE), GetTile(i, j), edge, EdgeTile.Direction.SE_Sides);
            
            if (EdgeInPair(firstTile, Id(i, j, NE)) == edge &&
                Id(i, j, North) == firstTile &&
                Id(i, j, East) == firstTile)         
                AddEdge(GetTile(i, j, NE), GetTile(i, j), edge, EdgeTile.Direction.NE_Sides);
            
            if (EdgeInPair(firstTile, Id(i, j, SW)) == edge &&
                Id(i, j, South) == firstTile &&
                Id(i, j, West) == firstTile)        
                AddEdge(GetTile(i, j, SW), GetTile(i, j), edge, EdgeTile.Direction.SW_Sides);           
        }

        private void EdgeSides(int i, int j, TileId firstTile, TileId secondTile, EdgeId edge)
        {
            TileId center = Id(i, j);
            if (center == invalid)
                return;

            if (center == firstTile && EdgeInPair(firstTile, Id(i, j, South)) == edge)         
                AddEdge(GetTile(i, j, South), GetTile(i, j), edge, South);
       
            if (center == firstTile && EdgeInPair(firstTile, Id(i, j, North)) == edge)          
                AddEdge(GetTile(i, j, North), GetTile(i, j), edge, North);
            
            if (center == firstTile && EdgeInPair(firstTile, Id(i, j, East)) == edge)         
                AddEdge(GetTile(i, j, East), GetTile(i, j), edge, East);
            
            if (center == firstTile && EdgeInPair(firstTile, Id(i, j, West)) == edge)       
                AddEdge(GetTile(i, j, West), GetTile(i, j), edge, West);
        }

        private EdgeId EdgeInPair(TileId firstTile, TileId secondTile)
        {
            if (edgeRules.ContainsKey(firstTile) && edgeRules[firstTile].ContainsKey(secondTile))
                return edgeRules[firstTile][secondTile];
            else
                return EdgeId.None;
        }

        private void EdgeTips(int i, int j, TileId firstTile, TileId secondTile, EdgeId edge)
        {
            TileId center = Id(i, j);
            if (center == invalid)
                return;

            if (center == firstTile &&
                EdgeInPair(firstTile, Id(i, j, South)) == edge &&
                EdgeInPair(firstTile, Id(i, j, East)) == edge &&
                EdgeInPair(firstTile, Id(i, j, SE)) == edge)
                AddEdge(GetTile(i, j, SE), GetTile(i, j), edge, EdgeTile.Direction.SE_Tip);

            if (center == firstTile &&
                EdgeInPair(firstTile, Id(i, j, North)) == edge &&
                EdgeInPair(firstTile, Id(i, j, West)) == edge &&
                EdgeInPair(firstTile, Id(i, j, NW)) == edge)
                AddEdge(GetTile(i, j, NW), GetTile(i, j), edge, EdgeTile.Direction.NW_Tip);

            if (center == firstTile &&
                EdgeInPair(firstTile, Id(i, j, East)) == edge &&
                EdgeInPair(firstTile, Id(i, j, North)) == edge &&
                EdgeInPair(firstTile, Id(i, j, NE)) == edge)
                AddEdge(GetTile(i, j, NE), GetTile(i, j), edge, EdgeTile.Direction.NE_Tip);

            if (center == firstTile &&
                EdgeInPair(firstTile, Id(i, j, South)) == edge &&
                EdgeInPair(firstTile, Id(i, j, West)) == edge &&
                EdgeInPair(firstTile, Id(i, j, SW)) == edge)
                AddEdge(GetTile(i, j, SW), GetTile(i, j), edge, EdgeTile.Direction.SW_Tip);
        }

        public static EdgeTile.Direction ToGlobalDir(EdgeTile.Direction dir)
        {
            if (dir == EdgeTile.Direction.East ||
               dir == EdgeTile.Direction.East_D ||
               dir == EdgeTile.Direction.East_E) 
                return EdgeTile.Direction.East;

            else if (dir == EdgeTile.Direction.NE_Tip ||
                     dir == EdgeTile.Direction.NE_Sides )
                return EdgeTile.Direction.NE_Tip;

            else if (dir == EdgeTile.Direction.North ||
                     dir == EdgeTile.Direction.North_08 ||
                     dir == EdgeTile.Direction.North_0A)
                return EdgeTile.Direction.North;

            else if (dir == EdgeTile.Direction.NW_Tip ||
                     dir == EdgeTile.Direction.NW_Sides)
                return EdgeTile.Direction.NW_Tip;

            else if (dir == EdgeTile.Direction.SE_Sides ||
                     dir == EdgeTile.Direction.SE_Tip )
                return EdgeTile.Direction.SE_Sides;

            else if (dir == EdgeTile.Direction.South ||
                     dir == EdgeTile.Direction.South_07 ||
                     dir == EdgeTile.Direction.South_09)
                return EdgeTile.Direction.South;

            else if (dir == EdgeTile.Direction.SW_Sides ||
                     dir == EdgeTile.Direction.SW_Tip)
                return EdgeTile.Direction.SW_Sides;

            else if (dir == EdgeTile.Direction.West ||
                     dir == EdgeTile.Direction.West_02 ||
                     dir == EdgeTile.Direction.West_03) 
                return EdgeTile.Direction.West;
            return 0;
        }


        private void AddEdge(
            MapView.TimeTile timeTileSrc, MapView.TimeTile timeTileDst, EdgeId edge, 
            EdgeTile.Direction edgeDir)
        {
            edgeDir = EdgeMakeTab.GetRandomVariation(GetSideDir(edgeDir));

            var tileSrc = timeTileSrc.Tile;
            var tileDst = timeTileDst.Tile;
            var edgeTile = new EdgeTile(tileDst.graphicId, tileDst.Variation, edgeDir, edge);

            var tileEdges = tileSrc.EdgeTiles.Cast<EdgeTile>();
            bool canAdd = true;
            foreach(var tileEdge in tileEdges)
            {
                if (tileEdge.Edge == edge)
                {
                    bool comp1 = Compatible(tileEdge.Dir, edgeDir);
                    bool comp2 = Compatible(edgeDir, tileEdge.Dir);
                    if (!comp1 && !comp2)
                    {
                        canAdd = false;
                        break;
                    }
                }
            }

            if (canAdd)
                tileSrc.EdgeTiles.Add(edgeTile);

            timeTileSrc.EdgeTiles = tileSrc.EdgeTiles;
        }

        private bool Compatible(EdgeTile.Direction dir, EdgeTile.Direction edgeDir)
        {
            var dir1 = ToGlobalDir(dir);
            var dir2 = ToGlobalDir(edgeDir);
            return
                dir1 == EdgeTile.Direction.East && (
                   dir2 == EdgeTile.Direction.North
                || dir2 == EdgeTile.Direction.South
                || dir2 == EdgeTile.Direction.West
                || dir2 == EdgeTile.Direction.NW_Sides
                || dir2 == EdgeTile.Direction.SW_Sides
                || dir2 == EdgeTile.Direction.NW_Tip
                || dir2 == EdgeTile.Direction.SW_Tip)
                ||
                dir1 == EdgeTile.Direction.West && (
                   dir2 == EdgeTile.Direction.North
                || dir2 == EdgeTile.Direction.South
                || dir2 == EdgeTile.Direction.East
                || dir2 == EdgeTile.Direction.NE_Sides
                || dir2 == EdgeTile.Direction.SE_Sides
                || dir2 == EdgeTile.Direction.NE_Tip
                || dir2 == EdgeTile.Direction.SE_Tip)
                ||
                dir1 == EdgeTile.Direction.North && (
                   dir2 == EdgeTile.Direction.West
                || dir2 == EdgeTile.Direction.South
                || dir2 == EdgeTile.Direction.East
                || dir2 == EdgeTile.Direction.SE_Sides
                || dir2 == EdgeTile.Direction.SW_Sides
                || dir2 == EdgeTile.Direction.SE_Tip
                || dir2 == EdgeTile.Direction.SW_Tip)
                ||
                dir1 == EdgeTile.Direction.South && (
                   dir2 == EdgeTile.Direction.West
                || dir2 == EdgeTile.Direction.North
                || dir2 == EdgeTile.Direction.East
                || dir2 == EdgeTile.Direction.NE_Sides
                || dir2 == EdgeTile.Direction.NW_Sides
                || dir2 == EdgeTile.Direction.NE_Tip
                || dir2 == EdgeTile.Direction.NW_Tip)
                ||            
                (dir1 == EdgeTile.Direction.NW_Sides 
                || dir1 == EdgeTile.Direction.NW_Tip) && (
                   dir2 == EdgeTile.Direction.South
                || dir2 == EdgeTile.Direction.East
                || dir2 == EdgeTile.Direction.SE_Sides
                || dir2 == EdgeTile.Direction.SE_Tip)                
                ||            
                (dir1 == EdgeTile.Direction.SE_Sides 
                || dir1 == EdgeTile.Direction.SE_Tip) && (
                   dir2 == EdgeTile.Direction.North
                || dir2 == EdgeTile.Direction.West
                || dir2 == EdgeTile.Direction.NW_Sides
                || dir2 == EdgeTile.Direction.NW_Tip)               
                ||            
                (dir1 == EdgeTile.Direction.SW_Sides 
                || dir1 == EdgeTile.Direction.SW_Tip) && (
                   dir2 == EdgeTile.Direction.North
                || dir2 == EdgeTile.Direction.East
                || dir2 == EdgeTile.Direction.NE_Sides
                || dir2 == EdgeTile.Direction.NE_Tip)                
                ||            
                (dir1 == EdgeTile.Direction.NE_Sides 
                || dir1 == EdgeTile.Direction.NE_Tip) && (
                   dir2 == EdgeTile.Direction.South
                || dir2 == EdgeTile.Direction.West
                || dir2 == EdgeTile.Direction.SW_Sides
                || dir2 == EdgeTile.Direction.SW_Tip)
                ;

        }

        private static EdgeTile.Direction GetSideDir(EdgeTile.Direction dir)
        {
            switch (dir)
            {
                case EdgeTile.Direction.West:
                case EdgeTile.Direction.West_02:
                case EdgeTile.Direction.West_03:
                    return EdgeTile.Direction.North;
                case EdgeTile.Direction.South:
                case EdgeTile.Direction.South_07:
                case EdgeTile.Direction.South_09:
                    return EdgeTile.Direction.East;
                case EdgeTile.Direction.North:
                case EdgeTile.Direction.North_08:
                case EdgeTile.Direction.North_0A:
                    return EdgeTile.Direction.West;
                case EdgeTile.Direction.East:
                case EdgeTile.Direction.East_D:
                case EdgeTile.Direction.East_E:
                    return EdgeTile.Direction.South;

                case EdgeTile.Direction.SW_Tip:
                    return EdgeTile.Direction.NE_Tip;
                case EdgeTile.Direction.SW_Sides:
                    return EdgeTile.Direction.NE_Sides;
                case EdgeTile.Direction.NE_Tip:
                    return EdgeTile.Direction.SW_Tip;
                case EdgeTile.Direction.NE_Sides:
                    return EdgeTile.Direction.SW_Sides;

                case EdgeTile.Direction.NW_Tip:
                    return EdgeTile.Direction.NW_Tip;
                case EdgeTile.Direction.NW_Sides:
                    return EdgeTile.Direction.NW_Sides;
                case EdgeTile.Direction.SE_Tip:
                    return EdgeTile.Direction.SE_Tip;
                case EdgeTile.Direction.SE_Sides:
                    return EdgeTile.Direction.SE_Sides;
                default:
                    return EdgeTile.Direction.East;
            }
        }

        private TileId Id(int i, int j)
        {
            if (i < 0 || j < 0 || i >= typeMap.GetLength(0) || j >= typeMap.GetLength(1))
                return invalid;
            return typeMap[i, j];
        }

        private TileId Id(int baseI, int baseJ, EdgeTile.Direction dir)
        {
            var ij = OffsetMap[dir];
            int i = baseI + ij[0];
            int j = baseJ + ij[1];
            return Id(i, j);
        }

        MapView.TimeTile GetTile(
            int baseI, int baseJ, EdgeTile.Direction dir = (EdgeTile.Direction)255)
        {
            int i = baseI;
            int j = baseJ;
            if (dir != (EdgeTile.Direction)255)
            {
                var ij = OffsetMap[dir];
                i += ij[0];
                j += ij[1];
            }
            if (i < 0 || j < 0 || i >= tile2dMap.GetLength(0) || j >= tile2dMap.GetLength(1))
                return null;
            return tile2dMap[i, j];
        }

        public static TileId[,] MakeTypeMap(MapView.TimeTile[,] tile2dMap)
        {
            TileId[,] types = new TileId[tile2dMap.GetLength(0), tile2dMap.GetLength(1)];

            for (int i = 0; i < tile2dMap.GetLength(0); ++i)
            {
                for (int j = 0; j < tile2dMap.GetLength(1); ++j)
                {
                    var timeTile = tile2dMap[i, j];
                    if (timeTile == null)
                        types[i, j] = invalid;
                    else
                        types[i, j] = timeTile.Tile.graphicId;
                }
            }

            return types;
        }

        private MapView.TimeContent MakeResult()
        {
            MapView.TimeContent res = new MapView.TimeContent();
            for (int i = 0; i < tile2dMap.GetLength(0); ++i)
            {
                for (int j = 0; j < tile2dMap.GetLength(1); ++j)
                {
                    var tile = tile2dMap[i, j];
                    if (tile == null)
                        continue;
                    
                    res.StoredTiles.Add(tile);                  
                }
            }

            return res;
        }
    }
}
