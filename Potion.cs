// Potion.cs
using System;
using PokemonBattle;

public class Potion : IItem
{
    public string Name { get; } = "Super Potion";
    public int Cost { get; }
    
    private readonly int _healAmount;

    public Potion(int healAmount)
    {
        _healAmount = healAmount;
        Cost = 300; // Coût arbitraire
    }

    public override string ToString() => Name;

    public void Use(Pokemon target)
    {
        // On délègue la vérification du K.O. à la méthode Heal du Pokémon.
        BattleAnnouncer.Say(
            $"Dresseur utilise {Name} et tente de soigner {target.Name}...",
            ConsoleColor.Yellow
        );

        // La logique de soin est dans la classe Pokemon.
        target.Heal(_healAmount);
        
        // Si la cible est le joueur, réduisez le stock de l'objet si vous gérez l'inventaire dans PlayerState
        // (Pour cette démo, nous supposons que l'inventaire est géré par la boucle de combat).
    }
}