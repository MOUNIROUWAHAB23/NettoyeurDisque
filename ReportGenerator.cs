using System;
using System.Collections.Generic;
using System.IO;
using iTS = iTextSharp.text; // Alias for iTextSharp.text
using iTextSharp.text.pdf;

namespace NettoyeurDisque
{
    public class ReportGenerator
    {
        /// <summary>
        /// Generates a text-based report summarizing the files that were deleted.
        /// </summary>
        /// <param name="deletedFiles">List of files deleted during the process.</param>
        /// <returns>A string report summarizing the cleanup operation.</returns>
        public string GenerateReport(List<FileItem> deletedFiles)
        {
            int totalFiles = deletedFiles.Count;
            long totalSize = 0;

            foreach (var file in deletedFiles)
            {
                totalSize += file.FileSize;
            }

            return $"Fichiers supprimés : {totalFiles}\nEspace libéré : {totalSize / (1024 * 1024.0):F2} Mo";
        }

        /// <summary>
        /// Generates a PDF report summarizing the files that were deleted.
        /// </summary>
        /// <param name="reportContent">The textual content of the report.</param>
        /// <param name="outputPath">The output path for the generated PDF file.</param>
        public void GeneratePDFReport(string reportContent, string outputPath = "RapportNettoyage.pdf")
        {
            try
            {
                using (FileStream fs = new FileStream(outputPath, FileMode.Create))
                {
                    // Create a new PDF document
                    iTS.Document document = new iTS.Document(iTS.PageSize.A4, 25, 25, 30, 30);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);

                    document.Open();

                    // Add a title
                    iTS.Font titleFont = iTS.FontFactory.GetFont(iTS.FontFactory.HELVETICA_BOLD, 16);
                    iTS.Paragraph title = new iTS.Paragraph("Rapport de Nettoyage", titleFont)
                    {
                        Alignment = iTS.Element.ALIGN_CENTER
                    };
                    document.Add(title);
                    document.Add(new iTS.Paragraph("\n")); // Add some spacing

                    // Add the report content
                    iTS.Font contentFont = iTS.FontFactory.GetFont(iTS.FontFactory.HELVETICA, 12);
                    iTS.Paragraph content = new iTS.Paragraph(reportContent, contentFont);
                    document.Add(content);

                    // Close the document
                    document.Close();
                }

                Console.WriteLine($"Rapport PDF généré avec succès : {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la génération du PDF : {ex.Message}");
                throw;
            }
        }
    }
}
