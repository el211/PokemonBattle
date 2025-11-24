using System;
using System.Linq;
using PokemonBattle;

public class Program
{
    public static void Main()
    {
        // 1) Charger les CSV
        GameDatabase.LoadAll();

        // 2) Afficher tous les Pokémon chargés depuis pokemon.csv
        BattleAnnouncer.Say("=== Pokémon chargés depuis pokemon.csv ===", ConsoleColor.Green);

        foreach (var p in GameDatabase.PokemonById.Values.OrderBy(p => p.Id))
        {
            Console.WriteLine($"{p.Id}: {p.Name} ({p.Type}) — Attaques: {string.Join("|", p.AttackIds)}");
        }

        Console.WriteLine();
        BattleAnnouncer.Say("Appuie sur une touche pour lancer un combat de démo (Pikachu vs Squirtle)...", ConsoleColor.Cyan);
        Console.ReadKey(true);
        Console.WriteLine();

        // 3) Essayer de récupérer Pikachu et Squirtle depuis la base
        if (!GameDatabase.PokemonByName.TryGetValue("Pikachu", out var pikachuData) ||
            !GameDatabase.PokemonByName.TryGetValue("Squirtle", out var squirtleData))
        {
            BattleAnnouncer.Say("Pikachu ou Squirtle introuvable dans le CSV, on prend les deux premiers Pokémon.", ConsoleColor.Red);
            var list = GameDatabase.PokemonById.Values.OrderBy(p => p.Id).Take(2).ToList();

            if (list.Count < 2)
            {
                BattleAnnouncer.Say("Pas assez de Pokémon dans le CSV pour faire un combat.", ConsoleColor.Red);
                return;
            }

            pikachuData = list[0];
            squirtleData = list[1];
        }

        // 4) Créer les objets de combat avec des PV arbitraires
        var pikachu = new Pokemon(pikachuData, hitPoints: 150);
        var squirtle = new Pokemon(squirtleData, hitPoints: 160);

        // 5) Afficher leurs attaques
        BattleAnnouncer.Say($"{pikachu.Name} connaît : {string.Join(", ", pikachu.Attacks.Select(a => a.Name))}", ConsoleColor.Yellow);
        BattleAnnouncer.Say($"{squirtle.Name} connaît : {string.Join(", ", squirtle.Attacks.Select(a => a.Name))}", ConsoleColor.Cyan);
        Console.WriteLine();

        // 6) Utiliser leurs premières attaques pour définir les dégâts de base du BattleLoop
        int baseDamagePikachu = pikachu.Attacks.Count > 0 ? pikachu.Attacks[0].Power : 10;
        int baseDamageSquirtle = squirtle.Attacks.Count > 0 ? squirtle.Attacks[0].Power : 10;
        BattleAnnouncer.Say($"Combat: {pikachu.Name} vs {squirtle.Name}", ConsoleColor.Magenta);

        BattleAnnouncer.Say($"Dégâts de base: {pikachu.Name} = {baseDamagePikachu}, {squirtle.Name} = {baseDamageSquirtle}", ConsoleColor.Magenta);

        
        Console.WriteLine();

        var loop = new BattleLoop(pikachu, squirtle);
        loop.Run();        

        Console.WriteLine();
        BattleAnnouncer.Say("Petit bonus: attaques nommées depuis le CSV :", ConsoleColor.Green);

        if (pikachu.Attacks.Count > 0 && !pikachu.IsKO && !squirtle.IsKO)
        {
            var move = pikachu.Attacks[0];
            pikachu.Attack(squirtle, move);
        }

        if (squirtle.Attacks.Count > 0 && !squirtle.IsKO && !pikachu.IsKO)
        {
            var move = squirtle.Attacks[0];
            squirtle.Attack(pikachu, move);
        }

        Console.WriteLine();
        BattleAnnouncer.Say("Fin de la démo CSV.", ConsoleColor.Green);
    }
}
