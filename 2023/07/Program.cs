using System.Security.Cryptography;
using System.Text;

internal class Program
{
    internal enum Card { _A = 14, _K = 13, _Q = 12, _T = 10, _9 = 9, _8 = 8, _7 = 7, _6 = 6, _5 = 5, _4 = 4, _3 = 3, _2 = 2, _J = -999 }

    internal enum HandType { FiveOfAKind = 7, FourOfAKind = 6, FullHouse = 5, ThreeOfAKind = 4, TwoPair = 3, OnePair = 2, HighCard = 1 }

    internal class Hand(string text)
    {
        private static readonly List<Card> NonJCards = Enum.GetValues(typeof(Card)).Cast<Card>().Where(card => card != Card._J).ToList();

        public List<Card> Cards { get; } = text.Select(character => (Card)Enum.Parse(typeof(Card), $"_{character}")).ToList();

        private void PopulateAllJokeredHandsInner(ref List<Hand> hands)
        {
            for (bool go = true; go;)
            {
                go = false;
                for (int iHand = 0; iHand < hands.Count; iHand++)
                {
                    Hand hand = hands[iHand];
                    int jIndex = hand.Cards.IndexOf(Card._J);
                    if (jIndex != -1)
                    {
                        char[] chars = hand.ToString().ToCharArray();
                        Hand[] newHands = NonJCards.Select(nonJCard =>
                        {
                            chars[jIndex] = nonJCard.ToString()[1];
                            return new Hand(new string(chars));
                        }).ToArray();
                        hands.AddRange(newHands);
                        hands.RemoveAt(iHand--);
                        go = true;
                    }
                }
            }
        }

        private List<Hand> GetAllJokeredHands()
        {
            List<Hand> result = [this];
            PopulateAllJokeredHandsInner(ref result);
            return result;
        }

        public HandType DetermineHandType(bool withAdditionalJRule)
        {
            List<(Card Card, int Frequency)> frequencies = [.. Cards.GroupBy(card => card)
                .Select(grouping => (grouping.Key, grouping.Count()))
                .OrderByDescending(pair => pair.Item2)
                .ThenByDescending(pair => (int)pair.Key)];

            if (withAdditionalJRule && frequencies.Any(frequency => frequency.Card == Card._J))
            {
                List<Hand> jokeredHands = GetAllJokeredHands();
                Console.WriteLine($" - Hand {this} -> jokered to {jokeredHands.Count} variants: {string.Join(", ", jokeredHands)}");
                return jokeredHands.Max(hand => hand.DetermineHandType(false));
            }

            int highestFrequency = frequencies.First().Frequency;
            if (frequencies.Count == 1)
            {
                return HandType.FiveOfAKind;
            }
            if (frequencies.Count == 2)
            {
                if (highestFrequency == 4)
                    return HandType.FourOfAKind;
                else
                    return HandType.FullHouse;
            }
            if (frequencies.Count == 3)
            {
                if (highestFrequency == 3)
                    return HandType.ThreeOfAKind;
                else
                    return HandType.TwoPair;
            }
            if (frequencies.Count == 4)
            {
                if (highestFrequency == 2)
                    return HandType.OnePair;
            }
            if (frequencies.Count == 5)
            {
                return HandType.HighCard;
            }
            throw new ArgumentException($"Unknown hand type ({this})!");
        }

        public override string ToString() => string.Join(null, Cards).Replace("_", string.Empty);
    }

    internal class HandComparer(bool withAdditionalJRule) : IComparer<Hand>
    {
        private bool WithAdditionalJRule { get; } = withAdditionalJRule;

        public int Compare(Hand? x, Hand? y)
        {
            if (x is null || y is null) throw new ArgumentException("Null!");

            HandType xAux = x.DetermineHandType(WithAdditionalJRule), yAux = y.DetermineHandType(WithAdditionalJRule);
            if (xAux != yAux) return xAux.CompareTo(yAux);

            for (int index = 0; index < x.Cards.Count; index++)
            {
                if (x.Cards[index] != y.Cards[index])
                {
                    return x.Cards[index].CompareTo(y.Cards[index]);
                }
            }

            return 0;
        }
    }

    internal class Game(string inputFile)
    {
        public List<(Hand Hand, int BidAmount)> HandBiddings { get; } = File.ReadLines(inputFile).Select(line => (new Hand(line.Split(' ')[0]), int.Parse(line.Split(' ')[1]))).ToList();

        public List<(int Rank, Hand Hand, int BidAmount)> GetSortedHandBiddings(bool withAdditionalJRule)
        {
            HandComparer cardComparer = new(withAdditionalJRule);
            List<(Hand Hand, int BidAmount)> sortedHandBiddings = [.. HandBiddings.OrderBy(bidding => bidding.Hand, cardComparer)];
            return Enumerable.Range(0, sortedHandBiddings.Count)
                .Select(index => (index + 1, sortedHandBiddings[index].Hand, sortedHandBiddings[index].BidAmount))
                .OrderByDescending(bidding => bidding.Item1)
                .ToList();
        }
    }

    private static void Main(string[] _)
    {
        bool withAdditionalJRule = true;
        bool useFullInput = true;

        Game game = new(useFullInput ? "input1.txt" : "input0.txt");
        Console.WriteLine($" > The game's {game.HandBiddings.Count} hand biddings: {string.Join(", ", game.HandBiddings.Select(bidding => $"{bidding.Hand}={bidding.BidAmount}"))}.");

        List<(int Rank, Hand Hand, int BidAmount)> sorted = game.GetSortedHandBiddings(withAdditionalJRule);
        Console.WriteLine($"\n >The sorted hand biddings are:\n{string.Join("\n", sorted.Select(bidding => $" - {bidding.Rank}. {bidding.Hand} ({bidding.BidAmount})"))}");
        Console.WriteLine($"The total winnings are: {sorted.Select(bidding => bidding.Rank * bidding.BidAmount).Sum()}");
    }
}