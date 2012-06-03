﻿using System;

namespace Game
{
    class PlatformRow
    {
        public const int RowLength = 5;
        private readonly bool[] platformSettings = new bool[RowLength];
        public bool []platformValues
        {
            get { return platformSettings; }
        }

        public PlatformRow(bool[] platformSettings)
        {
            if (platformSettings.Length != RowLength)
            {
                throw new ArgumentException();
            }
            this.platformSettings = platformSettings;
        }

        public PlatformRow()
        {
            for (int i = 0; i < RowLength; i++)
            {
                    platformSettings[i] = true;
            }
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
                for (int i = 0; i < RowLength; i++)
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
                for (int i = 0; i < RowLength; i++)
                {
                    if (platformSettings[i])
                    {
                        counter++;
                    }
                }
                if (counter == RowLength)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
