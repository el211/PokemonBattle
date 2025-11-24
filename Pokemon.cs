using System;
using System.Collections.Generic;
using PokemonBattle.Type;

namespace PokemonBattle
{
    public class Pokemon
    {
        // Attaques apprises par ce Pokémon (chargées depuis les CSV via PokemonData)
        public List<Attack> Attacks { get; } = new();

        public string Name { get; }
        public PokemonType Type { get; }
        public int HitPoints { get; private set; }

        public bool IsKO => HitPoints <= 0;

        // --- Constructeur "manuel" (sans CSV) ---
        public Pokemon(string name, PokemonType type, int hitPoints)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nom OBLIGATOIRE", nameof(name));
            if (hitPoints <= 0)
                throw new ArgumentOutOfRangeException(nameof(hitPoints),
                    "Point de vie doit être plus grand que 0");

            Name = name;
            Type = type;
            HitPoints = hitPoints;
        }

        // --- Constructeur depuis PokemonData (CSV) ---
        public Pokemon(PokemonData data, int hitPoints)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (hitPoints <= 0)
                throw new ArgumentOutOfRangeException(nameof(hitPoints),
                    "Point de vie doit être plus grand que 0");

            Name = data.Name;
            Type = data.Type;
            HitPoints = hitPoints;

            foreach (var id in data.AttackIds)
            {
                if (GameDatabase.AttacksById.TryGetValue(id, out var atk))
                {
                    Attacks.Add(atk);
                }
                else
                {
                    Console.WriteLine($"[WARN] Attaque id {id} introuvable pour {Name}");
                }
            }
        }

        // --- Ancienne version avec baseDamage brut ---
        // Peut encore servir de fallback, mais garde la logique "super efficace"
        public void Attack(Pokemon target, int baseDamage)
        {
            if (!CanAttack(target)) return;

            double mult = TypeChart.GetMultiplier(this.Type, target.Type);
            int finalDamage = Math.Max(1, (int)Math.Round(baseDamage * mult));

            if (mult > 1.0)
                Console.WriteLine($"{Name} attaque {target.Name} (super efficace) et inflige {finalDamage} dégâts !");
            else if (mult < 1.0)
                Console.WriteLine($"{Name} attaque {target.Name} (pas très efficace) et inflige {finalDamage} dégâts !");
            else
                Console.WriteLine($"{Name} attaque {target.Name} et inflige {finalDamage} dégâts !");

            target.ReceiveDamage(finalDamage);
        }

        // --- Nouvelle attaque en utilisant un move du CSV ---
        public void Attack(Pokemon target, Attack move)
        {
            if (move == null)
            {
                Console.WriteLine($"{Name} ne peut pas attaquer sans technique.");
                return;
            }

            if (!CanAttack(target)) return;

            // ICI : on utilise le TYPE DE L’ATTAQUE, pas le type du Pokémon
            double mult = TypeChart.GetMultiplier(move.Type, target.Type);
            int finalDamage = Math.Max(1, (int)Math.Round(move.Power * mult));

            if (mult > 1.0)
                Console.WriteLine($"{Name} utilise {move.Name} sur {target.Name} (super efficace) et inflige {finalDamage} dégâts !");
            else if (mult < 1.0)
                Console.WriteLine($"{Name} utilise {move.Name} sur {target.Name} (pas très efficace) et inflige {finalDamage} dégâts !");
            else
                Console.WriteLine($"{Name} utilise {move.Name} sur {target.Name} et inflige {finalDamage} dégâts !");

            target.ReceiveDamage(finalDamage);
        }

        private bool CanAttack(Pokemon target)
        {
            if (IsKO)
            {
                Console.WriteLine($"{Name} ne peut pas attaquer : il est K.O. !");
                return false;
            }
            if (target.IsKO)
            {
                Console.WriteLine($"{target.Name} est déjà K.O. !");
                return false;
            }
            return true;
        }

        public void ReceiveDamage(int damage)
        {
            if (damage <= 0) return;

            HitPoints = Math.Max(0, HitPoints - damage);
            Console.WriteLine($"{Name} possède encore {HitPoints} HP !");

            if (HitPoints == 0)
                Console.WriteLine($"{Name} est K.O. !");
        }

        public override string ToString() => $"{Name} ({Type}) — {HitPoints} HP";
    }
}
