using System.Text;

namespace AoC2016
{
    public class IPv7Address
    {
        private static bool ContainsPalindromeOfLength4(string text)
        {
            for (int start = 0, end = 3; end < text.Length; start++, end++)
                if (text[start] == text[end] && text[start + 1] == text[end - 1] && text[start] != text[start + 1])
                    return true;
            return false;
        }

        private static List<(string, string)> GetABAs(string text)
        {
            List<(string, string)> result = [];
            for (int i = 0; i <= text.Length - 3; i++)
            {
                char x = text[i], y = text[i + 1], z = text[i + 2];
                if (x != y && x == z)
                {
                    (string aba, string bab) pal = ($"{x}{y}{x}", $"{y}{x}{y}");
                    if (!result.Any(res => res.Item1 == pal.aba))
                        result.Add(pal);
                }
            }
            return result;
        }

        public string FullAddress { get; private set; }
        public List<string> NormalSequences { get; private set; }
        public List<string> HypernetSequences { get; private set; }
        public bool HasTLSSupport { get; private set; }
        public bool HasSSLSupport { get; private set; }

        public IPv7Address(string address)
        {
            FullAddress = address;
            NormalSequences = [];
            HypernetSequences = [];
            StringBuilder sb = new();
            for (int i = 0; i < address.Length; i++)
            {
                char c = address[i];
                if (c >= 'a' && c <= 'z')
                    sb.Append(c);
                if (c == '[' || i == address.Length - 1)
                {
                    NormalSequences.Add(sb.ToString());
                    sb = new();
                }
                else if (c == ']')
                {
                    HypernetSequences.Add(sb.ToString());
                    sb = new();
                }
            }
            HasTLSSupport = SupportsTLS();
            HasSSLSupport = SupportsSSL();
        }

        private bool SupportsTLS() // transport-layer snooping
            => NormalSequences.Any(ContainsPalindromeOfLength4) && !HypernetSequences.Any(ContainsPalindromeOfLength4);

        private bool SupportsSSL() // super-secret listening
        {
            List<(string ABA, string BAB)> pals = NormalSequences.SelectMany(GetABAs).ToList();
            if (pals.Count == 0)
                return false;
            return HypernetSequences.Any(seq => pals.Any(pal => seq.Contains(pal.BAB)));
        }
    }


    public class Day07 : IDayProgram
    {
        private static string FormatBool(bool value)
            => value.ToString().ToUpperInvariant().PadLeft(5);

        private static string Format(IPv7Address address)
            => $" - TLS = {FormatBool(address.HasTLSSupport)}, SSL = {FormatBool(address.HasSSLSupport)}: {address.FullAddress}";

        private static IPv7Address[] LoadAndPrintAddresses(string inputFile)
        {
            IPv7Address[] addresses = ParseFromFile(inputFile, line => new IPv7Address(line));
            $" > There are {addresses.Length} addresses in input file '{inputFile}':\n{string.Join("\n", addresses.Select(Format))}\n".Log();
            return addresses;
        }

        public override void Run()
        {
            IPv7Address[] addresses = LoadAndPrintAddresses(@"07");
            $" > Out of the {addresses.Length} addresses, a number of {addresses.Count(address => address.HasTLSSupport)} support TSL.".Log();
            $" > Out of the {addresses.Length} addresses, a number of {addresses.Count(address => address.HasSSLSupport)} support SSL.\n".Log();
        }
    }
}