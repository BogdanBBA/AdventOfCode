namespace AdventOfCode01
{
    using NUnit.Framework;

    [TestFixture]
    internal class Tests
    {
        private const string Input = @"two1nine
eightwothree
abcone2threexyz
xtwone3four
4nineeightseven2
zoneight234
7pqrstsixteen";

        private static readonly int[] Output = { 29, 83, 13, 24, 42, 14, 76 };

        [Test]
        public void TestProcessLine()
        {
            // Calculator calculator = new();
            // string[] lines = Input.Split("\n");
            // Assert.Equals(lines.Length, Output.Length);
            // for (int i = 0; i < lines.Length; i++)
            //     Assert.Equals(calculator.ProcessLine(lines[i]), Output[i]);
        }
    }
}