namespace AoC2015.Day21
{
    public static class StoreItems
    {
        public static readonly Item Dagger = new("Dagger", 8, 4, 0);
        public static readonly Item Shortsword = new("Shortsword", 10, 5, 0);
        public static readonly Item Warhammer = new("Warhammer", 25, 6, 0);
        public static readonly Item Longsword = new("Longsword", 40, 7, 0);
        public static readonly Item Greataxe = new("Greataxe", 74, 8, 0);
        public static readonly Item[] AllWeapons = [Dagger, Shortsword, Warhammer, Longsword, Greataxe];

        public static readonly Item Leather = new("Leather", 13, 0, 1);
        public static readonly Item Chainmail = new("Chainmail", 31, 0, 2);
        public static readonly Item Splintmail = new("Splintmail", 53, 0, 3);
        public static readonly Item Bandedmail = new("Bandedmail", 75, 0, 4);
        public static readonly Item Platemail = new("Platemail", 102, 0, 5);
        public static readonly Item[] AllArmours = [Leather, Chainmail, Splintmail, Bandedmail, Platemail];

        public static readonly Item Damage1 = new("Damage ring 1", 25, 1, 0);
        public static readonly Item Damage2 = new("Damage ring 2", 50, 2, 0);
        public static readonly Item Damage3 = new("Damage ring 3", 100, 3, 0);
        public static readonly Item Defense1 = new("Defense ring 1", 20, 0, 1);
        public static readonly Item Defense2 = new("Defense ring 2", 40, 0, 2);
        public static readonly Item Defense3 = new("Defense ring 3", 80, 0, 3);
        public static readonly Item[] AllRings = [Damage1, Damage2, Damage3, Defense1, Defense2, Defense3];
    }

    public class Item(string name, int cost, int damage, int armour)
    {
        public string Name { get; } = name;
        public int Cost { get; } = cost;
        public int Damage { get; } = damage;
        public int Armour { get; } = armour;
    }

    public interface IEntityStats
    {
        int Hitpoints { get; set; }
        int Damage { get; }
        int Armour { get; }
    }

    public class EnemyStats(int hitpoints, int damage, int armour) : IEntityStats
    {
        public int Hitpoints { get; set; } = hitpoints;
        public int Damage { get; } = damage;
        public int Armour { get; } = armour;
    }

    public class PlayerStats : IEntityStats
    {
        public Item[] AllItems { get; }
        public int Hitpoints { get; set; }
        public int Damage { get; }
        public int Armour { get; }
        public int TotalCost { get; }

        public PlayerStats(int hitpoints, Item weapon, Item? armour, Item?[] rings)
        {
            AllItems = new[] { weapon, armour }.Concat(rings).Where(x => x is not null).Cast<Item>().ToArray();
            Hitpoints = hitpoints;
            Damage = AllItems.Sum(item => item.Damage);
            Armour = AllItems.Sum(item => item.Armour);
            TotalCost = AllItems.Sum(item => item.Cost);
        }

        public PlayerStats(PlayerStats other)
        {
            AllItems = other.AllItems;
            Hitpoints = other.Hitpoints;
            Damage = other.Damage;
            Armour = other.Armour;
            TotalCost = other.TotalCost;
        }

        public override string ToString()
            => $"{string.Join(", ", AllItems.Select(item => item.Name))} (damage {Damage}, armour {Armour}, cost {TotalCost})";
    }

    public class Program : IDayProgram
    {
        private static bool PlayerBeatsEnemy(PlayerStats pPlayer, EnemyStats pEnemy)
        {
            static int CalculateDamage(IEntityStats attacker, IEntityStats defender)
                => Math.Max(1, attacker.Damage - defender.Armour);

            IEntityStats player = new PlayerStats(pPlayer);
            IEntityStats enemy = new EnemyStats(pEnemy.Hitpoints, pEnemy.Damage, pEnemy.Armour);
            for (bool playersTurn = true; player.Hitpoints > 0 && enemy.Hitpoints > 0; playersTurn = !playersTurn)
            {
                if (playersTurn)
                {
                    enemy.Hitpoints -= CalculateDamage(player, enemy);
                }
                else
                {
                    player.Hitpoints -= CalculateDamage(enemy, player);
                }
            }
            return player.Hitpoints > 0;
        }

        private static void Solve_Silly(int playerHitpoints, EnemyStats enemyStats)
        {
            Item?[] possibleArmours = [null, .. StoreItems.AllArmours];
            Item?[] possibleRings = [null, .. StoreItems.AllRings];
            List<PlayerStats> winningPlayers = [], losingPlayers = [];
            int playersSimulated = 0, playersSkipped = 0;

            foreach (Item weapon in StoreItems.AllWeapons)
            {
                foreach (Item? armour in possibleArmours)
                {
                    foreach (Item? ring1 in possibleRings)
                    {
                        foreach (Item? ring2 in possibleRings)
                        {
                            if (ring2 != null && ring2 == ring1)
                            {
                                playersSkipped++;
                                continue;
                            }

                            PlayerStats player = new(playerHitpoints, weapon, armour, [ring1, ring2]);
                            playersSimulated++;
                            if (PlayerBeatsEnemy(player, enemyStats))
                            {
                                winningPlayers.Add(player);
                            }
                            else
                            {
                                losingPlayers.Add(player);
                            }
                        }
                    }
                }
            }

            Console.WriteLine($" > Out of the total of {playersSimulated} simulated players, {playersSkipped} have been skipped and {winningPlayers.Count} are winners.\n");

            PlayerStats mostExpensiveWinner = winningPlayers.MaxBy(player => player.TotalCost)!;
            Console.WriteLine($" - The most expensive winning player is: {mostExpensiveWinner}.");

            PlayerStats cheapestWinner = winningPlayers.MinBy(player => player.TotalCost)!;
            Console.WriteLine($" > The cheapest winning player is: {cheapestWinner}.\n");

            PlayerStats cheapestLoser = losingPlayers.MinBy(player => player.TotalCost)!;
            Console.WriteLine($" - The cheapest losing player is: {cheapestLoser}.");

            PlayerStats mostExpensiveLoser = losingPlayers.MaxBy(player => player.TotalCost)!;
            Console.WriteLine($" > The most expensive losing player is: {mostExpensiveLoser}.");
        }

        public override int GetCurrentDay => 21;
        public override int GetCurrentPart => 1;

        public override void Run()
        {
            const int playerHitpoints = 100;
            EnemyStats enemyStats = GetCurrentPart == 0 ? new(8, 5, 5) : new(103, 9, 2);

            Solve_Silly(playerHitpoints, enemyStats);
        }
    }
}