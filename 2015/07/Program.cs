namespace AoC2015.Day07
{
    public class Operation(string line)
    {
        public static ushort ResolveConstantOrReferenceToValue(string code, Dictionary<string, Operation> dictionary)
        {
            if (ushort.TryParse(code, out ushort number))
                return number;
            return dictionary[code].DetermineValue(dictionary);
        }

        public string WireName { get; private set; } = line.Split(" -> ")[1];
        public string Code { get; private set; } = line.Split(" -> ")[0];
        public ushort? CachedValue { get; private set; } = null;

        public ushort DetermineValue(Dictionary<string, Operation> dictionary)
        {
            if (CachedValue is not null)
                return CachedValue.Value;

            string[] parts = Code.Split(' ');
            switch (parts.Length)
            {
                case 1:
                    return CachedValue ??= ResolveConstantOrReferenceToValue(Code, dictionary);
                case 2:
                    string reference = parts[1];
                    return CachedValue ??= (ushort)~ResolveConstantOrReferenceToValue(reference, dictionary);
                case 3:
                    string referenceA = parts[0], @operator = parts[1], lastPart = parts[2];
                    ushort valueA = ResolveConstantOrReferenceToValue(referenceA, dictionary);
                    if (@operator == "AND" || @operator == "OR")
                    {
                        ushort valueB = ResolveConstantOrReferenceToValue(lastPart, dictionary);
                        return CachedValue ??= @operator == "AND" ? (ushort)(valueA & valueB) : (ushort)(valueA | valueB);
                    }
                    else if (@operator == "LSHIFT" || @operator == "RSHIFT")
                    {
                        ushort amount = ushort.Parse(lastPart);
                        return CachedValue ??= @operator == "LSHIFT" ? (ushort)(valueA << amount) : (ushort)(valueA >> amount);
                    }
                    else throw new Exception();
                default:
                    throw new Exception();
            }
        }
    }

    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 7;
        public override int GetCurrentPart => 1;
        private readonly string WireNameToFind = "a";
        private readonly string? WireNameToOverride = "b";

        public override void Run()
        {
            string[] lines = File.ReadAllLines(GetInputFilePath(GetCurrentPart));
            Dictionary<string, Operation> operationDictionary = lines.Select(line => new Operation(line)).ToDictionary(o => o.WireName);

            IOrderedEnumerable<string> orderedKeys = operationDictionary.Keys.OrderBy(key => key.Length).ThenBy(x => x);
            Console.WriteLine($" > Distinct output names: {operationDictionary.Keys.Distinct().Count()} ({string.Join(", ", orderedKeys)})\n");
            foreach (string key in orderedKeys)
                Console.WriteLine($" - Wire {key,2} = {operationDictionary[key].DetermineValue(operationDictionary),5} // {operationDictionary[key].Code}");

            ushort valueOfWireToFind = operationDictionary[WireNameToFind].DetermineValue(operationDictionary);
            Console.WriteLine($"\n > Result (input file {GetCurrentPart}): value of '{WireNameToFind}' is {valueOfWireToFind}");

            // part 2
            if (WireNameToOverride is not null)
            {
                int toFindIndex = lines.ToList().FindIndex(line => line.EndsWith($" -> {WireNameToOverride}"));
                lines[toFindIndex] = $"{valueOfWireToFind} -> {WireNameToOverride}";
                operationDictionary = lines.Select(line => new Operation(line)).ToDictionary(o => o.WireName);
                ushort newValueOfWireToFind = operationDictionary[WireNameToFind].DetermineValue(operationDictionary);
                Console.WriteLine($"\n > Result (part 2): the NEW value of '{WireNameToFind}' is {newValueOfWireToFind}");
            }
        }
    }
}