using Microsoft.Xna.Framework;

namespace Game
{
    class Hero : Object
    {
        public int CurrentPlatformPosition;

        public Hero(ObjectData3D heroArrangement)
        {
            ArrangeObjectOnScene(heroArrangement);
            CurrentPlatformPosition = GameConstants.RowLength/2;
        }

        public void MoveRight()
        {
            if (CurrentPlatformPosition < GameConstants.RowLength-1)
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

    }
}