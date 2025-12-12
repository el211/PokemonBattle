using System;
using System.Collections.Generic;
using System.Linq;
using PokemonBattle.Type; 

namespace PokemonBattle
{
    public class Program
    {
        public static void Main()
        {
            // 1) Charger les CSV
            GameDatabase.LoadAll();

            // 2) Afficher tous les Pokémon chargés depuis pokemon.csv
            BattleAnnouncer.Say("=== Pokémon chargés depuis pokemon.csv ===", ConsoleColor.Green);

            // Affiche les 50 premiers Pokémon chargés pour la boutique
            foreach (var p in GameDatabase.PokemonById.Values.OrderBy(p => p.Id).Take(50)) 
            {
                Console.WriteLine($"{p.Id}: {p.Name} ({p.Type}) — Coût: {p.BaseCost}¥ — Attaques: {string.Join("|", p.AttackIds)}");
            }

            Console.WriteLine();
            BattleAnnouncer.Say("Appuie sur une touche pour commencer la sélection...", ConsoleColor.Cyan);
            Console.ReadKey(true);
            Console.WriteLine();

            // --- 3) SÉLECTION ET ÉCONOMIE ---

            //  3.1) Initialiser la collection du joueur (Donner PLUSIEURS Pokémon de départ)
            // Ceci assure que le joueur a toujours quelques choix même avec 0¥.
            if (GameDatabase.PokemonById.ContainsKey(1)) PlayerState.OwnedPokemonIds.Add(1);      // Bulbasaur
            if (GameDatabase.PokemonById.ContainsKey(4)) PlayerState.OwnedPokemonIds.Add(4);      // Charmander
            if (GameDatabase.PokemonById.ContainsKey(7)) PlayerState.OwnedPokemonIds.Add(7);      // Squirtle
            if (GameDatabase.PokemonById.ContainsKey(16)) PlayerState.OwnedPokemonIds.Add(16);    // Pidgey 
            if (GameDatabase.PokemonById.ContainsKey(19)) PlayerState.OwnedPokemonIds.Add(19);    // Rattata 
            if (GameDatabase.PokemonById.ContainsKey(25)) PlayerState.OwnedPokemonIds.Add(25);    // Pikachu
            

            //  3.2) Afficher la boutique
            ShopMenu();

            //  3.3) Choix du Pokémon du joueur (doit être possédé)
            var ownedPokemonData = GameDatabase.PokemonById.Values.Where(p => PlayerState.OwnedPokemonIds.Contains(p.Id));

            if (!ownedPokemonData.Any())
            {
                 BattleAnnouncer.Say("Vous ne possédez aucun Pokémon pour combattre.", ConsoleColor.Red);
                 return;
            }

            var playerPokemonData = SelectPokemon("Choisissez votre Pokémon (doit être possédé)", ownedPokemonData);
            if (playerPokemonData == null) return;

            //  3.4) Choix de l'adversaire (peut être n'importe quel Pokémon)
            var enemyPokemonData = SelectPokemon("Choisissez le Pokémon ennemi", GameDatabase.PokemonById.Values);
            if (enemyPokemonData == null) return;


            // 4) Créer les objets de combat avec des PV arbitraires
            var playerPokemon = new Pokemon(playerPokemonData, hitPoints: 150);
            var enemyPokemon = new Pokemon(enemyPokemonData, hitPoints: 160);

            // NOUVEAU: Liste d'objets IItem (Inventaire du joueur)
            List<IItem> inventory = new()
            {
                new Pokeball(captureRate: 0.6),
                new Potion(healAmount: 50),                 // 50 PV fixes (l'ancienne potion)
                new PercentagePotion(percentage: 0.25)      // 25% des PV Max (la nouvelle potion équilibrée)
            };

            // 5) Afficher leurs attaques
            Console.WriteLine();
            BattleAnnouncer.Say(
                $"{playerPokemon.Name} connaît : {string.Join(", ", playerPokemon.Attacks.Select(a => a.Name))}",
                ConsoleColor.Yellow
            );
            BattleAnnouncer.Say(
                $"{enemyPokemon.Name} connaît : {string.Join(", ", enemyPokemon.Attacks.Select(a => a.Name))}",
                ConsoleColor.Cyan
            );
            Console.WriteLine();

            // 6) Lancer la boucle de combat MODIFIÉE
            BattleAnnouncer.Say($"Combat: {playerPokemon.Name} vs {enemyPokemon.Name}", ConsoleColor.Magenta);
            Console.WriteLine();

            // Création de la boucle de combat en passant l'inventaire
            var loop = new BattleLoop(playerPokemon, enemyPokemon, inventory); 
            loop.Run();

            Console.WriteLine();

            // Vérifie si le combat s'est terminé par K.O. ou par Capture
            if (playerPokemon.IsKO || enemyPokemon.IsKO)
            {
                BattleAnnouncer.Say("Le combat est terminé. Saut de la phase bonus d'attaque.", ConsoleColor.DarkYellow);
            }
            else
            {
                BattleAnnouncer.Say("Petit bonus: attaques nommées depuis le CSV (si le combat n'est pas terminé) :", ConsoleColor.Green);

                // Bonus : on force chaque Pokémon à utiliser sa première attaque si possible
                if (playerPokemon.Attacks.Count > 0)
                {
                    var move = playerPokemon.Attacks[0];
                    playerPokemon.Attack(enemyPokemon, move);
                }

                if (enemyPokemon.Attacks.Count > 0 && !enemyPokemon.IsKO && !playerPokemon.IsKO)
                {
                    var move = enemyPokemon.Attacks[0];
                    enemyPokemon.Attack(playerPokemon, move);
                }
            }

            Console.WriteLine();
            BattleAnnouncer.Say("Fin de la démo CSV.", ConsoleColor.Green);
        }

        //  Ajout de l'appel à AudioService
private static PokemonData SelectPokemon(string prompt, IEnumerable<PokemonData> availablePokemon)
{
    BattleAnnouncer.Say("====================================", ConsoleColor.Magenta);
    BattleAnnouncer.Say($" {prompt} ", ConsoleColor.Magenta);
    BattleAnnouncer.Say("====================================", ConsoleColor.Magenta);

    var list = availablePokemon.OrderBy(p => p.Id).ToList();

    if (list.Count == 0)
    {
        BattleAnnouncer.Say("Aucun Pokémon disponible.", ConsoleColor.Red);
        return null;
    }

    // Affichage avec index
    for (int i = 0; i < list.Count; i++)
    {
        var p = list[i];
        
        // CORRIGÉ DU SPAM : Affichage propre sans la liste des attaques
        Console.WriteLine($"[{i + 1}] {p.Name} ({p.Type})"); 
        
    }

    while (true)
    {
        Console.Write($"\nEntrez le numéro du Pokémon (1-{list.Count}) ou le nom : ");
        string input = Console.ReadLine()?.Trim();
        PokemonData selectedData = null;

        // 1. Essayer de parser l'index
        if (int.TryParse(input, out int index) && index >= 1 && index <= list.Count)
        {
            selectedData = list[index - 1];
        }
        
        // Essayer de trouver par nom (et sassurerai quil est dans la liste filtrer)
        else if (GameDatabase.PokemonByName.TryGetValue(input, out var dataByName) && availablePokemon.Contains(dataByName))
        {
            selectedData = dataByName;
        }

        if (selectedData != null)
        {
            AudioService.PlayPokemonSound(selectedData.Name); 

            BattleAnnouncer.Say($"-> Vous avez choisi {selectedData.Name}.", ConsoleColor.Green);
            return selectedData;
        }

        BattleAnnouncer.Say("Saisie invalide. Veuillez entrer un numéro valide ou le nom exact d'un Pokémon disponible.", ConsoleColor.Red);
    }
}

        // Méthode pour le menu d'achat
        private static void ShopMenu()
        {
            while (true)
            {
                BattleAnnouncer.Say("\n=== BOUTIQUE POKÉMON ===", ConsoleColor.Yellow);
                Console.WriteLine($"Votre argent : {PlayerState.Money}¥");
                Console.WriteLine("----------------------------------");
                
                var available = GameDatabase.PokemonById.Values.OrderBy(p => p.Id).Take(50);

                foreach (var p in available)
                {
                    string status;
                    if (PlayerState.OwnedPokemonIds.Contains(p.Id))
                        status = "(Possédé)";
                    else if (p.BaseCost == 0)
                        status = "(N/A - Coût 0)";
                    else
                        status = $"({p.BaseCost}¥)";
                    
                    Console.WriteLine($"[{p.Id}] {p.Name} {status}");
                }

                Console.WriteLine("\n[Buy ID] : Acheter un Pokémon (ex: Buy 4)");
                Console.WriteLine("[Cont] : Continuer vers le combat");
                Console.WriteLine("----------------------------------");
                Console.Write("Action : ");

                var input = Console.ReadLine()?.Trim();

                if (input.Equals("Cont", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (input.StartsWith("Buy ", StringComparison.OrdinalIgnoreCase) && input.Length > 4)
                {
                    if (int.TryParse(input.Substring(4).Trim(), out int id) && 
                        GameDatabase.PokemonById.TryGetValue(id, out var data))
                    {
                        // 🟢 NOUVEAU: Jouer le son du Pokémon acheté
                        if (PlayerState.TryBuy(data))
                        {
                            AudioService.PlayPokemonSound(data.Name);
                        }
                    }
                    else
                    {
                        BattleAnnouncer.Say("ID de Pokémon invalide.", ConsoleColor.Red);
                    }
                }
                else
                {
                    BattleAnnouncer.Say("Commande non reconnue.", ConsoleColor.Red);
                }
            }
        }
    }
}