﻿using System.Collections.ObjectModel;
using System.Text;

namespace Blackjack
{
    enum DealerState
    {
        In,
        Standing,
        Bust
    }
    class Dealer {
        public ObservableCollection<Card> Cards { get; set; }
        public DealerState State { get; set; }

        public int Points
        {
            get
            {
                int points = 0, position = 0, numberOfAces = 0;
                ObservableCollection<Card> tempDeck = Cards;
                foreach (Card c in Cards) 
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

        public Dealer()
        {
            Cards = new ObservableCollection<Card>();
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
            Cards.Add(c);

            p.GiveCard(s.DrawCard());
            Cards.Add(s.DrawCard());

            if (p.Points == 21) p.State = PlayerState.BlackJack;
        }

        /// <summary>
        /// Reveal dealer's hole (hidden) card to facilitate UI update
        /// </summary>
        public void RevealCard()
        {
            Cards[0].Visible = Visibility.Visible;
            Card c = Cards[0];
            Cards.Remove(c);
            Cards.Insert(0, c);
        }

        /// <summary>
        /// Remove next card from shoe (deck) and add to dealer's hand.
        /// Dealer will stand on 17.
        /// </summary>
        /// <param name="s">The shoe object to draw a card from</param>
        public DealerState PlayCard(Shoe s)
        {
            // Dealer stands on 17  
            Cards.Add(s.DrawCard());
            if (Points > 21) State = DealerState.Bust;
            else if (Points >= 17) State = DealerState.Standing;
            return State;
        }

        /// <summary>
        /// Occurs when the Dealer has same score as the Player
        /// The player doesn't win, but also doesn't lose chips
        /// <param name="p">Player</param>
        public void Push(Player p) { p.CancelBet(); }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.Append("Dealer has a hand of: ");
            foreach (Card c in Cards)
            {
                if (c.Visible == Visibility.Visible) output.Append(c.ToString());
                else output.Append("Card Hidden");

                // Only append ", " for readability if it isn't the last card in the hand.
                if (Cards.IndexOf(c) + 1 != Cards.Count) output.Append(", ");
            }
            if (output.ToString() == "Dealer has a hand of: ") return "The dealer's hand is empty";
            else return output.ToString();

            // Output: "Dealer has a hand of: Nine of Diamonds, Four of Clubs, Jack of Clubs
        }

        internal void ClearCards()
        {
            Cards.Clear();
            
        }
    } 
}
