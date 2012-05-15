using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Security;
using System.Xml;
using System.Xml.Serialization;

namespace Game
{
    internal class HighScores : IEnumerable<Score>
    {
        private List<Score> scores;
        private const int maxCapacity = 10;
        public HighScores()
        {
            scores = new List<Score>(maxCapacity);
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

            try
            {
                if (scores.Count == maxCapacity)
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
        public void Load()
        {
            try
            {
                var storage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                if (storage.FileExists("highscores.xml"))
                {
                    var serializer = new XmlSerializer(typeof(List<Score>));
                    using (var stream = new IsolatedStorageFileStream("highscores.xml", FileMode.Open, storage))
                    {
                        using (var reader = XmlReader.Create(stream))
                        {
                            var tmpScores = new List<Score>((List<Score>)serializer.Deserialize(reader));
                            if (tmpScores.Count > maxCapacity)
                            {
                                throw new InvalidDataException();
                            }
                            else
                            {
                                scores = new List<Score>(tmpScores);
                            }
                        }
                    }
                }
            }
            catch (IsolatedStorageException)
            {
                //TODO: Handlig it
                throw;
            }
            catch (IOException)
            {
                //TODO: Handlig it
                throw;
            }
            catch (InvalidOperationException)
            {
                //TODO: Handlig it
                throw;
            }
            catch (SecurityException)
            {
                //TODO: Handlig it
                throw;
            }
        }
        public void Save()
        {
            try
            {
                var storage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                var serializer = new XmlSerializer(typeof(List<Score>));

                using (var stream = new IsolatedStorageFileStream("highscores.xml", FileMode.Create, storage))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        serializer.Serialize(writer, scores);
                    }
                }
            }
            catch (IsolatedStorageException)
            {
                //TODO: Handlig it
                throw;
            }
            catch (IOException)
            {
                //TODO: Handlig it
                throw;
            }
        }
        public void SortDescending()
        {
            scores = scores.OrderByDescending(s => s.Points).ThenBy(s => s.Name).ToList<Score>();
        }
        public IEnumerator<Score> GetEnumerator()
        {
            return scores.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (scores as IEnumerable).GetEnumerator();
        }

    }
}
