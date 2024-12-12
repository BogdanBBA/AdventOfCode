namespace AoC17
{
    public static class Utils
    {
        public static bool IsWithinGrid(int row, int col, int gridSize)
            => row >= 0 && row < gridSize && col >= 0 && col < gridSize;

        public static Vertex? GetIfWithinBounds(this Vertex[] vertices, int iRow, int iCol, int gridSize)
            => IsWithinGrid(iRow, iCol, gridSize) ? vertices[iRow * gridSize + iCol] : null;

        public static string NameOrNull(this Vertex? vertex)
            => vertex?.Name ?? "null";

        public static Edge? CreateEdgeTo(this Vertex vertex, Vertex? to)
            => to is null ? null : new(vertex, to, to.Value);

        public static void AddEdgeIfNotNull(this Dictionary<Vertex, List<Edge>> edges, Edge? edge)
        {
            if (edge is not null)
            {
                if (!edges.ContainsKey(edge.Start))
                    edges.Add(edge.Start, []);
                edges[edge.Start].Add(edge);
            }
        }
    }

    public class Vertex(string name, int index1d, int row, int col, int value)
    {
        public string Name { get; private set; } = name;
        public int Index1D { get; private set; } = index1d;
        public (int Row, int Col) Location { get; private set; } = (row, col);
        public int Value { get; private set; } = value;
        public Vertex? Top { get; internal set; }
        public Vertex? Right { get; internal set; }
        public Vertex? Bottom { get; internal set; }
        public Vertex? Left { get; internal set; }

        public override string ToString()
            => $"{Name} T:{Top.NameOrNull()} B:{Bottom.NameOrNull()} L:{Left.NameOrNull()} R:{Right.NameOrNull()}";
    }

    public class Edge(Vertex start, Vertex end, int cost)
    {
        public Vertex Start { get; private set; } = start;
        public Vertex End { get; private set; } = end;
        public int Cost { get; private set; } = cost;

        public override string ToString()
            => $"{Start.Name} -> {End.Name} ({Cost})";
    }

    public class Graph
    {
        public int[][] GridCells { get; private set; }
        public Vertex[] Vertices { get; private set; }
        public Dictionary<Vertex, List<Edge>> Edges { get; private set; }

        public Graph(string inputFile)
        {
            string[] lines = File.ReadAllLines(inputFile);
            GridCells = lines.Select(row => row.Select(character => int.Parse($"{character}")).ToArray()).ToArray();

            Vertices = new Vertex[GridCells.Length * GridCells.Length];
            Edges = [];

            IterateGridCells((iRow, iCol, index1d) => Vertices[index1d] = new($"[{iRow,3}][{iCol,3}]", index1d, iRow, iCol, GridCells[iRow][iCol]));
            IterateGridCells((iRow, iCol, index1d) =>
            {
                Vertex vertex = Vertices[index1d];
                vertex.Top = Vertices.GetIfWithinBounds(iRow - 1, iCol, GridCells.Length);
                vertex.Bottom = Vertices.GetIfWithinBounds(iRow + 1, iCol, GridCells.Length);
                vertex.Left = Vertices.GetIfWithinBounds(iRow, iCol - 1, GridCells.Length);
                vertex.Right = Vertices.GetIfWithinBounds(iRow, iCol + 1, GridCells.Length);
            });
            IterateGridCells((iRow, iCol, index1d) =>
            {
                Vertex vertex = Vertices[index1d];
                Edges.AddEdgeIfNotNull(vertex.CreateEdgeTo(vertex.Top));
                Edges.AddEdgeIfNotNull(vertex.CreateEdgeTo(vertex.Bottom));
                Edges.AddEdgeIfNotNull(vertex.CreateEdgeTo(vertex.Left));
                Edges.AddEdgeIfNotNull(vertex.CreateEdgeTo(vertex.Right));
            });
        }

        private void IterateGridCells(Action<int, int, int> action)
        {
            for (int iRow = 0; iRow < GridCells.Length; iRow++)
                for (int iCol = 0; iCol < GridCells[iRow].Length; iCol++)
                    action(iRow, iCol, iRow * GridCells.Length + iCol);
        }

        public Dictionary<Vertex, int> DijkstraGetShortestPathToAll(Vertex startVertex)
        {
            int[] distances = new int[Vertices.Length];
            int[] previous = new int[Vertices.Length];
            bool[] visited = new bool[Vertices.Length];

            for (int index = 0; index < Vertices.Length; index++)
            {
                distances[index] = int.MaxValue;
                previous[index] = -1;
            }

            PriorityQueue<int, int> priorityQueue = new();
            priorityQueue.Enqueue(startVertex.Index1D, 0);
            distances[startVertex.Index1D] = 0;

            while (priorityQueue.Count != 0)
            {
                int iCurrentVertex = priorityQueue.Dequeue();
                visited[iCurrentVertex] = true;

                foreach (Edge edge in Edges[Vertices[iCurrentVertex]])
                {
                    int iNeighbor = edge.End.Index1D;

                    if (visited[iNeighbor])
                        continue;

                    var distance = distances[iCurrentVertex] + edge.Cost;
                    if (distance < distances[iNeighbor])
                    {
                        distances[iNeighbor] = distance;
                        previous[iNeighbor] = iCurrentVertex;
                        priorityQueue.Enqueue(iNeighbor, distance);
                    }
                }
            }

            return Enumerable.Range(0, distances.Length).ToDictionary(index => Vertices[index], index => distances[index]);
        }

        public string GetGridDisplayText()
            => $"    {string.Join(null, Enumerable.Range(0, GridCells.Length).Select(iCol => iCol % 10))}\n{string.Join("\n", Enumerable.Range(0, GridCells.Length).Select(iRow => $"{iRow,3} {string.Join(null, GridCells[iRow])}"))}";

        public void Print(string title)
            => Console.WriteLine($"\n{title}:\n{GetGridDisplayText()}");
    }

    public class Program
    {
        private static void Main(string[] _)
        {
            string inputFile = true ? "input0.txt" : "input1.txt";

            Graph grid = new(inputFile);
            grid.Print("Original grid");
            Dictionary<Vertex, int> distances = grid.DijkstraGetShortestPathToAll(grid.Vertices.First());
            Console.WriteLine($" > Ther shortest Dijkstra distance between TL and BR cells is: {distances[grid.Vertices.Last()]} (not rule-abiding!)");
        }
    }
}