using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Game
{
    class Objects
    {
        public Model objectMesh;

        public ObjectArrangementIn3D ObjectArrangement;

        // Copy any parent transforms.
        Matrix[] transforms;

        public Objects()
        {
            ObjectArrangement = new ObjectArrangementIn3D( new Vector3(0.0f), new Vector3(0.0f), new Vector3(0.0f));
        }

        public Objects(ObjectArrangementIn3D objectArrangement)
        {
            ArrangeObjectOnScene(objectArrangement);
        }

        protected void ArrangeObjectOnScene(ObjectArrangementIn3D objectArrangement)
        {
            ObjectArrangement = objectArrangement;
        }

        //public void ChangeObjectPositionAxisZ(float newPositionAxisZ)
        //{
        //    Vector3 moveVector = new Vector3(ObjectArrangement.Position.X, ObjectArrangement.Position.Y, ObjectArrangement.Position.Z + newPositionAxisZ);
        //    ObjectArrangement.Position = moveVector;
        //}

        public void Draw(float aspectRatio, Vector3 cameraPosition)
        {
            transforms = new Matrix[objectMesh.Bones.Count];
            objectMesh.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (var mesh in objectMesh.Meshes)
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



    class Hero : Objects
    {

        public Hero(ObjectArrangementIn3D heroArrangement)
        {
            base.ArrangeObjectOnScene(heroArrangement);
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
        public Vector3 Position {get; set;}
        public Vector3 Scale { get; set; }
        public Vector3 Rotation { get; set; }
    }

    class Platform : Objects
    {
        public Platform(ObjectArrangementIn3D platformArrangement, ContentManager Content):base()
        {
            base.ArrangeObjectOnScene(platformArrangement);
            objectMesh = Content.Load<Model>("Models\\platforma");
        }

        public void MoveInAxisZ(float MoveValue)
        {
            Vector3 newPosition = new Vector3(ObjectArrangement.Position.X, ObjectArrangement.Position.Y, ObjectArrangement.Position.Z + MoveValue);
            ObjectArrangement.Position = newPosition;
        }

    }

}
