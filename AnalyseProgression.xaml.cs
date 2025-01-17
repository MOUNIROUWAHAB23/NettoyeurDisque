using System.Windows;

namespace NettoyeurDisque
{
    public partial class AnalyseProgression : Window
    {
        public AnalyseProgression()
        {
            InitializeComponent();
        }

        public void MettreAJourProgression(int progress)
        {
            BarreProgression.Value = progress;

            // Si la progression atteint 100 %, affiche un message
            if (progress >= 100)
            {
                MessageFinal.Text = "Analyse terminée !";
                MessageFinal.Visibility = Visibility.Visible;
            }
        }
    }
}
