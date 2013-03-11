using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Blackjack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int sourceImageIndex;
        Image sourceImage;
        /// <summary>
        /// How much a bet is worth
        /// </summary>
        const int BET = 5;
        DispatcherTimer DealerLogic;

        Random RNG;
        Player player;
        Shoe shoe;
        Dealer dealer;

        public MainWindow()
        {            
            InitializeComponent();
        }  

        /// <summary>
        /// Reset the player, dealer and shoe to default state 
        /// Deck is then shuffled: can't simply shuffle incase Deck contains under 52 cards
        /// This allows for a new game to be started
        /// </summary>
        private void newGame()
        {
            cvsPlayer.Children.Clear();
            cvsDealer.Children.Clear();

            dealer = new Dealer();
            player = new Player();
            shoe = new Shoe();
            updateScores(100, 0);
            shoe.Shuffle();

            btnHit.IsEnabled = false;
            btnStand.IsEnabled = false;
            btnDouble.IsEnabled = false;
            //btnSplit.IsEnabled = false;

            btnBet.IsEnabled = true;
            btnStart.IsEnabled = false;

            mnuNextRound.IsEnabled = false;

            Title = "Blackjack";
            player.Hand.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChanged);
            dealer.Cards.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChanged);
        }

        /// <summary>
        /// Setup game for another round (restart button states, but don't fully reset objects)
        /// </summary>
        private void nextRound()
        {
            player.ClearHand();
            dealer.ClearCards();

            shoe.ReFill();
            shoe.Shuffle();
            player.BetChips(0);
            updateScores(player.Chips, 0);
            DealerLogic.Stop();

            player.State = PlayerState.In;
            dealer.State = DealerState.In;

            btnBet.IsEnabled = true;
            btnHit.IsEnabled = false;  
            btnStand.IsEnabled = false;

            mnuNextRound.IsEnabled = false;
        }  

        private void addImage(Canvas cvs, Card c)
        {
            Image i = c.ImageLocation;
            i.MouseEnter += new MouseEventHandler(Image_MouseEnter);
            i.MouseLeave += new MouseEventHandler(Image_MouseLeave);
            int numChildren = cvs.Children.Count;
            cvs.Children.Add(i);
            Canvas.SetLeft(cvs.Children[numChildren], numChildren * 20);
        }
        
        void blackJack() 
        {
            if ((player.Points == 21) && (player.Bet > 0))
            {
                player.AwardChips();
                MessageBox.Show(String.Format("Blackjack! Player wins {0} chips.", player.Bet), "Blackjack!");

                btnHit.IsEnabled = false;
                btnStand.IsEnabled = false;
                btnDouble.IsEnabled = false;
                mnuNextRound.IsEnabled = true;
                player.BetChips(0);
            }
        }
        
        private void outOfChips()
        {
            MessageBox.Show("Player is out of chips.", "Out of Chips");
            updateScores(player.Chips, 0);
            this.Title = "Game Over: Out of Chips";

            btnBet.IsEnabled = false;
            btnStand.IsEnabled = false;
            btnSplit.IsEnabled = false;
            btnDouble.IsEnabled = false;
            mnuNextRound.IsEnabled = false;
        }
        /// <summary>
        /// Update the UI with player's new details
        /// </summary>
        /// <param name="chips">How many chips to display</param>
        /// <param name="bet">How many chips currently being bet to display</param>
        private void updateScores(int chips, int bet)
        {
            tbkPlayerChips.Text = String.Format("Player's Chips: {0:C0}", chips);
            tbkPlayerBet.Text = String.Format("Player's Bet: {0:C0}", bet);
        }
        #region Menus

        private void mnuNewGame_Click(object sender, RoutedEventArgs e)
        {
            newGame();
        }

        private void mnuNextRound_Click(object sender, RoutedEventArgs e)
        {
            nextRound();
        }

        private void mnuHelp_Click(object sender, RoutedEventArgs e)
        {
            string o = "Closest player to 21 (BlackJack) wins.\n\nGeneral advice: Stand on 17 or higher, otherwise Hit.\nThe dealer will Stand on 17.";
            MessageBox.Show(o, "Help...");
        }

        private void mnuPoints_Click(object sender, RoutedEventArgs e)
        {
            string o = @"
            Two:    2 points
            Three:  3 points
            Four:   4 points
            Five:   5 points
            Six:    6 points
            Seven:  7 points
            Eight:  8 points
            Nine:   9 points
            Ten:    10 points
            Jack:   10 points
            Queen:  10 points
            King:   10 points
            Ace:    1 or 11 points";
            MessageBox.Show(o.Replace("            ", String.Empty), "Point values of cards");
        }

        #endregion

        #region Buttons

        private void btnHit_Click(object sender, RoutedEventArgs e)
        {
            if (player.Bet == 0) MessageBox.Show("You didn't bet anything!");
            else
            {
                btnBet.IsEnabled = false;
                btnStand.IsEnabled = true;
                btnSplit.IsEnabled = false;
                player.Hit(shoe, dealer);

                if (player.State == PlayerState.BlackJack) blackJack();
                else if (player.State == PlayerState.Bust)
                {
                    MessageBox.Show("You've gone bust! Your cards were " + player.Points + " points!", "Player Bust!");
                    player.TakeChips();
                    btnHit.IsEnabled = false;
                    btnStand.IsEnabled = false;
                    mnuNextRound.IsEnabled = true;

                    if (player.State == PlayerState.OutOfChips) outOfChips();
                }
            }
        }

        private void btnStand_Click(object sender, RoutedEventArgs e)
        {
            btnBet.IsEnabled = false;
            btnStand.IsEnabled = false;
            btnHit.IsEnabled = false;
            dealer.RevealCard();
            DealerLogic.Start();
        }

        private void btnDouble_Click(object sender, RoutedEventArgs e)
        {
            btnHit.IsEnabled = false;
            btnStand.IsEnabled = false;
            //btnSplit.IsEnabled = false;
            btnDouble.IsEnabled = false;

            /*
             * How a double works
             * Player can bet UP TO their initial bet, for one final card
             */
            // Just actually double player's bet for now (input for actual bet isn't created yet)
            player.Double(player.Bet, shoe);
            // Don't need to account for blackjack? (if they win, they win) 
            // Can blackjack even occur with combos other than a 10card and an ace?
            // if (player.Points == 21) blackJack();
            if (player.Points > 21) 
            { 
                player.TakeChips(); 
                MessageBox.Show("Player went bust!"); 
            }

            else
            {
                dealer.RevealCard();
                if ((dealer.Points > player.Points) && (dealer.State != DealerState.Bust))
                {
                    MessageBox.Show("Dealer wins");
                    player.TakeChips();
                    if (player.State == PlayerState.OutOfChips) outOfChips();
                }
                else { 
                    MessageBox.Show("Player wins");
                    player.AwardChips();
                }
            }                      

            updateScores(player.Chips, 0); 
            mnuNextRound.IsEnabled = true; 
        }

        private void btnSplit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBet_Click(object sender, RoutedEventArgs e)
        {
            if (player.BetChips(player.Bet + BET))
            {
                updateScores(player.Chips, player.Bet);
                player.BetChips(player.Bet);
                btnStart.IsEnabled = true;
                
            }
            else MessageBox.Show("Not enough chips", "Error");
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (player.Bet > 0)
            {
                dealer.Deal(shoe, player);

                btnBet.IsEnabled = false;
                btnHit.IsEnabled = true;
                btnDouble.IsEnabled = true;
                btnStart.IsEnabled = false;
                btnStand.IsEnabled = true;
                //Should it be handled in the dealer class or th interface logic?
                if (player.State == PlayerState.BlackJack) blackJack(); 
                
            }
            else MessageBox.Show("Error", "You haven't made a bet");
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initalize the various classes on first run (to prevent ItemsSource from crashing due to NullReferenceException)
            newGame();

            // Update the UI with images
            player.Hand.CollectionChanged +=
                new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChanged);
            dealer.Cards.CollectionChanged +=
                new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChanged);

            DealerLogic = new DispatcherTimer();
            RNG = new Random(Environment.TickCount);

            DealerLogic.Interval = new TimeSpan(0, 0, 0, 1);
            DealerLogic.Tick += new EventHandler(DealerLogic_Tick);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Image scaling
            foreach (Image i in cvsDealer.Children) { i.Height = Height / 4.5; i.Width = Width / 7; }
            foreach (Image i in cvsPlayer.Children) { i.Height = Height / 4.5; i.Width = Width / 7; }
        }        

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Image)
            {
                sourceImage = (Image)sender;
                sourceImageIndex = Panel.GetZIndex(sourceImage);
                Panel.SetZIndex(sourceImage, 300);
            }
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sourceImage != null)
            {
                Panel.SetZIndex(sourceImage, sourceImageIndex);
                sourceImage = null;
                sourceImageIndex = 0;
            }
        }

        void DealerLogic_Tick(object sender, EventArgs e)
        {
            // if dealer has same score as player, a "Push" happens: Player doesn't win but also doesn't lose any money
            if (dealer.State != DealerState.Bust)
            {
                if (dealer.State == DealerState.Standing)
                {
                    if ((player.Points > dealer.Points) && (player.State != PlayerState.Bust))
                    {
                        // Player win: Dealer standing, with player higher score than dealer and isn't bust or out of chips.
                        player.AwardChips();
                        MessageBox.Show(String.Format("Player: {0} points, Dealer: {1} points\n{2:C0} won.",
                            player.Points, dealer.Points, player.Bet), "Player wins!");

                        DealerLogic.Stop();
                        mnuNextRound.IsEnabled = true;
                    }
                    else if ((dealer.Points > player.Points))
                    {
                        // Player loss
                        player.TakeChips();
                        MessageBox.Show(String.Format("Dealer had {0} points while player had {1} points.\nPlayer loses {2:C0}.",
                            dealer.Points, player.Points, player.Bet), "Dealer wins");

                        DealerLogic.Stop();
                        mnuNextRound.IsEnabled = true;

                        if (player.State == PlayerState.OutOfChips) outOfChips();
                    }
                    // Draw.
                    else if (dealer.Points == player.Points)
                    {
                        dealer.Push(player);

                        MessageBox.Show(String.Format("Dealer & Player had {0} points each. €0 lost.", dealer.Points), "A push occurred");
                        DealerLogic.Stop();
                        mnuNextRound.IsEnabled = true;
                    }
                }
                else
                {
                    // Thus, dealer is still in (Under 17 score)
                    dealer.PlayCard(shoe);
                }
            }
            else
            {
                // Dealer bust! If player isn't also bust, they win.

                // Player hasn't bust; player wins.
                if (player.Points <= 21)
                {
                    player.AwardChips();
                    MessageBox.Show(String.Format("Player awarded {0} chips.\nPlayer had {1} points, Dealer had {2} points",
                        player.Bet, player.Points, dealer.Points), "Dealer bust!");
                    mnuNextRound.IsEnabled = true;
                }

                // Player has also bust.
                else
                {
                    player.TakeChips();
                    MessageBox.Show(String.Format("Dealer bust, but player also went bust. Player loses {0} chips.",
                        player.Bet.ToString()), "Dealer & Player bust");
                    mnuNextRound.IsEnabled = true;

                    if (player.State == PlayerState.OutOfChips) outOfChips();
                }
                DealerLogic.Stop();
            }
        }

        // Update UI whenever dealer/player's hand changes
        void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (cvsDealer.Children.Count != dealer.Cards.Count)
            {
                cvsDealer.Children.Clear();
                foreach (Card c in dealer.Cards) addImage(cvsDealer, c);
            }
            if (cvsPlayer.Children.Count != player.Hand.Count)
            {
                cvsPlayer.Children.Clear();
                foreach (Card c in player.Hand) addImage(cvsPlayer, c);
            }
        }
        #endregion
    }
}
