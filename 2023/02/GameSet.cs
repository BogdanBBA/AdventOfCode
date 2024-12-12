namespace AdventOfCode02
{
    internal class GameSet
    {
        public List<Game> Games { get; private set; }

        public GameSet(string inputFilePath)
        {
            Games = File.ReadAllLines(inputFilePath).Select(line => new Game(line)).ToList();
        }

        public List<Game> GetPossibleGames(CubeCount limits) => Games.Where(game => game.IsPossible(limits)).ToList();

        public int GetSumOfPowersOfMinimumCubeCounts() => Games.Select(game => game.GetPowerOfMinimumCubeCounts()).Sum();
    }
}