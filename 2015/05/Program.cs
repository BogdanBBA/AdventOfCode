namespace AoC2015.Day05
{
    using NewReasoning = (string String, bool IsNice, NewNaughtinessDecider.NaughtyReason Reason);

    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 5;
        public override int GetCurrentPart => 1;

        public override void Run()
        {
            string[] strings = File.ReadAllLines(GetInputFilePath(GetCurrentPart));
            NewReasoning[] reasonings = strings.Select(NewNaughtinessDecider.DetermineNaughtiness).ToArray();
            Console.WriteLine(string.Join("\n", reasonings.Select(r => $" - {r.String}: {r.IsNice}{(r.IsNice ? "" : $" ({r.Reason})")}")));
            Console.WriteLine($"\n > Result (input file {GetCurrentPart}): {reasonings.Count(r => r.IsNice)} strings are nice");
        }
    }
}