using System.Security.Cryptography;
using System.Text;

namespace AoC2015.Day04
{
    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 4;
        public override int GetCurrentPart => 10;

        public static int Get(string key, string startsWith)
        {
            for (int number = 1; true; number++)
            {
                string input = $"{key}{number}";
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = MD5.HashData(inputBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                bool isResult = hash.StartsWith(startsWith);
                if (isResult || number % 100000 == 0) Console.WriteLine($" - {number,9:N0}: {input} → {hash}");
                if (isResult)
                    return number;
            }
        }

        public override void Run()
        {
            string secretKey = GetCurrentPart == 1 ? "abcdef" : (GetCurrentPart == 2 ? "pqrstuv" : "yzbqklnj");
            Console.WriteLine($" > Result (input file {GetCurrentPart}): {Get(secretKey, GetCurrentPart < 3 ? "00000" : "000000"):N0}");
        }
    }
}