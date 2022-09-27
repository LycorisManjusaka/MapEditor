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
using static MapEditor.BitmapExport.BitmapCommon;

namespace MapEditor.BitmapExport
{
    public class BitmapImporter
    {
        private readonly Bitmap bitmap;
        private const int pointCount = 252;
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


        private TileId Id(int i, int j)
        {
            if (i < 0 || j < 0 || i >= typeMap.GetLength(0) || j >= typeMap.GetLength(1))
                return TileId.Invalid;
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

        private MapView.TimeTile GetTile(
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

        private static TileId[,] MakeTypeMap(MapView.TimeTile[,] tile2dMap)
        {
            TileId[,] types = new TileId[tile2dMap.GetLength(0), tile2dMap.GetLength(1)];

            for (int i = 0; i < tile2dMap.GetLength(0); ++i)
            {
                for (int j = 0; j < tile2dMap.GetLength(1); ++j)
                {
                    var timeTile = tile2dMap[i, j];
                    if (timeTile == null)
                        types[i, j] = TileId.Invalid;
                    else
                        types[i, j] = timeTile.Tile.graphicId;
                }
            }

            return types;
        }
    }
}
