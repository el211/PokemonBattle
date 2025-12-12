using PokemonBattle.Type;

namespace PokemonBattle
{
    public abstract class Attack
    {
        public int Id { get; }
        public string Name { get; }
        public PokemonType Type { get; }

        protected Attack(int id, string name, PokemonType type)
        {
            Id = id;
            Name = name;
            Type = type;
        }


        public abstract void Use(Pokemon user, Pokemon target);

 
        public abstract void GetDescription();
    }
}
