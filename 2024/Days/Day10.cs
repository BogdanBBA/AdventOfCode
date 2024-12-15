using AoC2024.ForDay10;
using AoC2024.MatrixNavigation;
using static AoC2024.MatrixNavigation.NavigableMap;

namespace AoC2024
{
    namespace ForDay10
    {
        public static class ExtensionMethods
        {
            public static int GetTagAsHeight(this Cell cell)
                => (int)cell.Tag!;

            public static int GetTrailheadScore(this List<Trail> trails)
                => trails.Select(trail => trail.Path.Last()).Distinct().Count();

            public static int GetTrailheadRating(this List<Trail> trails)
                => trails.Count;
        }

        public class Trail(Cell trailhead)
        {
            public Trail(Trail trail, Cell plusCell) : this(trail.Trailhead) { Path = [.. trail.Path, plusCell]; }

            public Cell Trailhead { get; } = trailhead;
            public List<Cell> Path { get; } = [trailhead];
            public bool IsComplete => Path.Count == 10;

            public override string ToString() => string.Join(", ", Path.Select(c => $"[{c.Coord.X},{c.Coord.Y}]={c.GetTagAsHeight()}"));
        }
    }

    public class Day10 : DayProgram
    {
        private static void GetNextCellAndAddIfContinuesTrail(NavigableMap map, Cell cell, Cardinal cardinal, Trail trail, List<Cell> cells)
        {
            if (map.TryGetByCoord(cell.Coord.Move(cardinal), out Cell? next))
            {
                if (next!.GetTagAsHeight() == cell.GetTagAsHeight() + 1)
                {
                    cells.Add(next!);
                }
            }
        }

        public static List<Cell> GetNextCells(NavigableMap map, Trail trail)
        {
            List<Cell> cells = [];
            Cell current = trail.Path.Last();
            GetNextCellAndAddIfContinuesTrail(map, current, Cardinal.North, trail, cells);
            GetNextCellAndAddIfContinuesTrail(map, current, Cardinal.East, trail, cells);
            GetNextCellAndAddIfContinuesTrail(map, current, Cardinal.South, trail, cells);
            GetNextCellAndAddIfContinuesTrail(map, current, Cardinal.West, trail, cells);
            return cells;
        }

        private static List<Trail> Split(NavigableMap map, Trail trail)
        {
            if (trail.IsComplete)
                return [trail];
            List<Cell> nextCells = GetNextCells(map, trail);
            List<Trail> result = [.. nextCells.SelectMany(cell => Split(map, new(trail, cell))).Where(iTrail => iTrail.IsComplete)];
            return result;
        }

        public override void Run()
        {
            NavigableMap map = Parse(ReadFromFile_Strings(@"10"), (_, _) => true, (coord, ch) => ch - '0');
            map.DrawMapStr().LogDNL();

            List<Cell> trailheads = [.. map.Cells.Values.Where(cell => cell.GetTagAsHeight() == 0)];
            List<Trail> allTrails = trailheads.SelectMany(trailhead => Split(map, new(trailhead))).ToList();
            Dictionary<Coord, List<Trail>> trailsByTrailhead = allTrails.GroupBy(trail => trail.Trailhead.Coord).ToDictionary(g => g.Key, g => g.ToList());

            Dictionary<Coord, int> scores = trailsByTrailhead.Keys.ToDictionary(key => key, key => trailsByTrailhead[key].GetTrailheadScore());
            foreach (Coord trailhead in scores.Keys)
                $" - score of trailhead {trailhead} is {scores[trailhead]}".Log();
            $" > total score of the {scores.Count} trailheads: {scores.Values.Sum()}".LogDNL();

            Dictionary<Coord, int> ratings = trailsByTrailhead.Keys.ToDictionary(key => key, key => trailsByTrailhead[key].GetTrailheadRating());
            foreach (Coord trailhead in ratings.Keys)
                $" - rating of trailhead {trailhead} is {ratings[trailhead]}".Log();
            $" > total rating of the {ratings.Count} trailheads: {ratings.Values.Sum()}".LogDNL();
        }
    }
}