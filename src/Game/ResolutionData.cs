using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    public class ResolutionData
    {
        public int width;
        public int height;

        public ResolutionData(DisplayMode displayMode)
        {
            width = displayMode.Width;
            height = displayMode.Height;
        }

        public ResolutionData()
        {
            width = 0;
            height = 0;
        }

        public override string ToString()
        {
            return (width + "x" + height).ToString();
        }
    }
}
