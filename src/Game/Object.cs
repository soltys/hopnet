using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Game
{
    abstract class Object
    {
        public Model Mesh { get; set; }

        public ObjectArrangementIn3D ObjectArrangement;

        // Copy any parent transforms.
        Matrix[] transforms;

        protected Object()
        {
            ObjectArrangement = new ObjectArrangementIn3D(new Vector3(0.0f), new Vector3(0.0f), new Vector3(0.0f));
        }

        protected Object(ObjectArrangementIn3D objectArrangement)
        {
            ArrangeObjectOnScene(objectArrangement);
        }

        protected void ArrangeObjectOnScene(ObjectArrangementIn3D objectArrangement)
        {
            ObjectArrangement = objectArrangement;
        }

        public void Draw(float aspectRatio, Vector3 cameraPosition)
        {
            transforms = new Matrix[Mesh.Bones.Count];
            Mesh.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (var mesh in Mesh.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(ObjectArrangement.Rotation.Y)
                        * Matrix.CreateTranslation(ObjectArrangement.Position) * Matrix.CreateScale(ObjectArrangement.Scale);

                    effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                        aspectRatio, 1.0f, 10000.0f);

                }

                mesh.Draw();
            }
        }

    }
}
