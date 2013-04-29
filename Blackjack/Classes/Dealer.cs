using System.Collections.ObjectModel;
using System.Text;

namespace Blackjack.Classes
{
    enum DealerState
    {
        In,
        Standing,
        Bust
    }
    class Dealer : P
    {
        public DealerState State { get; set; }

        public Dealer()
        {
            Hand = new ObservableCollection<Card>();
            State = DealerState.In;
        }

        /// <summary>
        /// Deal out cards to player and dealer (player, dealer, player, dealer)
        /// </summary>
        /// <param name="s">Shoe</param>
        /// <param name="p">Player</param>
        public void Deal(Shoe s, Player p)
        {
            // Deal to player, then dealer (hidden), then player, then dealer
            p.GiveCard(s.DrawCard());

            Card c = s.DrawCard();
            c.Visible = Visibility.Hidden;
            Hand.Add(c);

            p.GiveCard(s.DrawCard());
            Hand.Add(s.DrawCard());

            if (p.Points == 21) p.State = PlayerState.BlackJack;
        }

        /// <summary>
        /// Reveal dealer's hole (hidden) card to facilitate UI update
        /// </summary>
        public void RevealCard()
        {
            Hand[0].Visible = Visibility.Visible;
            Card c = Hand[0];
            Hand.Remove(c);
            Hand.Insert(0, c);
        }

        /// <summary>
        /// Remove next card from shoe (deck) and add to dealer's hand.
        /// Dealer will stand on 17.
        /// </summary>
        /// <param name="s">The shoe object to draw a card from</param>
        public DealerState PlayCard(Shoe s)
        {
            // Dealer stands on 17  
            Hand.Add(s.DrawCard());
            if (Points > 21) State = DealerState.Bust;
            else if (Points >= 17) State = DealerState.Standing;
            return State;
        }

        /// <summary>
        /// Occurs when the Dealer has same score as the Player
        /// The player doesn't win, but also doesn't lose chips
        /// <param name="p">Player</param>
        /// </summary>
        public void Push(Player p) { p.CancelBet(); }

        internal void ClearCards()
        {
            Hand.Clear();

        }
    }
}
