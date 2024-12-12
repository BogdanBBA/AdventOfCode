namespace AoC2016
{
    public class Day06 : IDayProgram
    {
        private static string ErrorCorrectMessage(string[] messages, bool alternativeCommonness)
        {
            return new(Enumerable.Range(0, messages[0].Length).Select(index =>
            {
                IGrouping<char, char>[] orderedCharacterFrequencies = [..
                    messages.Select(msg => msg[index])
                        .GroupBy(ch => ch)
                        .OrderByDescending(group => group.Count())];
                return alternativeCommonness ? orderedCharacterFrequencies[^1].Key : orderedCharacterFrequencies[0].Key;
            }).ToArray());
        }

        public override void Run()
        {
            string[] messages = ParseFromFile(@"06", line => line);

            string correctedMessage1 = ErrorCorrectMessage(messages, false);
            $" > The error-corrected message from the given {messages.Length} recordings is '{correctedMessage1}'.".Log();

            string correctedMessage2 = ErrorCorrectMessage(messages, true);
            $" > The actual error-corrected message from Santa is '{correctedMessage2}'.".Log();
        }
    }
}