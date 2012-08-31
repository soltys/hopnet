using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Game
{
    abstract class Object
    {
        //public Model Mesh { get; set; }

        public ObjectData3D objectArrangement;
        public ObjectData3D oldArrangement;
        // Copy any parent transforms.
        Matrix[] transforms;

        protected Object()
        {
            objectArrangement = new ObjectData3D(new Vector3(0.0f), new Vector3(0.0f), new Vector3(0.0f));
            oldArrangement = new ObjectData3D(new Vector3(0.0f), new Vector3(0.0f), new Vector3(0.0f));
        }

        protected Object(ObjectData3D objectArrangement)
        {
            ArrangeObjectOnScene(objectArrangement);
        }

        protected void ArrangeObjectOnScene(ObjectData3D objectArrangementOnScene)
        {
            objectArrangement = objectArrangementOnScene;
            oldArrangement = objectArrangementOnScene;
        }

        public void Draw(float aspectRatio, Camera camera, Model mesh)
        {
            transforms = new Matrix[mesh.Bones.Count];
            mesh.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (var singleMesh in mesh.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in singleMesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.World = transforms[singleMesh.ParentBone.Index] * Matrix.CreateRotationY(objectArrangement.Rotation.Y)
                        * Matrix.CreateTranslation(objectArrangement.Position) * Matrix.CreateScale(objectArrangement.Scale);

                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                }

                singleMesh.Draw();
            }
        }

    }
}
