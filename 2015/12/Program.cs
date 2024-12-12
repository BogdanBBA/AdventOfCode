using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace AoC2015.Day12
{
    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 12;
        public override int GetCurrentPart => 1;

        private static Dictionary<string, int> GetTokenTypes(JToken token)
        {
            Dictionary<string, int> result = [];
            result[token.Type.ToString()] = 1;
            foreach (JToken child in token.Children())
            {
                Dictionary<string, int> sub = GetTokenTypes(child);
                foreach (KeyValuePair<string, int> pair in sub)
                {
                    if (!result.ContainsKey(pair.Key))
                        result[pair.Key] = 0;
                    result[pair.Key] += pair.Value;
                }
            }
            return result;
        }

        private static List<int> GetNonRedInts(JToken token)
        {
            if (token.Type == JTokenType.String && (token.Value<string>()?.Contains("red") ?? false))
                return [];

            if (token.Type == JTokenType.Integer)
                return [token.Value<int>()];

            if (token.Type == JTokenType.Array)
            {
                if (((JArray)token).Children().Any(child => child.Type == JTokenType.String && (child.Value<string>()?.Contains("red") ?? false)))
                    return [];
            }

            // token.Type == JTokenType.Object || token.Type == JTokenType.Property

            List<int> result = [];
            foreach (JToken child in token.Children())
                result.AddRange(GetNonRedInts(child));
            return result;
        }

        public override void Run()
        {
            string contents = File.ReadAllText(GetInputFilePath(1));
            int[] numbers = Regex.Matches(contents, @"\-?\d+", RegexOptions.Singleline).Select(match => int.Parse(match.Value)).ToArray();
            Console.WriteLine($" > The sum of the {numbers.Length} numbers ({string.Join(", ", numbers)}) is: {numbers.Sum()}\n");

            JObject root = JObject.Parse($"{{\"rootList\":{File.ReadAllText(GetInputFilePath(2))}}}");
            Dictionary<string, int> dict = GetTokenTypes(root);
            Console.WriteLine($" > Token types ({dict.Count}): {string.Join(", ", dict.OrderByDescending(p => p.Value).Select(p => $"{p.Key} ({p.Value})"))}");
            List<int> nonRedNumbers = GetNonRedInts(root);
            Console.WriteLine($" > The sum of the {nonRedNumbers.Count} numbers (...), all non-RED-related, is: {nonRedNumbers.Sum()}");
        }
    }
}