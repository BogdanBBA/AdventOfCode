using AoC2024.ForDay08;
using AoC2024.MatrixNavigation;
using Cell = AoC2024.MatrixNavigation.NavigableMap.Cell;

namespace AoC2024
{
    namespace ForDay08
    {
        public record AntennaData(char? Antenna, HashSet<char> Antinodes)
        {
            public override string ToString()
                => Antenna != null ? (Antinodes.Count > 0 ? "+" : $"{Antenna}") : (Antinodes.Count > 0 ? "#" : ".");
        }
    }

    public class Day08 : DayProgram
    {
        private static void ClearAllAntinodes(NavigableMap map)
            => map.Cells.Values.ToList().ForEach(cell =>
            {
                if (cell.Tag is AntennaData { Antinodes.Count: > 0 } cellWithAntinodes)
                    cellWithAntinodes.Antinodes.Clear();
            });

        private static int CountDistinctAntinodeCoords(NavigableMap map)
            => map.Cells.Values.Count(cell => cell.Tag is AntennaData { Antinodes.Count: > 0 });

        private static void ProjectAntinode(char antenna, NavigableMap map, Cell a, Cell b, bool useResonantHarmonics)
        {
            int dX = b.Coord.X - a.Coord.X, dY = b.Coord.Y - a.Coord.Y;

            if (!useResonantHarmonics)
            {
                Coord antinodeCoord = new(a.Coord.X + 2 * dX, a.Coord.Y + 2 * dY);
                if (map.TryGetByCoord(antinodeCoord, out Cell? antinodeCell))
                    (antinodeCell!.Tag as AntennaData)!.Antinodes.Add(antenna);
                return;
            }

            for (Coord coord = new(b.Coord.X, b.Coord.Y); map.TryGetByCoord(coord, out Cell? antinodeCell); coord = new(coord.X + dX, coord.Y + dY))
            {
                (antinodeCell!.Tag as AntennaData)!.Antinodes.Add(antenna);
            }
        }

        private static void CalculateAndPrintAntinodes(NavigableMap map, bool useResonantHarmonics)
        {
            ClearAllAntinodes(map);

            $" > Input map:\n{map.ToTagMapString()}".Log();

            Dictionary<char, IGrouping<char?, Cell>> antennaCells = map.Cells.Values
                .Where(cell => cell.Tag is AntennaData { Antenna: not null })
                .GroupBy(cell => (cell.Tag as AntennaData)!.Antenna)
                .ToDictionary(g => g.Key!.Value, g => g);

            foreach (char antenna in antennaCells.Keys)
            {
                List<Cell> cells = [.. antennaCells[antenna]];
                for (int i = 0; i < cells.Count - 1; i++)
                {
                    for (int j = i + 1; j < cells.Count; j++)
                    {
                        ProjectAntinode(antenna, map, cells[i], cells[j], useResonantHarmonics);
                        ProjectAntinode(antenna, map, cells[j], cells[i], useResonantHarmonics);
                    }
                }
            }

            $"\n > Map with calculated antinodes:\n{map.ToTagMapString()}".Log();
            $"\n > The number of unique locations containing an antinode is {CountDistinctAntinodeCoords(map)}.".Log();
        }

        public override void Run()
        {
            NavigableMap map = NavigableMap.Parse(ReadFromFile_Strings(@"08"), (_, _) => true, (_, chr) => new AntennaData(chr != '.' ? chr : null, []));

            CalculateAndPrintAntinodes(map, false);
            $"\n ... now using RESONANT HARMONICS\n".Log();
            CalculateAndPrintAntinodes(map, true);
        }
    }
}