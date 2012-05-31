using Microsoft.Xna.Framework;

namespace Game
{
    class ObjectArrangementIn3D
    {
        public ObjectArrangementIn3D()
        {

        }
        public ObjectArrangementIn3D(Vector3 position, Vector3 scale, Vector3 rotation)
        {
            Position = position;
            Scale = scale;
            Rotation = rotation;
        }
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Rotation { get; set; }
    }
}