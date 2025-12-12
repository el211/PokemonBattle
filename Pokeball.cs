// Pokeball.cs
using System;
using PokemonBattle;

public class Pokeball : IItem
{
    // 🔴 Nécessite que vous ayez ajouté la méthode Catch() à Pokemon.cs (fait précédemment)
    
    public string Name { get; } = "Poké Ball";
    public int Cost { get; }
    
    private readonly double _captureRate;
    private readonly Random _rng = new();

    public Pokeball(double captureRate)
    {
        _captureRate = captureRate;
        Cost = 200; // Coût arbitraire
    }

    public override string ToString() => Name;

    public void Use(Pokemon target)
    {
        if (target.IsKO)
        {
            BattleAnnouncer.Say($"{target.Name} est déjà K.O. - la capture est impossible.", ConsoleColor.Red);
            return;
        }

        BattleAnnouncer.Say($"Dresseur utilise {Name} sur {target.Name}...", ConsoleColor.Cyan);

        if (_rng.NextDouble() < _captureRate)
        {
            // La capture réussit
            target.Catch(); 
            // Le message de capture est affiché dans Catch()
        }
        else
        {
            // La capture échoue
            BattleAnnouncer.Say($"Oh non ! Le {target.Name} s'est libéré !", ConsoleColor.Red);
        }
    }
}