using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    class HighScores
    {
        private List<Score> scores;
        private const int maxCapacity = 10;
        public HighScores()
        {
            scores = new List<Score>(maxCapacity);
        }
        public void Add(Score score)
        {
            try
            {
                if (score.Points < 0)
                {
                    throw new ArgumentException();
                }
                if (scores.Count == scores.Capacity)
                {
                    if (scores.Min().Points < score.Points)
                    {
                        scores.Remove(scores.Min());
                    }
                    else
                    {
                        return;
                    }
                }
                scores.Add(score);
            }
            catch (NullReferenceException)
            {
                //TODO: Handlig it
                throw;
            }
            catch (ArgumentException)
            {
                //TODO: Handlig it
                throw;
            }
        }
        public void Clear()
        {
            scores.Clear();
        }
    }
}
