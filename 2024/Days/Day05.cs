using AoC2024.ForDay05;

namespace AoC2024
{
    namespace ForDay05
    {
        public class PageOrderingRule(List<int> pageNumbers)
        {
            public int A { get; } = pageNumbers[0];
            public int B { get; } = pageNumbers[1];
        }

        public class Order(List<int> pageNumbers)
        {
            public List<int> Numbers { get; } = pageNumbers;

            public Order(Order other) : this([.. other.Numbers]) { }

            public bool CouldApply(PageOrderingRule orderingRule)
                => Numbers.Contains(orderingRule.A) && Numbers.Contains(orderingRule.B);

            public List<PageOrderingRule> FilterRulesWhichCouldApply(List<PageOrderingRule> orderingRules)
                => orderingRules.Where(CouldApply).ToList();

            public bool Applies(PageOrderingRule orderingRule)
                => Numbers.IndexOf(orderingRule.A) < Numbers.IndexOf(orderingRule.B);

            public bool IsCorrect(List<PageOrderingRule> orderingRules)
                => FilterRulesWhichCouldApply(orderingRules).All(Applies);

            public List<PageOrderingRule> FilterRulesWhichDontApply(List<PageOrderingRule> orderingRules)
                => FilterRulesWhichCouldApply(orderingRules).Where(rule => !Applies(rule)).ToList();

            public int GetMiddleNumber()
                => Numbers[Numbers.Count / 2];

            public void FixForRule(PageOrderingRule orderingRule)
            {
                (int indexA, int indexB) = (Numbers.IndexOf(orderingRule.A), Numbers.IndexOf(orderingRule.B));
                (Numbers[indexA], Numbers[indexB]) = (Numbers[indexB], Numbers[indexA]);
            }

            public override string ToString()
                => string.Join(',', Numbers);
        }

        public class Database
        {
            public List<PageOrderingRule> PageOrderingRules { get; } = [];
            public List<Order> Orders { get; } = [];

            public (List<Order>, List<Order>) ClassifyOrderedUpdates(bool logCorrectOnes)
            {
                List<Order> correct = [], incorrect = [];
                foreach (Order order in Orders)
                {
                    if (order.IsCorrect(PageOrderingRules))
                        correct.Add(order);
                    else
                        incorrect.Add(order);
                }
                if (logCorrectOnes) correct.ForEach(seq => $" - {string.Join(", ", seq.Numbers)}".Log());
                return (correct, incorrect);
            }

            public List<Order> FixOrders(List<Order> orders, bool logging)
            {
                List<Order> result = [];

                foreach (Order originalOrder in orders)
                {
                    Order order = new(originalOrder);
                    $" - fixing order ({order})...".LogNNL();
                    List<PageOrderingRule> dontApply = order.FilterRulesWhichDontApply(PageOrderingRules); ;
                    while (dontApply.Count > 0)
                    {
                        order.FixForRule(dontApply.First());
                        dontApply = order.FilterRulesWhichDontApply(PageOrderingRules);
                    }
                    $" done ({order}).".Log();
                    result.Add(order);
                }
                return result;
            }
        }

        public static class ExtensionMethods
        {
            public static List<int> GetMiddleNumbers(this List<Order> orders)
                => orders.Select(order => order.GetMiddleNumber()).ToList();
        }
    }

    public class Day05 : DayProgram
    {
        private static Database ParseInput(string[] lines)
        {
            Database database = new();
            foreach (string line in lines)
            {
                if (line.Contains('|'))
                    database.PageOrderingRules.Add(new PageOrderingRule(line.Split('|').Select(int.Parse).ToList()));
                else if (line.Contains(','))
                    database.Orders.Add(new Order(line.Split(',').Select(int.Parse).ToList()));
            }
            return database;
        }

        public override void Run()
        {
            Database database = ParseInput(ReadFromFile_Strings(@"05"));
            $" > Database contains {database.PageOrderingRules.Count} page ordering rules and {database.Orders.Count} number sequences.".Log();

            (List<Order> correct, List<Order> incorrect) = database.ClassifyOrderedUpdates(false);

            List<int> correctMiddleNumbers = correct.GetMiddleNumbers();
            $" > There are {correct.Count} correctly ordered updates, whose middle numbers ({string.Join(", ", correctMiddleNumbers)}) sum up to {correctMiddleNumbers.Sum()}.".Log();

            List<Order> fixedOrders = database.FixOrders(incorrect, true);
            List<int> fixedMiddleNumbers = fixedOrders.GetMiddleNumbers();
            $" > The {fixedOrders.Count} incorrect orders, after being fixed, have the sum of their middle numbers ({string.Join(", ", fixedMiddleNumbers)}) sum up to {fixedMiddleNumbers.Sum()}.".Log();
        }
    }
}