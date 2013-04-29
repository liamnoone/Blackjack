using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Blackjack.Classes
{
    class Leaderboard
    {
        public BoardDataContext Board { get; set; }
        public string Name { get; set; }
        public short Score { get; set; }

        public Leaderboard()
        {
            Board = new BoardDataContext();
            Name = "";
            Score = 0;
        }

        public void SubmitScore(string name, short score)
        {
            Board.Leaderboards.InsertOnSubmit(new Blackjack.Leaderboard()
                {
                    Player_Name = Regex.Replace(name, @"[^a-zA-Z0-9\s]", ""),
                    Player_Score = score
                });
            Board.SubmitChanges();
        }

        public IQueryable<Blackjack.Leaderboard> GetScores()
        {
            return Board.Leaderboards.Select(l => l);
        }
    }
}

