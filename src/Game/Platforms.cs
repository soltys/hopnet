using System;
using System.Collections.Generic;
using System.Linq;


namespace Game
{
    class Platforms
    {
        private readonly LinkedList<PlatformRow> platformRows = new LinkedList<PlatformRow>();
        private const int numberOfLanes = 5;

        private const int chanceToCreatePlatform = 4;
        private const int maxValueOfChanceToCreatePlatform = 10;

        private const int minPlatformsToBeCreated = 2;
        private const int maxPlatformsToBeCreated = 4;
        private PlatformRow rowToBeAdded;
        private int numberOfRowsSinceLastEmptyRow = 5;
        private int numberOfGeneratedRowsSinceLastEmptyRow = 0;
        private const int minNumberOfNonEmptyRows = 3;
        private const int maxNumberOfNonEmptyRows = 6;


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
            for (int i = 0; i < numberOfLanes; i++)
            {
                GenerateNextRow();
            }
        }

        public void UpdatePlatforms()
        {
            RemoveLastRow();
            CalculatePlatformsForNewRow();
            GenerateNextRow();
        }
        
        private void CalculatePlatformsForNewRow()
        {
            var valuesForNewRow = new bool[PlatformRow.RowLength];
            var randomGenerator = new Random();
            int platformsCreated = 0;
            var lastAddedRow = platformRows.Last();

            if (numberOfGeneratedRowsSinceLastEmptyRow >= numberOfRowsSinceLastEmptyRow)
            {
                numberOfGeneratedRowsSinceLastEmptyRow = 0;
                numberOfRowsSinceLastEmptyRow = randomGenerator.Next(minNumberOfNonEmptyRows, maxNumberOfNonEmptyRows + 1);
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
                    int platformsToBeCreated = randomGenerator.Next(minPlatformsToBeCreated, maxPlatformsToBeCreated + 1);

                    while (platformsCreated < platformsToBeCreated)
                    {
                        for (int i = 0; i < PlatformRow.RowLength; i++)
                        {
                            if (!valuesForNewRow[i])
                            {
                                if (randomGenerator.Next(maxValueOfChanceToCreatePlatform) > chanceToCreatePlatform)
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
    }
}
