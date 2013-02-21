using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Blackjack
{
    class Shoe
    {
        // AKA the Deck
        List<Card> Contents { get; set; }

        /// <summary>
        /// Fill the shoe with a stock deck
        /// </summary>
        public Shoe()
        {            
            Contents = new List<Card>();
            reFill();
        }

        /// <summary>
        /// Get the top card from the deck
        /// </summary>
        /// <returns>Card object at first index of the deck</returns>
        public Card drawCard()
        {
            if (Contents.Count != 0)
            {
                Card c = Contents[0];
                Contents.Remove(c);
                return c;
            }
            else throw new Exception("No cards in Shoe!");
        }

        /// <summary>
        /// Shuffles the deck (puts the cards of the current deck into different 
        /// positions of a new deck, and replace the current deck with it)
        /// </summary>
        /// Seems complete.
        public void shuffle()
        {
            Random rng = new Random(Environment.TickCount);
            List<Card> newDeck = new List<Card>();
            Card card;
            while (Contents.Count != 0)
            {
                card = Contents[rng.Next(Contents.Count)];
                newDeck.Add(card);
                Contents.Remove(card);
            }
            Contents = newDeck;
        }

        /// <summary>
        /// Restores deck to default state
        /// </summary>
        public void reFill()
        {
            Contents.Clear();
            Card helperCard = new Card();
            foreach (string suit in helperCard.suits)
            {
                foreach (string face in helperCard.faces)
                {
                    Card c = new Card(suit, face);
                    Contents.Add(c);
                }
            }
        }

        public void remove(Card c)
        {
            if (Contents.Contains(c)) Contents.Remove(c);
            else { throw new NotImplementedException(); }
        }

        public override string ToString()
        {
            return String.Format("The shoe contains {0} cards", (Contents.Count == 0) ? "no" : Contents.Count.ToString());
        }
    }
}
