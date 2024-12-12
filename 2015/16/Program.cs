using System.Text.RegularExpressions;

namespace AoC2015.Day16
{
    public enum Property { children, cats, samoyeds, pomeranians, akitas, vizslas, goldfish, trees, cars, perfumes }

    public class Sue(int number, Dictionary<Property, int> properties)
    {
        public static Sue ParseSue(string line)
        {
            int number = int.Parse(Regex.Match(line, @"Sue (\d+):").Groups[1].Value);
            string[] pairs = line[(line.IndexOf(':') + 2)..].Split(", ");

            return new(number, pairs.Select(pair =>
            {
                string[] parts = pair.Split(": ");
                return ((Property)Enum.Parse(typeof(Property), parts[0]), int.Parse(parts[1]));
            }).ToDictionary(t => t.Item1, t => t.Item2));
        }

        public int Number { get; private set; } = number;
        public Dictionary<Property, int> Properties { get; private set; } = properties;

        public override string ToString()
            => $"Sue {Number}. {string.Join(", ", Properties.Keys.Select(key => $"{key}={Properties[key]}"))}";
    }

    public class Program : IDayProgram
    {

        public override int GetCurrentDay => 16;
        public override int GetCurrentPart => 1;

        /// <summary>Determines whether the given Sue passes the given property restrictions.<br/>
        /// Assumes the property dictionary contains values for all property types.</summary>
        /// <param name="sue">a given Sue</param>
        /// <param name="dict">the dictionary of property restrictions</param>
        /// <param name="useCorrectInterpretation">if false, uses the initial incorrect interpretation</param>
        /// <returns>a sublist (ideally containing exactly one item) of possible/valid Sues</returns>
        private static bool SueIsPossible(Sue sue, Dictionary<Property, int> dict, bool useCorrectInterpretation)
        {
            if (!useCorrectInterpretation)
            {
                return dict.Keys
                    .Where(sue.Properties.ContainsKey)
                    .All(key => sue.Properties.TryGetValue(key, out int value) && value == dict[key]);
            }

            foreach (Property key in dict.Keys)
            {
                if (!sue.Properties.TryGetValue(key, out int value))
                    continue;

                if (key == Property.cats || key == Property.trees) // must be >
                {
                    if (value <= dict[key])
                        return false;
                }
                else if (key == Property.pomeranians || key == Property.goldfish) // must be <
                {
                    if (value >= dict[key])
                        return false;
                }
                else // must be ==
                {
                    if (value != dict[key])
                        return false;
                }
            }

            return true;
        }

        private static List<Sue> FindSues(List<Sue> sues, Dictionary<Property, int> dict, bool useCorrectInterpretation)
            => sues.Where(sue => SueIsPossible(sue, dict, useCorrectInterpretation)).ToList();

        private static void PrintSues(string title, List<Sue> sues)
            => Console.WriteLine($"\n > {title}:\n{string.Join("\n", sues)}");

        public override void Run()
        {
            string[] lines = File.ReadAllLines(GetInputFilePath(GetCurrentPart));
            List<Sue> sues = lines.Select(Sue.ParseSue).ToList();
            Dictionary<Property, int> propertyRestrictions = new(){
                { Property.children, 3},
                { Property.cats, 7 },
                { Property.samoyeds, 2 },
                { Property.pomeranians, 3 },
                { Property.akitas, 0 },
                { Property.vizslas, 0 },
                { Property.goldfish, 5 },
                { Property.trees, 3 },
                { Property.cars, 2 },
                { Property.perfumes, 1 }
            };

            PrintSues("All Sues", sues);
            PrintSues("The real aunt Sue (as per the incorrect interpretation)", FindSues(sues, propertyRestrictions, false));
            PrintSues("The REAL aunt Sue ", FindSues(sues, propertyRestrictions, true));
        }
    }
}