using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    class Platform : Object
    {
        public Platform(ObjectArrangementIn3D platformArrangement, ContentManager Content)
            : base()
        {
            base.ArrangeObjectOnScene(platformArrangement);
            Mesh = Content.Load<Model>("Models\\platforma");
        }

        public void MoveInAxisZ(float MoveValue)
        {
            Vector3 newPosition = new Vector3(ObjectArrangement.Position.X, ObjectArrangement.Position.Y, ObjectArrangement.Position.Z + MoveValue);
            ObjectArrangement.Position = newPosition;
        }

    }
}