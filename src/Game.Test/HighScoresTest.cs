using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Game.Test
{
    [TestFixture]
    class HighScoresTest
    {
        [Test]
        public void new_highscores_is_empty()
        {
            var highScores = new HighScores();
            CollectionAssert.IsEmpty(highScores);
        }

        [Test]
        public void add_one_score_results_one_score_in_highscores()
        {
            var highScores = new HighScores();
            highScores.Add(new Score("hopnet", 777));
            Assert.AreEqual(1, highScores.Count());
        }

        [Test]
        public void added_20_scores_to_highscores_result_10_scores()
        {
            var highScores = new HighScores();
            for (int i = 0; i < 20; i++)
            {
                highScores.Add(new Score(i.ToString(CultureInfo.InvariantCulture), i * 100));
            }
            Assert.AreEqual(10, highScores.Count());
        }
    }
}
