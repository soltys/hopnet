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
    }
}