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
        private PlatformRow LastAddedRow;
        private const int NumberOfLanes=5;

        private int ChanceToCreatePlatform = 4; 
        private const int MaxValueOfChanceToCreatePlatform = 10;

        private int MinPlatformsToBeCreated = 2;
        private int MaxPlatformsToBeCreated = 4;

        public void GenerateNextRow()
        {
            QueueOfPlatformRowObjects.Enqueue(new PlatformRow(new bool[]{true,true,true,true,true}));
        }

        public void RemoveLastRow()
        {
            QueueOfPlatformRowObjects.Dequeue();
        }

        public Platforms()
        {
            for (int i = 0; i < NumberOfLanes; i++)
            {
                GenerateNextRow();
            }
        }

        private bool[] CalculateRouteForNewRow()
        {
            bool[] ValuesForNewRow = new bool[PlatformRow.RowLength];
            Random randomNumberObject = new Random();
            int randomNumber;
            int platformsCreated = 0;
            int platformsToBeCreated;

            if (LastAddedRow.IsEmpty())
            {
                for (int i = 0; i < PlatformRow.RowLength; i++)
                {
                        ValuesForNewRow[i] = true;
                }
            }
            else
            {
                platformsToBeCreated = randomNumberObject.Next(MinPlatformsToBeCreated, MaxPlatformsToBeCreated + 1);

                while (platformsCreated < platformsToBeCreated)
                {
                    for (int i = 0; i < PlatformRow.RowLength; i++)
                    {
                        if (!ValuesForNewRow[i])
                        {
                            randomNumber = randomNumberObject.Next(MaxValueOfChanceToCreatePlatform);
                            if (randomNumber > ChanceToCreatePlatform)
                            {
                                ValuesForNewRow[i] = true;
                                platformsCreated++;
                            }
                        }
                    }
                }
            }

            return ValuesForNewRow;
        }


        public void UpdatePlatforms() 
        {
            RemoveLastRow();
            GenerateNextRow();
        }

    }
}
