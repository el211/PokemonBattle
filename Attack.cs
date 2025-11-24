using PokemonBattle.Type;

namespace PokemonBattle
{
    public class Attack
    {
        public int Id { get; }
        public string Name { get; }
        public int Power { get; }
        public PokemonType Type { get; }

        public Attack(int id, string name, int power, PokemonType type)
        {
            Id = id;
            Name = name;
            Power = power;
            Type = type;
        }

        public override string ToString() => $"{Name} (Pwr {Power}, {Type})";
    }
}