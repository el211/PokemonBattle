using PokemonBattle.Type;

namespace PokemonBattle;

public static class DamageCalculator
{
    public static int ComputeDamage(int baseDamage, PokemonType attacker, PokemonType defender)
    {
        double mult = TypeChart.GetMultiplier(attacker, defender);
        return Math.Max(1, (int)Math.Round(baseDamage * mult));
    }
}
