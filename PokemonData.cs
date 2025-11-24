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

        public PokemonData(int id, string name, PokemonType type, List<int> attackIds)
        {
            Id = id;
            Name = name;
            Type = type;
            AttackIds = attackIds;
        }

        public override string ToString() =>
            $"{Id}: {Name} ({Type}) — Attaques: {string.Join("|", AttackIds)}";
    }
}