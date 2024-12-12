namespace AoC2015.Day22
{
    public static class Spells
    {
        public static readonly Spell __ENEMY_ATTACK = new(-1, "[ENEMY ATTACK]", 0, 1, (spell, player, enemy, activeSpells) =>
        {
            player.Hitpoints -= enemy.Damage; // doesn't consider defensive spells
        });

        public static readonly Spell MagicMissile = new(0, "Magic missile", 53, 1, (spell, player, enemy, activeSpells) =>
        {
            enemy.Hitpoints -= 4;
        }); // It instantly does 4 damage.

        public static readonly Spell Drain = new(1, "Drain", 73, 1, (spell, player, enemy, activeSpells) =>
        {
            enemy.Hitpoints -= 2;
            player.Hitpoints += 2;
        }); // It instantly does 2 damage and heals you for 2 hit points.

        public static readonly Spell Shield = new(2, "Shield", 113, 6, (spell, player, enemy, activeSpells) =>
        {
            // TODO
        }); // While it is active, your armor is increased by 7.

        public static readonly Spell Poison = new(3, "Poison", 173, 6, (spell, player, enemy, activeSpells) =>
        {
            enemy.Hitpoints -= 3;
        }); // At the start of each turn while it is active, it deals the boss 3 damage.

        public static readonly Spell Recharge = new(4, "Recharge", 229, 5, (spell, player, enemy, activeSpells) =>
        {
            player.Mana += 101;
        }); // At the start of each turn while it is active, it gives you 101 new mana.

        public static readonly Spell[] ALL_PLAYER_SPELLS = [Shield, Recharge, MagicMissile, Drain, Poison];

        public static readonly Dictionary<int, Spell> ALL_PLAYER_SPELLS_BY_ID = ALL_PLAYER_SPELLS.ToDictionary(spell => spell.ID, spell => spell);
    }

    public class Spell(int id, string name, int manaCost, int effectDuration, Action<Spell, PlayerStats, EnemyStats, List<Spell>> applySpell)
    {
        public int ID { get; } = id;
        public string Name { get; } = name;
        public int ManaCost { get; } = manaCost;
        public int EffectDuration { get; } = effectDuration;
        public Action<Spell, PlayerStats, EnemyStats, List<Spell>> ApplySpell { get; } = applySpell;
    }

    public class EnemyStats(int hitpoints, int damage)
    {
        public int Hitpoints { get; set; } = hitpoints;
        public int Damage { get; } = damage;

        public EnemyStats(EnemyStats other) : this(other.Hitpoints, other.Damage) { }
    }

    public class PlayerStats(int hitpoints, int mana)
    {
        public int Hitpoints { get; set; } = hitpoints;
        public int Mana { get; set; } = mana;
        public List<Spell> SpellsCast { get; private set; } = [];

        public PlayerStats(PlayerStats other) : this(other.Hitpoints, other.Mana) { }

        public override string ToString()
            => $"HP={Hitpoints}, mana={Mana}, {(SpellsCast.Count == 1 ? "1 spell" : $"{SpellsCast.Count} spells")} cast ({string.Join(", ", SpellsCast.Select(spell => spell.Name))})";
    }

    public class Game(PlayerStats playerStats, EnemyStats enemyStats, int[] spellIDs)
    {
        public PlayerStats Player { get; } = playerStats;
        public EnemyStats Enemy { get; } = enemyStats;
        public int[] SpellIDs { get; } = spellIDs;

        public (bool PlayerWon, int[] SpellIDsOnInterrupt) Simulate(bool print)
        {
            Spell[] spells = SpellIDs.Select(id => Spells.ALL_PLAYER_SPELLS_BY_ID[id]).ToArray();
            List<Spell> activeSpells = [];
            int iSpell = 0;
            if (print) Console.WriteLine($" --- Player at ({Player.Hitpoints} hp, {Player.Mana} mana), and enemy at ({Enemy.Hitpoints} hp).");

            for (bool playersTurn = true; iSpell < spells.Length && Player.Hitpoints > 0 && Enemy.Hitpoints > 0; playersTurn = !playersTurn)
            {
                if (playersTurn)
                {
                    Spell spell = spells[iSpell++];
                    if (print) Console.Write($" - Player to cast '{spell.Name}' (costs {spell.ManaCost} mana) ... ");
                    if (Player.Mana < spell.ManaCost)
                    {
                        if (print) Console.WriteLine($"player cannot afford the spell! FAIL!");
                        return (false, SpellIDs.Take(iSpell + 1).ToArray());
                    }

                    spell.ApplySpell(spell, Player, Enemy, activeSpells);
                    if (print) Console.Write($"enemy now at ({Enemy.Hitpoints} hp)");
                    if (Enemy.Hitpoints <= 0)
                    {
                        if (print) Console.WriteLine($" ... enemy has died! WIN!");
                        return (true, SpellIDs.Take(iSpell + 1).ToArray());
                    }
                    if (print) Console.WriteLine();
                }
                else
                {
                    if (print) Console.Write($" - Enemy to attack ...");
                    Spells.__ENEMY_ATTACK.ApplySpell(Spells.__ENEMY_ATTACK, Player, Enemy, activeSpells);
                    if (print) Console.Write($"player now at ({Player.Hitpoints} hp, {Player.Mana} mana)");
                    if (Player.Hitpoints <= 0)
                    {
                        if (print) Console.WriteLine($" ... player has died! FAIL!");
                        return (false, SpellIDs.Take(iSpell + 1).ToArray());
                    }
                    if (print) Console.WriteLine();
                }
            }

            return (false, SpellIDs);
        }
    }

    public class Program : IDayProgram
    {
        private static void Solve_Silly(PlayerStats playerStats, EnemyStats enemyStats, int rounds)
        {
            DateTime startMoment = DateTime.Now;
            List<int[]> spellPossibilities = UtilsOther.GenerateDigitPermutations(Spells.ALL_PLAYER_SPELLS.Select(spell => spell.ID).ToArray(), rounds);
            List<int[]> winningSpellIds = [];
            HashSet<int[]> failingSpellIdBeginnings = [];
            int possibilitiesSkipped = 0;
            TimeSpan permutationDuration = DateTime.Now.Subtract(startMoment);

            foreach (int[] spellPossibility in spellPossibilities)
            {
                if (failingSpellIdBeginnings.Any(spellPossibility.StartsWith))
                {
                    possibilitiesSkipped++;
                    continue;
                }

                (bool PlayerWon, int[] SpellIDsOnInterrupt) = new Game(new(playerStats), new(enemyStats), spellPossibility).Simulate(false);
                if (PlayerWon)
                {
                    winningSpellIds.Add(SpellIDsOnInterrupt);
                }
                else
                {
                    failingSpellIdBeginnings.Add(SpellIDsOnInterrupt);
                }
            }

            int[] cheapestWinningSpellSequence = winningSpellIds.MinBy(sequence => sequence.Sum(id => Spells.ALL_PLAYER_SPELLS_BY_ID[id].ManaCost))!;
            Console.WriteLine($" > Processing for {rounds} rounds took {DateTime.Now.Subtract(startMoment).TotalSeconds:N1} seconds (of which the permutation generation took {permutationDuration.TotalSeconds:N1}).");
            Console.WriteLine($" > Out of {spellPossibilities.Count:N0} possibilities, {winningSpellIds.Count:N0} were winning and {spellPossibilities.Count - winningSpellIds.Count:N0} losing (of which {possibilitiesSkipped:N0} not simulated).");
            Console.WriteLine($" > Of the {winningSpellIds.Count:N0} winning spell sequences, the cheapest costs {cheapestWinningSpellSequence.Sum(id => Spells.ALL_PLAYER_SPELLS_BY_ID[id].ManaCost):N0} mana: {string.Join(", ", cheapestWinningSpellSequence.Select(id => Spells.ALL_PLAYER_SPELLS_BY_ID[id].Name))}.");
            new Game(new(playerStats), new(enemyStats), cheapestWinningSpellSequence).Simulate(true);
        }

        public override int GetCurrentDay => 22;
        public override int GetCurrentPart => 0;

        public override void Run()
        {
            const int maxRounds = 5;
            HashSet<int> sumSet = [];
            for (int rounds = 1; rounds <= maxRounds; rounds++)
            {
                List<int[]> ids = UtilsOther.GenerateDigitPermutations(Spells.ALL_PLAYER_SPELLS.Select(spell => spell.ID).ToArray(), rounds);
                List<int> manaSums = ids.Select(sequence => sequence.Sum(id => Spells.ALL_PLAYER_SPELLS_BY_ID[id].ManaCost)).ToList();
                manaSums.ForEach(sum => sumSet.Add(sum));
            }
            Console.WriteLine($" > Possible {sumSet.Count} mana sums (for games up to {maxRounds} rounds) are:\n");
            int[] ordered = [.. sumSet.OrderBy(x => x)];
            for (int index = 0; index < ordered.Length; index++)
                Console.WriteLine($"  [{index,3}]. {ordered[index]}");
            return;

            PlayerStats playerStats = GetCurrentPart == 0 ? new(10, 250) : new(50, 500);
            EnemyStats enemyStats = GetCurrentPart == 0 ? new(13, 8) : new(51, 9);
            Solve_Silly(playerStats, enemyStats, 10);
        }
    }
}