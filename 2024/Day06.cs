using AoC2024.MatrixNavigation;
using Cell = AoC2024.MatrixNavigation.NavigableMap.Cell;

namespace AoC2024
{
    public class Day06 : DayProgram
    {
        private static HashSet<Cell> RunPatrol(NavigableMap map, Action<List<Cell>>? loopDetectedCallback = null)
        {
            // returns bool indicating whether the next cell is within map bounds
            // even if the cell is on the map, its value can still be null, meaning it's not free (it's a #)
            bool NextCellIsOnMap(Cell guard, DirectionsUDLR facing, out Cell? nextCell)
            {
                Coord nextCoord = guard.Coord.Move(facing);
                if (!map.TryGetByCoord(nextCoord, out nextCell))
                    return false;
                if (nextCell?.Character == '#')
                    nextCell = null;
                return true;
            };

            bool IsInALoop(IEnumerable<Coord> trace)
            {
                if (trace.Count() < 4)
                    return false;
                Coord[] array = trace.Reverse().ToArray();
                List<int> indexes = Enumerable.Range(1, array.Length - 1)
                    .Select(index => (index, array[index]))
                    .Where(pair => array[0] == pair.Item2)
                    .Select(pair => pair.index)
                    .ToList();
                if (indexes.Count == 0)
                    return false;
                int firstIndex = indexes.First();
                for (int mainIndex = 1; mainIndex < firstIndex && indexes.Count > 0; mainIndex++)
                {
                    for (int indexIndex = 0; indexIndex < indexes.Count; indexIndex++)
                    {
                        int subIndex = indexes[indexIndex] + mainIndex;
                        if (subIndex >= array.Length || array[mainIndex] != array[subIndex])
                        {
                            indexes.RemoveAt(indexIndex--);
                        }
                    }
                }
                return indexes.Count > 0;
            }

            Cell guard = map.GetByCharacter('^');
            DirectionsUDLR facing = DirectionsUDLR.Up;
            List<Cell> trace = [guard];

            // the guard patrols the map according to its rules until it exits the map
            while (true)
            {
                Cell? nextCell;
                // determine the next cell and whether the guard can go to it
                do
                {
                    // if next cell is outside the map, then the guard has left: return result
                    if (!NextCellIsOnMap(guard, facing, out nextCell))
                        return [.. trace];
                    // the next cell is on the map and free, the guard will go to it
                    if (nextCell != null)
                        break;
                    // since the next cell is not a good one (#), the guard will turn right
                    facing = facing.RotateOnce(DirectionsLR.Right);
                } while (nextCell == null);

                // the guard goes to the next cell, and will then repeat
                guard = nextCell;
                trace.Add(guard);

                // detect whether the guard is in a loop by this point; if it is, return the trace as normal
                if (IsInALoop(trace.Select(cell => cell.Coord)))
                {
                    loopDetectedCallback?.Invoke(trace);
                    return [.. trace];
                }
            }
        }

        private int CountLoopCausingPlacementVariants(NavigableMap map)
        {
            Cell[] cellsToTry = map.Cells.Values.Where(cell => cell.Character != '#' && cell.Character != '^').ToArray();
            int loops = 0;
            for (int index = 0; index < cellsToTry.Length; index++)
            {
                Cell cell = cellsToTry[index];
                $" - testing for loops with obstacle at [{cell.Coord.X,3}, {cell.Coord.Y,3}] ({(index + 1.0) / cellsToTry.Length:P3}, ETA {GetETA(index + 1, cellsToTry.Length).TotalSeconds:N0}s)...".LogNNL();
                NavigableMap mapVariant = new(map);
                mapVariant.Cells[cell.Coord] = new('#', cell.Coord, cell.MovementOptions);
                RunPatrol(mapVariant, trace =>
                {
                    $" loop detected!".LogNNL();
                    loops++;
                });
                $" done.".Log();
            }
            return loops;
        }

        public override void Run()
        {
            NavigableMap map = NavigableMap.Parse(ReadFromFile_Strings(@"06"), (_, _) => true);

            HashSet<Cell> distinctCells = RunPatrol(map);
            $" > The number of distinct cells visited by the patrol on the {map.MapSize.Width} x {map.MapSize.Height} map is {distinctCells.Count}.".Log();

            // took 9000 seconds and resulted in 1995
            $" > The number of obstacles that can be placed to cause the guard to go in a loop is {CountLoopCausingPlacementVariants(map)}.".Log();
        }
    }
}