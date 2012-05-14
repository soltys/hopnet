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
    }
}
