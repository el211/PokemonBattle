using System;

public class Pokemon
{
    public PokemonSpecies Species { get; }
    public PokemonType Type { get; }
    public int HitPoints { get; private set; }

    public string Name => Pokedex.GetDisplayName(Species);

    public Pokemon(PokemonSpecies species, int hitPoints)
    {
        Species = species;
        Type = Pokedex.GetTypeOf(species);
        HitPoints = hitPoints;
    }

    public void DisplayInChat()
    {
        Console.WriteLine($"{Name} ({Type}) — {HitPoints} HP");
    }

    public void Attack(Pokemon target, int damage)
    {
        Console.WriteLine($"{Name} attaque {target.Name} et fait {damage} de degat!");
        target.ReceiveDamage(damage);
    }

    public void ReceiveDamage(int damage)
    {
        HitPoints = Math.Max(0, HitPoints - damage);
        Console.WriteLine($"{Name} possede {HitPoints} HP encore!");
    }

    public override string ToString() => $"{Name} ({Type})";
}