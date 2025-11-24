using PokemonBattle;
using PokemonBattle.Type;

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
        double mult = TypeChart.GetMultiplier(this.Type, target.Type);
        int totalDamage = Math.Max(1, (int)Math.Round(Damage * mult));

        Console.WriteLine($"{user.Name} utilise {Name} et inflige {totalDamage} dégâts (x{mult}) !");
        target.ReceiveDamage(totalDamage);
    }

    public override void GetDescription()
    {
        Console.WriteLine($"- {Name} (Damage: {Damage}, Type: {Type})");
    }
}