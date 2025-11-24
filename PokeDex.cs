using System.Collections.Generic;
using PokemonBattle.Type;

namespace PokemonBattle
{
    public enum PokemonSpecies
    {
        Pikachu,
        Bulbasaur,
        Charmander,
        Squirtle,
        Eevee
    }

    public static class Pokedex
    {
        private static readonly Dictionary<PokemonSpecies, PokemonType> _speciesType =
            new Dictionary<PokemonSpecies, PokemonType>
            {
                { PokemonSpecies.Pikachu,    PokemonType.Electrik },
                { PokemonSpecies.Bulbasaur,  PokemonType.Plante   },
                { PokemonSpecies.Charmander, PokemonType.Feu      },
                { PokemonSpecies.Squirtle,   PokemonType.Eau      },
                { PokemonSpecies.Eevee,      PokemonType.Normal   }
            };

        public static PokemonType GetTypeOf(PokemonSpecies species) => _speciesType[species];

        public static string GetDisplayName(PokemonSpecies species) => species.ToString();
    }
}