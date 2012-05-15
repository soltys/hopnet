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
    class HighScores : IEnumerable<Score>
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
                IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                if (isoStorage.FileExists("highscores.xml"))
                {
                    XmlSerializer xSerializer = new XmlSerializer(typeof(List<Score>));
                    using (IsolatedStorageFileStream oStream = new IsolatedStorageFileStream("highscores.xml", FileMode.Open, isoStorage))
                    {

                        using (XmlReader xReader = XmlReader.Create(oStream))
                        {
                            List<Score> tmpScores = new List<Score>((List<Score>)xSerializer.Deserialize(xReader));
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
                IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                XmlSerializer xSerializer = new XmlSerializer(typeof(List<Score>));

                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("highscores.xml", FileMode.Create, isoStore))
                {

                    using (StreamWriter sWriter = new StreamWriter(isoStream))
                    {
                        xSerializer.Serialize(sWriter, scores);
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
