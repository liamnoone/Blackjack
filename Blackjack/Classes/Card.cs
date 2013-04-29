using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Blackjack.Classes
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
        //Two = 2, Three = 3, Four = 4,
        //Five = 5, Six = 6, Seven = 7, Eight = 8,
        //Nine = 9, Ten = 10, Jack = 10, Queen = 10, King = 10,
        //Ace = 11
        Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace
    }

    enum Visibility
    {
        Visible,
        Hidden
    }

    class Card
    {        
        public static string[] CardSuits = new[] { "Hearts", "Diamonds", "Spades", "Clubs" };
        public static string[] CardFaces = new[] { "Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "King", "Queen" };
        
        // For deriving the actual value of a card.
        // Ace is 11 (or 1 when an ace will cause the player to go bust), Jack/Queen/King are 10, Four is 4, etc

        public Dictionary<string, int> CardValue = new Dictionary<string, int>
            {
            {"Two", 2}, {"Three", 3}, {"Four", 4}, {"Five", 5}, {"Six", 6}, 
            {"Seven", 7}, {"Eight", 8}, {"Nine", 9}, {"Ten", 10}, 
            {"Jack", 10}, {"Queen", 10}, {"King", 10}, {"Ace", 11}
        };

        public Visibility Visible { get; set; }
        public Suits Suit { get; set; }
        public Faces Face { get; set; }
        public Image ImageLocation
        {
            get
            {
                var src = new Image();

                string URI = Visible == Visibility.Hidden ? "/Images/Hidden.png" : String.Format("/Images/{0}/{1}.png", Suit.ToString(), Face.ToString());

                src.Source = new BitmapImage(new Uri(URI, UriKind.RelativeOrAbsolute));
                return src;
            }
        }

        /// <summary>
        /// Create random card object
        /// </summary>
        public Card()
        {
            // Create a random card
            Random r = new Random(Environment.TickCount);
            Suit = (Suits)Enum.Parse(typeof(Suits), CardSuits[r.Next(CardSuits.Length)]);
            Face = (Faces)Enum.Parse(typeof(Faces), CardFaces[r.Next(CardSuits.Length)]);
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

            // Prevent ambiguity between 10value cards
            //if (f == "Ten") Face = Faces.Ten;
            //else if (f == "Jack") Face = Faces.Jack;
            //else if (f == "Queen") Face = Faces.Queen;
            //else if (f == "King") Face = Faces.King;
            //else Face = (Faces)Enum.Parse(typeof(Faces), f);
            Face = (Faces)Enum.Parse(typeof(Faces), f);
            Visible = Visibility.Visible;
        }

        public override string ToString()
        {
            if (Visible == Visibility.Visible) return String.Format("{0} of {1}",
                Face.ToString(), Suit.ToString());
            return "Card Hidden";
        }
    }
}
