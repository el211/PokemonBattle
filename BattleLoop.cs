using System;
using System.Linq;

namespace PokemonBattle
{
    public class BattleLoop
    {
        private readonly Pokemon p1;
        private readonly Pokemon p2;

        private readonly Random _rng = new();

        public BattleLoop(Pokemon first, Pokemon second)
        {
            p1 = first;
            p2 = second;
        }

        public void Run()
        {
            int turn = 1;
            Console.WriteLine("\n==== Début du combat ====\n");

            while (!p1.IsKO && !p2.IsKO)
            {
                Console.WriteLine($"- Tour {turn} -");

                if (!p1.IsKO && !p2.IsKO)
                    DoTurn(p1, p2);

                if (!p1.IsKO && !p2.IsKO)
                    DoTurn(p2, p1);

                Console.WriteLine();
                turn++;
            }

            var winner = p1.IsKO ? p2 : p1;
            Console.WriteLine($"=== Vainqueur : {winner.Name} ===");
        }

        private void DoTurn(Pokemon attacker, Pokemon defender)
        {
            if (attacker.Attacks.Count > 0)
            {
                // Choisir une attaque au hasard parmi celles du CSV
                var move = attacker.Attacks[_rng.Next(attacker.Attacks.Count)];
                attacker.Attack(defender, move);
            }
            else
            {
                // Fallback : pas d’attaque connue → dégâts bruts
                attacker.Attack(defender, 10);
            }
        }
    }
}