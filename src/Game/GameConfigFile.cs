using System.Xml.Serialization;
using System.IO;

namespace Game
{
    public class GameConfigFile
    {
        public ResolutionData resolutionData;
        public bool fullscreenEnabled;

        public GameConfigFile()
        {
            resolutionData = new ResolutionData();
            fullscreenEnabled = false;
        }

        public GameConfigFile(ResolutionData resolutionInfo, bool fullscreen)
        {
            resolutionData = resolutionInfo;
            fullscreenEnabled = fullscreen;
        }

        public static void Save(GameConfigFile gameConfiguration)
        {
            var serializer = new XmlSerializer(typeof(GameConfigFile));
            var textWriter = new StreamWriter(@"config.xml");
            serializer.Serialize(textWriter, gameConfiguration);
            textWriter.Close();
        }

        public static GameConfigFile Load()
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(GameConfigFile));
            GameConfigFile gameConfigData = null;
            try
            {
                TextReader textReader = new StreamReader(@"config.xml");
                gameConfigData = (GameConfigFile)deserializer.Deserialize(textReader);
                textReader.Close();
            }
            catch
            {
                return null;
            }
            


            return gameConfigData;
        }

    }
}
