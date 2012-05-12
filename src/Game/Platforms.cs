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
    class Platforms
    {
        private Queue<PlatformRow> QueueOfPlatformRowObjects = new Queue<PlatformRow>();


        public void GenerateNextRow()
        {
            QueueOfPlatformRowObjects.Enqueue(new PlatformRow());
        }

    }
}
