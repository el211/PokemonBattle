using System.Collections.Generic;
using PokemonBattle.Type;

namespace PokemonBattle
{
    public class PokemonData
    {
        public int Id { get; }
        public string Name { get; }
        public PokemonType Type { get; }
        public List<int> AttackIds { get; }
        
        // 🔴 NOUVEAU: Propriété pour stocker le coût d'achat de base
        public int BaseCost { get; } 

        // 🔴 CONSTRUCTEUR MODIFIÉ: Accepte un paramètre supplémentaire pour BaseCost
        public PokemonData(int id, string name, PokemonType type, List<int> attackIds, int baseCost)
        {
            Id = id;
            Name = name;
            Type = type;
            AttackIds = attackIds;
            
            // 🔴 Initialisation du coût
            BaseCost = baseCost; 
        }

        public override string ToString() =>
            $"{Id}: {Name} ({Type}) — Coût: {BaseCost}¥ — Attaques: {string.Join("|", AttackIds)}";
    }
}