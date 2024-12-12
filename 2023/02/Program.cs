namespace AdventOfCode02
{
    internal class Program
    {
        private static void Main(string[] _)
        {
            GameSet gameSet = new("input.txt");

            CubeCount limits = new() { { Cube.Red, 12 }, { Cube.Green, 13 }, { Cube.Blue, 14 } };
            List<Game> possibleGames = gameSet.GetPossibleGames(limits);
            Console.WriteLine($"The sum of the {possibleGames.Count} (out of {gameSet.Games.Count}) games' IDs is: {possibleGames.Select(game => game.ID).Sum()}");
            Console.WriteLine($"The sum of the powers of the minimum cube sets (of all games) is: {gameSet.GetSumOfPowersOfMinimumCubeCounts()}");
        }
    }
}