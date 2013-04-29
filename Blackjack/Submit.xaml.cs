using System;
using System.Windows;

namespace Blackjack
{
    /// <summary>
    /// Interaction logic for Submit.xaml
    /// </summary>
    public partial class Submit
    {
        private readonly Classes.Leaderboard _leaderboard;
        public int Score;

        public Submit()
        {
            InitializeComponent();
            _leaderboard = new Classes.Leaderboard();
            Loaded += (sender, args) => TxtName.Focus();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(TxtName.Text))
            {
                MessageBox.Show("Enter a name");
                return;
            }

            try
            {
                _leaderboard.SubmitScore(TxtName.Text, Convert.ToInt16(Score));
                DialogResult = true;
            }

            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}
