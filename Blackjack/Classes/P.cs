using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Blackjack.Classes
{
    class P
    {
        public ObservableCollection<Card> Hand { get; set; } 
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

        public override string ToString()
        {
            string owner = GetType().ToString();
            StringBuilder output = new StringBuilder();
            output.Append(owner + " has a hand of: ");
            foreach (Card c in Hand)
            {
                if (c.Visible == Visibility.Visible) output.Append(c.ToString());
                else output.Append("Card Hidden");

                // Only append ", " for readability if it isn't the last card in the hand.
                if (Hand.IndexOf(c) + 1 != Hand.Count) output.Append(", ");
            }
            if (output.ToString() == owner + " has a hand of: ") return "The " + owner +"'s hand is empty";
            else return output.ToString();

            // Output: "[OWNER] has a hand of: Nine of Diamonds, Four of Clubs, Jack of Clubs
        }
    }

}
