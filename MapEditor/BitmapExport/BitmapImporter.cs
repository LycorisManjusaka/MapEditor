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
        private HashSet<TileId> usedTileIds;
        private TileId[,] typeMap;
        private MapView.TimeTile[,] tile2dMap;
        public const BaseDir South = BaseDir.South;
        public const BaseDir North = BaseDir.North;
        public const BaseDir East = BaseDir.East;
        public const BaseDir West = BaseDir.West;
        public const BaseDir SE = BaseDir.SE;
        public const BaseDir NW = BaseDir.NW;
        public const BaseDir NE = BaseDir.NE;
        public const BaseDir SW = BaseDir.SW;
        public static readonly Dictionary<BaseDir, int[]> OffsetMap
            = new Dictionary<BaseDir, int[]>
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

        public BitmapImporter(
            Bitmap bitmap, Dictionary<TileId, Dictionary<TileId, EdgeId>> edgeRules)
        {
            this.bitmap = bitmap;
            for (int i = 0; i < Map.tilecolors.Count(); ++i)
            {
                colorMap[Map.tilecolors[i]] = (TileId)i;
            }
            this.edgeRules = edgeRules;
            //edgeRules = Analizer.RulesFromFile("E:/Dev/Nox/EdgeRules/Common.rul");
        }

        public MapView.TimeContent ToTiles()
        {
            MakeTile2dMap();
            AddEdges();
            return MakeResult();
        }

        public static EdgeBaseDir ToEdgeBaseDir(EdgeTile.Direction dir, EdgeId edgeId)
        {
            var edgeGroup = GetEdgeGroup(edgeId);

            if (edgeGroup == EdgeGroup.Usual)
            {
                if (dir == EdgeTile.Direction.East ||
                   dir == EdgeTile.Direction.East_D ||
                   dir == EdgeTile.Direction.East_E)
                    return EdgeBaseDir.East;

                else if (dir == EdgeTile.Direction.NE_Tip)
                    return EdgeBaseDir.NE_Tip;
                else if (dir == EdgeTile.Direction.NE_Sides)
                    return EdgeBaseDir.NE_Sides;

                else if (dir == EdgeTile.Direction.North ||
                         dir == EdgeTile.Direction.North_08 ||
                         dir == EdgeTile.Direction.North_0A)
                    return EdgeBaseDir.North;

                else if (dir == EdgeTile.Direction.NW_Tip)
                    return EdgeBaseDir.NW_Tip;
                else if (dir == EdgeTile.Direction.NW_Sides)
                    return EdgeBaseDir.NW_Sides;

                else if (dir == EdgeTile.Direction.SE_Tip)
                    return EdgeBaseDir.SE_Tip;
                else if (dir == EdgeTile.Direction.SE_Sides)
                    return EdgeBaseDir.SE_Sides;

                else if (dir == EdgeTile.Direction.South ||
                         dir == EdgeTile.Direction.South_07 ||
                         dir == EdgeTile.Direction.South_09)
                    return EdgeBaseDir.South;

                else if (dir == EdgeTile.Direction.SW_Tip)
                    return EdgeBaseDir.SW_Tip;
                else if (dir == EdgeTile.Direction.SW_Sides)
                    return EdgeBaseDir.SW_Sides;

                else if (dir == EdgeTile.Direction.West ||
                         dir == EdgeTile.Direction.West_02 ||
                         dir == EdgeTile.Direction.West_03)
                    return EdgeBaseDir.West;
            }
            else
            {
                if (dir == EdgeTile.Direction.NW_Tip)
                    return EdgeBaseDir.North;
                else if (dir == EdgeTile.Direction.West_03)
                    return EdgeBaseDir.South;
                else if (dir == EdgeTile.Direction.West)
                    return EdgeBaseDir.West;
                else if (dir == EdgeTile.Direction.North)
                    return EdgeBaseDir.East;

                else if (dir == EdgeTile.Direction.South_07)
                    return EdgeBaseDir.NE_Tip;
                else if (dir == EdgeTile.Direction.North_0A)
                    return EdgeBaseDir.NE_Sides;

                else if (dir == EdgeTile.Direction.West_02)
                    return EdgeBaseDir.NW_Tip;
                else if (dir == EdgeTile.Direction.South_09)
                    return EdgeBaseDir.NW_Sides;

                else if (dir == EdgeTile.Direction.South)
                    return EdgeBaseDir.SE_Tip;
                else if (dir == EdgeTile.Direction.SE_Tip)
                    return EdgeBaseDir.SE_Sides;

                else if (dir == EdgeTile.Direction.SW_Tip)
                    return EdgeBaseDir.SW_Tip;
                else if (dir == EdgeTile.Direction.North_08)
                    return EdgeBaseDir.SW_Sides;
            }
            return EdgeBaseDir.None;
        }

        private static EdgeGroup GetEdgeGroup(EdgeId edgeId)
        {
            if (edgeId == EdgeId.DirtDarkedge
                || edgeId == EdgeId.GrassSparseEdge
                || edgeId == EdgeId.MudEdge)
                return EdgeGroup.Odd;
            else
                return EdgeGroup.Usual;
        }

        public static EdgeTile.Direction ToEdgeTileDirection(EdgeBaseDir dir, EdgeId edgeId)
        {
            var edgeGroup = GetEdgeGroup(edgeId);
            if (edgeGroup == EdgeGroup.Usual)
            {
                EdgeTile.Direction res = (EdgeTile.Direction)255;
                if (dir == EdgeBaseDir.East)
                    res = EdgeTile.Direction.East;

                else if (dir == EdgeBaseDir.NE_Tip)
                    res = EdgeTile.Direction.NE_Tip;
                else if (dir == EdgeBaseDir.NE_Sides)
                    res = EdgeTile.Direction.NE_Sides;

                else if (dir == EdgeBaseDir.North)
                    res = EdgeTile.Direction.North;

                else if (dir == EdgeBaseDir.NW_Tip)
                    res = EdgeTile.Direction.NW_Tip;
                else if (dir == EdgeBaseDir.NW_Sides)
                    res = EdgeTile.Direction.NW_Sides;

                else if (dir == EdgeBaseDir.SE_Tip)
                    res = EdgeTile.Direction.SE_Tip;
                else if (dir == EdgeBaseDir.SE_Sides)
                    res = EdgeTile.Direction.SE_Sides;

                else if (dir == EdgeBaseDir.South)
                    res = EdgeTile.Direction.South;

                else if (dir == EdgeBaseDir.SW_Tip)
                    res = EdgeTile.Direction.SW_Tip;
                else if (dir == EdgeBaseDir.SW_Sides)
                    res = EdgeTile.Direction.SW_Sides;

                else if (dir == EdgeBaseDir.West)
                    res = EdgeTile.Direction.West;
                var edgeDir = EdgeMakeTab.GetRandomVariation(res);
                return edgeDir;
            }
            else
            {
                if (dir == EdgeBaseDir.North)
                    return EdgeTile.Direction.NW_Tip;
                else if (dir == EdgeBaseDir.South)
                    return EdgeTile.Direction.West_03;
                else if (dir == EdgeBaseDir.West)
                    return EdgeTile.Direction.West;
                else if (dir == EdgeBaseDir.East)
                    return EdgeTile.Direction.North;

                else if (dir == EdgeBaseDir.NE_Tip)
                    return EdgeTile.Direction.South_07;
                else if (dir == EdgeBaseDir.NE_Sides)
                    return EdgeTile.Direction.North_0A;

                else if (dir == EdgeBaseDir.NW_Tip)
                    return EdgeTile.Direction.West_02;
                else if (dir == EdgeBaseDir.NW_Sides)
                    return EdgeTile.Direction.South_09;

                else if (dir == EdgeBaseDir.SE_Tip)
                    return EdgeTile.Direction.South;
                else if (dir == EdgeBaseDir.SE_Sides)
                    return EdgeTile.Direction.SE_Tip;

                else if (dir == EdgeBaseDir.SW_Tip)
                    return EdgeTile.Direction.SW_Tip;
                else if (dir == EdgeBaseDir.SW_Sides)
                    return EdgeTile.Direction.North_08;
            }
            return (EdgeTile.Direction)255;
        }

        public static BaseDir ToBaseDir(EdgeTile.Direction dir, EdgeId edgeId)
        {
            return ToBaseDir(ToEdgeBaseDir(dir, edgeId));
        }

        public static BaseDir ToBaseDir(EdgeBaseDir dir)
        {
            if (dir == EdgeBaseDir.East)
                return BaseDir.East;

            else if (dir == EdgeBaseDir.NE_Tip || dir == EdgeBaseDir.NE_Sides)
                return BaseDir.NE;

            else if (dir == EdgeBaseDir.North)
                return BaseDir.North;

            else if (dir == EdgeBaseDir.NW_Tip || dir == EdgeBaseDir.NW_Sides)
                return BaseDir.NW;

            else if (dir == EdgeBaseDir.SE_Tip || dir == EdgeBaseDir.SE_Sides)
                return BaseDir.SE;

            else if (dir == EdgeBaseDir.South)
                return BaseDir.South;

            else if (dir == EdgeBaseDir.SW_Tip || dir == EdgeBaseDir.SW_Sides)
                return BaseDir.SW;

            else if (dir == EdgeBaseDir.West)
                return BaseDir.West;
            return 0;
        }


        private void MakeTile2dMap()
        {
            usedTileIds = new HashSet<TileId>();

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

            for (int i = 0; i < locations.Count; ++i)
            {
                var color = colors[i];
                if (color == black)
                    continue;

                uint colorInt = (uint)color.ToArgb();
                if (!colorMap.ContainsKey(colorInt))
                    continue;

                TileId graphicsId = colorMap[colorInt];
                usedTileIds.Add(graphicsId);
                var location = locations[i];
                var tilePoint = new Point(location.X - pointMin.X, location.Y - pointMin.Y);

                ushort vari = GetTileVariation(graphicsId, tilePoint);

                MapView.TimeTile timeTile = new MapView.TimeTile
                {
                    Tile = new Map.Tile(tilePoint, graphicsId, vari)
                };

                timeTile.EdgeTiles = timeTile.Tile.EdgeTiles;
                tile2dMap[tilePoint.X, tile2dMap.GetLength(1) - tilePoint.Y - 1] = timeTile;
            }
        }

        private static ushort GetTileVariation(TileId graphicsId, Point tilePoint)
        {
            int x = tilePoint.X % pointCount;
            int y = tilePoint.Y % pointCount;
            int cols = FloorTiles[(int)graphicsId].numCols;
            int rows = FloorTiles[(int)graphicsId].numRows;

            ushort vari = (ushort)(((x + y) / 2 % cols) + ((y % rows) + 1 + cols
                - (x + y) / 2 % cols) % rows * cols);
            return vari;
        }

        private void AddEdges()
        {
            typeMap = MakeTypeMap(tile2dMap);
            foreach (var firstTileRule in edgeRules)
            {               
                TileId firstTile = firstTileRule.Key;
                if (!usedTileIds.Contains(firstTile))
                    continue;
                var secondTileRules = firstTileRule.Value;
                foreach (var secondTileRule in secondTileRules)
                {
                    TileId secondTile = secondTileRule.Key;
                    if (!usedTileIds.Contains(secondTile))
                        continue;
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
            //  .*
            //  **
            if (Id(i, j) == firstTile &&
                Id(i, j, North) == firstTile &&
                Id(i, j, West) == firstTile &&
                Id(i, j, NW) == secondTile)
                AddEdge(GetTile(i, j), GetTile(i, j, NW), edge, EdgeBaseDir.NW_Sides);

            //  **
            //  *.
            if (Id(i, j) == firstTile &&
                Id(i, j, South) == firstTile &&
                Id(i, j, East) == firstTile &&
                Id(i, j, SE) == secondTile)
                AddEdge(GetTile(i, j), GetTile(i, j, SE), edge, EdgeBaseDir.SE_Sides);

            //  *.
            //  **        
            if (Id(i, j) == firstTile &&
                Id(i, j, North) == firstTile &&
                Id(i, j, East) == firstTile &&
                Id(i, j, NE) == secondTile)
                AddEdge(GetTile(i, j), GetTile(i, j, NE), edge, EdgeBaseDir.NE_Sides);

            //  **
            //  .*
            if (Id(i, j) == firstTile &&
                Id(i, j, South) == firstTile &&
                Id(i, j, West) == firstTile &&
                Id(i, j, SW) == secondTile)
                AddEdge(GetTile(i, j), GetTile(i, j, SW), edge, EdgeBaseDir.SW_Sides);
        }

        private void EdgeSides(int i, int j, TileId firstTile, TileId secondTile, EdgeId edge)
        {
            //  *
            //  .
            if (Id(i, j) == firstTile && Id(i, j, South) == secondTile)         
                AddEdge(GetTile(i, j), GetTile(i, j, South), edge, EdgeBaseDir.South);

            //  .
            //  *
            if (Id(i, j) == firstTile && Id(i, j, North) == secondTile)          
                AddEdge(GetTile(i, j), GetTile(i, j, North), edge, EdgeBaseDir.North);

            //  *.
            if (Id(i, j) == firstTile && Id(i, j, East) == secondTile)         
                AddEdge(GetTile(i, j), GetTile(i, j, East), edge, EdgeBaseDir.East);

            //  .*
            if (Id(i, j) == firstTile && Id(i, j, West) == secondTile)       
                AddEdge(GetTile(i, j), GetTile(i, j, West), edge, EdgeBaseDir.West);
        }

        private void EdgeTips(int i, int j, TileId firstTile, TileId secondTile, EdgeId edge)
        {
            //  **
            //  *.
            if (Id(i, j) == firstTile &&
                (EdgeInPair(firstTile, Id(i, j, South)) == edge) &&
                (EdgeInPair(firstTile, Id(i, j, East)) == edge) &&
                EdgeInPair(firstTile, Id(i, j, SE)) == edge)
                AddEdge(GetTile(i, j), GetTile(i, j, SE), edge, EdgeBaseDir.SE_Tip);

            //  .*
            //  **
            if (Id(i, j) == firstTile &&
                (EdgeInPair(firstTile, Id(i, j, North)) == edge) &&
                (EdgeInPair(firstTile, Id(i, j, West)) == edge )&&
                EdgeInPair(firstTile, Id(i, j, NW)) == edge)
                AddEdge(GetTile(i, j), GetTile(i, j, NW), edge, EdgeBaseDir.NW_Tip);
            
            //  *.
            //  **
            if (Id(i, j) == firstTile &&
                (EdgeInPair(firstTile, Id(i, j, East)) == edge) &&
                (EdgeInPair(firstTile, Id(i, j, North)) == edge) &&
                EdgeInPair(firstTile, Id(i, j, NE)) == edge)
                AddEdge(GetTile(i, j), GetTile(i, j, NE), edge, EdgeBaseDir.NE_Tip);

            //  **
            //  .*
            if (Id(i, j) == firstTile &&
                (EdgeInPair(firstTile, Id(i, j, South)) == edge) &&
                (EdgeInPair(firstTile, Id(i, j, West)) == edge )&&
                EdgeInPair(firstTile, Id(i, j, SW)) == edge)
                AddEdge(GetTile(i, j), GetTile(i, j, SW), edge, EdgeBaseDir.SW_Tip);
        }

        private EdgeId EdgeInPair(TileId firstTile, TileId secondTile)
        {
            if (firstTile == secondTile)
                return EdgeId.None;
            if (edgeRules.ContainsKey(firstTile) && edgeRules[firstTile].ContainsKey(secondTile))
                return edgeRules[firstTile][secondTile];
            else
                return EdgeId.None;
        }


        private void AddEdge(
            MapView.TimeTile timeTileSrc, MapView.TimeTile timeTileDst, EdgeId edge,
            EdgeBaseDir edgeBaseDir)
        {
            var sideDir = GetSideDir(edgeBaseDir);
            var edgeDir = ToEdgeTileDirection(sideDir, edge);

            var tileDst = timeTileDst.Tile;
            var tileSrc = timeTileSrc.Tile;

            ushort vari = GetTileVariation(tileSrc.graphicId, tileDst.Location);

            var edgeTile = new EdgeTile(tileSrc.graphicId, vari, edgeDir, edge);

            var tileEdges = tileDst.EdgeTiles.Cast<EdgeTile>();
            bool canAdd = true;
            foreach(var tileEdge in tileEdges)
            {
                if (tileEdge.Edge == edge)
                {
                    bool comp1 = Compatible(
                        ToEdgeBaseDir(tileEdge.Dir, tileEdge.Edge), 
                        ToEdgeBaseDir(edgeDir, edge));

                    bool comp2 = Compatible(
                        ToEdgeBaseDir(edgeDir, edge), 
                        ToEdgeBaseDir(tileEdge.Dir, tileEdge.Edge));

                    if (!comp1 && !comp2)
                    {
                        canAdd = false;
                        break;
                    }
                }
            }

            if (canAdd)
                tileDst.EdgeTiles.Add(edgeTile);

            timeTileDst.EdgeTiles = tileDst.EdgeTiles;
        }

        private bool Compatible(EdgeBaseDir dir1, EdgeBaseDir dir2)
        {
            return
                dir1 == EdgeBaseDir.East && (
                   dir2 == EdgeBaseDir.North
                || dir2 == EdgeBaseDir.South
                || dir2 == EdgeBaseDir.West
                || dir2 == EdgeBaseDir.NW_Sides
                || dir2 == EdgeBaseDir.SW_Sides
                || dir2 == EdgeBaseDir.NW_Tip
                || dir2 == EdgeBaseDir.SW_Tip)
                ||
                dir1 == EdgeBaseDir.West && (
                   dir2 == EdgeBaseDir.North
                || dir2 == EdgeBaseDir.South
                || dir2 == EdgeBaseDir.East
                || dir2 == EdgeBaseDir.NE_Sides
                || dir2 == EdgeBaseDir.SE_Sides
                || dir2 == EdgeBaseDir.NE_Tip
                || dir2 == EdgeBaseDir.SE_Tip)
                ||
                dir1 == EdgeBaseDir.North && (
                   dir2 == EdgeBaseDir.West
                || dir2 == EdgeBaseDir.South
                || dir2 == EdgeBaseDir.East
                || dir2 == EdgeBaseDir.SE_Sides
                || dir2 == EdgeBaseDir.SW_Sides
                || dir2 == EdgeBaseDir.SE_Tip
                || dir2 == EdgeBaseDir.SW_Tip)
                ||
                dir1 == EdgeBaseDir.South && (
                   dir2 == EdgeBaseDir.West
                || dir2 == EdgeBaseDir.North
                || dir2 == EdgeBaseDir.East
                || dir2 == EdgeBaseDir.NE_Sides
                || dir2 == EdgeBaseDir.NW_Sides
                || dir2 == EdgeBaseDir.NE_Tip
                || dir2 == EdgeBaseDir.NW_Tip)
                ||            
                (dir1 == EdgeBaseDir.NW_Sides 
                || dir1 == EdgeBaseDir.NW_Tip) && (
                   dir2 == EdgeBaseDir.South
                || dir2 == EdgeBaseDir.East
                || dir2 == EdgeBaseDir.SE_Sides
                || dir2 == EdgeBaseDir.SE_Tip)                
                ||            
                (dir1 == EdgeBaseDir.SE_Sides 
                || dir1 == EdgeBaseDir.SE_Tip) && (
                   dir2 == EdgeBaseDir.North
                || dir2 == EdgeBaseDir.West
                || dir2 == EdgeBaseDir.NW_Sides
                || dir2 == EdgeBaseDir.NW_Tip)               
                ||            
                (dir1 == EdgeBaseDir.SW_Sides 
                || dir1 == EdgeBaseDir.SW_Tip) && (
                   dir2 == EdgeBaseDir.North
                || dir2 == EdgeBaseDir.East
                || dir2 == EdgeBaseDir.NE_Sides
                || dir2 == EdgeBaseDir.NE_Tip)                
                ||            
                (dir1 == EdgeBaseDir.NE_Sides 
                || dir1 == EdgeBaseDir.NE_Tip) && (
                   dir2 == EdgeBaseDir.South
                || dir2 == EdgeBaseDir.West
                || dir2 == EdgeBaseDir.SW_Sides
                || dir2 == EdgeBaseDir.SW_Tip)
                ;

        }

        private static EdgeBaseDir GetSideDir(EdgeBaseDir dir)
        {
            switch (dir)
            {
                case EdgeBaseDir.West:
                    return EdgeBaseDir.North;
                case EdgeBaseDir.South:
                    return EdgeBaseDir.East;
                case EdgeBaseDir.North:
                    return EdgeBaseDir.West;
                case EdgeBaseDir.East:
                    return EdgeBaseDir.South;

                case EdgeBaseDir.SW_Tip:
                    return EdgeBaseDir.NE_Tip;
                case EdgeBaseDir.SW_Sides:
                    return EdgeBaseDir.NE_Sides;
                case EdgeBaseDir.NE_Tip:
                    return EdgeBaseDir.SW_Tip;
                case EdgeBaseDir.NE_Sides:
                    return EdgeBaseDir.SW_Sides;

                case EdgeBaseDir.NW_Tip:
                    return EdgeBaseDir.NW_Tip;
                case EdgeBaseDir.NW_Sides:
                    return EdgeBaseDir.NW_Sides;
                case EdgeBaseDir.SE_Tip:
                    return EdgeBaseDir.SE_Tip;
                case EdgeBaseDir.SE_Sides:
                    return EdgeBaseDir.SE_Sides;
                default:
                    return EdgeBaseDir.East;
            }
        }

        private TileId Id(int i, int j)
        {
            if (i < 0 || j < 0 || i >= typeMap.GetLength(0) || j >= typeMap.GetLength(1))
                return invalid;
            return typeMap[i, j];
        }

        private TileId Id(int baseI, int baseJ, BaseDir dir = BaseDir.None)
        {
            if (dir == BaseDir.None)      
                return Id(baseI, baseI);
            
            var ij = OffsetMap[dir];
            int i = baseI + ij[0];
            int j = baseJ + ij[1];
            return Id(i, j);
        }

        MapView.TimeTile GetTile(
            int baseI, int baseJ, BaseDir dir = BaseDir.None)
        {
            int i = baseI;
            int j = baseJ;
            if (dir != BaseDir.None)
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



        public enum EdgeGroup
        {
            Usual,
            Odd
        }


        public enum BaseDir
        {
            South,
            North,
            East,
            West,
            SE,
            NW,
            NE,
            SW,
            None,
        };

        public enum EdgeBaseDir
        {
            South,
            North,
            East,
            West,
            SE_Tip,
            NW_Tip,
            NE_Tip,
            SW_Tip,
            SE_Sides,
            NW_Sides,
            NE_Sides,
            SW_Sides,
            None,
        };
    }
}
