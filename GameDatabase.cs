using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PokemonBattle.Type;

namespace PokemonBattle
{
    public static class GameDatabase
    {
        public static readonly Dictionary<int, Attack> AttacksById = new();
        public static readonly Dictionary<int, PokemonData> PokemonById = new();
        public static readonly Dictionary<string, PokemonData> PokemonByName =
            new(StringComparer.OrdinalIgnoreCase);

        private static bool _isLoaded = false;

        public static void LoadAll()
        {
            if (_isLoaded) return;

            // Dossier de l'exe : ...\PokemonBattle\bin\Debug\net10.0\
            string baseDir = AppContext.BaseDirectory;

            // On remonte de 3 niveaux pour revenir au projet :
            // net10.0 -> Debug -> bin -> PokemonBattle
            string projectDir = Path.GetFullPath(
                Path.Combine(baseDir, "..", "..", "..")
            );

            // Dossier "data" à la racine du projet (là où tu as mis les CSV)
            string dataDir = Path.Combine(projectDir, "data");

            string attacksPath = Path.Combine(dataDir, "attacks.csv");
            string pokemonPath = Path.Combine(dataDir, "pokemon.csv");

            LoadAttacks(attacksPath);
            LoadPokemon(pokemonPath);
            _isLoaded = true;
        }

        // ---------- mapping string CSV -> ton enum PokemonType ----------

        private static PokemonType ParseType(string typeStr)
        {
            if (string.IsNullOrWhiteSpace(typeStr))
                return PokemonType.Normal;

            switch (typeStr.Trim().ToLowerInvariant())
            {
                case "normal":   return PokemonType.Normal;

                // CSV anglais -> enum FR
                case "fire":     return PokemonType.Feu;
                case "water":    return PokemonType.Eau;
                case "grass":    return PokemonType.Plante;
                case "electric": return PokemonType.Electrik;

                // Autres types pas encore dans l'enum -> Normal par défaut
                case "fairy":
                case "psychic":
                case "dark":
                case "ghost":
                case "ice":
                case "rock":
                case "ground":
                case "steel":
                case "dragon":
                case "flying":
                case "fighting":
                case "poison":
                case "bug":
                    return PokemonType.Normal;

                default:
                    Console.WriteLine($"[WARN] Type inconnu dans CSV: '{typeStr}', remplacé par Normal.");
                    return PokemonType.Normal;
            }
        }

        // ----------------- attacks.csv -----------------

        private static void LoadAttacks(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"attacks.csv introuvable à: {path}");
            }

            var lines = File.ReadAllLines(path);

            // première ligne = header, on la saute
            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                if (parts.Length < 4) continue;

                int id = int.Parse(parts[0].Trim());
                string name = parts[1].Trim();
                int power = int.Parse(parts[2].Trim());
                string typeStr = parts[3].Trim();

                var type = ParseType(typeStr);

                var atk = new Attack(id, name, power, type);
                AttacksById[id] = atk;
            }
        }

        // ----------------- pokemon.csv -----------------

        private static void LoadPokemon(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"pokemon.csv introuvable à: {path}");
            }

            var lines = File.ReadAllLines(path);

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                // id,name,type,attack_ids
                var parts = line.Split(',');
                if (parts.Length < 4) continue;

                int id = int.Parse(parts[0].Trim());
                string name = parts[1].Trim();
                string typeStr = parts[2].Trim();
                string attackIdsStr = parts[3].Trim();

                var type = ParseType(typeStr);

                var attackIds = attackIdsStr
                    .Split('|', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.Parse(s.Trim()))
                    .ToList();

                var data = new PokemonData(id, name, type, attackIds);
                PokemonById[id] = data;
                PokemonByName[name] = data;
            }
        }
    }
}
