using System;
using System.Collections.Generic;
using System.Linq;
using PokemonBattle.Type;   // ✅ we *use* PokemonType from this namespace

namespace PokemonBattle
{
    public class Pokemon
    {
        // Attaques apprises par ce Pokémon (chargées depuis les CSV via PokemonData)
        public List<Attack> Attacks { get; } = new();

        public string Name { get; }
        public PokemonType Type { get; }

        // PV maximum et PV actuels
        public int MaxHitPoints { get; }
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

            MaxHitPoints = hitPoints;
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

            MaxHitPoints = hitPoints;
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
        // Peut encore servir de fallback
        public void Attack(Pokemon target, int baseDamage)
        {
            if (!CanAttack(target)) return;
            
            // 🔴 Intégration du son pour l'attaque (même pour le fallback)
            AudioService.PlayPokemonSound(this.Name); 

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

        // --- Nouvelle attaque en utilisant un move du CSV / polymorphe ---
        public void Attack(Pokemon target, Attack move)
        {
            if (move == null)
            {
                Console.WriteLine($"{Name} ne peut pas attaquer sans technique.");
                return;
            }

            if (!CanAttack(target)) return;

            //  NOUVEAU: Jouer le son du Pokémon attaquant
            AudioService.PlayPokemonSound(this.Name); 

            // On délègue à l'attaque concrète (DamageAttack, HealingAttack, VampireAttack, etc.)
            move.Use(this, target);
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
            Console.WriteLine($"{Name} possède encore {HitPoints}/{MaxHitPoints} HP !");

            if (HitPoints == 0)
                Console.WriteLine($"{Name} est K.O. !");
        }

        public void Heal(int amount)
        {
            if (amount <= 0) return;

            if (IsKO)
            {
                Console.WriteLine($"{Name} est K.O. et ne peut pas etre soigne.");
                return;
            }

            int oldHp = HitPoints;
            HitPoints = Math.Min(MaxHitPoints, HitPoints + amount);

            int healed = HitPoints - oldHp;
            Console.WriteLine($"{Name} recupere {healed} PV ({HitPoints}/{MaxHitPoints}) !");
        }

        // 🟢 NOUVEAU: Méthode Catch pour la Poké Ball
        public void Catch()
        {
            // On considère le Pokémon comme K.O. pour mettre fin au BattleLoop.
            HitPoints = 0;
            Console.WriteLine($"\n***** {Name} a été capturé ! Le combat prend fin. *****\n");
        }


        public override string ToString() => $"{Name} ({Type}) — {HitPoints}/{MaxHitPoints} HP";
    }
}