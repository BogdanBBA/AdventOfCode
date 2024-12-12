// using System.Text.RegularExpressions;

// namespace AoC2015.Day07
// {
//     using OperationDictionary = Dictionary<string, Operation>;
//     using ResultDictionary = Dictionary<string, ushort>;

//     public abstract class Operation(string outputName)
//     {

//         public string OutputName { get; private set; } = outputName;

//         public abstract ushort GetValue(OperationDictionary dictionary, ResultDictionary resultsDictionary);

//         protected static void Log(string message)
//             => Console.WriteLine(message);
//     }

//     public class ConstantInputOperation(string outputName, ushort number)
//         : Operation(outputName)
//     {
//         private readonly ushort _number = number;

//         public override ushort GetValue(OperationDictionary dictionary, ResultDictionary resultsDictionary)
//             => _number;
//     }

//     public abstract class OneReferenceOperation(string outputName, string reference)
//         : Operation(outputName)
//     {
//         protected static ushort GetReferenceValue(OperationDictionary dictionary, string name, ResultDictionary resultsDictionary)
//         {
//             return !dictionary.ContainsKey(name) && ushort.TryParse(name, out ushort number) ? number : dictionary[name].GetValue(dictionary, resultsDictionary);
//         }

//         protected readonly string _reference = reference;

//         protected ushort ReferenceAndCacheValue(OperationDictionary dictionary, ResultDictionary resultsDictionary)
//             => GetReferenceValue(dictionary, _reference, resultsDictionary);
//     }

//     public class DirectReferenceOperation(string outputName, string reference)
//         : OneReferenceOperation(outputName, reference)
//     {
//         public override ushort GetValue(OperationDictionary dictionary, ResultDictionary resultsDictionary)
//         {
//             if (_cachedValue is not null) return _cachedValue.Value;
//             ushort v0 = ReferenceAndCacheValue(dictionary, resultsDictionary);
//             ushort result = v0;
//             Log($"{_reference}={v0} -> {OutputName}={result}");
//             return result;
//         }
//     }

//     public class DirectReferenceNotOperation(string outputName, string reference)
//         : OneReferenceOperation(outputName, reference)
//     {
//         public override ushort GetValue(OperationDictionary dictionary, ResultDictionary resultsDictionary)
//         {
//             if (_cachedValue is not null) return _cachedValue.Value;
//             ushort v0 = ReferenceAndCacheValue(dictionary, resultsDictionary);
//             ushort result = (ushort)~v0;
//             Log($"{_reference}={v0} -> {OutputName}={result}");
//             return result;
//         }
//     }

//     public class ShiftReferenceOperation(string outputName, string reference, bool rightShift, ushort amount)
//         : OneReferenceOperation(outputName, reference)
//     {
//         private readonly bool _rightShift = rightShift;
//         private readonly ushort _amount = amount;

//         public override ushort GetValue(OperationDictionary dictionary, ResultDictionary resultsDictionary)
//         {
//             if (_cachedValue is not null) return _cachedValue.Value;
//             ushort v0 = ReferenceAndCacheValue(dictionary, resultsDictionary);
//             ushort result = (ushort)(_rightShift ? v0 >> _amount : v0 << _amount);
//             Log($"{_reference}={v0} {(_rightShift ? "RSHIFT" : "LSHIFT")} {_amount} -> {OutputName}={result}");
//             return result;
//         }
//     }

//     public class BinaryOperation(string outputName, string referenceA, string referenceB, bool operationIsAnd)
//         : OneReferenceOperation(outputName, referenceA)
//     {
//         private readonly string _otherReference = referenceB;
//         private readonly bool _operationIsAnd = operationIsAnd;

//         public override ushort GetValue(OperationDictionary dictionary, ResultDictionary resultsDictionary)
//         {
//             if (_cachedValue is not null) return _cachedValue.Value;
//             ushort v0 = GetReferenceValue(dictionary, _reference, resultsDictionary);
//             ushort v1 = GetReferenceValue(dictionary, _otherReference, resultsDictionary);
//             ushort result = (ushort)(_operationIsAnd ? v0 & v1 : v0 | v1);
//             Log($"{_reference}={v0} {(_operationIsAnd ? "AND" : "OR")} {_otherReference}={v1} -> {OutputName}={result}");
//             Thread.Sleep(50);
//             return result;
//         }
//     }

//     public static class OperationFactory
//     {
//         private static Operation CreateOperation(string operationText, string output)
//         {
//             int spaces = operationText.Count(ch => ch == ' ');
//             switch (spaces)
//             {
//                 case 0:
//                     return ushort.TryParse(operationText, out ushort number)
//                         ? new ConstantInputOperation(output, number)
//                         : new DirectReferenceOperation(output, operationText);
//                 case 1:
//                     string secondPart = operationText.Split(' ')[1];
//                     return new DirectReferenceNotOperation(output, secondPart);
//                 case 2:
//                     string[] parts = operationText.Split(' ');
//                     return parts[1] switch
//                     {
//                         "LSHIFT" => new ShiftReferenceOperation(output, parts[0], false, ushort.Parse(parts[2])),
//                         "RSHIFT" => new ShiftReferenceOperation(output, parts[0], true, ushort.Parse(parts[2])),
//                         "OR" => new BinaryOperation(output, parts[0], parts[2], false),
//                         "AND" => new BinaryOperation(output, parts[0], parts[2], true),
//                         _ => throw new Exception()
//                     };
//                 default:
//                     throw new Exception();
//             }
//         }

//         public static Operation Create(string line)
//         {
//             Match match = Regex.Match(line, @"(.+) -> (\w+)");
//             (string operationText, string output) = (match.Groups[1].Value, match.Groups[2].Value);
//             return CreateOperation(operationText, output);
//         }

//         public static OperationDictionary Create(string[] lines)
//             => lines.Select(Create).ToDictionary(g => g.OutputName);
//     }

//     public class Program : IDayProgram
//     {
//         public override int GetCurrentDay => 7;
//         public override int GetCurrentPart => 0;
//         private readonly string toFind = "h";

//         public override void Run()
//         {
//             string[] lines = File.ReadAllLines(GetInputFilePath(GetCurrentPart));
//             OperationDictionary operationDictionary = OperationFactory.Create(lines);
//             ResultDictionary resultsDictionary = [];

//             Console.WriteLine($" // distinct output names: {operationDictionary.Keys.Distinct().Count()}");
//             Console.WriteLine($" > Result (input file {GetCurrentPart}): value of '{toFind}' is {operationDictionary[toFind].GetValue(operationDictionary, resultsDictionary)} ");
//         }
//     }
// }