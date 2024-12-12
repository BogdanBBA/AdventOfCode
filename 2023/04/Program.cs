namespace AoC04
{
    internal class Card(int id, List<int> winningNumbers, List<int> myNumbers)
    {
        public int ID { get; } = id;
        public List<int> WinningNumbers { get; } = winningNumbers;
        public List<int> MyNumbers { get; } = myNumbers;

        public override string ToString() => $"Card {ID}";
    }

    internal class Program
    {
        private static Card ParseLine(string line, int id)
        {
            string[] parts = line.Split(':')[1].Split('|');
            List<int> winning = parts[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            List<int> mine = parts[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            return new(id, winning, mine);
        }

        private static int GetPoints(Card card)
            => (int)Math.Pow(2, card.MyNumbers.Count(card.WinningNumbers.Contains) - 1);

        private static int GetWinningCardCount(Card card)
            => card.WinningNumbers.Count(card.MyNumbers.Contains);

        private static List<Card> GetWinningCards(Card card, List<Card> originalCards)
        {
            int myWinnindCardCount = GetWinningCardCount(card);
            return originalCards.Skip(originalCards.IndexOf(card) + 1).Take(myWinnindCardCount).ToList();
        }

        public static int ProcessCards(List<Card> originalCards)
        {
            List<Card> cards = new(originalCards);
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];
                List<Card> newCards = GetWinningCards(card, originalCards);
                cards.AddRange(newCards);
            }
            return cards.Count;
        }

        private static void Main(string[] _)
        {
            string[] lines = File.ReadAllLines(@"input.txt");
            List<Card> cards = Enumerable.Range(0, lines.Length).Select(index => ParseLine(lines[index], index + 1)).ToList();
            Console.WriteLine($"The total amount of points of the cards is: {cards.Select(GetPoints).Sum()} (incorrect!)");
            Console.WriteLine($"As for the more correct processing of the cards, the result (total amount of cards) is: {ProcessCards(cards)}");
        }
    }
}