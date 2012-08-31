using Microsoft.Xna.Framework;

namespace Game
{
    class ObjectData3D
    {
        public ObjectData3D()
        {

        }

        public ObjectData3D(Vector3 position, Vector3 scale, Vector3 rotation)
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