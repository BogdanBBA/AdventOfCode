using System.Globalization;
using System.Text.RegularExpressions;

namespace AoC15
{
    public static class Utils
    {
        public static byte CalculateHash(string value)
        {
            byte current = 0;
            for (int index = 0; index < value.Length; index++)
                current = (byte)((current + value[index]) * 17 % 256);
            return current;
        }
    }

    public class LensSlot(int boxNumber, int slotNumber, string label, int focalLength)
    {
        public int BoxNumber { get; private set; } = boxNumber;
        public int SlotNumber { get; set; } = slotNumber;
        public string Label { get; private set; } = label;
        public int FocalLength { get; private set; } = focalLength;

        public int FocusingPower
            => (1 + BoxNumber) * SlotNumber * FocalLength;

        public void Set(string label, int focalLength)
        {
            Label = label;
            FocalLength = focalLength;
        }

        public override string ToString()
            => $"[{Label} {FocalLength}]={FocusingPower}";
    }

    public class Box(int index)
    {
        public int Index { get; private set; } = index;
        public List<LensSlot> Lenses { get; private set; } = [];

        public LensSlot? GetLensSlot(string label)
            => Lenses.FirstOrDefault(lens => lens?.Label == label);

        public void ProcessStep(Step step, bool log = false)
        {
            if (Index != step.LetterHash)
                throw new ArgumentException($"Box index {Index} != step letter-hash {step.LetterHash}");

            string label = step.Letters;
            LensSlot? slot = GetLensSlot(label);

            if (step.IsRemove)
            {
                if (slot is not null)
                {
                    Lenses.Remove(slot);
                    for (int iSlot = 0; iSlot < Lenses.Count; iSlot++)
                        Lenses[iSlot].SlotNumber = iSlot + 1;
                }
            }
            else
            {
                int focalLength = step.Number!.Value;
                if (slot is not null)
                {
                    slot.Set(label, focalLength);
                }
                else
                {
                    Lenses.Add(new(Index, Lenses.Count + 1, label, focalLength));
                }
            }
            if (log) Console.WriteLine($" - Step \"{step.OriginalValue,-4}\" - {this}");
        }

        public override string ToString()
            => $"Box {Index,3}: {string.Join(' ', Lenses)}";
    }

    public class Step
    {
        public string OriginalValue { get; private set; }
        public string Letters { get; private set; }
        public int FullHash { get; private set; }
        public int LetterHash { get; private set; }
        public bool IsRemove { get; private set; }
        public int? Number { get; private set; }

        public Step(string originalValue)
        {
            OriginalValue = originalValue;
            Letters = Regex.Match(originalValue, @"(\w+)").Groups[1].Value;
            FullHash = Utils.CalculateHash(OriginalValue);
            LetterHash = Utils.CalculateHash(Letters);
            IsRemove = originalValue[Letters.Length] == '-';
            Number = IsRemove ? null : int.Parse(originalValue[(Letters.Length + 1)..]);
        }

        public override string ToString()
            => $"'{OriginalValue}' -> '{Letters}'(={LetterHash}) remove:{IsRemove} {(Number is null ? "(null)" : Number.ToString())} = {FullHash}";
    }

    public class InstructionsFile(string inputFile)
    {
        public List<Step> Steps { get; private set; } = File.ReadAllText(inputFile).Split(',').Select(step => new Step(step)).ToList();

        public void CalculateAndPrintFullHashSum(bool printStepHashes)
        {
            Console.WriteLine($" > Instructions file '{inputFile}'");
            if (printStepHashes)
                Console.WriteLine($" - Steps and hashes ({Steps.Count}): {string.Join("; ", Steps.Select(step => $"\"{step.OriginalValue}\"={step.FullHash}"))}");
            Console.WriteLine($" > Sum of the {Steps.Count} full hashes: {Steps.Sum(step => step.FullHash)}");
        }

        public void ProcessSteps(List<Box> boxes)
        {
            Console.WriteLine($" > Processing the {Steps.Count} steps...");
            foreach (Step step in Steps)
                boxes[step.LetterHash].ProcessStep(step, true);
        }
    }

    public class Program
    {
        private const string InputFile0 = "input0.txt";
        private const string InputFile1 = "input1.txt";

        private static void Print(List<Box> boxes, string title)
        {
            Box[] boxesWithLenses = boxes.Where(box => box.Lenses.Count != 0).ToArray();
            int lensCount = boxesWithLenses.Sum(box => box.Lenses.Count), totalFocusingPower = 0;

            Console.WriteLine($" > {title}:");
            foreach (Box box in boxesWithLenses)
            {
                Console.WriteLine($" - {box}");
                totalFocusingPower += box.Lenses.Select(lens => lens.FocusingPower).Sum();
            }
            Console.WriteLine($" > The total focusing power of these {lensCount} lenses (in {boxesWithLenses.Length} boxes) is: {totalFocusingPower}\n");
        }

        private static void Main(string[] _)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            foreach (string inputFile in new[] { InputFile0, InputFile1 })
            {
                List<Box> boxes = Enumerable.Range(0, 256).Select(index => new Box(index)).ToList();

                InstructionsFile instructions = new(inputFile);
                instructions.CalculateAndPrintFullHashSum(true);
                instructions.ProcessSteps(boxes);
                Print(boxes, "After instruction processing");
            }
        }
    }
}