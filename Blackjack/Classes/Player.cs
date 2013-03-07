using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Blackjack
{   
    enum PlayerState
    {
        In,
        Standing,
        Bust,
        OutOfChips,
        BlackJack // Occurs when player gets 21
    }      

    class Player {
        public ObservableCollection<Card> Hand { get; private set; }
        public PlayerState State { get; set; }

        public int Chips { get; private set; }
        /// <summary>
        /// How many chips the player is currently betting
        /// </summary>
        public int Bet { get; private set; }

        public int Points 
        {
            get
            {
                int points = 0, position = 0, numberOfAces = 0;
                ObservableCollection<Card> tempDeck = Hand;
                foreach (Card c in Hand)
                {
                    string face = c.Face.ToString();
                    if (face == "Ace") { numberOfAces++; }
                    points += c.CardValue[face];
                }
                // To ensure the player can be saved from going bust if aces can be worth 1 instead of 11
                while ((points > 21) && (numberOfAces > 0) && (tempDeck.Count > 0))
                {
                    Card card = tempDeck[position];
                    //tempDeck.Remove(card);
                    if (card.Face.ToString() == "Ace")
                    {
                        points -= 10;
                        numberOfAces--;
                    }
                    position++;
                }
                return points;
            }
        }

        public Player()
        {    
            // Chips = 0;
            Chips = 100;
            Bet = 0;
            Hand = new ObservableCollection<Card>();
            State = PlayerState.In;
        }

        /// <summary>
        /// Award the player a specific number of chips.
        /// </summary>
        /// <param name="c">The number of chips to give to the player</param>
        /// <returns>The player's new number of chips</returns>
        public int AwardChips(int c) 
        { 
            Chips += c;
            return Chips;
        }

        /// <summary>
        /// Award the player the number of chips they've previously bet
        /// </summary>
        /// <returns></returns>
        public int AwardChips()
        {
            Chips += Bet;
            return Bet;
        }

        /// <summary>
        /// Takes from the player the number of chips they've previously bet
        /// </summary>
        /// <returns>The player's new number of chips</returns>
        public int TakeChips() 
        {             
            Chips -= Bet;

            if (Chips <= 0) State = PlayerState.OutOfChips;
            return Chips;
        }

        /// <summary>
        /// Takes a specific number of chips
        /// </summary>
        /// <param name="c">The number of chips to take from the player</param>
        /// <returns>The player's new number of chips</returns>
        public int TakeChips(int c)
        {
            Chips -= c;
            return Chips;
        }
        /// <summary>
        /// Bets chips
        /// </summary>
        /// <param name="c">The amount of chips to bet</param>
        /// <returns>Returns false if the player doesn't have enough chips to bet, otherwise true</returns>
        public bool BetChips(int c)
        {
            if (c <= Chips)
            {
                Bet = c;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Cancel the player's bet
        /// </summary>
        public void CancelBet() { Bet = 0; }

        /// <summary>
        /// Give a card to the player
        /// </summary>
        /// <param name="c">The card to give to the player</param>
        public void GiveCard(Card c)
        {   
            // Can't remove a non-existant card
            if (!Hand.Contains(c)) Hand.Add(c);
        }

        /// <summary>
        /// Removes a card from the player's hand
        /// </summary>
        /// <param name="c">The card to remove</param>
        public void TakeCard(Card c)
        {
            // Only give the player the card if they don't already have it
            if (Hand.Contains(c)) Hand.Remove(c);
        }

        /// <summary>
        /// Get the next card
        /// </summary>
        /// <param name="s">Shoe (remove card from here)</param>
        /// <param name="d">Dealer (to call the dealer method)</param>
        public PlayerState Hit(Shoe s, Dealer d) 
        {
            Hand.Add(s.DrawCard());
            if (Points == 21) State = PlayerState.BlackJack;
            else if (Points > 21) State = PlayerState.Bust;

            return State;
        }

        /// <summary>
        /// Player will take no further action
        /// </summary>
        public void Stand() 
        {
            throw new NotImplementedException("Uncoded");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Double(int newBet, Shoe s)
        {
            GiveCard(s.DrawCard());
            BetChips(Bet + newBet);
        }

        /// <summary>
        /// Split player's hand into two hands.
        /// Can only be performed on first turn.
        /// </summary>
        public void Split() 
        {
            throw new NotImplementedException("Uncoded");
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.Append("Player has a hand of: ");
            foreach (Card c in Hand)
            {
                if (c.Visible == Visibility.Visible) output.Append(c.ToString());
                else { output.Append("Card Hidden"); }

                // Only append ", " for readability if it isn't the last card in the hand.
                if (Hand.IndexOf(c) + 1 != Hand.Count) output.Append(", ");                
            }
            if (output.ToString() == "Player has a hand of: ") return "Player's hand is empty";
            else return output.ToString();

            // Output: "Player has a hand of: Nine of Diamonds, Four of Clubs, Jack of Clubs"
        }

        internal void ClearHand()
        {
            Hand.Clear();
        }
    }
}
