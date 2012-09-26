using Microsoft.Xna.Framework;

namespace Game
{
    public class Camera
    {
        private Vector3 position;
        private Vector3 lookAtPoint;
        private float nearPlane;
        private float farPlane;
        private float aspectRatio;
        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }      
        public Vector3 LookAtPoint
        {
            get { return lookAtPoint; }
            set { lookAtPoint = value; }
        }   
        public float NearPlane
        {
            get { return nearPlane; }
        }       
        public float FarPlane
        {
            get { return farPlane; }
        }
        public float AspectRatio
        {
            get { return aspectRatio; }
            set { aspectRatio = value; }
        }
        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }
        public Matrix ProjectionMatrix
        {
            get { return projectionMatrix; }
        }


        public Camera(GraphicsDeviceManager graphics)
        {
            position = GameConstants.CameraPosition;
            lookAtPoint = GameConstants.CameraLookAtPoint;
            nearPlane = GameConstants.NearPlane;
            farPlane = GameConstants.FarPlane;
            aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, nearPlane, farPlane);
            Update();
        }

        public void Update()
        {
            viewMatrix = Matrix.CreateLookAt(position, lookAtPoint, Vector3.Up);
        }



    }
}
