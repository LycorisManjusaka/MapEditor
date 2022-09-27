using NoxShared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using static MapEditor.BitmapExport.BitmapImporter;
using static NoxShared.Map.Tile;
using static NoxShared.ThingDb;
using static MapEditor.BitmapExport.BitmapCommon;

namespace MapEditor.BitmapExport
{
    public class Analizer
    {
        private MapView.TimeContent copiedArea;
        private List<string> resultLines;
        private MapView.TimeTile[,] tile2dMap;

        public Analizer(MapView.TimeContent copiedArea)
        {
            this.copiedArea = copiedArea;
        }

        public Dictionary<TileId, Dictionary<TileId, EdgeId>> GetRules()
        {
            MakeResultLines();
            var stats = new Dictionary<TileId, Dictionary<TileId, Dictionary<EdgeId, int>>>();
            CollectStatsFromLines(resultLines.ToArray(), ref stats);
            var singleStats = MakeSingleStats(stats);
            return MakeRules(singleStats);
        }

        public void EdgeRuleStatisticsToFile(string filePath)
        {
            MakeResultLines();
            File.WriteAllLines(filePath, resultLines.ToArray());
        }

        public static Dictionary<TileId, Dictionary<TileId, EdgeId>> StatFilesToRules(
            string[] filePaths)
        {
            var stats = new Dictionary<TileId, Dictionary<TileId, Dictionary<EdgeId, int>>>();
            foreach (var filePath in filePaths)
            {
                CollectStatsFromFile(filePath, ref stats);
            }

            var singleStats = MakeSingleStats(stats);
            return MakeRules(singleStats);
        }
        
        public static Dictionary<TileId, Dictionary<TileId, EdgeId>> 
            StatFileToRules(string filePath)
        {
            return StatFilesToRules(new string[] { filePath });
        }

        public static void RulesToFile(
            Dictionary<TileId, Dictionary<TileId, EdgeId>> rules, string filePath)
        {
            List<string> res = RulesToLines(rules);
            File.WriteAllLines(filePath, res.ToArray());
        }

        public static List<string> RulesToLines(
            Dictionary<TileId, Dictionary<TileId, EdgeId>> rules)
        {
            List<string> res = new List<string>();
            foreach (var tileItem in rules)
            {
                var tileId = tileItem.Key;
                var neighItems = tileItem.Value;
                foreach (var neighItem in neighItems)
                {
                    var neighId = neighItem.Key;
                    var edgeId = neighItem.Value;
                    res.Add(string.Format(
                        "{0}\t{1}\t{2}", tileId.ToString(), neighId.ToString(), edgeId.ToString()));
                }
            }

            return res;
        }

        private static Dictionary<TileId, Dictionary<TileId, EdgeId>>
            MakeRules(Dictionary<TileId, Dictionary<TileId, KeyValuePair<EdgeId, int>>> singleStats)
        {
            var res = new Dictionary<TileId, Dictionary<TileId, EdgeId>>();
            foreach (var neighTileItem in singleStats)
            {
                var neighTileId = neighTileItem.Key;

                if (!res.ContainsKey(neighTileId))
                    res[neighTileId] = new Dictionary<TileId, EdgeId>();

                var tileItems = neighTileItem.Value;
                foreach (var tileItem in tileItems)
                {
                    var tileId = tileItem.Key;
                    var edgeItems = tileItem.Value;

                    if (!singleStats.ContainsKey(tileId) ||
                        !singleStats[tileId].ContainsKey(neighTileId))
                        res[neighTileId][tileId] = edgeItems.Key;
                    else
                    {
                        var reverseEdgeItems = singleStats[tileId][neighTileId];
                        int reverseCount = reverseEdgeItems.Value;

                        int count = edgeItems.Value;
                        if (count > reverseCount)
                            res[neighTileId][tileId] = edgeItems.Key;
                        else
                        {
                            if (!res.ContainsKey(tileId))
                                res[tileId] = new Dictionary<TileId, EdgeId>();
                            res[tileId][neighTileId] = reverseEdgeItems.Key;
                        }
                    }
                }
            }

            return res;
        }

        public static Dictionary<TileId, Dictionary<TileId, EdgeId>> RulesFromLines(string[] lines)
        {
            var rules = new Dictionary<TileId, Dictionary<TileId, EdgeId>>();
            
            foreach (var line in lines)
            {
                var words = line.Split(new char[] { '\t', ' ' });
                var neighTileId = (TileId)Enum.Parse(typeof(TileId), words[0]);
                var tileId = (TileId)Enum.Parse(typeof(TileId), words[1]);
                var edgeId = (EdgeId)Enum.Parse(typeof(EdgeId), words[2]);

                if (!rules.ContainsKey(neighTileId))
                    rules[neighTileId] = new Dictionary<TileId, EdgeId>();

                if (!rules[neighTileId].ContainsKey(tileId))
                    rules[neighTileId][tileId] = edgeId;
            }
            return rules;
        }

        public static Dictionary<TileId, Dictionary<TileId, EdgeId>> RulesFromFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            return RulesFromLines(lines);
        }


        private static Dictionary<TileId, Dictionary<TileId, KeyValuePair<EdgeId, int>>>
            MakeSingleStats(Dictionary<TileId, Dictionary<TileId, Dictionary<EdgeId, int>>> stats)
        {
            var singleStats
                = new Dictionary<TileId, Dictionary<TileId, KeyValuePair<EdgeId, int>>>();

            foreach (var neighTileItem in stats)
            {
                var neighTileId = neighTileItem.Key;

                if (!singleStats.ContainsKey(neighTileId))
                    singleStats[neighTileId] = new Dictionary<TileId, KeyValuePair<EdgeId, int>>();

                var tileItems = neighTileItem.Value;
                foreach (var tileItem in tileItems)
                {
                    var tileId = tileItem.Key;
                    var edgeItems = tileItem.Value;

                    if (edgeItems.Count == 1)
                    {
                        singleStats[neighTileId][tileId] = edgeItems.First();
                    }
                    else if (edgeItems.Count > 1)
                    {
                        int maxCount = 0;
                        EdgeId useEdgeId = EdgeId.None;
                        GetUseEdgeId(edgeItems, ref maxCount, ref useEdgeId);
                        singleStats[neighTileId][tileId]
                            = new KeyValuePair<EdgeId, int>(useEdgeId, maxCount);
                    }
                }
            }

            return singleStats;
        }

        private static void GetUseEdgeId(Dictionary<EdgeId, int> edgeItems, ref int maxCount, 
            ref EdgeId useEdgeId)
        {
            foreach (var edgeItem in edgeItems)
            {
                var edgeId = edgeItem.Key;
                var count = edgeItem.Value;
                if (count > maxCount)
                {
                    maxCount = count;
                    useEdgeId = edgeId;
                }
            }
        }

        private static void CollectStatsFromFile(
            string filePath, 
            ref Dictionary<TileId, Dictionary<TileId, Dictionary<EdgeId, int>>> stats)
        {
            var lines = File.ReadAllLines(filePath);
            CollectStatsFromLines(lines, ref stats);

        }

        private static void CollectStatsFromLines(
            string[] lines,
            ref Dictionary<TileId, Dictionary<TileId, Dictionary<EdgeId, int>>> stats)
        {
            foreach (var line in lines)
            {
                var words = line.Split(new char[] { '\t' });
                var neighTileId = (TileId)Enum.Parse(typeof(TileId), words[0]);
                var tileId = (TileId)Enum.Parse(typeof(TileId), words[1]);
                var edgeId = (EdgeId)Enum.Parse(typeof(EdgeId), words[2]);

                if (!stats.ContainsKey(neighTileId))
                    stats[neighTileId] = new Dictionary<TileId, Dictionary<EdgeId, int>>();

                if (!stats[neighTileId].ContainsKey(tileId))
                    stats[neighTileId][tileId] = new Dictionary<EdgeId, int>();

                if (!stats[neighTileId][tileId].ContainsKey(edgeId))
                    stats[neighTileId][tileId][edgeId] = 0;
                stats[neighTileId][tileId][edgeId]++;
            }
        }

        private void MakeResultLines()
        {
            tile2dMap = MakeTile2dMap();
            resultLines = new List<string>();

            for (int i = 0; i < tile2dMap.GetLength(0); ++i)
            {
                for (int j = 0; j < tile2dMap.GetLength(1); ++j)
                {
                    AnalizeTile(i, j);
                }
            }
        }

        private void AnalizeTile(int i, int j)
        {
            var timeTile = GetTile(i, j);
            if (timeTile == null)
                return;

            var tile = timeTile.Tile;
            var tileId = tile.graphicId;

            foreach (var dir in new BaseDir[] {
                BaseDir.South,
                BaseDir.North,
                BaseDir.East,
                BaseDir.West,
                BaseDir.SE,
                BaseDir.NW,
                BaseDir.NE,
                BaseDir.SW
            })
            {
                var neighTimeTile = GetTile(i, j, dir);
                if (neighTimeTile == null)
                    continue;
                var neighTile = neighTimeTile.Tile;
                var neighTileId = neighTile.graphicId;

                if (tileId != neighTileId)
                {
                    EdgeId edgeId = EdgeId.None;

                    foreach (var item in tile.EdgeTiles)
                    {
                        if (item is EdgeTile edgeTile)
                        {
                            var currentEdgeId = edgeTile.Edge;
                            if (((EdgeIncludesTile(currentEdgeId)
                                && edgeTile.Graphic == neighTileId)
                                || !EdgeIncludesTile(currentEdgeId))
                                && GetOppositeDir(ToBaseDir(edgeTile.Dir, currentEdgeId)) == dir)
                            {
                                edgeId = currentEdgeId;
                                break;
                            }
                        }
                    }

                    if (edgeId != EdgeId.None)
                    {
                        resultLines.Add(string.Format(
                            "{0}\t{1}\t{2}", neighTileId.ToString(), tileId.ToString(),
                            edgeId.ToString()));
                    }
                    else
                    {
                        foreach (var item in neighTile.EdgeTiles)
                        {
                            if (item is EdgeTile neighEdgeTile)
                            {
                                var currentNeighEdgeId = neighEdgeTile.Edge;
                                if (((EdgeIncludesTile(currentNeighEdgeId)
                                    && (neighEdgeTile.Graphic == tileId))
                                    || !EdgeIncludesTile(currentNeighEdgeId))
                                    && ToBaseDir(neighEdgeTile.Dir, currentNeighEdgeId) == dir)
                                {
                                    edgeId = currentNeighEdgeId;
                                    break;
                                }
                            }
                        }

                        if (edgeId != EdgeId.None)
                        {
                            resultLines.Add(string.Format(
                                "{0}\t{1}\t{2}", tileId.ToString(),
                                neighTileId.ToString(),
                                edgeId.ToString()));

                        }
                    }
                }
            }
        }



        private MapView.TimeTile[,] MakeTile2dMap()
        {
            var tiles = copiedArea.StoredTiles;

            var pointMin = new Point(int.MaxValue, int.MaxValue);
            var pointMax = new Point(int.MinValue, int.MinValue);

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

            var size = new Size(pointMax.X - pointMin.X + 1, pointMax.Y - pointMin.Y + 1);

            MapView.TimeTile[,] res = new MapView.TimeTile[size.Width, size.Height];
            foreach (var timeTile in tiles)
            {
                var tile = timeTile.Tile;
                var point = tile.Location;
                res[point.X - pointMin.X, point.Y - pointMin.Y] = timeTile;
            }
            return res;
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
    }
}
