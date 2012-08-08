using Microsoft.Xna.Framework;

namespace Game
{
    class Hero : Object
    {
        public int CurrentPlatformPosition;

        public Hero(ObjectData3D heroArrangement)
        {
            ArrangeObjectOnScene(heroArrangement);
            CurrentPlatformPosition = PlatformRow.rowLength/2;
        }

        public void MoveRight()
        {
            if (CurrentPlatformPosition < PlatformRow.rowLength-1)
            {
                objectArrangement.Position = new Vector3(
                    objectArrangement.Position.X + 4.0f,
                    objectArrangement.Position.Y,
                    objectArrangement.Position.Z);
                ++CurrentPlatformPosition;
            }
        }

        public void MoveLeft()
        {
            if (CurrentPlatformPosition > 0)
            {
                objectArrangement.Position = new Vector3(
                    objectArrangement.Position.X - 4.0f,
                    objectArrangement.Position.Y,
                    objectArrangement.Position.Z);
                --CurrentPlatformPosition;
            }
        }

        private double timer = 0.0f;
        public bool jumpReady=true;
        public void MoveUp(double dTimeBetweenPlatforms, double groundLevel,double platformsSpeed, double gravity)
        {
            if(jumpReady)
            {
                if (timer*10<dTimeBetweenPlatforms)
                {
                    timer += 10;
                    objectArrangement.Position = new Vector3(objectArrangement.Position.X, (float)(oldArrangement.Position.Y + platformsSpeed * timer - gravity * timer * timer / 2), objectArrangement.Position.Z);
                }
                else
                {
                    objectArrangement.Position = new Vector3(objectArrangement.Position.X, 0.0f, objectArrangement.Position.Z);
                    oldArrangement = objectArrangement;
                    jumpReady = false;

                }
            }




        }



    }
}