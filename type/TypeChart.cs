using System;
using System.Collections.Generic;
using System.IO;

namespace PokemonBattle.Type
{
    public enum PokemonType
    {
        Normal,
        Feu,
        Eau,
        Plante,
        Electrik,
        Glace,
        Roche,
        Sol,
        Acier,
        Dragon,
        Vol,
        Combat,
        Poison,
        Insecte,
        Psy,
        Spectre,
        Tenebres,
        Fee
    }

    public static class TypeChart
    {
        private static readonly Dictionary<(PokemonType atk, PokemonType def), double> _table = new();
        private static bool _loaded = false;

        public static double GetMultiplier(PokemonType attacker, PokemonType defender)
        {
            EnsureLoaded();

            return _table.TryGetValue((attacker, defender), out var m)
                ? m
                : 1.0; // par défaut neutre
        }

        private static void EnsureLoaded()
        {
            if (_loaded) return;

            // même logique que GameDatabase : remonter jusqu'au dossier projet
            string baseDir = AppContext.BaseDirectory;
            string projectDir = Path.GetFullPath(
                Path.Combine(baseDir, "..", "..", "..")
            );

            string dataDir = Path.Combine(projectDir, "data");
            string path = Path.Combine(dataDir, "type_effectiveness.csv");

            if (!File.Exists(path))
            {
                Console.WriteLine($"[WARN] type_effectiveness.csv introuvable à: {path}. Tous les types seront neutres (x1).");
                _loaded = true;
                return;
            }

            var lines = File.ReadAllLines(path);

            // sauter le header
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                if (parts.Length < 3) continue;

                string atkStr = parts[0].Trim();
                string defStr = parts[1].Trim();
                string multStr = parts[2].Trim();

                if (!TryParseType(atkStr, out var atkType) ||
                    !TryParseType(defStr, out var defType))
                {
                    Console.WriteLine($"[WARN] Type invalide dans type_effectiveness.csv: '{atkStr}' ou '{defStr}'");
                    continue;
                }

                if (!double.TryParse(multStr, System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out var mult))
                {
                    Console.WriteLine($"[WARN] Multiplicateur invalide dans type_effectiveness.csv: '{multStr}'");
                    continue;
                }

                _table[(atkType, defType)] = mult;
            }

            _loaded = true;
        }

        // on réutilise la même logique de mapping que dans GameDatabase.ParseType
        private static bool TryParseType(string typeStr, out PokemonType type)
        {
            type = PokemonType.Normal;
            if (string.IsNullOrWhiteSpace(typeStr)) return false;

            switch (typeStr.Trim().ToLowerInvariant())
            {
                case "normal":   type = PokemonType.Normal;   return true;
                case "fire":     type = PokemonType.Feu;      return true;
                case "water":    type = PokemonType.Eau;      return true;
                case "grass":    type = PokemonType.Plante;   return true;
                case "electric": type = PokemonType.Electrik; return true;
                case "ice":      type = PokemonType.Glace;    return true;
                case "rock":     type = PokemonType.Roche;    return true;
                case "ground":   type = PokemonType.Sol;      return true;
                case "steel":    type = PokemonType.Acier;    return true;
                case "dragon":   type = PokemonType.Dragon;   return true;
                case "flying":   type = PokemonType.Vol;      return true;
                case "fighting": type = PokemonType.Combat;   return true;
                case "poison":   type = PokemonType.Poison;   return true;
                case "bug":      type = PokemonType.Insecte;  return true;
                case "psychic":  type = PokemonType.Psy;      return true;
                case "ghost":    type = PokemonType.Spectre;  return true;
                case "dark":     type = PokemonType.Tenebres; return true;
                case "fairy":    type = PokemonType.Fee;      return true;
                default:
                    return false;
            }
        }
    }
}
