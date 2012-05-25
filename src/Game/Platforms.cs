using System;
using System.Collections.Generic;
using System.Linq;


namespace Game
{
    class Platforms
    {
        private readonly LinkedList<PlatformRow> platformRows = new LinkedList<PlatformRow>();
        private const int lanesNumber = 5;

        private const int createPlatformChance = 4;
        private const int maxCreatePlatformChance = 10;

        private const int minPlatformNumber = 2;
        private const int maxPlatformNumber = 4;
        private PlatformRow lastRow=new PlatformRow();
        private int rowNumberSinceLastEmptyRow = 5;
        private int rowNumberGeneratedSinceLastEmptyRow = 0;
        private const int minNonEmptyRowNumber = 5;
        private const int maxNonEmptyRowNumber = 6;

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
        
       
    }
}
