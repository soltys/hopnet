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
            highScores.Add(new Score(777));
            Assert.AreEqual(1, highScores.Count());
        }

        [Test]
        public void added_20_scores_to_highscores_result_10_scores()
        {
            var highScores = new HighScores();
            for (int i = 0; i < 20; i++)
            {
                highScores.Add(new Score(i));
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
                highScores.Add(new Score(number));
                generatedNumbers.Add(number);
            }

            Assert.AreEqual(generatedNumbers.OrderByDescending(x => x).Take(10).Min(), highScores.Min().Points);
        }

        [Test]
        public void when_score_added_is_lower_than_zero_throws_argument_exception()
        {
            var highScores = new HighScores();
            var score = new Score(-1);
            Assert.Throws<ArgumentException>(() => highScores.Add(score));
        }

        [Test]
        public void when_score_added_is_null_then_highscore_throws_argument_exception()
        {
            var highScores = new HighScores();
            Score score = null;
            Assert.Throws<ArgumentNullException>(() => highScores.Add(score));
        }

        [Test]
        public void highscore_cleared_highscore_is_empty()
        {
            var highScores = new HighScores();
            for (int i = 0; i < 20; i++)
            {
                highScores.Add(new Score(i));
            }
            highScores.Clear();

            CollectionAssert.IsEmpty(highScores);
        }
    }
}
