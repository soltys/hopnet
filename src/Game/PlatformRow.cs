using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;



namespace Game
{
    class PlatformRow
    {
        public PlatformRow(bool[] platformSettings)
        {
            if (platformSettings.Length != ROW_LENGTH)
            {
                throw new ArgumentException();
            }
            this.platformSettings = platformSettings;
        }


        public PlatformRow()
        {
            for (int i = 0; i < ROW_LENGTH; i++)
            {
                this.platformSettings[i] = true;
            }
        }



        private const int ROW_LENGTH = 5;
        public int row_length
        {
            get
            {
            return ROW_LENGTH;
            }
        }
        private readonly bool[] platformSettings = new bool[ROW_LENGTH];
        public bool this[int i]
        {
            get { return platformSettings[i]; }
        }

        public bool IsEmpty()
        {
            for (int i = 0; i < ROW_LENGTH; i++)
            {
                if (platformSettings[i]) { return false; }
            }

            return true;
        }


    }
}
