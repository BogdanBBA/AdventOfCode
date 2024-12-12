using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace AoC2015.Day19
{
    public class Data(Dictionary<string, string[]> transformMapping, string input)
    {
        public Dictionary<string, string[]> TransformMapping { get; private set; } = transformMapping;
        public string Input { get; private set; } = input;

        public void Print()
            => Console.WriteLine($"{string.Join("\n", TransformMapping.Keys.Select(key => $" > {key} -> {string.Join(", ", TransformMapping[key])}"))}\n");

        public void Calibrate(bool printEachReplacement)
        {
            List<string> replacements = [];
            foreach (string pattern in TransformMapping.Keys)
            {
                MatchCollection matches = Regex.Matches(Input, pattern);
                foreach (Match match in matches)
                {
                    foreach (string replacement in TransformMapping[pattern])
                    {
                        string replaced = Input.Remove(match.Index, match.Length).Insert(match.Index, replacement);
                        if (printEachReplacement) Console.WriteLine($"   - {pattern} / {replacement} -> {replaced}");
                        replacements.Add(replaced);
                    }
                }
            }
            List<string> distinctReplacements = replacements.Distinct().ToList();
            Console.WriteLine($" > Generated {replacements.Count} replacements, but {distinctReplacements.Count} are distinct.\n");
        }

        public void Fabricate()
        {

        }
    }

    public class Program : IDayProgram
    {
        private Data GetTransformMapping()
        {
            if (GetCurrentPart == 0)
            {
                return new(new()
                {
                    { "H", [ "HO", "OH" ] },
                    { "O", [ "HH" ] }
                }, "HOHOHO");
            }
            Dictionary<string, string[]> mapping = File.ReadAllLines(GetInputFilePath(GetCurrentPart))
                .GroupBy(line => line.Split(" => ")[0])
                .ToList()
                .ToDictionary(grouping => grouping.Key, grouping => grouping.Select(line => line.Split(" => ")[1]).ToArray());
            return new(mapping, @"CRnCaCaCaSiRnBPTiMgArSiRnSiRnMgArSiRnCaFArTiTiBSiThFYCaFArCaCaSiThCaPBSiThSiThCaCaPTiRnPBSiThRnFArArCaCaSiThCaSiThSiRnMgArCaPTiBPRnFArSiThCaSiRnFArBCaSiRnCaPRnFArPMgYCaFArCaPTiTiTiBPBSiThCaPTiBPBSiRnFArBPBSiRnCaFArBPRnSiRnFArRnSiRnBFArCaFArCaCaCaSiThSiThCaCaPBPTiTiRnFArCaPTiBSiAlArPBCaCaCaCaCaSiRnMgArCaSiThFArThCaSiThCaSiRnCaFYCaSiRnFYFArFArCaSiRnFYFArCaSiRnBPMgArSiThPRnFArCaSiRnFArTiRnSiRnFYFArCaSiRnBFArCaSiRnTiMgArSiThCaSiThCaFArPRnFArSiRnFArTiTiTiTiBCaCaSiRnCaCaFYFArSiThCaPTiBPTiBCaSiThSiRnMgArCaF");
        }

        public override int GetCurrentDay => 19;
        public override int GetCurrentPart => 0;

        public override void Run()
        {
            Data data = GetTransformMapping();
            data.Print();
            data.Calibrate(false);
            data.Fabricate();
        }
    }
}