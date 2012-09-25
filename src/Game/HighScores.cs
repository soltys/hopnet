using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Game
{
    internal class HighScores : IEnumerable<Score>
    {
        private List<Score> scores;
        private readonly IsolatedStorageFile storage;
        private const string highScoreFileName = "highscores.xml";

        public HighScores()
        {
            scores = new List<Score>(GameConstants.MaxCapacity);
            storage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
        }
        public void Add(Score score)
        {
            if (score == null)
            {
                throw new ArgumentNullException();
            }
            if (score.Points < 0)
            {
                throw new ArgumentException();
            }

            if (scores.Count == GameConstants.MaxCapacity)
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
        public void Clear()
        {
            scores.Clear();
        }
        public void Load()
        {
            if (storage.FileExists(highScoreFileName))
            {
                var serializer = new XmlSerializer(typeof(List<Score>));
                using (var stream = new IsolatedStorageFileStream(highScoreFileName, FileMode.Open, storage))
                {
                    using (var reader = XmlReader.Create(stream))
                    {
                        var tmpScores = new List<Score>((List<Score>)serializer.Deserialize(reader));
                        if (tmpScores.Count > GameConstants.MaxCapacity)
                        {
                            throw new InvalidDataException("Amount of scores in file is greater than maxCapacity");
                        }
                        scores = new List<Score>(tmpScores);
                    }
                }
            }
        }

        public void SaveToFile()
        {
            var serializer = new XmlSerializer(typeof(List<Score>));
            using (var stream = new IsolatedStorageFileStream(highScoreFileName, FileMode.Create, storage))
            {
                using (var writer = new StreamWriter(stream))
                {
                    serializer.Serialize(writer, scores);
                }
            }
        }

        public IEnumerator<Score> GetEnumerator()
        {
            return scores.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return scores.GetEnumerator();
        }

    }
}