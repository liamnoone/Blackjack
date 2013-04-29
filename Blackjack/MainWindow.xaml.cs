using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Blackjack.Classes;

namespace Blackjack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        int _sourceImageIndex;
        Image _sourceImage;
        /// <summary>
        /// How much a bet is worth
        /// </summary>
        const int Bet = 5;
        DispatcherTimer _dealerLogic;

        Random _rng;
        Player _player;
        Shoe _shoe;
        Dealer _dealer;

        public MainWindow()
        {            
            InitializeComponent();
        }  

        /// <summary>
        /// Reset the player, dealer and shoe to default state 
        /// Deck is then shuffled: can't simply shuffle incase Deck contains under 52 cards
        /// This allows for a new game to be started
        /// </summary>
        private void NewGame()
        {
            cvsPlayer.Children.Clear();
            cvsDealer.Children.Clear();

            _dealer = new Dealer();
            _player = new Player();
            _shoe = new Shoe();
            UpdateScores(100, 0);
            _shoe.Shuffle();

            btnHit.IsEnabled = false;
            btnStand.IsEnabled = false;
            btnDouble.IsEnabled = false;
            //btnSplit.IsEnabled = false;

            btnBet.IsEnabled = true;
            btnStart.IsEnabled = false;

            MnuNextRound.IsEnabled = false;
            MnuSubmit.IsEnabled = false;

            Title = "Blackjack";
            _player.Hand.CollectionChanged += CollectionChanged;
            _dealer.Hand.CollectionChanged += CollectionChanged;
        }

        /// <summary>
        /// Setup game for another round (restart button states, but don't fully reset objects)
        /// </summary>
        private void NextRound()
        {
            _player.ClearHand();
            _dealer.ClearCards();

            _shoe.ReFill();
            _shoe.Shuffle();
            _player.BetChips(0);
            UpdateScores(_player.Chips, 0);
            _dealerLogic.Stop();

            _player.State = PlayerState.In;
            _dealer.State = DealerState.In;

            btnBet.IsEnabled = true;
            btnHit.IsEnabled = false;  
            btnStand.IsEnabled = false;

            MnuNextRound.IsEnabled = false;
            MnuSubmit.IsEnabled = false;
        }  

        private void AddImage(Canvas cvs, Card c)
        {
            Image i = c.ImageLocation;
            i.MouseEnter += Image_MouseEnter;
            i.MouseLeave += Image_MouseLeave;
            int numChildren = cvs.Children.Count;
            cvs.Children.Add(i);
            Canvas.SetLeft(cvs.Children[numChildren], numChildren * 20);
        }
        
        void BlackJack() 
        {
            if ((_player.Points == 21) && (_player.Bet > 0))
            {
                _player.AwardChips();
                MessageBox.Show(String.Format("Blackjack! Player wins {0} chips.", _player.Bet), "Blackjack!");

                btnHit.IsEnabled = false;
                btnStand.IsEnabled = false;
                btnDouble.IsEnabled = false;
                MnuNextRound.IsEnabled = true;
                MnuSubmit.IsEnabled = true;
                _player.BetChips(0);
            }
        }
        
        private void OutOfChips()
        {
            MessageBox.Show("Player is out of chips.", "Out of Chips");
            UpdateScores(_player.Chips, 0);
            Title = "Game Over: Out of Chips";

            btnBet.IsEnabled = false;
            btnStand.IsEnabled = false;
            btnSplit.IsEnabled = false;
            btnDouble.IsEnabled = false;
            MnuNextRound.IsEnabled = false;
            MnuSubmit.IsEnabled = true;
        }
        /// <summary>
        /// Update the UI with player's new details
        /// </summary>
        /// <param name="chips">How many chips to display</param>
        /// <param name="bet">How many chips currently being bet to display</param>
        private void UpdateScores(int chips, int bet)
        {
            tbkPlayerChips.Text = String.Format("Player's Chips: {0:C0}", chips);
            tbkPlayerBet.Text = String.Format("Player's Bet: {0:C0}", bet);
        }
        #region Menus

        private void mnuNewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void mnuNextRound_Click(object sender, RoutedEventArgs e)
        {
            NextRound();
        }

        private void mnuHelp_Click(object sender, RoutedEventArgs e)
        {
            const string o = "Closest player to 21 (BlackJack) wins.\n\nGeneral advice: Stand on 17 or higher, otherwise Hit.\nThe dealer will Stand on 17.";
            MessageBox.Show(o, "Help...");
        }

        private void mnuPoints_Click(object sender, RoutedEventArgs e)
        {
            const string o = "Two:    2 points\nThree:  3 points\nFour:   4" + 
                             " points\nFive:   5 points\nSix:    " + 
                             "6 points\nSeven:  7 points\nEight:  8 points\nNine:   9 points" + 
                             "\nTen:    10 points\nJack:   10 points\nQueen:  10 points\n" + 
                             "King:   10 points\nAce:    1 or 11 points";
            MessageBox.Show(o.Replace("            ", String.Empty), "Point values of cards");
        }

        private void MnuDisplay_OnClick(object sender, RoutedEventArgs e)
        {
            new HighScores().ShowDialog();
        }

        private void MnuSubmit_OnClick(object sender, RoutedEventArgs e)
        {
            MnuNextRound.IsEnabled = false;
            MnuNewGame.IsEnabled = true;

            var s = new Submit { Score = _player.Chips };
            if (s.ShowDialog() == true) MnuSubmit.IsEnabled = false;
        }

        #endregion

        #region Buttons

        private void btnHit_Click(object sender, RoutedEventArgs e)
        {
            if (_player.Bet == 0) MessageBox.Show("You didn't bet anything!");
            else
            {
                btnBet.IsEnabled = false;
                btnStand.IsEnabled = true;
                btnSplit.IsEnabled = false;
                _player.Hit(_shoe, _dealer);

                if (_player.State == PlayerState.BlackJack) BlackJack();
                else if (_player.State == PlayerState.Bust)
                {
                    MessageBox.Show("You've gone bust! Your cards were " + _player.Points + " points!", "Player Bust!");
                    _player.TakeChips();
                    btnHit.IsEnabled = false;
                    btnStand.IsEnabled = false;
                    btnDouble.IsEnabled = false;
                    MnuNextRound.IsEnabled = true;

                    if (_player.State == PlayerState.OutOfChips) OutOfChips();
                    MnuSubmit.IsEnabled = true;
                }
            }
        }

        private void btnStand_Click(object sender, RoutedEventArgs e)
        {
            btnBet.IsEnabled = false;
            btnStand.IsEnabled = false;
            btnHit.IsEnabled = false;
            btnDouble.IsEnabled = false;

            _dealer.RevealCard();
            _dealerLogic.Start();
        }

        private void btnDouble_Click(object sender, RoutedEventArgs e)
        {
            var dbl = new Double { SldAmount = { Maximum = (_player.Chips - _player.Bet) } };
            if (dbl.ShowDialog() == true) _player.Double((int) dbl.SldAmount.Value, _shoe);
            else return;

            btnHit.IsEnabled = false;
            btnStand.IsEnabled = false;
            //btnSplit.IsEnabled = false;
            btnDouble.IsEnabled = false;

            /*
             * How a double works
             * Player can bet UP TO their initial bet, for one final card
             */

            _player.Double(_player.Bet, _shoe);

            if (_player.Points > 21) 
            { 
                _player.TakeChips(); 
                MessageBox.Show("Player went bust!");
                btnDouble.IsEnabled = false;
                MnuSubmit.IsEnabled = true;
            }

            else
            {
                _dealer.RevealCard();
                if ((_dealer.Points > _player.Points) && (_dealer.State != DealerState.Bust))
                {
                    MessageBox.Show("Dealer wins");
                    _player.TakeChips();
                    if (_player.State == PlayerState.OutOfChips) OutOfChips();
                    MnuSubmit.IsEnabled = true;
                }
                else
                {
                    MnuSubmit.IsEnabled = true;
                    MessageBox.Show("Player wins");
                    _player.AwardChips();
                    
                }
            }                      

            UpdateScores(_player.Chips, 0); 
            MnuNextRound.IsEnabled = true;
            MnuSubmit.IsEnabled = true;
        }

        private void btnSplit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBet_Click(object sender, RoutedEventArgs e)
        {
            if (_player.BetChips(_player.Bet + Bet))
            {
                UpdateScores(_player.Chips, _player.Bet);
                _player.BetChips(_player.Bet);
                btnStart.IsEnabled = true;
            }
            else MessageBox.Show("Not enough chips", "Error");
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_player.Bet > 0)
            {
                _dealer.Deal(_shoe, _player);

                btnBet.IsEnabled = false;
                btnHit.IsEnabled = true;
                btnDouble.IsEnabled = true;
                btnStart.IsEnabled = false;
                btnStand.IsEnabled = true;
                MnuSubmit.IsEnabled = false;
                //Should it be handled in the dealer class or th interface logic?
                if (_player.State == PlayerState.BlackJack) BlackJack(); 
                
            }
            else MessageBox.Show("Error", "You haven't made a bet");
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initalize the various classes on first run (to prevent ItemsSource from crashing due to NullReferenceException)
            NewGame();

            // Update the UI with images
            _player.Hand.CollectionChanged += CollectionChanged;
            _dealer.Hand.CollectionChanged += CollectionChanged;

            _dealerLogic = new DispatcherTimer();
            _rng = new Random(Environment.TickCount);

            _dealerLogic.Interval = new TimeSpan(0, 0, 0, 1);
            _dealerLogic.Tick += DealerLogic_Tick;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Image scaling: Doesn't Work.
            foreach (Image i in cvsDealer.Children) { i.Height = Height / 4.5; i.Width = Width / 7; }
            foreach (Image i in cvsPlayer.Children) { i.Height = Height / 4.5; i.Width = Width / 7; }
        }        

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            var image = sender as Image;
            if (image != null)
            {
                _sourceImage = image;
                _sourceImageIndex = Panel.GetZIndex(_sourceImage);
                Panel.SetZIndex(_sourceImage, 300);
            }
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_sourceImage != null)
            {
                Panel.SetZIndex(_sourceImage, _sourceImageIndex);
                _sourceImage = null;
                _sourceImageIndex = 0;
            }
        }

        void DealerLogic_Tick(object sender, EventArgs e)
        {
            // if dealer has same score as player, a "Push" happens: Player doesn't win but also doesn't lose any money
            if (_dealer.State != DealerState.Bust)
            {
                if (_dealer.State == DealerState.Standing)
                {
                    if ((_player.Points > _dealer.Points) && (_player.State != PlayerState.Bust))
                    {
                        // Player win: Dealer standing, with player higher score than dealer and isn't bust or out of chips.
                        _player.AwardChips();
                        MessageBox.Show(String.Format("Player: {0} points, Dealer: {1} points\n{2:C0} won.",
                            _player.Points, _dealer.Points, _player.Bet), "Player wins!");

                        _dealerLogic.Stop();
                        MnuSubmit.IsEnabled = true;
                        MnuNextRound.IsEnabled = true;
                    }
                    else if ((_dealer.Points > _player.Points))
                    {
                        // Player loss
                        _player.TakeChips();
                        MessageBox.Show(String.Format("Dealer had {0} points while player had {1} points.\nPlayer loses {2:C0}.",
                            _dealer.Points, _player.Points, _player.Bet), "Dealer wins");

                        _dealerLogic.Stop();
                        MnuSubmit.IsEnabled = true;
                        MnuNextRound.IsEnabled = true;
                        btnDouble.IsEnabled = false;

                        if (_player.State == PlayerState.OutOfChips) OutOfChips();
                    }
                    // Draw.
                    else if (_dealer.Points == _player.Points)
                    {
                        _dealer.Push(_player);

                        MessageBox.Show(String.Format("Dealer & Player had {0} points each. €0 lost.", _dealer.Points), "A push occurred");
                        _dealerLogic.Stop();
                        MnuNextRound.IsEnabled = true;
                        MnuSubmit.IsEnabled = true;
                    }
                }
                else
                {
                    // Thus, dealer is still in (Under 17 score)
                    _dealer.PlayCard(_shoe);
                }
            }
            else
            {
                // Dealer bust! If player isn't also bust, they win.

                // Player hasn't bust; player wins.
                if (_player.Points <= 21)
                {
                    _player.AwardChips();
                    MessageBox.Show(String.Format("Player awarded {0} chips.\nPlayer had {1} points, Dealer had {2} points",
                        _player.Bet, _player.Points, _dealer.Points), "Dealer bust!");
                    MnuNextRound.IsEnabled = true;
                }

                // Player has also bust.
                else
                {
                    _player.TakeChips();
                    MessageBox.Show(String.Format("Dealer bust, but player also went bust. Player loses {0} chips.",
                        _player.Bet.ToString()), "Dealer & Player bust");
                    MnuNextRound.IsEnabled = true;

                    if (_player.State == PlayerState.OutOfChips) OutOfChips();
                }
                _dealerLogic.Stop();
                MnuSubmit.IsEnabled = true;
            }
        }

        // Update UI whenever dealer/player's hand changes
        void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (cvsDealer.Children.Count != _dealer.Hand.Count)
            {
                cvsDealer.Children.Clear();
                foreach (Card c in _dealer.Hand) AddImage(cvsDealer, c);
            }
            if (cvsPlayer.Children.Count != _player.Hand.Count)
            {
                cvsPlayer.Children.Clear();
                foreach (Card c in _player.Hand) AddImage(cvsPlayer, c);
            }
        }
        #endregion
    }
}
