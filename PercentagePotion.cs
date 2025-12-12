// PercentagePotion.cs
using System;
using PokemonBattle;

public class PercentagePotion : IItem
{
    public string Name { get; } = "Super Potion %";
    public int Cost { get; }
    

    private readonly double _healPercentage;

    public PercentagePotion(double percentage)
    {
        _healPercentage = percentage;
        Cost = 500; 
    }

    public override string ToString() => Name;

    public void Use(Pokemon target)
    {
        if (target.IsKO)
        {
            BattleAnnouncer.Say($"{target.Name} est K.O. et ne peut pas être soigné.", ConsoleColor.Red);
            return;
        }
        
        // Calcul du soin basé sur le pourcentage des PV Max
        int amountToHeal = (int)Math.Round(target.MaxHitPoints * _healPercentage);

        BattleAnnouncer.Say(
            $"Dresseur utilise {Name} et tente de soigner {target.Name} ({_healPercentage * 100}%)...",
            ConsoleColor.Yellow
        );

        target.Heal(amountToHeal);
    }
}