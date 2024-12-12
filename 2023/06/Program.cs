using System.Text.RegularExpressions;

internal class Program
{
    public class Race(ulong timeMs, ulong distanceMm)
    {
        public ulong TimeMs { get; } = timeMs;
        public ulong DistanceMm { get; } = distanceMm;

        public List<ulong> GetAllPossibleDistances()
        {
            //return Enumerable.Range(0, TimeMs + 1).Select(time => time/*==speed*/ * (TimeMs - time)).ToList();
            List<ulong> result = [];
            for (ulong time = 0; time <= TimeMs; time++)
            {
                result.Add(time/*==speed*/ * (TimeMs - time));
            }
            return result;
        }

        public int GetCountOfWinningPossibilities()
            => GetAllPossibleDistances().Count(distance => distance > DistanceMm);

        public override string ToString() => $"{TimeMs}ms {DistanceMm}mm";
    }

    private static List<Race> ParseRaces(string intputFile)
    {
        static ulong[] GetNumbers(string line)
            => Regex.Matches(line, @"[\d ]+").Select(match => ulong.Parse(match.Value.Replace(" ", string.Empty))).ToArray();
        string[] lines = File.ReadAllLines(intputFile);
        ulong[] times = GetNumbers(lines[0]), distances = GetNumbers(lines[1]);
        return Enumerable.Range(0, times.Length).Select(index => new Race(times[index], distances[index])).ToList();
    }

    private static void Main(string[] _)
    {
        List<Race> races = ParseRaces("input1.txt");
        Console.WriteLine($" > Races:\n{string.Join("\n", races.Select(race => $" - race {race}: {"(not printed)"/*string.Join(", ", race.GetAllPossibleDistances())*/} (of which {race.GetCountOfWinningPossibilities()} winning)"))}");
        Console.WriteLine($" > Product of each race's count of winning possibilities is: {races.Select(race => race.GetCountOfWinningPossibilities()).Aggregate(1, (total, next) => total * next)}");
    }
}