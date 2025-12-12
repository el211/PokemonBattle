using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonBattle
{
    public class BattleLoop
    {
        private readonly Pokemon p1;
        private readonly Pokemon p2;
        private readonly List<IItem> _inventory;
        
        private readonly Random _rng = new();

        public BattleLoop(Pokemon first, Pokemon second, List<IItem> inventory) 
        {
            p1 = first;
            p2 = second;
            _inventory = inventory;
        }

        public void Run()
        {
            int turn = 1;
            Console.WriteLine("\n==== Début du combat ====\n");

            while (!p1.IsKO && !p2.IsKO)
            {
                Console.WriteLine($"\n--- Tour {turn} ---");
                
                // P1 (Joueur) - Gestion de l'action
                if (!p1.IsKO && !p2.IsKO)
                    HandlePlayerAction(p1, p2, isPlayer: true);

                // Vérification après l'action du joueur (capture possible)
                if (p1.IsKO || p2.IsKO) 
                    break;

                // P2 (Adversaire/IA) - Gestion de l'action
                if (!p1.IsKO && !p2.IsKO)
                    HandlePlayerAction(p2, p1, isPlayer: false);

                turn++;
            }

            var winner = p1.IsKO ? p2 : p1;
            Console.WriteLine($"\n=== Vainqueur : {winner.Name} ===");
        }

        private void HandlePlayerAction(Pokemon user, Pokemon target, bool isPlayer)
        {
            if (user.IsKO || target.IsKO) return;

            if (!isPlayer)
            {
                // attaque aléatoire
                PerformRandomAttack(user, target);
                return;
            }

            //Tour du Joueur 
            Console.WriteLine($"\n{user.Name} ({user.HitPoints}/{user.MaxHitPoints} HP) turn. Votre argent: {PlayerState.Money}¥.");
            Console.WriteLine("1. Attaquer");
            Console.WriteLine($"2. Utiliser un objet (Inventaire: {_inventory.Count} objets)");
            Console.Write("Choix (1/2, Attaquer par défaut) : ");

            var choice = Console.ReadLine()?.Trim().ToLowerInvariant();

            if (choice == "2" || choice == "objet")
            {
                DoItemTurn(user, target);
            }
            else // Choix 1, par défaut, ou invalide donc menu dattake
            {
                Attack chosenMove = SelectAttack(user); //la selection de lattaque

                if (chosenMove != null)
                {
                    user.Attack(target, chosenMove);
                }
                else
                {
                    // fallback si la selection manuelle échoue ou est annuler
                    BattleAnnouncer.Say($"{user.Name} n'a pas pu choisir une attaque valide. Attaque aléatoire de secours.", ConsoleColor.DarkRed);
                    PerformRandomAttack(user, target);
                }
            }
        }
        
        // methode pour selectionner lattake par le joueur
        private Attack SelectAttack(Pokemon user)
        {
            if (user.Attacks.Count == 0)
            {
                BattleAnnouncer.Say($"{user.Name} ne connaît aucune attaque!", ConsoleColor.Red);
                return null;
            }

            Console.WriteLine($"\n--- Attaques de {user.Name} ---");
            for (int i = 0; i < user.Attacks.Count; i++)
            {
                var move = user.Attacks[i];
                Console.WriteLine($"[{i + 1}] {move.Name} (Type: {move.Type})");
            }
            Console.WriteLine("------------------------------");
            Console.Write("Choisir une attaque (index ou nom) : ");

            var input = Console.ReadLine()?.Trim();

            // 1. Tenter par index
            if (int.TryParse(input, out int index) && index >= 1 && index <= user.Attacks.Count)
            {
                return user.Attacks[index - 1];
            }

            // 2. Tenter par nom
            var chosenMove = user.Attacks.FirstOrDefault(a => 
                a.Name.Equals(input, StringComparison.OrdinalIgnoreCase)
            );
            //comme sa chosenMove devi1 une instance concrete (par example, 1 objet DamageAttack representent 'Thunderbolt')
            if (chosenMove != null)
            {
                return chosenMove;
            }

            BattleAnnouncer.Say("Attaque invalide.", ConsoleColor.Red);
            return null;
        }

        // Gère l'utilisation d'un objet par le joueur
        private void DoItemTurn(Pokemon user, Pokemon target)
        {
            if (_inventory.Count == 0)
            {
                BattleAnnouncer.Say("Inventaire vide. Attaque par défaut.", ConsoleColor.Red);
                PerformRandomAttack(user, target);
                return;
            }

            Console.WriteLine("--- Inventaire ---");
            for (int i = 0; i < _inventory.Count; i++)
            {
                var item = _inventory[i];
                string effectTarget = (item is Pokeball) ? target.Name : user.Name;
                Console.WriteLine($"[{i}] {item.Name} (Coût: {item.Cost}¥, Cible: {effectTarget})");
            }
            Console.WriteLine("------------------");
            Console.Write("Choisir un objet (index ou nom) : ");

            var input = Console.ReadLine()?.Trim();
            IItem chosenItem = null;

            // 1. Tenter par index (0 ou 1)
            if (int.TryParse(input, out int index) && index >= 0 && index < _inventory.Count)
            {
                chosenItem = _inventory[index];
            }
            // 2. Tenter par nom
            else
            {
                chosenItem = _inventory.FirstOrDefault(i => 
                    i.Name.Replace(" ", "").Equals(input, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (chosenItem != null)
            {
                // Déterminer et utiliser la bonne cible (Polymorphisme)
                Pokemon actualTarget = (chosenItem is Pokeball) ? target : user;
        
                // Exécuter l'action de l'objet
                chosenItem.Use(actualTarget); 

                // Consommer l'objet
                _inventory.Remove(chosenItem); 
                BattleAnnouncer.Say($"{chosenItem.Name} a été utilisé et retiré de l'inventaire.", ConsoleColor.DarkGreen);
            }
            else 
            {
                BattleAnnouncer.Say("Objet invalide ou introuvable. Attaque par défaut.", ConsoleColor.Red);
                PerformRandomAttack(user, target);
            }
        }

        // Méthode d'aide pour l'attaque aléatoire 
        private void PerformRandomAttack(Pokemon attacker, Pokemon defender)
        {
            if (attacker.Attacks.Count > 0)
            {
                // Choisir une attaque au hasard
                var move = attacker.Attacks[_rng.Next(attacker.Attacks.Count)];
                attacker.Attack(defender, move);
            }
            else
            {
                // Fallback dégâts bruts
                attacker.Attack(defender, 10);
            }
        }
    }
}