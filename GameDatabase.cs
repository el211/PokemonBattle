using System;
using System.Collections.Generic;
using System.Globalization;
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

            string baseDir = AppContext.BaseDirectory;

            // net10.0 -> Debug -> bin -> PokemonBattle
            string projectDir = Path.GetFullPath(
                Path.Combine(baseDir, "..", "..", "..")
            );

            // Dossier data à la racine du projet (là où tu as mis les CSV)
            string dataDir = Path.Combine(projectDir, "data");

            string attacksPath = Path.Combine(dataDir, "attacks.csv");
            string pokemonPath = Path.Combine(dataDir, "pokemon.csv");

            LoadAttacks(attacksPath);
            LoadPokemon(pokemonPath);
            _isLoaded = true;
        }
        
    //mapping    

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

        // Gestion des noms de types en français 
        case "plante":   return PokemonType.Plante; 
        case "feu":      return PokemonType.Feu; 
        case "eau":      return PokemonType.Eau; 
        case "electrik": return PokemonType.Electrik;
        case "insecte":  return PokemonType.Insecte;
        case "sol":      return PokemonType.Sol;
        case "fee":      return PokemonType.Fee;
        case "poison":   return PokemonType.Poison;
        case "roche":    return PokemonType.Roche;
        case "spectre":  return PokemonType.Spectre;
        case "tenebres": return PokemonType.Tenebres;
        case "combat":   return PokemonType.Combat;
        case "psy":      return PokemonType.Psy;
        case "vol":      return PokemonType.Vol;


        // Autres types pas encore dans l'enum c'est  Normal par défaut 
        // je les garde pour etr eplus robuste apres
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
        case "bug":
        case "glace":    
        case "acier":    
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

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                
                // Le nombre de parties dépend du fait que vamp_coeff soit présent ou non (5 ou 6)
                if (parts.Length < 5)
                {
                    Console.WriteLine($"[WARN] Ligne invalide dans attacks.csv : '{line}'");
                    continue;
                }

                // id,name,kind,power,type,vamp_coeff
                int id = int.Parse(parts[0].Trim());
                string name = parts[1].Trim();
                string kind = parts[2].Trim().ToLowerInvariant(); // damage / heal / vampire
                int power = int.Parse(parts[3].Trim());
                string typeStr = parts[4].Trim();
                // Assure que vampCoeffStr est vide si la partie n'existe pas
                string vampCoeffStr = parts.Length > 5 ? parts[5].Trim() : ""; 

                var type = ParseType(typeStr);

                Attack atk;

                switch (kind)
                {
                    case "damage":
                        atk = new DamageAttack(id, name, power, type);
                        break;

                    case "heal":
                        atk = new HealingAttack(id, name, power, type);
                        break;

                    case "vampire":
                        // Attaque vampire : power = dégâts de base, vamp_coeff = % de vol de vie
                        double coeff = 0.5;
                        if (!string.IsNullOrEmpty(vampCoeffStr))
                        {
                            double.TryParse(
                                vampCoeffStr,
                                NumberStyles.Float,
                                CultureInfo.InvariantCulture,
                                out coeff
                            );
                        }

                        atk = new VampireAttack(id, name, power, coeff, type);
                        break;

                    default:
                        // Sécurité si jamais le kind est inconnu on traite comme damage
                        Console.WriteLine($"[WARN] kind '{kind}' inconnu pour l'attaque {name}, traité comme 'damage'.");
                        atk = new DamageAttack(id, name, power, type);
                        break;
                }

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

                // Apres la maj de aujourdhui le format attendu est maintenant: id,name,type,attack_ids,cost
                var parts = line.Split(',');
                
                // Mise à jour: il faut au moins 5 parties
                if (parts.Length < 5) 
                {
                    Console.WriteLine($"[WARN] Ligne invalide (manque le coût) dans pokemon.csv : '{line}'");
                    continue;
                }

                int id = int.Parse(parts[0].Trim());
                string name = parts[1].Trim();
                string typeStr = parts[2].Trim();
                string attackIdsStr = parts[3].Trim();
                
                // Lecture du cout d'achat
                int cost = int.Parse(parts[4].Trim());

                var type = ParseType(typeStr);

                var attackIds = attackIdsStr
                    .Split('|', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.Parse(s.Trim()))
                    .ToList();

                var data = new PokemonData(id, name, type, attackIds, cost); 
                PokemonById[id] = data;
                PokemonByName[name] = data;
            }
        }
    }
}