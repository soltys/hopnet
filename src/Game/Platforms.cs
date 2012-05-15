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
        private LinkedList<PlatformRow> PlatformRows = new LinkedList<PlatformRow>();
        private const int NumberOfLanes=5;

        private int ChanceToCreatePlatform = 4; 
        private const int MaxValueOfChanceToCreatePlatform = 10;

        private int MinPlatformsToBeCreated = 2;
        private int MaxPlatformsToBeCreated = 4;
        private PlatformRow RowToBeAdded;
        private int NumberOfRowsSinceLastEmptyRow = 5;
        private int NumberOfGeneratedRowsSinceLastEmptyRow = 0;
        private int MinNumberOfNonEmptyRows = 3;
        private int MaxNumberOfNonEmptyRows = 6;


        private void GenerateNextRow()
        {
            PlatformRows.AddFirst(RowToBeAdded);
        }

        private void RemoveLastRow()
        {
            PlatformRows.RemoveLast();
        }

        public Platforms()
        {
            for (int i = 0; i < NumberOfLanes; i++)
            {
                GenerateNextRow();
            }
        }

        private void CalculatePlatformsForNewRow()
        {
            bool[] ValuesForNewRow = new bool[PlatformRow.RowLength];
            Random randomNumberObject = new Random();
            int randomNumber;
            int platformsCreated = 0;
            int platformsToBeCreated;
            PlatformRow lastAddedRow = PlatformRows.Last();

            if(NumberOfGeneratedRowsSinceLastEmptyRow>=NumberOfRowsSinceLastEmptyRow)
            {
                NumberOfGeneratedRowsSinceLastEmptyRow = 0;
                randomNumber = randomNumberObject.Next(MinNumberOfNonEmptyRows, MaxNumberOfNonEmptyRows + 1);
                NumberOfRowsSinceLastEmptyRow = randomNumber;
            }
            else
            {
                if (lastAddedRow.IsEmpty())
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
            }
            NumberOfGeneratedRowsSinceLastEmptyRow++;
            RowToBeAdded = new PlatformRow(ValuesForNewRow);
        }


        public void UpdatePlatforms() 
        {
            RemoveLastRow();
            CalculatePlatformsForNewRow();
            GenerateNextRow();
        }

    }
}
