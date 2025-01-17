using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NettoyeurDisque
{
    public partial class MainWindow : Window
    {
        private DiskScanner diskScanner;
        private List<FileItem> filesToClean = new List<FileItem>();

        public MainWindow()
        {
            InitializeComponent();
            diskScanner = new DiskScanner();
            UpdateDiskStatus(); // Met à jour les informations sur l'état du disque au démarrage
        }


        private async void BoutonAnalyse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Répertoires à scanner
                string[] directories = { Path.GetTempPath(), "C:\\Windows\\Temp", "C:\\Users\\YourUsername\\AppData\\Local\\Temp" };

                // Afficher la fenêtre de progression
                var fenetreProgression = new AnalyseProgression();
                fenetreProgression.Show();

                // Scanner les fichiers
                var diskScanner = new DiskScanner();
                List<FileItem> fichiersTrouves = new List<FileItem>();

                await Task.Run(() =>
                {
                    fichiersTrouves = diskScanner.ScanDirectories(directories, progress =>
                    {
                        // Mettre à jour la progression dans l'interface
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            fenetreProgression.MettreAJourProgression(progress);
                        });
                    });
                });

                // Assurez-vous que la barre atteint 100 % avant fermeture
                Application.Current.Dispatcher.Invoke(() =>
                {
                    fenetreProgression.MettreAJourProgression(100); // Forcer à 100 %
                    fenetreProgression.MessageFinal.Text = "Analyse terminée !"; // Affiche un message final
                });

                // Délai pour permettre à l'utilisateur de voir 100 %
                await Task.Delay(2000); // 1,5 seconde avant de fermer la fenêtre

                // Fermer la fenêtre de progression
                fenetreProgression.Close();

                // Afficher les résultats dans l'onglet Analyse
                filesToClean = fichiersTrouves; // Mettre à jour la liste globale
                ListeFichiers.ItemsSource = filesToClean;

                // Naviguer vers l'onglet "Analyse"
                MainTabControl.SelectedIndex = 1; // Index de l'onglet "Analyse"
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'analyse : {ex.Message}");
            }
        }



        private void UpdateDiskStatus()
        {
            try
            {
                // Sélectionnez un lecteur, par exemple "C:"
                DriveInfo drive = new DriveInfo("C");

                // Vérifiez si le lecteur est prêt
                if (drive.IsReady)
                {
                    long totalSpace = drive.TotalSize; // Taille totale en octets
                    long freeSpace = drive.TotalFreeSpace; // Espace libre en octets
                    long usedSpace = totalSpace - freeSpace; // Espace utilisé en octets

                    // Convertir en gigaoctets (Go) pour affichage
                    double totalSpaceGB = totalSpace / (1024.0 * 1024 * 1024);
                    double usedSpaceGB = usedSpace / (1024.0 * 1024 * 1024);

                    // Mettre à jour l'interface utilisateur
                    TextEspaceUtilise.Text = $"{usedSpaceGB:F2} Go / {totalSpaceGB:F2} Go";
                    ProgressBarEspaceTotal.Value = (usedSpaceGB / totalSpaceGB) * 100; // Pourcentage d'utilisation
                }
                else
                {
                    // Afficher un message si le lecteur n'est pas prêt
                    TextEspaceUtilise.Text = "Le lecteur n'est pas prêt";
                    ProgressBarEspaceTotal.Value = 0;
                }
            }
            catch (Exception ex)
            {
                // Gérer les erreurs et afficher un message
                MessageBox.Show($"Erreur lors de la récupération de l'état du disque : {ex.Message}");
                TextEspaceUtilise.Text = "Erreur";
                ProgressBarEspaceTotal.Value = 0;
            }
        }



        

        private void BoutonSelectTout_Click(object sender, RoutedEventArgs e)
        {
            // Sélectionner tous les fichiers
            foreach (var file in filesToClean)
            {
                file.IsSelected = true;
            }
            ListeFichiers.Items.Refresh(); // Actualiser l'affichage
        }


        private void DeselectAllFiles_Click(object sender, RoutedEventArgs e)
        {
            // Désélectionner tous les fichiers
            foreach (var file in filesToClean)
            {
                file.IsSelected = false;
            }
            ListeFichiers.Items.Refresh(); // Actualiser l'affichage
        }

        private async void BoutonNettoyage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Récupérer les fichiers sélectionnés
                var selectedFiles = filesToClean.Where(f => f.IsSelected).ToList();

                if (!selectedFiles.Any())
                {
                    MessageBox.Show("Aucun fichier sélectionné pour la suppression.", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                long totalFreedSpace = 0;
                int deletedFilesCount = 0; // Compteur de fichiers supprimés
                int progress = 0;
                int totalFiles = selectedFiles.Count;

                // Naviguer vers l'onglet "Nettoyage"
                MainTabControl.SelectedIndex = 2; // Index de l'onglet "Nettoyage"

                // Supprimer les fichiers et mettre à jour la progression
                foreach (var file in selectedFiles)
                {
                    try
                    {
                        // Supprimer le fichier
                        File.Delete(file.FilePath);
                        totalFreedSpace += file.FileSize; // Ajouter la taille du fichier supprimé à l'espace libéré
                        filesToClean.Remove(file); // Retirer le fichier de la liste
                        deletedFilesCount++; // Incrémenter le compteur

                        // Mettre à jour la progression
                        progress++;
                        int percentage = (progress * 100) / totalFiles;

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ProgressionNettoyage.Value = percentage;
                            TexteProgression.Text = $"{percentage}%";
                        });

                        // Simuler un délai pour visualiser la progression
                        await Task.Delay(200); // 200 ms entre chaque suppression
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression de {file.FileName}: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                // Mettre à jour l'affichage de la liste
                ListeFichiers.ItemsSource = null;
                ListeFichiers.ItemsSource = filesToClean;

                // Mettre à jour l'état du disque
                UpdateDiskStatus();

                // Afficher un message de succès
                MessageBox.Show($"Nettoyage terminé !\nEspace libéré : {totalFreedSpace / (1024.0 * 1024):F2} Mo\nFichiers supprimés : {deletedFilesCount}",
                                "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                // Mettre à jour le rapport
                UpdateReport(totalFreedSpace, deletedFilesCount);

                // Naviguer vers l'onglet "Rapport"
                MainTabControl.SelectedIndex = 3; // Index de l'onglet "Rapport"
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du nettoyage : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





        private void BoutonGenererPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Générer le contenu du rapport
                ReportGenerator reportGen = new ReportGenerator();
                string reportContent = reportGen.GenerateReport(filesToClean);

                // Afficher le rapport dans la TextBox
                RapportTextBox.Text = reportContent;

                // Ouvrir la boîte de dialogue pour enregistrer le PDF
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    DefaultExt = "pdf",
                    FileName = "RapportNettoyage"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    // Générer le rapport PDF
                    reportGen.GeneratePDFReport(reportContent, saveFileDialog.FileName);

                    // Notifier l'utilisateur
                    MessageBox.Show($"Rapport PDF généré avec succès : {saveFileDialog.FileName}",
                        "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la génération du rapport : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void UpdateReport(long freedSpace, int deletedFilesCount)
        {
            try
            {
                int remainingFiles = filesToClean.Count;
                long totalRemainingSize = filesToClean.Sum(f => f.FileSize);

                // Mettre à jour la TextBox de rapport
                RapportTextBox.Text = $"Nettoyage terminé !\n" +
                                      $"Nombre de fichiers supprimés : {deletedFilesCount}\n" +
                                      $"Espace libéré : {freedSpace / (1024.0 * 1024):F2} Mo\n" +
                                      $"Fichiers restants : {remainingFiles}\n" +
                                      $"Espace total restant des fichiers : {totalRemainingSize / (1024.0 * 1024):F2} Mo\n";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour du rapport : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





        private void MenuItemQuitter_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItemOptions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Options de configuration - en cours de développement");
        }
    }
}
