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
        private PlatformRow LastAddedRowValueHolder;
        private const int NUMBER_OF_LANES=5;

        public void GenerateNextRow(PlatformRow rowToBeAddedtoQueue)
        {
            QueueOfPlatformRowObjects.Enqueue(rowToBeAddedtoQueue);
            LastAddedRowValueHolder = rowToBeAddedtoQueue;
        }

        public void RemoveLastRow()
        {
            QueueOfPlatformRowObjects.Dequeue();
        }

        public Platforms()
        {
            for (int i = 0; i < NUMBER_OF_LANES; i++)
            {
                GenerateNextRow(new PlatformRow(new bool[]{true,true,true,true,true}));
            }
        }

        public void UpdatePlatforms()
        {
            RemoveLastRow();
        }


        private PlatformRow CalculateRouteForNewRow()
        {
            bool []ValuesForNewRow=new bool[LastAddedRowValueHolder.row_length];

            //return new PlatformRow(
        }





    }
}
