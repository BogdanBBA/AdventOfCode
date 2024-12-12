using System.Text;

namespace AoC2015.Day10
{
    public static class Utils
    {
        private static readonly bool LOG = false;

        public static string GroupByConsecutiveCharacters(this string sequence, int iterations)
        {
            string latestOutput = new(sequence);
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                Group current = new(latestOutput[0]);
                StringBuilder sb = new();
                for (int i = 1; i < latestOutput.Length; i++)
                {
                    if (latestOutput[i] != current.Value)
                    {
                        sb.Append(current.ToString());
                        current = new(latestOutput[i]);
                    }
                    else
                    {
                        current.Count++;
                    }
                }
                sb.Append(current.ToString());
                latestOutput = sb.ToString();
                if (LOG) Console.WriteLine($" - {iteration + 1,2}. {latestOutput}");
            }
            return latestOutput;
        }
    }

    public class Group(char value)
    {
        public int Count { get; internal set; } = 1;
        public char Value { get; internal set; } = value;

        public override string ToString()
            => $"{Count}{Value}";
    }

    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 10;
        public override int GetCurrentPart => 2;

        public override void Run()
        {
            const string input = @"1321131112";
            int iterations = GetCurrentPart == 1 ? 40 : 50;
            string output = input.GroupByConsecutiveCharacters(iterations);
            Console.WriteLine($" > Result (part {GetCurrentPart}): input {input} processed {iterations} times is... {output.Length} in length");
        }
    }
}