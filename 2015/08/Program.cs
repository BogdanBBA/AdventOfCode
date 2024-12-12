using System.Text;

namespace AoC2015.Day08
{
    public class MyString(string line)
    {
        public string Line { get; private set; } = line;

        public int GetNumberOfCharactersOfCodeForStringLiterals()
            => Line.Length;

        public int GetNumberOfCharactersInMemoryForTheValuesOfTheStrings()
        {
            if (!Line.StartsWith('"') || !Line.EndsWith('"')) throw new Exception();
            int size = 0;
            for (int index = 1; index < Line.Length - 1; index++)
            {
                char ch = Line[index];
                if (ch == '\\')
                {
                    char next = Line[index + 1];
                    if (next == '\\' || next == '"')
                        index++;
                    else if (next == 'x')
                        index += 3;
                    else throw new Exception();
                }
                size++;
            }
            return size;
        }

        public MyString Encode()
        {
            const int cQuote = 34;
            const int cApostrophe = 39;
            const int cSlash = 92;

            string lineToWorkOn = $"\"{Line}\"";
            StringBuilder newLine = new();
            for (int index = 0; index < lineToWorkOn.Length - 1;)
            {
                char current = lineToWorkOn[index];
                if (current != cSlash)
                {
                    newLine.Append(current);
                    index += 1; // don't skip next
                }
                else
                {
                    char next = lineToWorkOn[index + 1];
                    if (next == cQuote)
                    {
                        newLine.Append(@$"\\\""");
                        index += 2; // skip next
                    }
                    else if (next == cApostrophe)
                    {
                        newLine.Append(@$"\\\\");
                        index += 2; // skip next
                    }
                    else if (next == 'x')
                    {
                        newLine.Append(@$"\\{next}{lineToWorkOn.Substring(index + 2, 2)}");
                        index += 4;
                    }
                    else throw new Exception();
                }
            }
            return new(newLine.Insert(0, '"').Append('"').ToString());
        }
    }

    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 8;
        public override int GetCurrentPart => 0;

        public override void Run()
        {
            string[] lines = File.ReadAllLines(GetInputFilePath(GetCurrentPart));
            List<MyString> myStrings = lines.Select(line => new MyString(line)).ToList();

            Console.WriteLine(string.Join("\n", myStrings.Select(s => $"{s.Line,45} -> code {s.GetNumberOfCharactersOfCodeForStringLiterals(),3}, memory {s.GetNumberOfCharactersInMemoryForTheValuesOfTheStrings(),3} ... {s.Encode().Line,60} -> code {s.Encode().GetNumberOfCharactersOfCodeForStringLiterals(),3}")));

            int forCodeSum_ = myStrings.Sum(s => s.GetNumberOfCharactersOfCodeForStringLiterals());
            int inMemorySum = myStrings.Sum(s => s.GetNumberOfCharactersInMemoryForTheValuesOfTheStrings());
            Console.WriteLine($" > Result (part 1): for the {myStrings.Count} strings, {forCodeSum_} - {inMemorySum} = {forCodeSum_ - inMemorySum}");

            List<MyString> encoded = myStrings.Select(s => s.Encode()).ToList();
            forCodeSum_ = encoded.Sum(s => s.GetNumberOfCharactersOfCodeForStringLiterals());
            inMemorySum = encoded.Sum(s => s.GetNumberOfCharactersInMemoryForTheValuesOfTheStrings());
            Console.WriteLine($" > Result (part 2): for the {encoded.Count} strings, {forCodeSum_} - {inMemorySum} = {forCodeSum_ - inMemorySum}");
        }
    }
}