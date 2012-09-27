using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Game
{
    class PlatformCollection : IEnumerable<PlatformRow>
    {
        private readonly LinkedList<PlatformRow> platformRows = new LinkedList<PlatformRow>();
        
        

        private PlatformRow lastRow = new PlatformRow();
        private int platformsRequiredToChangeDirection;
        private int platformsGeneratedAfterDirectionChange;
        private const int maxPlatformsRequiredToChangeDirection = 2;
        private const int minPlatformsRequiredToChangeDirection = 0;
        private int lastInsertedPlatformIndex;


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
            for (int i = 0; i < GameConstants.LanesNumber; i++)
            {
                GenerateNextRow();
            }
            lastRow = platformRows.Last();
            platformsRequiredToChangeDirection = maxPlatformsRequiredToChangeDirection;

            lastInsertedPlatformIndex = GameConstants.RowLength / 2;
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

        public void NewGameReset()
        {
            platformRows.Clear();
            lastRow = new PlatformRow();

            for (int i = 0; i < GameConstants.LanesNumber; i++)
            {
                GenerateNextRow();
            }

                    lastInsertedPlatformIndex = GameConstants.RowLength/2;

        }


        private void CalculatePlatformsForNewRow()
        {
            var valuesForNewRow = new bool[GameConstants.RowLength];

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
                            randomValue = (int)GameConstants.Direction.Right;
                            break;
                        case (GameConstants.RowLength - 1):
                            randomValue = (int)GameConstants.Direction.Left;
                            break;
                        default:
                            randomValue = randomGenerator.Next((int)GameConstants.Direction.Left,(int)GameConstants.Direction.Right+1);
                            break;
                    }

                    if (randomValue == (int)GameConstants.Direction.Left)
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
