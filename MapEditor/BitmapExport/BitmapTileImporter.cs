using NoxShared;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static NoxShared.Map.Tile;
using static NoxShared.ThingDb;
using static MapEditor.BitmapExport.BitmapCommon;
using System;
using static NoxShared.Map.Wall;

namespace MapEditor.BitmapExport
{
    public class BitmapTileImporter : BitmapImporter
    {
        private static readonly Dictionary<uint, TileId> tileColorMap
            = new Dictionary<uint, TileId>();       
        private readonly Dictionary<TileId, Dictionary<TileId, EdgeId>> edgeRules = null;
        private readonly Dictionary<TileId, WallId> wallRules = null;
        private HashSet<TileId> usedTileIds;
        private TileId[,] tileIdMap;
        private MapView.TimeTile[,] tile2dMap;
        private readonly MapView.TimeContent res = new MapView.TimeContent();
        private Random rnd = new Random((int)DateTime.Now.Ticks);

        public BitmapTileImporter(
            Bitmap bitmap, Dictionary<TileId, Dictionary<TileId, EdgeId>> edgeRules, 
            Dictionary<TileId, WallId> wallRules = null) 
            : base(bitmap)
        {
            for (int i = 0; i < Map.tilecolors.Count(); ++i)
            {
                tileColorMap[Map.tilecolors[i]] = (TileId)i;
            }
            
            this.edgeRules = edgeRules;
            this.wallRules = wallRules;
        }

        public MapView.TimeContent ToTiles()
        {
            tile2dMap = MakeTile2dMap(bitmap);
            tileIdMap = MakeTileMap(tile2dMap);
            AddEdges();

            MakeResultTiles();

            if (wallRules != null)
                AddWalls();

            if (res.StoredTiles.Count > 0)
                res.Location = res.StoredTiles[0].Tile.Location;
            res.Mode = MapInt.EditMode.WALL_PLACE;
            return res;
        }

        private void AddWalls()
        {
            for (int i = 0; i < tile2dMap.GetLength(0); ++i)
            {
                for (int j = 0; j < tile2dMap.GetLength(1); ++j)
                {
                    AddWallsToTile(i, j);
                }
            }
        }


        private void AddWallsToTile(int i, int j)
        {
            var tileId = Id(i, j);

            if (tileId == TileId.Invalid || !wallRules.ContainsKey(tileId))
                return;

            if (SideId(i, j, BaseDir.West) == TileId.Invalid
                && SideId(i, j, BaseDir.North) != TileId.Invalid
                && SideId(i, j, BaseDir.NW) == TileId.Invalid
                )
                AddWall(i, j, tileId, WallFacing.WEST, BaseDir.West);
            
            if (SideId(i, j, BaseDir.West) == TileId.Invalid
                && SideId(i, j, BaseDir.North) != TileId.Invalid
                && SideId(i, j, BaseDir.NW) != TileId.Invalid
                )
                AddWall(i, j, tileId, WallFacing.NE_CORNER, BaseDir.West);
            
            if (SideId(i, j, BaseDir.West) == TileId.Invalid
                && SideId(i, j, BaseDir.North) == TileId.Invalid
                )
                AddWall(i, j, tileId, WallFacing.NW_CORNER, BaseDir.West);
            
            if (SideId(i, j, BaseDir.North) == TileId.Invalid
                && SideId(i, j, BaseDir.East) != TileId.Invalid
                && SideId(i, j, BaseDir.NE) == TileId.Invalid
                )
                AddWall(i, j, tileId, WallFacing.NORTH);
            
            if (SideId(i, j, BaseDir.North) == TileId.Invalid
                && SideId(i, j, BaseDir.East) != TileId.Invalid
                && SideId(i, j, BaseDir.NE) != TileId.Invalid
                )
                AddWall(i, j, tileId, WallFacing.SE_CORNER);
            
            if (SideId(i, j, BaseDir.North) == TileId.Invalid
                && SideId(i, j, BaseDir.East) == TileId.Invalid
                )
                AddWall(i, j, tileId, WallFacing.NE_CORNER);
            
            if (SideId(i, j, BaseDir.East) == TileId.Invalid
                && SideId(i, j, BaseDir.North) != TileId.Invalid
                && SideId(i, j, BaseDir.NE) == TileId.Invalid
                )
                AddWall(i, j, tileId, WallFacing.WEST);
            
            if (SideId(i, j, BaseDir.East) == TileId.Invalid
                && SideId(i, j, BaseDir.North) != TileId.Invalid
                && SideId(i, j, BaseDir.NE) != TileId.Invalid
                )
                AddWall(i, j, tileId, WallFacing.NW_CORNER);
            
            if (SideId(i, j, BaseDir.South) == TileId.Invalid
                && SideId(i, j, BaseDir.West) == TileId.Invalid
                )
                AddWall(i, j, tileId, WallFacing.SW_CORNER, BaseDir.SW);
            
            if (SideId(i, j, BaseDir.South) == TileId.Invalid
                && SideId(i, j, BaseDir.East) != TileId.Invalid
                && SideId(i, j, BaseDir.SE) == TileId.Invalid
                )
                AddWall(i, j, tileId, WallFacing.NORTH, BaseDir.South);
            
            if (SideId(i, j, BaseDir.North) == TileId.Invalid
                && SideId(i, j, BaseDir.West) != TileId.Invalid
                && SideId(i, j, BaseDir.NW) != TileId.Invalid
                )
                AddWall(i, j, tileId, WallFacing.SW_CORNER, BaseDir.West);
            
            if (SideId(i, j, BaseDir.South) == TileId.Invalid
                && SideId(i, j, BaseDir.East) == TileId.Invalid
                )
                AddWall(i, j, tileId, WallFacing.SE_CORNER, BaseDir.South);
        }

        private TileId SideId(int i, int j, BaseDir dir)
        {
            return Id(i, j, GetSideDir(GetOppositeDir(dir)));
        }

        private void AddWall(
            int baseI, int baseJ, TileId tileId, WallFacing facing, BaseDir dir = BaseDir.None)
        {
            int i = baseI;
            int j = baseJ;
            if (dir != BaseDir.None)
            {
                var ij = OffsetMap[dir];
                i -= ij[0];
                j += ij[1];
            }

            var wallId = wallRules[tileId];

            Wall wall = Walls[(int)wallId];
            Wall.WallRenderInfo[] ria = wall.RenderNormal[0];
            int hoho = ria.Length;
            hoho = (hoho / 2) - 1;

            byte variation = (byte)rnd.Next(0, hoho);

            if (facing == WallFacing.CROSS
                || facing == WallFacing.SOUTH_T
                || facing == WallFacing.EAST_T
                || facing == WallFacing.NORTH_T
                || facing == WallFacing.WEST_T
                || facing == WallFacing.SW_CORNER
                || facing == WallFacing.NW_CORNER
                || facing == WallFacing.NE_CORNER
                || facing == WallFacing.SE_CORNER
                )
                variation = 0;

            var timeWall = new MapView.TimeWall
            {
                Wall = new Map.Wall(
                    new Point(i, tile2dMap.GetLength(1) - j - 1), facing, wallId),

                Facing = facing,
            };
            timeWall.Wall.Variation = variation;

            if (!res.StoredWalls.Any(x => x.Wall.Location == timeWall.Wall.Location))
                res.StoredWalls.Add(timeWall);
        }

        private MapView.TimeTile[,] MakeTile2dMap(Bitmap bitmap)
        {
            usedTileIds = new HashSet<TileId>();

            GetColorsAndLocations(
                bitmap, out List<Color> colors, out List<Point> locations, out Point pointMin,
                out Point pointMax);

            var tile2dMap
                = new MapView.TimeTile[pointMax.X - pointMin.X + 1, pointMax.Y - pointMin.Y + 1];

            for (int i = 0; i < locations.Count; ++i)
            {
                var color = colors[i];
                uint colorInt = (uint)color.ToArgb();
                if (!tileColorMap.ContainsKey(colorInt))
                    continue;

                TileId graphicsId = tileColorMap[colorInt];
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
            return tile2dMap;
        }


        private void AddEdges()
        {
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
            if (i < 0 || j < 0 || i >= tileIdMap.GetLength(0) || j >= tileIdMap.GetLength(1))
                return TileId.Invalid;
            return tileIdMap[i, j];
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

        private MapView.TimeTile GetTile(int baseI, int baseJ, BaseDir dir = BaseDir.None)
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

        

        private void MakeResultTiles()
        {
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
        }
    }
}
