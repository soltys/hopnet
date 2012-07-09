using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Game
{
    class PlatformCollection : IEnumerable<PlatformRow>
    {
        private readonly LinkedList<PlatformRow> platformRows = new LinkedList<PlatformRow>();
        public const int lanesNumber = 5;
        

        private PlatformRow lastRow = new PlatformRow();
        private int platformsRequiredToChangeDirection;
        private int platformsGeneratedAfterDirectionChange = 0;
        private const int maxPlatformsRequiredToChangeDirection = 2;
        private const int minPlatformsRequiredToChangeDirection = 0;
        private int lastInsertedPlatformIndex;
        private enum Direction : int { Left, Right };

        private void GenerateNextRow()
        {
            platformRows.AddFirst(lastRow);
        }

        private void RemoveLastRow()
        {
            platformRows.RemoveLast();
        }

        public PlatformCollection()
        {
            for (int i = 0; i < lanesNumber; i++)
            {
                GenerateNextRow();
            }
            lastRow = platformRows.Last();
            platformsRequiredToChangeDirection = maxPlatformsRequiredToChangeDirection;
            for (int i = 0; i < PlatformRow.rowLength; i++)
            {
                if (lastRow.PlatformValues[i]) 
                { 
                    lastInsertedPlatformIndex = i;
                    break; 
                }
            }
        }

        public bool[] GetLastAddedRowValues
        {
            get { return lastRow.PlatformValues; }
        }


        public void UpdatePlatforms()
        {
                RemoveLastRow();
                CalculatePlatformsForNewRow();
                GenerateNextRow();
        }

        private void CalculatePlatformsForNewRow()
        {
            var valuesForNewRow = new bool[PlatformRow.rowLength];

                if (platformsGeneratedAfterDirectionChange >= platformsRequiredToChangeDirection)
                {
                    var randomGenerator = new Random();
                    int randomValue;

                    platformsRequiredToChangeDirection = randomGenerator.Next(
                        minPlatformsRequiredToChangeDirection,
                        maxPlatformsRequiredToChangeDirection + 1);
                    platformsGeneratedAfterDirectionChange = 0;


                    switch (lastInsertedPlatformIndex)
                    {
                        case 0:
                            randomValue = (int)Direction.Right;
                            break;
                        case (PlatformRow.rowLength - 1):
                            randomValue = (int)Direction.Left;
                            break;
                        default:
                            randomValue = randomGenerator.Next((int)Direction.Left,(int)Direction.Right+1);
                            break;
                    }

                    if (randomValue == (int)Direction.Left)
                    {
                        valuesForNewRow[lastInsertedPlatformIndex - 1] = true;
                        lastInsertedPlatformIndex--;
                    }
                    else
                    {
                        valuesForNewRow[lastInsertedPlatformIndex + 1] = true;
                        lastInsertedPlatformIndex++;
                    }
                    lastRow = new PlatformRow(valuesForNewRow);
                }
                else
                {
                    platformsGeneratedAfterDirectionChange++;
                }

            
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
