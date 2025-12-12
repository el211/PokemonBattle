// IItem.cs
using PokemonBattle;

public interface IItem
{
    string Name { get; }
    int Cost { get; }
    
    // Use() doit être implémenté pour affecter le Pokémon cible (qui peut être l'utilisateur ou l'ennemi).
    void Use(Pokemon target);
}