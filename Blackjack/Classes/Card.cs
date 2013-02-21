using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blackjack
{
    enum Suits
    {
        Hearts,
        Diamonds,
        Spades,
        Clubs
    }

    enum Faces
    {
        Two = 2, Three = 3, Four = 4,
        Five = 5, Six = 6, Seven = 7, Eight = 8,
        Nine = 9, Ten = 10, Jack = 10, Queen = 10, King = 10,
        Ace = 11
    }

    enum Visibility
    {
        Visible,
        Hidden
    }

    class Card
    {
        
        public string[] suits = new string[] { "Hearts", "Diamonds", "Spades", "Clubs" };
        public string[] faces = new string[] { "Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "King", "Queen" };

        // For deriving the actual value of a card.
        // Ace is 11 (or 1 when an ace will cause the player to go bust), Jack/Queen/King are 10, Four is 4, etc
        public Dictionary<string, int> faceValue = new Dictionary<string, int>() {
            {"Two", 2}, {"Three", 3}, {"Four", 4}, {"Five", 5}, {"Six", 6}, 
            {"Seven", 7}, {"Eight", 8}, {"Nine", 9}, {"Ten", 10}, 
            {"Jack", 10}, {"Queen", 10}, {"King", 10}, {"Ace", 11}
        };

        public Visibility Visible { get; set; }
        public Suits Suit { get; set; }
        public Faces Face { get; set; }

        /// <summary>
        /// Create random card object
        /// </summary>
        public Card()
        {
            // Create a random card
            Random r = new Random(Environment.TickCount);
            Suit = (Suits)Enum.Parse(typeof(Suits), suits[r.Next(suits.Length)]);
            Face = (Faces)Enum.Parse(typeof(Faces), faces[r.Next(suits.Length)]);
            Visible = Visibility.Visible;
        }

        /// <summary>
        /// Create a 'custom' card object
        /// </summary>
        /// <param name="s">The card's suit</param>
        /// <param name="f">The card's face</param>
        public Card(string s, string f)
        {
            Suit = (Suits)Enum.Parse(typeof(Suits), s);
            Face = (Faces)Enum.Parse(typeof(Faces), f);
            Visible = Visibility.Visible;
        }

        public override string ToString()
        {
            if (Visible == Visibility.Visible) return String.Format("{0} of {1}",
                Face.ToString(), Suit.ToString());
            else return "Card Hidden";
        }
    }
}
