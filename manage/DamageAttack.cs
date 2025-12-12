using System;
using PokemonBattle;
using PokemonBattle.Type;

namespace PokemonBattle
{
    public class DamageAttack : Attack
    {

        public int Damage { get; }

        public DamageAttack(int id, string name, int damage, PokemonType type)
            : base(id, name, type)
        {
            Damage = damage;
        }
        
        public override void Use(Pokemon user, Pokemon target)
        {
            // Calcule les dégâts totaux en utilisant la classe utilitaire
            double mult = TypeChart.GetMultiplier(this.Type, target.Type);
            int totalDamage = DamageCalculator.ComputeDamage(Damage, this.Type, target.Type);

            // Annoncer l'attaque
            BattleAnnouncer.Say($"{user.Name} utilise **{Name}** ({Type}) sur {target.Name} et inflige {totalDamage} dégâts (x{mult:0.##})!", ConsoleColor.DarkYellow);

            // Annoncer l'efficacité
            if (mult > 1.0)
                BattleAnnouncer.Say("C'est super efficace !", ConsoleColor.Red);
            else if (mult < 1.0 && mult > 0)
                BattleAnnouncer.Say("Ce n'est pas très efficace...", ConsoleColor.Yellow);
            else if (mult == 0)
                BattleAnnouncer.Say("C'est sans effet.", ConsoleColor.Gray);

            // La cible reçoit les dégas
            target.ReceiveDamage(totalDamage);
        }


        public override void GetDescription()
        {
            Console.WriteLine($"- {Name} (Type: {Type}, Dégâts de base: {Damage})");
        }
    }
}