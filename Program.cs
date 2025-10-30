public class Program
{
    public static void Main()
    {
        var pikachu   = new Pokemon(PokemonSpecies.Pikachu,   35);
        var bulbasaur = new Pokemon(PokemonSpecies.Bulbasaur, 45);

        pikachu.DisplayInChat();    
        bulbasaur.DisplayInChat();  

        pikachu.Attack(bulbasaur, 12);
    }
}