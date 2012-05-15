using System;
using System.Collections.Generic;
using System.Linq;


namespace Game
{
    class Platforms
    {
        private readonly LinkedList<PlatformRow> platformRows = new LinkedList<PlatformRow>();
        private const int NumberOfLanes = 5;

        private const int ChanceToCreatePlatform = 4;
        private const int MaxValueOfChanceToCreatePlatform = 10;

        private const int MinPlatformsToBeCreated = 2;
        private const int MaxPlatformsToBeCreated = 4;
        private PlatformRow rowToBeAdded;
        private int numberOfRowsSinceLastEmptyRow = 5;
        private int numberOfGeneratedRowsSinceLastEmptyRow = 0;
        private const int MinNumberOfNonEmptyRows = 3;
        private const int MaxNumberOfNonEmptyRows = 6;


        private void GenerateNextRow()
        {
            platformRows.AddFirst(rowToBeAdded);
        }

        private void RemoveLastRow()
        {
            platformRows.RemoveLast();
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
            var valuesForNewRow = new bool[PlatformRow.RowLength];
            var randomGenerator = new Random();
            int randomNumber;
            int platformsCreated = 0;
            var lastAddedRow = platformRows.Last();

            if (numberOfGeneratedRowsSinceLastEmptyRow >= numberOfRowsSinceLastEmptyRow)
            {
                numberOfGeneratedRowsSinceLastEmptyRow = 0;
                randomNumber = randomGenerator.Next(MinNumberOfNonEmptyRows, MaxNumberOfNonEmptyRows + 1);
                numberOfRowsSinceLastEmptyRow = randomNumber;
            }
            else
            {
                if (lastAddedRow.IsEmpty())
                {
                    for (int i = 0; i < PlatformRow.RowLength; i++)
                    {
                        valuesForNewRow[i] = true;
                    }
                }
                else
                {
                    int platformsToBeCreated = randomGenerator.Next(MinPlatformsToBeCreated, MaxPlatformsToBeCreated + 1);

                    while (platformsCreated < platformsToBeCreated)
                    {
                        for (int i = 0; i < PlatformRow.RowLength; i++)
                        {
                            if (!valuesForNewRow[i])
                            {
                                randomNumber = randomGenerator.Next(MaxValueOfChanceToCreatePlatform);
                                if (randomNumber > ChanceToCreatePlatform)
                                {
                                    valuesForNewRow[i] = true;
                                    platformsCreated++;
                                }
                            }
                        }
                    }
                }
            }
            numberOfGeneratedRowsSinceLastEmptyRow++;
            rowToBeAdded = new PlatformRow(valuesForNewRow);
        }


        public void UpdatePlatforms()
        {
            RemoveLastRow();
            CalculatePlatformsForNewRow();
            GenerateNextRow();
        }

    }
}
