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
        private const int NUMBER_OF_LANES=5;

        private int ChanceToCreatePlatform = 4; 
        private const int MaxValueOfChanceToCreatePlatform = 10;

        private int MinPlatformsToBeCreated = 2;
        private int MaxPlatformsToBeCreated = 4;

        public void GenerateNextRow(PlatformRow rowToBeAddedtoQueue)
        {
            QueueOfPlatformRowObjects.Enqueue(rowToBeAddedtoQueue);
            LastAddedRow = rowToBeAddedtoQueue;
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

        private bool[] CalculateRouteForNewRow()
        {
            bool[] ValuesForNewRow = new bool[LastAddedRow.row_length];
            Random randomNumberObject = new Random();
            int randomNumber;
            int platformsCreated = 0;
            int platformsToBeCreated;

            if (LastAddedRow.IsEmpty())
            {
                for (int i = 0; i < LastAddedRow.row_length; i++)
                {
                        ValuesForNewRow[i] = true;
                }
            }
            else
            {
                platformsToBeCreated = randomNumberObject.Next(MinPlatformsToBeCreated, MaxPlatformsToBeCreated + 1);

                while (platformsCreated < platformsToBeCreated)
                {
                    for (int i = 0; i < LastAddedRow.row_length; i++)
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
            GenerateNextRow(new PlatformRow(CalculateRouteForNewRow()));
        }

    }
}
