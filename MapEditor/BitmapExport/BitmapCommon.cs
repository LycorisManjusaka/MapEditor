using MapEditor.newgui;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using static MapEditor.BitmapExport.BitmapEdgeImporter;
using static NoxShared.Map.Tile;
using static NoxShared.ThingDb;

namespace MapEditor.BitmapExport
{
    public static class BitmapCommon
    {
        public static int PointCount = 252;

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

        public static bool EdgeIncludesTile(EdgeId edgeId)
        {
            return edgeId == EdgeId.LavaEdge
                || edgeId == EdgeId.GrassEdge
                || edgeId == EdgeId.BlendEdge
                || edgeId == EdgeId.DenseGrassEdge
                || edgeId == EdgeId.YellowDenseGrassEdge
                || edgeId == EdgeId.DirtRidge
                || edgeId == EdgeId.IceRidge
                || edgeId == EdgeId.BrickEdgeBrown
                || edgeId == EdgeId.CobbleStoneTrim
                || edgeId == EdgeId.LavaEdgeBrownDirt
                || edgeId == EdgeId.LavaEdgeBlackDirt
                || edgeId == EdgeId.ShallowWaterAndGrass
                || edgeId == EdgeId.SwampEdge;
        }


        public static BaseDir GetOppositeDir(BaseDir dir)
        {
            switch (dir)
            {
                case BaseDir.West:
                    return BaseDir.East;
                case BaseDir.South:
                    return BaseDir.North;
                case BaseDir.North:
                    return BaseDir.South;
                case BaseDir.East:
                    return BaseDir.West;
                case BaseDir.SW:
                    return BaseDir.NE;
                case BaseDir.NE:
                    return BaseDir.SW;
                case BaseDir.NW:
                    return BaseDir.SE;
                case BaseDir.SE:
                    return BaseDir.NW;
                default:
                    return BaseDir.None;
            }
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

        public static ushort GetTileVariation(TileId graphicsId, Point tilePoint)
        {
            int x = tilePoint.X % PointCount;
            int y = tilePoint.Y % PointCount;
            int cols = FloorTiles[(int)graphicsId].numCols;
            int rows = FloorTiles[(int)graphicsId].numRows;

            ushort vari = (ushort)(((x + y) / 2 % cols) + ((y % rows) + 1 + cols
                - (x + y) / 2 % cols) % rows * cols);
            return vari;
        }


        public static bool Compatible(EdgeBaseDir dir1, EdgeBaseDir dir2)
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


        public static EdgeBaseDir GetSideDir(EdgeBaseDir dir)
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

        public static Dictionary<WallId, Color> WallColors => new Dictionary<WallId, Color>
        {
            { WallId.AncientRuin,                 Color.FromArgb(190, 203, 23 ) },        //Color.FromArgb(73, 75, 46) },
            { WallId.AncientRuinShort,		      Color.FromArgb(168, 194, 19 ) },        //Color.FromArgb(63, 67, 40) },
            { WallId.AspenSparse,		          Color.FromArgb(255, 174, 0  ) },       //Color.FromArgb(138, 108, 44) },
            { WallId.AspenTallWall,		          Color.FromArgb(255, 162, 0  ) },       //Color.FromArgb(135, 109, 64) },
            { WallId.AspenThick,		          Color.FromArgb(244, 160, 0  ) },       //Color.FromArgb(117, 86, 27) },
            { WallId.BlackWall,		              Color.FromArgb(192, 96, 96  ) },       //Color.FromArgb(64, 64, 64) },
            { WallId.BrickBlue,		              Color.FromArgb(195, 114, 86 ) },        //Color.FromArgb(68, 65, 64) },
            { WallId.BrickCollegiate,		      Color.FromArgb(219, 142, 52 ) },        //Color.FromArgb(91, 80, 67) },
            { WallId.BrickFancy,		          Color.FromArgb(210, 149, 77 ) },        //Color.FromArgb(82, 77, 71) },
            { WallId.BrickFancyBright,		      Color.FromArgb(227, 115, 7  ) },       //Color.FromArgb(100, 76, 53) },
            { WallId.BrickGray,		              Color.FromArgb(196, 152, 84 ) },        //Color.FromArgb(69, 67, 64) },
            { WallId.BrickPlain,		          Color.FromArgb(216, 151, 76 ) },        //Color.FromArgb(88, 82, 75) },
            { WallId.BrickRed,		              Color.FromArgb(178, 85, 0   ) },      //Color.FromArgb(50, 37, 25) },
            { WallId.CathedralBlue,		          Color.FromArgb(171, 85, 85  ) },       //Color.FromArgb(43, 43, 43) },
            { WallId.CathedralBlue2,		      Color.FromArgb(45, 96, 206  ) },       //Color.FromArgb(56, 63, 78) },
            { WallId.CathedralBlueFace,		      Color.FromArgb(168, 122, 76 ) },        //Color.FromArgb(41, 40, 39) },
            { WallId.CathedralGreen,		      Color.FromArgb(172, 154, 63 ) },        //Color.FromArgb(45, 44, 39) },
            { WallId.CathedralGreen2,		      Color.FromArgb(79, 208, 105 ) },        //Color.FromArgb(71, 81, 73) },
            { WallId.CathedralRed,		          Color.FromArgb(174, 101, 64 ) },        //Color.FromArgb(46, 42, 40) },
            { WallId.CathedralRed2,		          Color.FromArgb(214, 130, 40 ) },        //Color.FromArgb(86, 73, 59) },
            { WallId.CathedralRedFace,		      Color.FromArgb(168, 118, 68 ) },        //Color.FromArgb(41, 39, 37) },
            { WallId.CaveWall,		              Color.FromArgb(227, 144, 16 ) },        //Color.FromArgb(100, 83, 57) },
            { WallId.CaveWall2,		              Color.FromArgb(195, 114, 3  ) },       //Color.FromArgb(68, 54, 35) },
            { WallId.Cobblestone,		          Color.FromArgb(192, 127, 61 ) },        //Color.FromArgb(65, 59, 53) },
            { WallId.ConiWall1,		              Color.FromArgb(131, 188, 19 ) },        //Color.FromArgb(52, 60, 36) },
            { WallId.CrystalBlue,		          Color.FromArgb(46, 255, 199 ) },        //Color.FromArgb(103, 151, 138) },
            { WallId.CrystalCyan,		          Color.FromArgb(0, 172, 255  ) },       //Color.FromArgb(71, 129, 157) },
            { WallId.DecidiousWall,		          Color.FromArgb(172, 108, 25 ) },        //Color.FromArgb(45, 38, 29) },
            { WallId.DecidiousWallBrown,	      Color.FromArgb(142, 174, 0  ) },       //Color.FromArgb(40, 47, 9) },
            { WallId.DecidiousWallGreen,	      Color.FromArgb(148, 170, 0  ) },       //Color.FromArgb(38, 42, 10) },
            { WallId.DecidiousWallRed,		      Color.FromArgb(174, 63, 0   ) },      //Color.FromArgb(46, 23, 10) },
            { WallId.Dilapidated,		          Color.FromArgb(206, 101, 0  ) },       //Color.FromArgb(78, 54, 31) },
            { WallId.DilapidatedShort,		      Color.FromArgb(194, 97, 0   ) },      //Color.FromArgb(67, 46, 25) },
            { WallId.Dirt,		                  Color.FromArgb(203, 94, 0   ) },      //Color.FromArgb(75, 45, 19) },
            { WallId.DungeonCobble,		          Color.FromArgb(227, 172, 98 ) },        //Color.FromArgb(100, 97, 93) },
            { WallId.DungeonStone,		          Color.FromArgb(203, 136, 53 ) },        //Color.FromArgb(75, 67, 57) },
            { WallId.DunMirCathedral,		      Color.FromArgb(255, 147, 12 ) },        //Color.FromArgb(163, 130, 89) },
            { WallId.FieldStoneShort,		      Color.FromArgb(214, 163, 62 ) },        //Color.FromArgb(86, 80, 68) },
            { WallId.GalavaTowerWall,		      Color.FromArgb(250, 152, 35 ) },        //Color.FromArgb(122, 102, 78) },
            { WallId.GalavaTownWall,		      Color.FromArgb(246, 168, 21 ) },        //Color.FromArgb(118, 101, 69) },
            { WallId.Hedge1,		              Color.FromArgb(150, 160, 0  ) },       //Color.FromArgb(30, 32, 0) },
            { WallId.IceWall,		              Color.FromArgb(63, 173, 255 ) },        //Color.FromArgb(145, 173, 194) },
            { WallId.InvisibleBlockingWallSet,    Color.FromArgb(230, 230, 230) },         //Color.FromArgb(230, 230, 230) },
            { WallId.InvisibleStoneWallSet,		  Color.FromArgb(200, 200, 200) },         //Color.FromArgb(200, 200, 200) },
            { WallId.InvisibleWallSet,		      Color.FromArgb(180, 180, 180) },         //Color.FromArgb(180, 180, 180) },
            { WallId.IronFence,		              Color.FromArgb(162, 58, 58  ) },       //Color.FromArgb(35, 30, 30) },
            { WallId.IronFenceDamaged,		      Color.FromArgb(160, 85, 60  ) },       //Color.FromArgb(32, 29, 28) },
            { WallId.IxTempleWall,		          Color.FromArgb(248, 158, 76 ) },        //Color.FromArgb(120, 108, 97) },
            { WallId.Log,		                  Color.FromArgb(226, 123, 6  ) },       //Color.FromArgb(99, 77, 52) },
            { WallId.LOTDBrick,		              Color.FromArgb(192, 123, 54 ) },        //Color.FromArgb(64, 57, 50) },
            { WallId.LOTDMagicOrnate,		      Color.FromArgb(0, 182, 255  ) },       //Color.FromArgb(108, 185, 216) },
            { WallId.LOTDOrnate,		          Color.FromArgb(184, 121, 49 ) },        //Color.FromArgb(56, 50, 43) },
            { WallId.MagicWall,		              Color.FromArgb(51, 73, 255  ) },       //Color.FromArgb(134, 140, 191) },
            { WallId.MagicWallSystemUseOnly,	  Color.FromArgb(0, 180, 255  ) },       //Color.FromArgb(94, 180, 216) },
            { WallId.ManaMineWall,		          Color.FromArgb(230, 97, 0   ) },      //Color.FromArgb(102, 72, 50) },
            { WallId.OgreCage,		              Color.FromArgb(200, 133, 4  ) },       //Color.FromArgb(73, 61, 38) },
            { WallId.OgreWall,		              Color.FromArgb(202, 122, 19 ) },        //Color.FromArgb(74, 61, 44) },
            { WallId.Rock,		                  Color.FromArgb(200, 148, 65 ) },        //Color.FromArgb(73, 68, 60) },
            { WallId.RockDark,		              Color.FromArgb(166, 122, 49 ) },        //Color.FromArgb(39, 36, 31) },
            { WallId.RootDark,		              Color.FromArgb(163, 115, 0  ) },       //Color.FromArgb(36, 26, 2) },
            { WallId.RootLight,		              Color.FromArgb(178, 138, 18 ) },        //Color.FromArgb(50, 45, 30) },
            { WallId.SewerWall,		              Color.FromArgb(202, 177, 22 ) },        //Color.FromArgb(74, 70, 45) },
            { WallId.Shard,		                  Color.FromArgb(204, 140, 52 ) },        //Color.FromArgb(77, 69, 58) },
            { WallId.Shrub,		                  Color.FromArgb(164, 116, 0  ) },       //Color.FromArgb(37, 26, 0) },
            { WallId.StoneBlue,		              Color.FromArgb(211, 154, 83 ) },        //Color.FromArgb(84, 80, 75) },
            { WallId.StoneGray,		              Color.FromArgb(192, 138, 61 ) },        //Color.FromArgb(65, 60, 53) },
            { WallId.StuccoDarkWood,		      Color.FromArgb(243, 140, 31 ) },        //Color.FromArgb(116, 95, 73) },
            { WallId.StuccoLightWood,		      Color.FromArgb(254, 120, 16 ) },        //Color.FromArgb(126, 95, 71) },
            { WallId.StuccoLightWoodDamaged,	  Color.FromArgb(248, 114, 5  ) },       //Color.FromArgb(121, 89, 63) },
            { WallId.Thorn,		                  Color.FromArgb(166, 67, 0   ) },      //Color.FromArgb(38, 22, 11) },
            { WallId.Tree,		                  Color.FromArgb(210, 101, 0  ) },       //Color.FromArgb(82, 55, 30) },
            { WallId.TristoneBlue,		          Color.FromArgb(75, 121, 168 ) },        //Color.FromArgb(38, 39, 40) },
            { WallId.TristoneGreen,		          Color.FromArgb(182, 128, 61 ) },        //Color.FromArgb(54, 50, 45) },
            { WallId.Volcano,                     Color.FromArgb(188, 44, 0   ) },      //Color.FromArgb(61, 35, 27) },
        };











































































    }
}
