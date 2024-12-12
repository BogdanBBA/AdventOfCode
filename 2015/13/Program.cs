using System.Text.RegularExpressions;

namespace AoC2015.Day13
{
    public class HappinessChange(string person, string otherPerson, int changeInHappiness)
    {
        public string Person { get; private set; } = person;
        public string OtherPerson { get; private set; } = otherPerson;
        public int ChangeInHappiness { get; private set; } = changeInHappiness;

        public static HappinessChange Parse(string line)
        {
            Match match = Regex.Match(line, @"(\w+) would (\w+) (\d+) .+ next to (\w+).");
            return new(match.Groups[1].Value, match.Groups[4].Value, (match.Groups[2].Value == "gain" ? 1 : -1) * int.Parse(match.Groups[3].Value));
        }

        public static int CalculateTotalHappinessChange(Dictionary<(string, string), int> dict, string[] orderedPersons)
        {
            int result = 0;
            for (int i = 0; i < orderedPersons.Length; i++)
            {
                int j = i < orderedPersons.Length - 1 ? i + 1 : 0;
                result += dict[(orderedPersons[i], orderedPersons[j])] + dict[(orderedPersons[j], orderedPersons[i])];
            }
            return result;
        }
    }

    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 13;
        public override int GetCurrentPart => 2;

        public override void Run()
        {
            string[] lines = File.ReadAllLines(GetInputFilePath(GetCurrentPart == 0 ? 0 : 1));
            List<HappinessChange> changes = lines.Select(HappinessChange.Parse).ToList();
            string[] persons = [.. changes.Select(change => change.Person).Distinct().OrderBy(x => x)];
            if (GetCurrentPart == 2)
            {
                foreach (string person in persons)
                {
                    changes.Add(new(person, "ME", 0));
                    changes.Add(new("ME", person, 0));
                }
                persons = [.. changes.Select(change => change.Person).Distinct().OrderBy(x => x)];
            }
            Dictionary<(string, string), int> dict = changes.ToDictionary(c => (c.Person, c.OtherPerson), c => c.ChangeInHappiness);
            (string[] personOrder, int)[] changesInHappinessByPersonOrder = [.. UtilsOther.GeneratePermutations(persons)
                .Select(personOrder => (personOrder, HappinessChange.CalculateTotalHappinessChange(dict, personOrder)))
                .OrderBy(p => p.Item2)];
            Console.WriteLine(string.Join("\n", changesInHappinessByPersonOrder.Select(p => $" - {string.Join("-", p.personOrder)}: {p.Item2}")));
        }
    }
}