using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    class Platform : Object
    {
        public Platform(ObjectData3D platformArrangement)
            : base()
        {
            base.ArrangeObjectOnScene(platformArrangement);
            //Mesh = Content.Load<Model>("Models\\platforma");
        }

        public void MoveInAxisZ(float MoveValue)
        {
            Vector3 newPosition = new Vector3(objectArrangement.Position.X, objectArrangement.Position.Y, objectArrangement.Position.Z + MoveValue);
            objectArrangement.Position = newPosition;
        }

    }
}