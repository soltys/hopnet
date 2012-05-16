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
                highScores.Add(new Score(i.ToString(CultureInfo.InvariantCulture), i));
            }
            Assert.AreEqual(10, highScores.Count());
        }

        [Test]
        public void only_highest_scores_are_stored_in_highscores()
        {
            var highScores = new HighScores();
            var randomGenerator = new Random();
            var generatedNumbers = new List<int>();
            for (int i = 0; i < 20; i++)
            {
                var number = randomGenerator.Next();
                highScores.Add(new Score(i.ToString(CultureInfo.InvariantCulture), number));
                generatedNumbers.Add(number);
            }

            Assert.AreEqual(generatedNumbers.OrderByDescending(x => x).Take(10).Min(), highScores.Min().Points);
        }
    }
}
