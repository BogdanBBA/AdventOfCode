namespace AoC2015.Day11
{
    public static class PasswordGenerator
    {
        public enum Rules { None, SantasDefault, NewSecurityElf }

        private static readonly List<char> AllowedSymbols = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'];

        private static int[] ConvertToIndexArray(string password)
            => password.Select(character => AllowedSymbols.IndexOf(character)).ToArray();

        private static string ConvertToString(int[] indexArray)
            => string.Join(string.Empty, indexArray.Select(index => AllowedSymbols[index]));

        private static void IncrementPassword(int[] indexArray, int count = 1)
        {
            indexArray[^1] += count;
            for (int p = indexArray.Length - 1; p > 0; p--)
            {
                if (indexArray[p] >= AllowedSymbols.Count)
                {
                    indexArray[p - 1] += indexArray[p] / AllowedSymbols.Count;
                    indexArray[p] %= AllowedSymbols.Count;
                }
                // does not yet support wrapping around, I'm sure it'll be necessary
            }
        }

        private static string IncrementPasswordAsPerSanta(string password)
        {
            int[] indexArray = ConvertToIndexArray(password);
            IncrementPassword(indexArray);
            return ConvertToString(indexArray);
        }

        private static bool NewSecurityElfCriteriaAreMet(string password)
        {
            if (password.Contains('i') || password.Contains('o') || password.Contains('l'))
                return false;

            int[] indexArray = ConvertToIndexArray(password);

            int pairs = 0;
            for (int i = 1, startI = 0; i < indexArray.Length; i++)
            {
                if (indexArray[i] == indexArray[i - 1])
                {
                    if (startI == i - 1)
                        pairs++;
                }
                else
                {
                    startI = i;
                }
            }
            if (pairs < 2)
                return false;

            bool containsIncreasingStraight = false;
            for (int i = 0; i < indexArray.Length - 3; i++)
            {
                if (indexArray[i + 1] == indexArray[i] + 1 && indexArray[i + 2] == indexArray[i] + 2)
                {
                    containsIncreasingStraight = true;
                    break;
                }
            }
            if (!containsIncreasingStraight)
                return false;

            return true;
        }

        private static string IncrementPasswordAsPerNewSecurityElf(string password)
        {
            string newPassword = new(password);
            do { newPassword = IncrementPasswordAsPerSanta(newPassword); }
            while (!NewSecurityElfCriteriaAreMet(newPassword));
            return newPassword;
        }

        public static string GetNextPassword(string password, Rules rules)
            => rules switch
            {
                Rules.None => new(password),
                Rules.SantasDefault => IncrementPasswordAsPerSanta(password),
                Rules.NewSecurityElf => IncrementPasswordAsPerNewSecurityElf(password),
                _ => throw new NotImplementedException()
            };
    }

    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 11;
        public override int GetCurrentPart => 1;

        public override void Run()
        {
            const string currentPassword = @"vzbxkghb";
            string nextPassword = PasswordGenerator.GetNextPassword(currentPassword, PasswordGenerator.Rules.NewSecurityElf);
            Console.WriteLine($" > Result (part {1}): the next password after '{currentPassword}' would be '{nextPassword}'");
            string nextNextPassword = PasswordGenerator.GetNextPassword(nextPassword, PasswordGenerator.Rules.NewSecurityElf);
            Console.WriteLine($" > Result (part {2}): the next password after '{nextPassword}' would be '{nextNextPassword}'");
        }
    }
}