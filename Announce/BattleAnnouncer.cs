using System;

namespace PokemonBattle
{
    public static class BattleAnnouncer
    {
        // Version simple (jaune par défaut)
        public static void Say(string message)
        {
            Say(message, ConsoleColor.Yellow);
        }

        // Version avec couleur personnalisée
        public static void Say(string message, ConsoleColor color)
        {
            var previous = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = previous;
        }
    }
}