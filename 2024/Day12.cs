using AoC2024.MatrixNavigation;
using AoC2024.ForDay12;
using Fence = (AoC2024.MatrixNavigation.Coord, AoC2024.MatrixNavigation.Coord);
using static AoC2024.MatrixNavigation.NavigableMap;

namespace AoC2024
{
    namespace ForDay12
    {
        public static class ExtensionMethods
        {
            public static List<Cell> GetUpTo4NeighbouringCells(this Region region, Coord coord)
            {
                List<Cell> result = [];
                if (region.TryGetValue(coord.Move(Cardinal.North), out Cell? cell)) result.Add(cell!);
                if (region.TryGetValue(coord.Move(Cardinal.East), out cell)) result.Add(cell!);
                if (region.TryGetValue(coord.Move(Cardinal.West), out cell)) result.Add(cell!);
                if (region.TryGetValue(coord.Move(Cardinal.South), out cell)) result.Add(cell!);
                return result;
            }

            public static HashSet<Coord> Get4Neighbours(this Coord coord)
                => [coord.Move(Cardinal.North), coord.Move(Cardinal.East), coord.Move(Cardinal.South), coord.Move(Cardinal.West)];

            public static Region? IsNeighbourOfAndMatchesRegion(this List<Region> regions, Coord coord, char ch)
                => regions.FirstOrDefault(region => coord.Get4Neighbours().Select(nC => region.TryGetValue(nC, out Cell? cell) ? cell!.Character : ' ').Distinct().Contains(ch));

            public static bool IsNeighbourOf(this Region a, Region b)
            {
                if (a.AssumedCharacter != b.AssumedCharacter)
                    return false;
                List<Coord> neighboursA = [.. a.Cells.Select(cell => cell.Key).SelectMany(Get4Neighbours).Distinct()];
                List<Coord> neighboursB = [.. b.Cells.Select(cell => cell.Key).SelectMany(Get4Neighbours).Distinct()];
                return neighboursA.Any(neighboursB.Contains);
            }

            public static List<Region> MergeNeighbouringRegionsOfSameType(this List<Region> regions)
            {
                List<Region> result = [];
                while (regions.Count > 0)
                {
                    Region nextRegion = regions[0];
                    regions.RemoveAt(0);
                    List<int> neighbouringRegionIndexes = [.. Enumerable.Range(0, regions.Count).Where(index => regions[index].IsNeighbourOf(nextRegion))];
                    List<Region> conglomerate = [nextRegion];
                    foreach (int index in neighbouringRegionIndexes.OrderByDescending(i => i))
                    {
                        conglomerate.Add(regions[index]);
                        regions.RemoveAt(index);
                    }
                    result.Add(new([.. conglomerate]));
                }
                return result;
            }
        }

        public class Region
        {
            public Dictionary<Coord, Cell> Cells { get; init; }

            public Region()
            {
                Cells = [];
            }

            public Region(params Region[] regions)
                : this()
            {
                foreach (Region region in regions)
                    foreach ((Coord coord, Cell cell) in region.Cells)
                        Cells.Add(coord, cell);
            }

            public char AssumedCharacter => Cells.First().Value.Character;

            public int Count => Cells.Count;
            public bool ContainsKey(Coord coord) => Cells.ContainsKey(coord);
            public bool TryGetValue(Coord coord, out Cell? cell) => Cells.TryGetValue(coord, out cell);
        }
    }

    public class Day12 : DayProgram
    {
        private static int CalculateRegionArea(Region region)
            => region.Count;

        private static HashSet<Fence> CalculateRegionFences(Region region)
        {
            HashSet<Fence> fences = [];
            foreach ((Coord coord, Cell cell) in region.Cells)
            {
                HashSet<Coord> neighbours = coord.Get4Neighbours();
                foreach (Coord neighbour in neighbours)
                {
                    if (!region.ContainsKey(neighbour))
                        fences.Add(new(coord, neighbour));
                }
            }
            return fences;
        }

        private static (int Area, HashSet<Fence> Fences, int PriceOfFences) CalculatePriceOfFences(Region region)
        {
            int area = CalculateRegionArea(region);
            HashSet<Fence> fences = [.. CalculateRegionFences(region).Distinct()];
            return (area, fences, area * fences.Count);
        }

        private static List<Region> DetermineRegions(NavigableMap map)
        {
            List<Region> result = [];
            foreach ((Coord coord, Cell cell) in map.Cells)
            {
                Region? region = result.IsNeighbourOfAndMatchesRegion(coord, cell.Character);
                if (region is null)
                {
                    region = new();
                    result.Add(region);
                }
                region.Cells[coord] = cell;
            }
            return result.MergeNeighbouringRegionsOfSameType();
        }

        private static void CalculateFencePricingForAllRegions(NavigableMap map)
        {
            map.DrawMapStr().LogDNL();
            List<Region> regions = DetermineRegions(map);
            Dictionary<Region, (int Area, HashSet<Fence> Fences, int PriceOfFences)> fencePriceByRegion = regions.ToDictionary(region => region, CalculatePriceOfFences);

            $" > There are {fencePriceByRegion.Count} regions (with {fencePriceByRegion.Keys.Distinct().Count()} types of vegetables):".Log();
            string.Join("\n", fencePriceByRegion.Select(pair => $" - region of {pair.Key.Cells.First().Value.Character}: area {pair.Value.Area,3} x {pair.Value.Fences.Count,3} fences = price of {pair.Value.PriceOfFences,6:N0}")).Log();
            $" > In total, the fences cost {fencePriceByRegion.Sum(pair => pair.Value.PriceOfFences)}.".Log();

            // Dictionary<char, List<(int Area, HashSet<(Coord, Coord)> Fences, int PriceOfFences)>> dict = fencePriceByRegion.GroupBy(pair => pair.Key.AssumedCharacter).ToDictionary(g => g.Key, g => g.Select(pair => pair.Value).ToList());
            // $"\n > In summary, there are {dict.Count} types of vegetables:\n{string.Join("\n", dict.Keys.OrderBy(x => x).Select(key => $" - {key} ({dict[key].Count} regions): {string.Join(", ", dict[key].Select(r => $"{r.Area}x{r.Fences.Count}={r.PriceOfFences}"))}; for a total price of {dict[key].Sum(x => x.PriceOfFences)}"))}".Log();
        }

        public override void Run()
            => CalculateFencePricingForAllRegions(Parse(ReadFromFile_Strings(@"tests\12-3"), (_, _) => true, (_, ch) => ch));
    }
}