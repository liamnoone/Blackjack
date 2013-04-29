using System;
using System.Collections.ObjectModel;

namespace Blackjack.Classes
{   
    enum PlayerState
    {
        In,
        Standing,
        Bust,
        OutOfChips,
        BlackJack // Occurs when player gets 21
    }      

    class Player : P 
    {
        public PlayerState State { get; set; }

        public int Chips { get; private set; }
        /// <summary>
        /// How many chips the player is currently betting
        /// </summary>
        public int Bet { get; private set; }

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
            return false;
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

        internal void ClearHand()
        {
            Hand.Clear();
        }
    }
}
