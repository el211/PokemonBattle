using System;
using System.IO;

using System.Media; 
using PokemonBattle; 

namespace PokemonBattle
{
    public static class AudioService
    {
        // Dossier contenant tous les fichiers sonores 
        private static readonly string AudioDir;

        static AudioService()
        {
            string baseDir = AppContext.BaseDirectory;
            string projectDir = Path.GetFullPath(
                Path.Combine(baseDir, "..", "..", "..")
            );
            AudioDir = Path.Combine(projectDir, "audio"); 

            if (!Directory.Exists(AudioDir))
            {
                Console.WriteLine($"[WARN] Dossier audio non trouvé à: {AudioDir}. La lecture audio sera désactivée.");
            }
        }


        public static void PlayPokemonSound(string pokemonName)
        {
            if (!Directory.Exists(AudioDir)) return;

            // Construit le chemin du fichier 
            string filename = $"{pokemonName.ToLowerInvariant()}.wav";
            string filePath = Path.Combine(AudioDir, filename);

            if (File.Exists(filePath))
            {
                using (var player = new SoundPlayer(filePath))
                {
                    player.Play();
                }
            }
            else
            {
                Console.WriteLine($"[WARN] Fichier sonore non trouvé pour {pokemonName}");
                
            }
        }
    }
}