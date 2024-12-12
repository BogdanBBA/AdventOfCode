namespace AoC2015.Day01
{
    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 1;

        private static int Process(string input, bool returnFirstBasementPositionInstead)
        {
            int result = 0;
            for (int iCh = 0; iCh < input.Length; iCh++)
            {
                result += input[iCh] == '(' ? +1 : -1;
                if (returnFirstBasementPositionInstead && result < 0)
                    return iCh + 1;
            }

            return result;
        }

        public override void Run()
        {
            string input = File.ReadAllText(GetInputFilePath(GetCurrentPart));
            Console.WriteLine($" > Result (part 1): {Process(input, false)}");
            Console.WriteLine($" > Result (part 2): {Process(input, true)}");
        }
    }
}