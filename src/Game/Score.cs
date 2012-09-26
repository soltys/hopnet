using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [Serializable]
    public class Score : IComparable<Score>
    {
        public DateTime Time { get; set; }
        public int Points { get; set; }
        private Score()
        {
        }
        public Score(int points)
        {
            Points = points;
            Time = DateTime.Now;
        }
        public int CompareTo(Score other)
        {
            if (other == null)
            {
                return 1;
            }
            return Points.CompareTo(other.Points);
        }
    }
}
