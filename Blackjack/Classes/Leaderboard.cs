using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Blackjack.Classes
{
    class Leaderboard
    {
        public DatabaseContext Board { get; set; }
        public string Name { get; set; }
        public short Score { get; set; }

        public Leaderboard()
        {
            Board = new DatabaseContext("Database.sdf");
            Name = "";
            Score = 0;
        }

        public void SubmitScore(string name, short score)
        {
            Board.Scores.InsertOnSubmit(new Scores
                {
                    Player_Name = Regex.Replace(name, @"[^a-zA-Z0-9\s]", ""),
                    Player_Score = score
                });
            Board.SubmitChanges();
        }

        public IQueryable<Scores> GetScores()
        {
            return Board.Scores.Select(s => s);
        }
    }
}

