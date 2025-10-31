using System;
using System.Collections.Generic;
using System.Linq;
using PokemonBattle.Type;


public static class TypeHelper
{
    private static readonly PokemonType[] AllTypes =
        (PokemonType[])Enum.GetValues(typeof(PokemonType));

    public static double GetEffectiveness(PokemonType attacker, PokemonType defender)
    {
        return TypeChart.GetMultiplier(attacker, defender);
    }

    public static List<PokemonType> GetStrongAgainst(PokemonType attacker)
    {
        return AllTypes
            .Where(def => TypeChart.GetMultiplier(attacker, def) > 1.0)
            .ToList();
    }

    public static List<PokemonType> GetNotVeryEffectiveAgainst(PokemonType attacker)
    {
        return AllTypes
            .Where(def => TypeChart.GetMultiplier(attacker, def) < 1.0)
            .ToList();
    }

    public static List<PokemonType> GetWeakAgainst(PokemonType defender)
    {
        return AllTypes
            .Where(atk => TypeChart.GetMultiplier(atk, defender) > 1.0)
            .ToList();
    }

    public static List<PokemonType> GetResistantAgainst(PokemonType defender)
    {
        return AllTypes
            .Where(atk => TypeChart.GetMultiplier(atk, defender) < 1.0)
            .ToList();
    }

    public static void ShowTypeSummary(PokemonType type)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"=== {type} ===");
        Console.ResetColor();

        Console.WriteLine($"➡️ Fort contre :     {FormatList(GetStrongAgainst(type))}");
        Console.WriteLine($"➡️ Pas très efficace contre : {FormatList(GetNotVeryEffectiveAgainst(type))}");
        Console.WriteLine($"❌ Faible face à :   {FormatList(GetWeakAgainst(type))}");
        Console.WriteLine($"🛡️ Résiste à :       {FormatList(GetResistantAgainst(type))}");
        Console.WriteLine();
    }

    private static string FormatList(List<PokemonType> list)
    {
        return list.Count > 0 ? string.Join(", ", list) : "Aucun";
    }

    public static void ShowFullMatrix()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n===== TABLEAU DES COEFFICIENTS =====");
        Console.ResetColor();

        var types = AllTypes;

        Console.Write("Att\\Def |");
        foreach (var def in types)
            Console.Write($"{def,8}");
        Console.WriteLine("\n" + new string('-', 10 + 8 * types.Length));

        foreach (var atk in types)
        {
            Console.Write($"{atk,8}|");
            foreach (var def in types)
            {
                var coeff = TypeChart.GetMultiplier(atk, def);
                Console.Write($"{coeff,8:0.##}");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}
