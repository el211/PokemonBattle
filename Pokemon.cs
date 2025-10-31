using System;
using PokemonBattle.Type;

public class Pokemon
{
    public string Name { get; }
    public PokemonType Type { get; }
    public int HitPoints { get; private set; }

    public bool IsKO => HitPoints <= 0;

    public Pokemon(string name, PokemonType type, int hitPoints)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nom OBLIGATOIRE", nameof(name));
        if (hitPoints <= 0) throw new ArgumentOutOfRangeException(nameof(hitPoints), "Point de degat doit etre plus grand que 0");

        Name = name;
        Type = type;
        HitPoints = hitPoints;
    }
    
    public void Attack(Pokemon target, int baseDamage)
    {
        if (IsKO) { Console.WriteLine($"{Name} ne peut pas attaquer : il est K.O. !"); return; }
        if (target.IsKO) { Console.WriteLine($"{target.Name} est déjà K.O. !"); return; }

        double mult = TypeChart.GetMultiplier(this.Type, target.Type); 
        int finalDamage = Math.Max(1, (int)Math.Round(baseDamage * mult));

        if (mult > 1.0)
            Console.WriteLine($"{Name} attaque {target.Name} (super efficace) et inflige {finalDamage} degats !");
        else if (mult < 1.0)
            Console.WriteLine($"{Name} attaque {target.Name} (pas tres efficace) et inflige {finalDamage} degats !");
        else
            Console.WriteLine($"{Name} attaque {target.Name} et inflige {finalDamage} degats !");

        target.ReceiveDamage(finalDamage);
    }
    
    public void ReceiveDamage(int damage)
    {
        if (damage <= 0) return;

        HitPoints = Math.Max(0, HitPoints - damage);

        Console.WriteLine($"{Name} possede  {HitPoints} HP encore !");

        if (HitPoints == 0)
        {
            Console.WriteLine($"{Name} is K.O.!");
        }
    }

    public override string ToString() => $"{Name} ({Type}) — {HitPoints} HP";
}
