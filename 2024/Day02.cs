namespace AoC2024
{
    public class Report(int[] pLevels)
    {
        private static int[] GetDiffs(int[] levels)
            => Enumerable.Range(0, levels.Length - 1).Select(index => levels[index + 1] - levels[index]).ToArray();


        private static bool AllAscendingOrDescending(int[] levels)
        {
            int[] diffs = GetDiffs(levels);

            if (diffs.Any(diff => diff == 0))
                return false;

            return diffs[0] > 0
                ? diffs.All(diff => diff >= 1 && diff <= 3)
                : diffs.All(diff => diff >= -3 && diff <= -1);
        }

        public int[] Levels { get; private set; } = pLevels;

        public bool IsSafe(bool useProblemDampener, bool debugLogs)
        {
            if (debugLogs)
                $" - {string.Join(' ', Levels.Select(l => l))} (diffs: {string.Join(", ", GetDiffs(Levels).Select(d => d == 0 ? "--" : (d < 0 ? $"{d}" : $"+{d}")))})".Log();

            bool allAscendingOrDescending = AllAscendingOrDescending(Levels);

            if (useProblemDampener)
            {
                if (allAscendingOrDescending)
                    return true;
                return Enumerable.Range(0, Levels.Length).Any(index =>
                {
                    List<int> list = [.. Levels];
                    list.RemoveAt(index);
                    return AllAscendingOrDescending([.. list]);
                });
            }
            else
            {
                return allAscendingOrDescending;
            }
        }
    }

    public class Day02 : DayProgram
    {
        public override void Run()
        {
            dynamic settings = new { InputFile = @"02", UseProblemDampener = true, DebuggingLogs = true };

            Report[] reports = ParseFromFile((string)settings.InputFile, line => new Report([.. line.Split(" ").Select(int.Parse)]));

            $" > Out of the {reports.Length} reports in '{settings.InputFile}', only {reports.Count(rep => rep.IsSafe(settings.UseProblemDampener, settings.DebuggingLogs))} are safe.".Log();
        }
    }
}