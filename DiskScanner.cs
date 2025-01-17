using System;
using System.Collections.Generic;
using System.IO;

namespace NettoyeurDisque
{
    public class FileItem
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public bool IsSelected { get; set; } // Pour la gestion des cases à cocher
    }

    public class DiskScanner
    {
        /// <summary>
        /// Scanne les répertoires spécifiés pour trouver des fichiers temporaires.
        /// Ignore les fichiers en cours d'utilisation.
        /// </summary>
        /// <param name="directories">Liste des répertoires à analyser.</param>
        /// <param name="onProgress">Action appelée pour signaler la progression (en pourcentage).</param>
        /// <returns>Liste des fichiers trouvés.</returns>
        public List<FileItem> ScanDirectories(string[] directories, Action<int> onProgress)
        {
            var foundFiles = new List<FileItem>();
            int totalDirectories = directories.Length;
            int processedDirectories = 0;

            foreach (var dir in directories)
            {
                if (Directory.Exists(dir))
                {
                    try
                    {
                        // Parcourt tous les fichiers du répertoire
                        foreach (var file in Directory.GetFiles(dir))
                        {
                            // Ignorer les fichiers en cours d'utilisation
                            if (IsFileInUse(file)) continue;

                            var fileInfo = new FileInfo(file);
                            foundFiles.Add(new FileItem
                            {
                                FileName = fileInfo.Name,
                                FilePath = fileInfo.FullName,
                                FileSize = fileInfo.Length,
                                IsSelected = false
                            });
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Ignorer les répertoires ou fichiers non accessibles
                    }
                    catch (Exception ex)
                    {
                        // Logger l'erreur ou afficher un message
                        Console.WriteLine($"Erreur lors de l'analyse de {dir} : {ex.Message}");
                    }
                }

                // Mise à jour de la progression
                processedDirectories++;
                int progress = (processedDirectories * 100) / totalDirectories;
                onProgress?.Invoke(progress);
            }

            return foundFiles;
        }

        /// <summary>
        /// Vérifie si un fichier est en cours d'utilisation.
        /// </summary>
        /// <param name="filePath">Chemin du fichier à vérifier.</param>
        /// <returns>True si le fichier est en cours d'utilisation, sinon false.</returns>
        private bool IsFileInUse(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // Si on peut ouvrir le fichier, il n'est pas en cours d'utilisation
                    return false;
                }
            }
            catch (IOException)
            {
                // Si une exception est levée, cela signifie que le fichier est utilisé par un autre processus
                return true;
            }
        }

        public List<FileItem> ScanDirectory(string directory)
        {
            var files = new List<FileItem>();

            if (Directory.Exists(directory))
            {
                try
                {
                    foreach (var file in Directory.GetFiles(directory))
                    {
                        // Ignorer les fichiers en cours d'utilisation
                        if (IsFileInUse(file)) continue;

                        var fileInfo = new FileInfo(file);
                        files.Add(new FileItem
                        {
                            FileName = fileInfo.Name,
                            FilePath = fileInfo.FullName,
                            FileSize = fileInfo.Length
                        });
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // Ignorer les erreurs d'accès
                }
            }

            return files;
        }


    }
}
