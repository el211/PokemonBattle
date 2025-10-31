namespace PokemonBattle.Type;

public static class TypeChart
{
    private static readonly Dictionary<(PokemonType atk, PokemonType def), double> _table =
        new Dictionary<(PokemonType, PokemonType), double>
        {
            {(PokemonType.Feu,     PokemonType.Plante), 2.0},
            {(PokemonType.Eau,     PokemonType.Feu),    2.0},
            {(PokemonType.Plante,  PokemonType.Eau),    2.0},
            {(PokemonType.Electrik,PokemonType.Eau),    2.0},

            {(PokemonType.Feu,     PokemonType.Eau),    0.5},
            {(PokemonType.Eau,     PokemonType.Plante), 0.5},
            {(PokemonType.Plante,  PokemonType.Feu),    0.5},
        };

    public static double GetMultiplier(PokemonType attacker, PokemonType defender)
        => _table.TryGetValue((attacker, defender), out var m) ? m : 1.0;
}

public static class DamageCalculator
{
    public static int ComputeDamage(int baseDamage, PokemonType attacker, PokemonType defender)
    {
        double mult = TypeChart.GetMultiplier(attacker, defender);
        return Math.Max(1, (int)Math.Round(baseDamage * mult));
    }
}