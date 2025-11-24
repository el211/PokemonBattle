using PokemonBattle.Type;

namespace PokemonBattle
{
    public class VampireAttack : DamageAttack
    {
        public double VampireCoefficient { get; }

        public VampireAttack(int id, string name, int damage, double vampireCoefficient, PokemonType type)
            : base(id, name, damage, type)
        {
            VampireCoefficient = vampireCoefficient;
        }

        public override void Use(Pokemon user, Pokemon target)
        {
            if (user.IsKO || target.IsKO)
                return;



            double mult = TypeChart.GetMultiplier(this.Type, target.Type);
            int totalDamage = Math.Max(1, (int)Math.Round(Damage * mult));

            Console.WriteLine($"{user.Name} utilise {Name} sur {target.Name} ({Type}) " +
                              $"et inflige {totalDamage} dégâts (x{mult}) !");
            target.ReceiveDamage(totalDamage);

            int heal = (int)(totalDamage * VampireCoefficient);
            user.Heal(heal);
            Console.WriteLine($"{user.Name} récupère {heal} PV grâce à l’effet vampire !");
        }

        public override void GetDescription()
        {
            base.GetDescription();
            Console.WriteLine($"  (Vampire: soigne {VampireCoefficient * 100:0}% des dégâts)");
        }
    }
}