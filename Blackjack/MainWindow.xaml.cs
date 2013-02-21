using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Blackjack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int BET = 5;
        DispatcherTimer DealerLogic;

        Random RNG;
        Player player;
        Shoe shoe;
        Dealer dealer;
        Int32 bet = 0;

        public MainWindow()
        {            
            InitializeComponent();
        }
        //private void updateDealer()
        //{
        //    Card c = dealer.Cards[0];
        //    dealer.Cards.Remove(c);
        //    dealer.Cards.Insert(0, c);
        //}
        private void btnHit_Click(object sender, RoutedEventArgs e)
        {
            if (player.BetChips == 0) MessageBox.Show("You didn't bet anything!");
            else
            {
                btnBet.IsEnabled = false;
                btnStand.IsEnabled = true;
                btnSplit.IsEnabled = false;

                if (player.State == PlayerState.In)
                {
                    if (player.hit(shoe, dealer) == PlayerState.Bust)
                    {
                        MessageBox.Show("You've gone bust! Your cards were " + player.Score + " points!");
                        player.takeChips();
                        btnHit.IsEnabled = false;
                        btnStand.IsEnabled = false;

                        if (player.State == PlayerState.OutOfChips)
                        {
                            MessageBox.Show("Player is out of chips.", "Out of Chips");
                            tbkPlayerChips.Text = "Player's Chips: €0.00";
                            tbkPlayerBet.Text = "Player's Bet: €0.00";
                            this.Title = "Game Over";

                            btnBet.IsEnabled = false;
                            mnuNextRound.IsEnabled = false;
                        }
                    }
                }
            }
        }

        private void btnStand_Click(object sender, RoutedEventArgs e)
        {
            btnBet.IsEnabled = false;
            btnStand.IsEnabled = false;
            btnHit.IsEnabled = false;
            
            // To change from "Hidden Card" to the actual card to prevent usage of IPropertyChangedNotification
            dealer.update(); 
            DealerLogic.Start();
        }

        private void btnDouble_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnSplit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initalize the various classes on first run (to prevent ItemsSource from crashing due to NullReferenceException)
            newGame();            

            lvwPlayerCards.ItemsSource = player.Hand;
            lvwDealerCards.ItemsSource = dealer.Cards;
            
            DealerLogic = new DispatcherTimer();
            RNG = new Random(Environment.TickCount);

            // Dealer will play a card every half a second?
            DealerLogic.Interval = new TimeSpan(0, 0, 0, 1);
            DealerLogic.Tick += new EventHandler(DealerLogic_Tick);
        }

        void DealerLogic_Tick(object sender, EventArgs e)
        {
            // if dealer has same score as player, a "Push happens": Player doesn't win but also doesn't lose any money
            if (dealer.State != DealerState.Bust)
            {
                if (dealer.State == DealerState.Standing)
                {
                    // For some reason a bug occurs when dealer has 26 points and still wins.

                    // Will the dealer only get an action when the player is standing?                    
                    if ((player.Score > dealer.Points) && (player.State != PlayerState.Bust))
                    //if ((player.Points > dealer.Points) && ((player.State != PlayerState.Bust) && (player.State != PlayerState.OutOfChips))
                    {
                        // Player win: Dealer standing, with player higher score than dealer and isn't bust or out of chips.
                        player.awardChips();
                        MessageBox.Show(String.Format("Player: {0} points, Dealer: {1} points\n{2:C} won.", 
                            player.Score, dealer.Points, player.BetChips), "Player wins!");

                        DealerLogic.Stop();
                    }
                    else if ((dealer.Points > player.Score))
                    {
                        // Player loss
                        player.takeChips();
                        MessageBox.Show(String.Format("Dealer had {0} points while player had {1} points.\nPlayer loses {2:C}.", 
                            dealer.Points, player.Score, player.BetChips), "Dealer wins");

                        DealerLogic.Stop();
                    }
                    // Draw.
                    else if (dealer.Points == player.Score) 
                    { 
                        dealer.push(player); 

                        MessageBox.Show("Dealer & Player had {0} points each. €0 lost.", "A push occurred");
                        DealerLogic.Stop();
                    }
                }
                else
                {
                    // Thus, dealer is still in (Under 17 score)
                    dealer.playCard(shoe);
                }
            }
            else 
            { 
                // Dealer bust! If player isn't also bust, they win.
                // TODO: Ensure this works.

                // Player hasn't bust; player wins.
                if (player.Score <= 21) 
                { 
                    player.awardChips();
                    MessageBox.Show(String.Format("Player awarded {0} chips.\nPlayer had {1} points, Dealer had {2} points", 
                        player.BetChips, player.Score, dealer.Points), "Dealer bust!");
                }

                // Player has also bust.
                else
                {
                    player.takeChips();
                    MessageBox.Show(String.Format("Dealer bust, but player also went bust. Player loses {0} chips.", 
                        player.BetChips.ToString()), "Dealer & Player bust");
                }
                DealerLogic.Stop();
            }
        }

        /// <summary>
        /// Reset the player, dealer and shoe to default state 
        /// Deck is then shuffled: can't simply shuffle incase Deck contains under 52 cards
        /// This allows for a new game to be started
        /// </summary>
        private void newGame()
        {
            dealer = new Dealer();
            player = new Player();
            shoe = new Shoe();
            bet = 0;

            shoe.shuffle();

            btnHit.IsEnabled = false;
            btnBet.IsEnabled = true;
            btnStart.IsEnabled = false;

            btnStand.IsEnabled = false;
            //btnSplit.IsEnabled = false;
            //btnDouble.IsEnabled = false;
        }
        /// <summary>
        /// Setup game for another round (restart button states, but don't fully reset objects)
        /// </summary>
        private void nextRound()
        {
            shoe.reFill();
            shoe.shuffle();
            dealer.deal(shoe, player);
            player.betChips(0);
            tbkPlayerChips.Text = String.Format("Player's Chips: {0:C}", player.Chips);
            tbkPlayerBet.Text = String.Format("Player's Bet: {0:C}", 0);
            bet = 0;
            player.clearHand();
            dealer.clearCards();
            DealerLogic.Stop();

            player.State = PlayerState.In;
            dealer.State = DealerState.In;

            btnBet.IsEnabled = true;
            btnHit.IsEnabled = false;  
            btnStand.IsEnabled = false;
            //btnSplit.IsEnabled = true;
            //btnDouble.IsEnabled = true;
        }

        private void btnBet_Click(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = true;
            bet += BET;
            if (bet > player.Chips) MessageBox.Show("Not enough chips", "Error");
            else
            {
                tbkPlayerBet.Text = "Player's Bet: " + String.Format("{0:C}", bet);
                player.betChips(bet);
            }
        }

        private void cardDetails(object sender, MouseButtonEventArgs e)
        {
            ListView l = (ListView)sender;
            if (l.SelectedIndex != -1)
            {
                Card c = (Card)l.SelectedItem;
                string f = c.Face.ToString();
                if (c.Visible == Blackjack.Visibility.Visible)
                    MessageBox.Show(String.Format("{0} {1} is worth {2} points", 
                        (f == "Eight" || f == "Ace") ? "An" : "A", 
                        f.ToLower(), 
                        f == "Ace" ? "1 or 11" : c.faceValue[f].ToString()
                        ));
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            newGame();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (player.BetChips > 0)
            {
                dealer.deal(shoe, player);

                btnBet.IsEnabled = false;
                btnHit.IsEnabled = true;
                btnStart.IsEnabled = false;
                btnStand.IsEnabled = true;
            }
            else MessageBox.Show("Error", "You haven't made a bet");
        }

        private void nextRound(object sender, RoutedEventArgs e)
        {
            nextRound();
        }
    }
}
