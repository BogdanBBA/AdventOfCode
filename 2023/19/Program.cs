using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AoC19
{
    public static class Utils
    {

    }

    public class Part(string line)
    {
        public Dictionary<char, int> ValuesByParameter { get; private set; } = line.Trim('{', '}').Split(',').Select(part => part.Split('=')).ToDictionary(subParts => subParts[0][0], subParts => int.Parse(subParts[1]));

        public int Rating
            => ValuesByParameter.Values.Sum();

        public override string ToString()
            => string.Join(",", ValuesByParameter.Keys.Select(key => $"{key}:{ValuesByParameter[key]}"));
    }

    public class Condition(string outcomeName)
    {
        public string OutcomeName { get; private set; } = outcomeName;

        public virtual bool Passes(Part part)
            => true;

        public override string ToString()
            => $"TRUE→{OutcomeName}";
    }

    public class ActualCondition(char parameter, char op, int value, string outcomeName) : Condition(outcomeName)
    {
        public char Parameter { get; private set; } = parameter;
        public char Operator { get; private set; } = op;
        public int Value { get; private set; } = value;

        public override bool Passes(Part part)
        {
            int value = part.ValuesByParameter[Parameter];
            return Operator == '<' ? value < Value : value > Value;
        }

        public override string ToString()
            => $"{Parameter}{Operator}{Value}→{OutcomeName}";
    }

    public class Rule
    {
        private string OriginalText { get; set; }
        private Factory Factory { get; set; }
        public Condition Condition { get; private set; }

        public Rule(string text, Factory factory)
        {
            OriginalText = text;
            Factory = factory;
            if (text.Contains(':'))
            {
                Match match = Regex.Match(text, @"(\w+)(<|>)(\d+):(\w+)");
                Condition = new ActualCondition(match.Groups[1].Value[0], match.Groups[2].Value[0], int.Parse(match.Groups[3].Value), match.Groups[4].Value);
            }
            else
            {
                Condition = new(text);
            }
        }

        public (bool accepted, bool rejected, Workflow? workflow) EvaluatePart(Part part)
        {
            if (!Condition.Passes(part))
                return (false, false, null);
            if (Condition.OutcomeName == "A")
                return (true, false, null);
            if (Condition.OutcomeName == "R")
                return (false, true, null);
            return (false, false, Factory.Workflows[Condition.OutcomeName]);

            throw new Exception($"Unable to determine outcome for part {part}!");
        }

        public override string ToString()
            => $"'{OriginalText}': {Condition}";
    }

    public class Workflow
    {
        public string Name { get; private set; }
        public List<Rule> Rules { get; private set; }

        public Workflow(string line, Factory factory)
        {
            Name = line[..line.IndexOf('{')];
            string rules = line.Substring(line.IndexOf('{') + 1, line.Length - Name.Length - 2);
            Rules = rules.Split(',').Select(rule => new Rule(rule, factory)).ToList();
        }

        public bool IsPartAccepted(Part part)
        {
            foreach (Rule rule in Rules)
            {
                (bool accepted, bool rejected, Workflow? workflow) = rule.EvaluatePart(part);
                if (accepted) return true;
                if (rejected) return false;
                if (workflow is null) continue;
                return workflow.IsPartAccepted(part);
            }
            throw new Exception($"?");
        }

        public override string ToString()
            => $"{Name} has {Rules.Count}: {string.Join("; ", Rules)}";
    }

    public class Factory
    {
        public List<Part> Parts { get; private set; }
        public Dictionary<string, Workflow> Workflows { get; private set; }

        public Factory(string partsFile, string workflowFile)
        {
            Parts = File.ReadAllLines(partsFile).Select(line => new Part(line)).ToList();
            Workflows = File.ReadAllLines(workflowFile).Select(line => new Workflow(line, this)).ToDictionary(w => w.Name, w => w);
        }

        public bool IsPartAccepted(Part part)
            => Workflows["in"].IsPartAccepted(part);

        public List<Part> GetAcceptedParts()
            => Parts.Where(IsPartAccepted).ToList();
    }

    internal class Program
    {
        private static void Main(string[] _)
        {
            bool useTestInput = false;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            DateTime startMoment = DateTime.Now;

            Factory factory = new(useTestInput ? "input0-p.txt" : "input1-p.txt", useTestInput ? "input0-w.txt" : "input1-w.txt");
            Console.WriteLine($" > Parts ({factory.Parts.Count}):\n{string.Join("\n", factory.Parts.Select(part => $" - {part}"))}");
            Console.WriteLine($" > Workflows ({factory.Workflows.Count}):\n{string.Join("\n", factory.Workflows.Select(workflow => $" - {workflow}"))}");

            List<Part> acceptedParts = factory.GetAcceptedParts();
            Console.WriteLine($"\n > The total rating of the {acceptedParts.Count} accepted parts is: {acceptedParts.Sum(part => part.Rating)}");
            Console.WriteLine($" > Processing took {DateTime.Now.Subtract(startMoment).TotalSeconds:N2}");
        }
    }
}