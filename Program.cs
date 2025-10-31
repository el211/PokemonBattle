using System;
using PokemonBattle.Type;

public class Program
{
    public static void Main()
    {
        var salameche = new Pokemon("Salamèche", PokemonType.Feu, 200);
        var carapuce  = new Pokemon("Carapuce",  PokemonType.Eau, 120);
        var loop = new BattleLoop(salameche, carapuce, baseDamageFirst: 10, baseDamageSecond: 25);
        loop.Run();
        double coeff = TypeChart.GetMultiplier(carapuce.Type, salameche.Type);
        Say($"[COEFFICIANT] {carapuce.Type} -> {salameche.Type} = x{coeff:0.##}", ConsoleColor.Magenta);

        Say("Carapuce attaque !", ConsoleColor.Cyan);
        carapuce.Attack(salameche, baseDamage: 25);

        Say("Salameche tente de riposter...", ConsoleColor.Cyan);
        salameche.Attack(carapuce, baseDamage: 10);

        Say("Carapuce a fini le combat :", ConsoleColor.Cyan);
        carapuce.Attack(salameche, baseDamage: 30);

        if (salameche.HitPoints <= 0)
            Say($"{salameche.Name} est K.O.  combat terminé !", ConsoleColor.Red);
        else
            Say($"{salameche.Name} tien encore debout avec {salameche.HitPoints} PV.", ConsoleColor.Green);

        if (carapuce.HitPoints <= 0)
            Say($"{carapuce.Name} est K.O.", ConsoleColor.Red);
        else
            Say($"{carapuce.Name} a {carapuce.HitPoints} PV restant encore.", ConsoleColor.Green);
    }

    private static void Say(string msg, ConsoleColor color)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ForegroundColor = prev;
    }
}
