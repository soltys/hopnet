using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Game
{
    class Platforms:IEnumerable<PlatformRow>
    {
        private readonly LinkedList<PlatformRow> platformRows = new LinkedList<PlatformRow>();
        private const int lanesNumber = 5;

        private const int createPlatformChance = 4;
        private const int maxCreatePlatformChance = 10;

        private const int minPlatformNumber = 2;
        private const int maxPlatformNumber = PlatformRow.RowLength-1;
        private PlatformRow lastRow=new PlatformRow();
        private int rowNumberSinceLastEmptyRow = 5;
        private int rowNumberGeneratedSinceLastEmptyRow = 0;
        private const int minNonEmptyRowNumber = 8;
        private const int maxNonEmptyRowNumber = 12;

        private void GenerateNextRow()
        {
            platformRows.AddFirst(lastRow);
        }

        private void RemoveLastRow()
        {
            platformRows.RemoveLast();
        }

        public Platforms()
        {
            for (int i = 0; i < lanesNumber; i++)
            {
                GenerateNextRow();
            }
            lastRow = platformRows.Last();

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

            if (lastRow.IsEmpty)
            {
                for (int i = 0; i < PlatformRow.RowLength; i++)
                {
                    valuesForNewRow[i] = true;
                }
            }
            else
            {
                if ((rowNumberGeneratedSinceLastEmptyRow >= rowNumberSinceLastEmptyRow) & !lastRow.IsFull)
                {
                    rowNumberGeneratedSinceLastEmptyRow = 0;
                    rowNumberSinceLastEmptyRow = randomGenerator.Next(minNonEmptyRowNumber, maxNonEmptyRowNumber + 1);
                }
                else
                {
                    int platformsCreated = 0;
                    int requiredPlatformNumber = randomGenerator.Next(minPlatformNumber, maxPlatformNumber + 1);
                    while (platformsCreated < requiredPlatformNumber-1)
                    {
                        for (int i = 0; i < PlatformRow.RowLength; i++)
                        {
                            if (!valuesForNewRow[i])
                            {
                                if (randomGenerator.Next(maxCreatePlatformChance) > createPlatformChance)
                                {
                                    valuesForNewRow[i] = true;
                                    platformsCreated++;
                                }
                            }
                        }
                    }
                }
            }
            rowNumberGeneratedSinceLastEmptyRow++;
            lastRow = new PlatformRow(valuesForNewRow);
        }

        public IEnumerator<PlatformRow> GetEnumerator()
        {
            return platformRows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
