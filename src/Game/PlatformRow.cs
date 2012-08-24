using System;

namespace Game
{
    class PlatformRow
    {
        private readonly bool[] platformSettings = new bool[GameConstants.RowLength];

        public bool []PlatformValues
        {
            get { return platformSettings; }
        }

        public PlatformRow(bool[] platformSettings)
        {
            if (platformSettings.Length != GameConstants.RowLength)
            {
                throw new ArgumentException();
            }
            this.platformSettings = platformSettings;
        }

        public PlatformRow()
        {
            platformSettings[GameConstants.RowLength / 2] = true;
        }

        public bool this[int i]
        {
            get
            {
                return platformSettings[i];
            }
        }
    }
}
