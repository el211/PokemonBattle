// PlayerState.cs
using System;
using System.Collections.Generic;
using System.Linq; // Nécessaire pour .Contains()

namespace PokemonBattle
{
    public static class PlayerState
    {
        public static int Money { get; private set; } = 500; // Argent initial du joueur
        
        // HashSet pour un accès rapide (OwnedPokemonIds.Contains(id))
        public static readonly HashSet<int> OwnedPokemonIds = new();

        public static bool TryBuy(PokemonData data)
        {
            if (OwnedPokemonIds.Contains(data.Id))
            {
                BattleAnnouncer.Say($"{data.Name} est déjà dans votre collection.", ConsoleColor.Red);
                return false;
            }

            if (Money >= data.BaseCost)
            {
                Money -= data.BaseCost;
                OwnedPokemonIds.Add(data.Id);
                BattleAnnouncer.Say($"{data.Name} a été acheté pour {data.BaseCost}¥. Il vous reste {Money}¥.", ConsoleColor.Green);
                return true;
            }
            
            BattleAnnouncer.Say($"Achat impossible. Vous avez {Money}¥, il coûte {data.BaseCost}¥.", ConsoleColor.Red);
            return false;
        }
    }
}