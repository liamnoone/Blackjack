using System.Windows;

namespace Blackjack
{
    /// <summary>
    /// Interaction logic for HighScores.xaml
    /// </summary>
    public partial class HighScores
    {
        private readonly Classes.Leaderboard _leaderboard;

        public HighScores()
        {
            InitializeComponent();
            _leaderboard = new Classes.Leaderboard();
        }

        private void HighScores_OnLoaded(object sender, RoutedEventArgs e)
        {
            LvwScores.DataContext = _leaderboard.GetScores();
        }
    }
}
