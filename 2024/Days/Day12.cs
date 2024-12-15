using AoC2024.MatrixNavigation;
using AoC2024.ForDay12;
using Fence = (AoC2024.MatrixNavigation.Coord A, AoC2024.MatrixNavigation.Coord B);
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

            public static bool IsNeighbourOf(this Region a, Region b)
            {
                if (a.AssumedCharacter != b.AssumedCharacter)
                    return false;
                List<Coord> neighboursA = [.. a.Cells.Select(cell => cell.Key).SelectMany(Get4Neighbours).Distinct()];
                List<Coord> neighboursB = [.. b.Cells.Select(cell => cell.Key).SelectMany(Get4Neighbours).Distinct()];
                return neighboursA.Any(neighboursB.Contains);
            }
        }

        public class Region
        {
            public Dictionary<Coord, Cell> Cells { get; init; }

            public Region()
            {
                Cells = [];
            }

            public Region(params Cell[] cells)
                : this()
            {
                AddCells(cells);
            }

            public Region(params Region[] regions)
                : this()
            {
                foreach (Region region in regions)
                    foreach ((Coord coord, Cell cell) in region.Cells)
                        Cells.Add(coord, cell);
            }

            public char AssumedCharacter => Cells.First().Value.Character;

            public void AddCells(params Cell[] cells)
            {
                foreach (Cell cell in cells)
                    Cells.Add(cell.Coord, cell);
            }

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
            return fences.Select(fence =>
            {
                Coord[] orderedCoords = [.. new Coord[] { fence.A, fence.B }.OrderBy(coord => coord.Y).ThenBy(coord => coord.X)];
                return new Fence(orderedCoords[0], orderedCoords[1]);
            }).ToHashSet();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "it's irritating")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "I might use it")]
        private static (int Area, HashSet<Fence> Fences, int PriceOfFences) CalculatePriceOfFences_ByFencePieces(Region region)
        {
            int area = CalculateRegionArea(region);
            HashSet<Fence> fences = [.. CalculateRegionFences(region).Distinct()];
            return (area, fences, area * fences.Count);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "it's irritating")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "I might use it")]
        private static (int Area, HashSet<Fence> Fences, int PriceOfFences) CalculatePriceOfFences_ByFenceSides(Region region)
        {
            int area = CalculateRegionArea(region);
            HashSet<Fence> fences = [.. CalculateRegionFences(region).Distinct()];
            int priceOfFences = 0; // TODO; see coordinate ordering in CalculateRegionFences return statement
            return (area, fences, priceOfFences);
        }

        private static List<Region> DetermineRegions(NavigableMap map)
        {
            static List<Coord> GetValidNeighbours(Region megaRegion, Coord coord, char character, Dictionary<Coord, bool> included)
                   => megaRegion.GetUpTo4NeighbouringCells(coord)
                        .Where(cell => cell.Character == character && !included[cell.Coord])
                        .Select(cell => cell.Coord)
                        .ToList();

            static List<Coord> GetDistinctValidNeighbours(Region megaRegion, List<Coord> coords, char character, Dictionary<Coord, bool> included)
                => coords.SelectMany(coord => GetValidNeighbours(megaRegion, coord, character, included))
                        .Distinct()
                        .ToList();

            List<Region> result = [];
            Region megaRegion = new(map.Cells.Values.ToArray());
            Dictionary<Coord, bool> included = map.Cells.Keys.ToDictionary(coord => coord, _ => false);
            while (included.Values.Any(value => !value))
            {
                Coord coord = included.First(pair => !pair.Value).Key;
                char character = megaRegion.Cells[coord].Character;
                included[coord] = true;
                Region region = new(megaRegion.Cells[coord]);
                List<Coord> neighbours = GetValidNeighbours(megaRegion, coord, character, included);
                do
                {
                    foreach (Coord neighbour in neighbours)
                    {
                        included[neighbour] = true;
                        region.AddCells(megaRegion.Cells[neighbour]);
                    }
                    neighbours = GetDistinctValidNeighbours(megaRegion, neighbours, character, included);
                } while (neighbours.Count > 0);
                result.Add(region);
            }
            return result;
        }

        private static void CalculateFencePricingForAllRegions(NavigableMap map, Func<Region, (int, HashSet<Fence>, int)> priceCalculatorFunc)
        {
            map.DrawMapStr().LogDNL();
            List<Region> regions = DetermineRegions(map);
            Dictionary<Region, (int Area, HashSet<Fence> Fences, int PriceOfFences)> fencePriceByRegion = regions.ToDictionary(region => region, priceCalculatorFunc);

            $" > There are {fencePriceByRegion.Count} regions (with {fencePriceByRegion.Keys.Distinct().Count()} types of vegetables):".Log();
            string.Join("\n", fencePriceByRegion.Select(pair => $" - region of {pair.Key.Cells.First().Value.Character}: area {pair.Value.Area,3} x {pair.Value.Fences.Count,3} fences = price of {pair.Value.PriceOfFences,6:N0}")).Log();
            $" > In total, the fences cost {fencePriceByRegion.Sum(pair => pair.Value.PriceOfFences)}.".Log();

            // Dictionary<char, List<(int Area, HashSet<(Coord, Coord)> Fences, int PriceOfFences)>> dict = fencePriceByRegion.GroupBy(pair => pair.Key.AssumedCharacter).ToDictionary(g => g.Key, g => g.Select(pair => pair.Value).ToList());
            // $"\n > In summary, there are {dict.Count} types of vegetables:\n{string.Join("\n", dict.Keys.OrderBy(x => x).Select(key => $" - {key} ({dict[key].Count} regions): {string.Join(", ", dict[key].Select(r => $"{r.Area}x{r.Fences.Count}={r.PriceOfFences}"))}; for a total price of {dict[key].Sum(x => x.PriceOfFences)}"))}".Log();
        }

        public override void Run()
        {
            // price calculation options are CalculatePriceOfFences_ByFenceCount (part 1) or CalculatePriceOfFences_ByFenceSides (part 2)
            Func<Region, (int, HashSet<Fence>, int)> priceCalculatorFunc = CalculatePriceOfFences_ByFenceSides;

            CalculateFencePricingForAllRegions(Parse(ReadFromFile_Strings(@"12"), (_, _) => true, (_, ch) => ch), priceCalculatorFunc);
        }
    }
}