using System;

namespace Game
{
    class PlatformRow
    {
        public const int rowLength = 5;
        private readonly bool[] platformSettings = new bool[rowLength];


        public bool []PlatformValues
        {
            get { return platformSettings; }
        }


        public PlatformRow(bool[] platformSettings)
        {
            if (platformSettings.Length != rowLength)
            {
                throw new ArgumentException();
            }
            this.platformSettings = platformSettings;
        }

        public PlatformRow()
        {
            platformSettings[rowLength / 2] = true;
        }

        public bool this[int i]
        {
            get
            {
                return platformSettings[i];
            }
        }

        public bool IsEmpty
        {
            get
            {
                for (int i = 0; i < rowLength; i++)
                {
                    if (platformSettings[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }


        public bool IsFull
        {
            get
            {
                int counter = 0;
                for (int i = 0; i < rowLength; i++)
                {
                    if (platformSettings[i])
                    {
                        counter++;
                    }
                }
                if (counter == rowLength)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
