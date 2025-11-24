using PokemonBattle.Type;

namespace PokemonBattle
{
    public class HealingAttack : Attack
    {
        public int HealingAmount { get; }

        public HealingAttack(int id, string name, int healingAmount, PokemonType type)
            : base(id, name, type)
        {
            HealingAmount = healingAmount;
        }

        public override void Use(Pokemon user, Pokemon target)
        {
            if (user.IsKO)
            {
                Console.WriteLine($"{user.Name} est K.O. et ne peut pas utiliser {Name}.");
                return;
            }

            user.Heal(HealingAmount); 
            Console.WriteLine($"{user.Name} utilise {Name} et se soigne de {HealingAmount} PV !");
        }

        public override void GetDescription()
        {
            Console.WriteLine($"- {Name} (Soin: {HealingAmount}, Type: {Type})");
        }
    }
}