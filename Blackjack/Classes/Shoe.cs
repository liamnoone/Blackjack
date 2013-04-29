using System;
using System.Collections.Generic;

namespace Blackjack.Classes
{
    class Shoe
    {
        List<Card> Contents { get; set; }

        public Card this[int index]
        {
            get
            {
                if (index <= Contents.Count) return Contents[index];
                throw new IndexOutOfRangeException("The Shoe doesn't contain that many cards");
            }
            set { 
                Contents[index] = value; 
            }
        }

        /// <summary>
        /// Fill the shoe with a stock deck
        /// </summary>
        public Shoe()
        {            
            Contents = new List<Card>();
            ReFill();
        }

        /// <summary>
        /// Get the top card from the deck
        /// </summary>
        /// <returns>Card object at first index of the deck</returns>
        public Card DrawCard()
        {
            if (Contents.Count != 0)
            {
                Card c = Contents[0];
                Contents.Remove(c);
                return c;
            }
            throw new Exception("No cards in Shoe!");
        }

        /// <summary>
        /// Shuffles the deck (puts the cards of the current deck into different 
        /// positions of a new deck, and replace the current deck with it)
        /// </summary>
        /// Seems complete.
        public void Shuffle()
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
        public void ReFill()
        {
            Contents.Clear();
            foreach (string suit in Card.CardSuits)
            {
                foreach (string face in Card.CardFaces)
                {
                    Card c = new Card(suit, face);
                    Contents.Add(c);
                }
            }
        }

        public void Remove(Card c)
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
