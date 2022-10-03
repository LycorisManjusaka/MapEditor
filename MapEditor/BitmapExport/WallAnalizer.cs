using System.Collections.Generic;
using System.Drawing;
using System.IO;
using static NoxShared.ThingDb;
using static MapEditor.BitmapExport.BitmapCommon;
using System;

namespace MapEditor.BitmapExport
{
    public class WallAnalizer
    {
        private readonly MapView.TimeContent copiedArea;
        private List<string> resultLines;
        private KeyValuePair<TileId, WallId>[,] map;
        private static readonly KeyValuePair<TileId, WallId> invalid
            = new KeyValuePair<TileId, WallId>(TileId.Invalid, WallId.Invalid);

        public WallAnalizer(MapView.TimeContent copiedArea)
        {
            this.copiedArea = copiedArea;
        }

        public void WallRuleStatisticsToFile(string filePath)
        {
            MakeResultLines();
            File.WriteAllLines(filePath, resultLines.ToArray());
        }

        private void MakeResultLines()
        {
            map = MakeMap(copiedArea);
            resultLines = new List<string>();

            for (int i = 0; i < map.GetLength(0); ++i)
            {
                for (int j = 0; j < map.GetLength(1); ++j)
                {
                    AnalizeItem(i, j);
                }
            }
        }

        public static Dictionary<TileId, WallId> StatisticsToRules(string[] filePaths)
        {
            return StatisticsToRules(ReadStatistics(filePaths));
        }

        internal static List<string> RulesToLines(Dictionary<TileId, WallId> wallRules)
        {
            List<string> res = new List<string>();
            foreach (var tileItem in wallRules)
            {
                var tileId = tileItem.Key;
                var wallId = tileItem.Value;
                res.Add(string.Format("{0}\t{1}\t", tileId.ToString(), wallId.ToString()));           
            }

            return res;
        }

        public static Dictionary<TileId, WallId> StatisticsToRules(string filePath)
        {
            return StatisticsToRules(ReadStatistics(new string[] { filePath }));
        }

        public static void RulesToFile(Dictionary<TileId, WallId> rules, string filePath)
        {
            List<string> lines = new List<string>();
            foreach (var rule in rules)
            {
                lines.Add(string.Format("{0}\t{1}", rule.Key.ToString(), rule.Value.ToString()));
            }
            File.WriteAllLines(filePath, lines.ToArray());
        }

        public Dictionary<TileId, WallId> GetRules()
        {
            MakeResultLines();
            var stats = new Dictionary<TileId, Dictionary<WallId, int>>();
            CollectStatsFromLines(resultLines.ToArray(), ref stats);
            return MakeRules(stats);
        }

        private static List<KeyValuePair<TileId, WallId>> ReadStatistics(string[] filePaths)
        {
            var res = new List<KeyValuePair<TileId, WallId>>();
            foreach(var filePath in filePaths)
            {
                ReadStatistics(filePath, ref res);
            }
            return res;
        }

        private static void ReadStatistics(
            string filePath, ref List<KeyValuePair<TileId, WallId>> stats)
        {
            var lines = File.ReadAllLines(filePath);
            ReadStatisticsFromLines(lines, ref stats);
        }

        private static void ReadStatisticsFromLines(
            string[] lines, ref List<KeyValuePair<TileId, WallId>> stats)
        {
            foreach (var line in lines)
            {
                var words = line.Split(new char[] { '\t' });
                if (words.Length < 2)
                    continue;
                var tileId = (TileId)Enum.Parse(typeof(TileId), words[0]);
                var wallId = (WallId)Enum.Parse(typeof(WallId), words[1]);
                stats.Add(new KeyValuePair<TileId, WallId>(tileId, wallId));
            }
        }

        private static Dictionary<TileId, WallId> StatisticsToRules(
            List<KeyValuePair<TileId, WallId>> stats)
        {
            var counted = StatisticLinesToStats(stats);
            return MakeRules(counted);
        }

        private static Dictionary<TileId, WallId> MakeRules(
            Dictionary<TileId, Dictionary<WallId, int>> counted)
        {
            var res = new Dictionary<TileId, WallId>();
            foreach (var tileItems in counted)
            {
                var tileId = tileItems.Key;
                var walls = tileItems.Value;
                int maxCount = 0;
                WallId maxCountWallId = WallId.Invalid;
                foreach (var wallItem in walls)
                {
                    var wallId = wallItem.Key;
                    int count = wallItem.Value;
                    if (count > maxCount)
                    {
                        maxCount = count;
                        maxCountWallId = wallId;
                    }
                }
                res[tileId] = maxCountWallId;
            }
            return res;
        }

        private static Dictionary<TileId, Dictionary<WallId, int>> StatisticLinesToStats(
            List<KeyValuePair<TileId, WallId>> stats)
        {
            var counted = new Dictionary<TileId, Dictionary<WallId, int>>();
            foreach (var item in stats)
            {
                if (!counted.ContainsKey(item.Key))
                {
                    counted[item.Key] = new Dictionary<WallId, int>();
                }
                if (!counted[item.Key].ContainsKey(item.Value))
                {
                    counted[item.Key][item.Value] = 0;
                }
                ++counted[item.Key][item.Value];
            }

            return counted;
        }

        private static KeyValuePair<TileId, WallId>[,] MakeMap(MapView.TimeContent copiedArea)
        {
            var pointMin = new Point(int.MaxValue, int.MaxValue);
            var pointMax = new Point(int.MinValue, int.MinValue);

            var tiles = copiedArea.StoredTiles;

            foreach (var timeTile in tiles)
            {
                var tile = timeTile.Tile;
                var point = tile.Location;

                if (point.X < pointMin.X)
                    pointMin.X = point.X;
                if (point.Y < pointMin.Y)
                    pointMin.Y = point.Y;
                if (point.X > pointMax.X)
                    pointMax.X = point.X;
                if (point.Y > pointMax.Y)
                    pointMax.Y = point.Y;
            }

            var walls = copiedArea.StoredWalls;
            foreach (var timeWall in walls)
            {
                var wall = timeWall.Wall;
                var point = wall.Location;

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

            KeyValuePair<TileId, WallId>[,] res 
                    = new KeyValuePair<TileId, WallId>[size.Width, size.Height];

            for (int i = 0; i < res.GetLength(0); ++i)
            {
                for (int j = 0; j < res.GetLength(1); ++j)
                {
                    res[i, j] = invalid;
                }
            }
            
            foreach (var timeTile in tiles)
            {
                var tile = timeTile.Tile;
                var point = tile.Location;
                res[point.X - pointMin.X, point.Y - pointMin.Y] 
                    = new KeyValuePair<TileId, WallId>(tile.graphicId, WallId.Invalid);
            }

            foreach (var timeWall in walls)
            {
                var wall = timeWall.Wall;
                var point = wall.Location;

                res[point.X - pointMin.X, point.Y - pointMin.Y] 
                    = new KeyValuePair<TileId, WallId>(
                        res[point.X - pointMin.X, point.Y - pointMin.Y].Key, wall.matId);
            }

            return res;
        }


        private void AnalizeItem(int i, int j)
        {
            var item = GetItem(i, j);
            if (item.Key == TileId.Invalid)
                return;

            foreach(var baseDir in new BaseDir[]{ South, North, East, West}) 
            {
                var offsetted = GetItem(i, j, baseDir);
                if (offsetted.Value != WallId.Invalid)
                    resultLines.Add(string.Format(
                        "{0}\t{1}", item.Key.ToString(), offsetted.Value.ToString()));
            }
        }


        KeyValuePair<TileId, WallId> GetItem(int baseI, int baseJ, BaseDir dir = BaseDir.None)
        {
            int i = baseI;
            int j = baseJ;
            if (dir != BaseDir.None)
            {
                var ij = OffsetMap[dir];
                i += ij[0];
                j += ij[1];
            }
            if (i < 0 || j < 0 || i >= map.GetLength(0) || j >= map.GetLength(1))
                return invalid;
            return map[i, j];
        }


        private void CollectStatsFromLines(string[] lines, 
            ref Dictionary<TileId, Dictionary<WallId, int>> stats)
        {
            var statLines = new List<KeyValuePair<TileId, WallId>>();
            ReadStatisticsFromLines(lines, ref statLines);
            stats = StatisticLinesToStats(statLines);
        }
    }
}
