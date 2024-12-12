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

            public static Region? IsNeighbourOfAndMatchesRegion(this List<Region> regions, Coord coord, char ch, int goDeeper)
                => regions.FirstOrDefault(region =>
                {
                    HashSet<Coord> neighbourCoords = coord.Get4Neighbours();
                    for (int _ = 0; _ < goDeeper; _++)
                    {
                        HashSet<Coord> newNeighbours = [.. neighbourCoords.SelectMany(nC => nC.Get4Neighbours())];
                        foreach (Coord newNeighbour in newNeighbours)
                            neighbourCoords.Add(newNeighbour);
                    }
                    return neighbourCoords.Select(nC => region.TryGetValue(nC, out Cell? cell) ? cell!.Character : ' ').Distinct().Contains(ch);
                });
        }

        public class Region
        {
            public Dictionary<Coord, Cell> Cells { get; init; } = [];

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
            while (result.Sum(region => region.Count) < map.Cells.Count)
            {
                foreach ((Coord coord, Cell cell) in map.Cells)
                {
                    Region? region = result.IsNeighbourOfAndMatchesRegion(coord, cell.Character, 5);
                    if (region is null)
                    {
                        region = new();
                        result.Add(region);
                    }
                    region.Cells[coord] = cell;
                }
            }
            return result;
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