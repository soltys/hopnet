using Microsoft.Xna.Framework;

namespace Game
{
    class Hero : Object
    {

        public Hero(ObjectArrangementIn3D heroArrangement)
        {
            ArrangeObjectOnScene(heroArrangement);
            CurrentPlatformPosition = 2;
        }

        public int CurrentPlatformPosition;
        public void MoveRight()
        {
            ObjectArrangement.Position = new Vector3(
                ObjectArrangement.Position.X + 4.0f,
                ObjectArrangement.Position.Y,
                ObjectArrangement.Position.Z);
            ++CurrentPlatformPosition;
        }
        public void MoveLeft()
        {
            ObjectArrangement.Position = new Vector3(
                ObjectArrangement.Position.X - 4.0f,
                ObjectArrangement.Position.Y,
                ObjectArrangement.Position.Z);
            --CurrentPlatformPosition;
        }

    }
}