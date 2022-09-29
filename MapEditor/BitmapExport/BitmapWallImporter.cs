using NoxShared;
using System.Collections.Generic;
using System.Drawing;
using static NoxShared.ThingDb;
using static MapEditor.BitmapExport.BitmapCommon;
using System;
using System.Diagnostics;

namespace MapEditor.BitmapExport
{
    public class BitmapWallImporter : BitmapImporter
    { 
        private readonly Dictionary<Color, WallId> wallColorMap = new Dictionary<Color, WallId>();       
        private WallId[,] typeMap;
        private Random rnd = new Random((int)DateTime.Now.Ticks);

        public BitmapWallImporter(Bitmap bitmap)
            : base(bitmap)
        {
            foreach(var item in WallColors)
            {  
                wallColorMap[item.Value] = item.Key;
            }
        }

        public MapView.TimeContent ToWalls()
        {
            MakeWallId2dMap();
            return MakeResultWalls();
        }

        private MapView.TimeContent MakeResultWalls()
        {
            MapView.TimeContent res = new MapView.TimeContent
            {
                StoredWalls = new List<MapView.TimeWall>()
            };
            for (int i = 0; i < typeMap.GetLength(0); ++i)
            {
                for (int j = 0; j < typeMap.GetLength(1); ++j)
                {
                    var id = typeMap[i, j];
                    if (id == WallId.Invalid)
                        continue;
                    Wall wall = Walls[(int)id];
                    Wall.WallRenderInfo[] ria = wall.RenderNormal[0];
                    int hoho = ria.Length;
                    hoho = (hoho / 2) - 1;
                    Map.Wall.WallFacing facing = GetFacing(i, j);
                    byte variation = (byte)rnd.Next(0, hoho);

                    if (facing == Map.Wall.WallFacing.CROSS
                        || facing == Map.Wall.WallFacing.SOUTH_T
                        || facing == Map.Wall.WallFacing.EAST_T
                        || facing == Map.Wall.WallFacing.NORTH_T
                        || facing == Map.Wall.WallFacing.WEST_T
                        || facing == Map.Wall.WallFacing.SW_CORNER
                        || facing == Map.Wall.WallFacing.NW_CORNER
                        || facing == Map.Wall.WallFacing.NE_CORNER
                        || facing == Map.Wall.WallFacing.SE_CORNER
                        ) 
                        variation = 0;

                    var timeWall = new MapView.TimeWall
                    {
                        Wall = new Map.Wall(new Point(i, j), facing, id),
                        Facing = facing,
                    };
                    timeWall.Wall.Variation = variation;
                    res.StoredWalls.Add(timeWall);
                }
            }
            return res;
        }

        private Map.Wall.WallFacing GetFacing(int i, int j)
        {
            int neigborCount = 0;
            if (Id(i, j, BaseDir.West) != WallId.Invalid)
                ++neigborCount;
            if (Id(i, j, BaseDir.North) != WallId.Invalid)
                ++neigborCount;
            if (Id(i, j, BaseDir.East) != WallId.Invalid)
                ++neigborCount;
            if (Id(i, j, BaseDir.South) != WallId.Invalid)
                ++neigborCount;

            if (neigborCount == 4 || neigborCount == 0)
                return Map.Wall.WallFacing.CROSS;

            if (                       Id(i, j, BaseDir.North) == WallId.Invalid
                 &&  Id(i, j, BaseDir.West) != WallId.Invalid && Id(i, j, BaseDir.East) != WallId.Invalid        
                                    && Id(i, j, BaseDir.South) == WallId.Invalid
                 ||
                                        Id(i, j, BaseDir.North) == WallId.Invalid
                 && Id(i, j, BaseDir.West) != WallId.Invalid && Id(i, j, BaseDir.East) == WallId.Invalid
                                    && Id(i, j, BaseDir.South) == WallId.Invalid
                 ||
                                        Id(i, j, BaseDir.North) == WallId.Invalid
                 && Id(i, j, BaseDir.West) == WallId.Invalid && Id(i, j, BaseDir.East) != WallId.Invalid
                                    && Id(i, j, BaseDir.South) == WallId.Invalid
                                    )
                return Map.Wall.WallFacing.NORTH;

            if (                       Id(i, j, BaseDir.North) != WallId.Invalid
                 &&  Id(i, j, BaseDir.West) == WallId.Invalid && Id(i, j, BaseDir.East) == WallId.Invalid        
                                    && Id(i, j, BaseDir.South) == WallId.Invalid
                ||
                                           Id(i, j, BaseDir.North) != WallId.Invalid
                 && Id(i, j, BaseDir.West) == WallId.Invalid && Id(i, j, BaseDir.East) == WallId.Invalid
                                    && Id(i, j, BaseDir.South) != WallId.Invalid
                ||
                                       Id(i, j, BaseDir.North) == WallId.Invalid
                 && Id(i, j, BaseDir.West) == WallId.Invalid && Id(i, j, BaseDir.East) == WallId.Invalid
                                    && Id(i, j, BaseDir.South) != WallId.Invalid)
                return Map.Wall.WallFacing.WEST;

            if (                       Id(i, j, BaseDir.North) == WallId.Invalid
                 &&  Id(i, j, BaseDir.West) == WallId.Invalid && Id(i, j, BaseDir.East) != WallId.Invalid        
                                    && Id(i, j, BaseDir.South) != WallId.Invalid)
                return Map.Wall.WallFacing.SE_CORNER;

            if (                       Id(i, j, BaseDir.North) == WallId.Invalid
                 &&  Id(i, j, BaseDir.West) != WallId.Invalid && Id(i, j, BaseDir.East) == WallId.Invalid        
                                    && Id(i, j, BaseDir.South) != WallId.Invalid)
                return Map.Wall.WallFacing.SW_CORNER;
            
            if (                       Id(i, j, BaseDir.North) != WallId.Invalid
                 &&  Id(i, j, BaseDir.West) == WallId.Invalid && Id(i, j, BaseDir.East) != WallId.Invalid        
                                    && Id(i, j, BaseDir.South) == WallId.Invalid)
                return Map.Wall.WallFacing.NE_CORNER;

            if (                       Id(i, j, BaseDir.North) != WallId.Invalid
                 &&  Id(i, j, BaseDir.West) != WallId.Invalid && Id(i, j, BaseDir.East) == WallId.Invalid        
                                    && Id(i, j, BaseDir.South) == WallId.Invalid)
                return Map.Wall.WallFacing.NW_CORNER;

            if (                       Id(i, j, BaseDir.North) != WallId.Invalid
                 &&  Id(i, j, BaseDir.West) == WallId.Invalid && Id(i, j, BaseDir.East) != WallId.Invalid        
                                    && Id(i, j, BaseDir.South) != WallId.Invalid)
                return Map.Wall.WallFacing.WEST_T;

            if (                       Id(i, j, BaseDir.North) != WallId.Invalid
                 &&  Id(i, j, BaseDir.West) != WallId.Invalid && Id(i, j, BaseDir.East) != WallId.Invalid        
                                    && Id(i, j, BaseDir.South) == WallId.Invalid)
                return Map.Wall.WallFacing.NORTH_T;

            if (                       Id(i, j, BaseDir.North) != WallId.Invalid
                 &&  Id(i, j, BaseDir.West) != WallId.Invalid && Id(i, j, BaseDir.East) == WallId.Invalid        
                                    && Id(i, j, BaseDir.South) != WallId.Invalid)
                return Map.Wall.WallFacing.EAST_T;

            if (                       Id(i, j, BaseDir.North) == WallId.Invalid
                 &&  Id(i, j, BaseDir.West) != WallId.Invalid && Id(i, j, BaseDir.East) != WallId.Invalid        
                                    && Id(i, j, BaseDir.South) != WallId.Invalid)
                return Map.Wall.WallFacing.SOUTH_T;
            
            return Map.Wall.WallFacing.CROSS;
        }

        private void MakeWallId2dMap()
        {
            GetColorsAndLocations(
                out List<Color> colors, out List<Point> locations, out Point pointMin,
                out Point pointMax);

            typeMap
                = new WallId[pointMax.X - pointMin.X + 1, pointMax.Y - pointMin.Y + 1];

            for (int i = 0; i < typeMap.GetLength(0); ++i)
            {
                for (int j = 0; j < typeMap.GetLength(1); ++j)
                {
                    typeMap[i, j] = WallId.Invalid;
                }
            }

            for (int i = 0; i < locations.Count; ++i)
            {
                var color = colors[i];
                if (!wallColorMap.ContainsKey(color))
                    continue;

                WallId wallId = wallColorMap[color];
                var location = locations[i];
                typeMap[location.X - pointMin.X, location.Y - pointMin.Y] = wallId;
            }
        }

        private WallId Id(int i, int j)
        {
            if (i < 0 || j < 0 || i >= typeMap.GetLength(0) || j >= typeMap.GetLength(1))
                return WallId.Invalid;
            return typeMap[i, j];
        }

        private WallId Id(int baseI, int baseJ, BaseDir dir = BaseDir.None)
        {
            if (dir == BaseDir.None)
                return Id(baseI, baseI);

            var ij = OffsetMap[dir];
            int i = baseI + ij[0];
            int j = baseJ + ij[1];
            return Id(i, j);
        }
    }
}
