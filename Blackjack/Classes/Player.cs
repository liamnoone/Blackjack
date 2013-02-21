using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Blackjack
{   
    enum PlayerState
    {
        In,
        Standing,
        Bust,
        OutOfChips
    }      

    class Player {
        // To disable Split() when it's the player's second turn onwards (Split can only be done on turn one)
        public int Turn { get; set; }
        public ObservableCollection<Card> Hand { get; private set; }
        public PlayerState State { get; set; }

        public int Chips { get; private set; }
        public int BetChips { get; private set; }

        public int Score 
        {
            get
            {
                int points = 0, position = 0, numberOfAces = 0;
                ObservableCollection<Card> tempDeck = Hand;
                foreach (Card c in Hand)
                {
                    string face = c.Face.ToString();
                    if (face == "Ace") { numberOfAces++; }
                    points += c.faceValue[face];
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
            BetChips = 0;
            Hand = new ObservableCollection<Card>();
            State = PlayerState.In;
        }

        /// <summary>
        /// Award the player a specific number of chips.
        /// </summary>
        /// <param name="c">The number of chips to give to the player</param>
        /// <returns>The player's new number of chips</returns>
        public int awardChips(int c) 
        { 
            Chips += c;
            return Chips;
        }

        /// <summary>
        /// Award the player the number of chips they've previously bet
        /// </summary>
        /// <returns></returns>
        public int awardChips()
        {
            Chips += BetChips;
            return BetChips;
        }

        /// <summary>
        /// Takes from the player the number of chips they've previously bet
        /// </summary>
        /// <returns>The player's new number of chips</returns>
        public int takeChips() 
        {             
            Chips -= BetChips;

            if (Chips <= 0) State = PlayerState.OutOfChips;
            return Chips;
        }

        /// <summary>
        /// Takes a specific number of chips
        /// </summary>
        /// <param name="c">The number of chips to take from the player</param>
        /// <returns>The player's new number of chips</returns>
        public int takeChips(int c)
        {
            Chips -= c;
            return Chips;
        }
        
        public void betChips(int c) {  BetChips = c; }

        /// <summary>
        /// Cancel the player's bet
        /// </summary>
        public void cancelBet() { BetChips = 0; }

        /// <summary>
        /// Give a card to the player
        /// </summary>
        /// <param name="c">The card to give to the player</param>
        public void giveCard(Card c)
        {   
            // Can't remove a non-existant card
            if (!Hand.Contains(c)) Hand.Add(c);
        }

        /// <summary>
        /// Removes a card from the player's hand
        /// </summary>
        /// <param name="c">The card to remove</param>
        public void takeCard(Card c)
        {
            // Only give the player the card if they don't already have it
            if (Hand.Contains(c)) Hand.Remove(c);
        }

        /// <summary>
        /// Get the next card
        /// </summary>
        /// <param name="s">Shoe (remove card from here)</param>
        /// <param name="d">Dealer (to call the dealer method)</param>
        public PlayerState hit(Shoe s, Dealer d) 
        {
            Hand.Add(s.drawCard());

            if (Score > 21) State = PlayerState.Bust;

            return State;
        }

        /// <summary>
        /// Player will take no further action
        /// </summary>
        public void stand() 
        {
            throw new NotImplementedException("Uncoded");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Double()
        {
            throw new NotImplementedException("Uncoded");
        }

        /// <summary>
        /// Split player's hand into two hands.
        /// Can only be performed on first turn.
        /// </summary>
        public void split() 
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

        internal void clearHand()
        {
            Hand.Clear();
        }
    }
}
