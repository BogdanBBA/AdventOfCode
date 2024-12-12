using System.Text.RegularExpressions;

namespace AoC2015.Day15
{
    public static class Utils
    {
        public static int GroupAsInt(this Match match, int groupIndex)
            => int.Parse(match.Groups[groupIndex].Value);
    }

    public class Ingredient(string name, int capacity, int durability, int flavor, int texture, int calories)
    {
        public string Name { get; private set; } = name;
        public int Capacity { get; private set; } = capacity;
        public int Durability { get; private set; } = durability;
        public int Flavor { get; private set; } = flavor;
        public int Texture { get; private set; } = texture;
        public int Calories { get; private set; } = calories;

        public override string ToString()
            => $"{Name}: capacity {Capacity}, durability {Durability}, flavor {Flavor}, texture {Texture}, calories {Calories}";
    }

    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 15;
        public override int GetCurrentPart => 1;

        private static Ingredient ParseIngredient(string line)
        {
            Match match = Regex.Match(line, @"(\w+): capacity ([\-\d]+), durability ([\-\d]+), flavor ([\-\d]+), texture ([\-\d]+), calories ([\-\d]+)");
            return new(match.Groups[1].Value, match.GroupAsInt(2), match.GroupAsInt(3), match.GroupAsInt(4), match.GroupAsInt(5), match.GroupAsInt(6));
        }

        private static int CalculateCookieScore(List<(Ingredient ingredient, int amount)> ingredientCounts)
        {
            int totalCapacity = 0, totalDurability = 0, totalFlavor = 0, totalTexture = 0;
            for (int index = 0; index < ingredientCounts.Count; index++)
            {
                totalCapacity += ingredientCounts[index].amount * ingredientCounts[index].ingredient.Capacity;
                totalDurability += ingredientCounts[index].amount * ingredientCounts[index].ingredient.Durability;
                totalFlavor += ingredientCounts[index].amount * ingredientCounts[index].ingredient.Flavor;
                totalTexture += ingredientCounts[index].amount * ingredientCounts[index].ingredient.Texture;
            }
            return Math.Max(0, totalCapacity) * Math.Max(0, totalDurability) * Math.Max(0, totalFlavor) * Math.Max(0, totalTexture);
        }

        private static int CalculateCookieCalories(List<(Ingredient ingredient, int amount)> ingredientCounts)
        {
            return Math.Max(0, ingredientCounts.Sum(ic => ic.amount * ic.ingredient.Calories));
        }

        private static void Solve(Ingredient[] ingredients)
        {
            List<List<int>> partitions = UtilsOther.GeneratePartitions(ingredients.Length, 100);
            List<List<(Ingredient, int)>> ingredientCountsList = partitions.Select(amounts => Enumerable.Range(0, amounts.Count).Select(index => (ingredients[index], amounts[index])).ToList()).ToList();
            List<int> scores = ingredientCountsList.Select(CalculateCookieScore).ToList();
            List<int> calories = ingredientCountsList.Select(CalculateCookieCalories).ToList();
            int maxScoreIndex = scores.IndexOf(scores.Max());
            List<int> indexesOf500Calories = Enumerable.Range(0, calories.Count).Where(index => scores[index] > 0 && calories[index] == 500).ToList();
            int indexOfMaxScoreWith500Calorie = indexesOf500Calories.Select(index => (index, scores[index])).OrderByDescending(pair => pair.Item2).First().index;
            
            // Console.WriteLine(string.Join("\n", Enumerable.Range(0, partitions.Count).Where(index => scores[index] > 0).Select(index =>
            //     $"{string.Join(", ", partitions[index].Select(count => $"{count,3:N0}"))}   ({calories[index],3:N0})   =   {scores[index],11:N0}{(index == maxScoreIndex ? "   * max" : "")}")));
            Console.WriteLine(string.Join("\n", Enumerable.Range(0, partitions.Count).Where(index => scores[index] > 0 && calories[index] == 500).Select(index =>
                $"{string.Join(", ", partitions[index].Select(count => $"{count,3:N0}"))}   ({calories[index],3:N0})   =   {scores[index],11:N0}")));
            Console.WriteLine($"\n > Best score is {scores[maxScoreIndex]:N0} ({scores[maxScoreIndex]}) (using {string.Join(", ", partitions[maxScoreIndex])}).");
            Console.WriteLine($"\n > Best score with 500 calories is {scores[indexOfMaxScoreWith500Calorie]:N0} ({scores[indexOfMaxScoreWith500Calorie]}) (using {string.Join(", ", partitions[maxScoreIndex])}).");
        }

        public override void Run()
        {
            string[] lines = File.ReadAllLines(GetInputFilePath(GetCurrentPart));
            Ingredient[] ingredients = lines.Select(ParseIngredient).ToArray();
            Console.WriteLine($" > Ingredients ({ingredients.Length}):\n{string.Join("\n", ingredients.Select(i => $"   - {i}"))}\n");

            Solve(ingredients);
        }
    }
}