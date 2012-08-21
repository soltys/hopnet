using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLog;
using Microsoft.Kinect;
using System.Diagnostics;


namespace Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class HopnetGame : Microsoft.Xna.Framework.Game
    {
        bool isUserHasKinect = true;

        KinectSensor kinect;
        Texture2D colorVideo,depthVideo;
        Skeleton[] skeletonData;
        Skeleton skeleton;

        Texture2D jointTexture; 

        private static Logger logger = LogManager.GetCurrentClassLogger();

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MainMenu mainMenu;
        List<Platform> platformList;
        Model platformModel;
        PlatformCollection platformGenerator;
        Hero player;
        Model heroModel;
        SpriteFont debugFont;
        Vector3 cameraPosition;
        KinectPlayer kinectPlayer;

        // The aspect ratio determines how to scale 3d to 2d projection.
        float aspectRatio;

        // The array that determines in which column the platform must be drawn
        private bool[] rowFromGenerator = new bool[PlatformRow.rowLength];

        //The constants that define range of board
        const float EndOfBoardPositionZ = 9.0f+2.0f;
        const float BeginningOfBoardPositionZ = 8.0f;

        static float speedLevelFactor = 0.5f;
        static float SpeedOfPlatforms = 0.1f * speedLevelFactor;

        float spaceBetweenPlatforms = 4f;
        float FirstPlatformPosition = -8.0f;
        float spaceBetweenRows = 5f;
        float platformGroundLevel = 0.0f;
        float platformRadius = 1.8f;
        private const float safeRangeForJump = 0.1f;

        private float PlatformUpdateSpeed;

        public HopnetGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base. Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {         
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            IsMouseVisible = true;
            graphics.ApplyChanges();

            PlatformUpdateSpeed = SpeedOfPlatforms;

            colorVideo = new Texture2D(graphics.GraphicsDevice,320,240);
            depthVideo = new Texture2D(graphics.GraphicsDevice, 320, 240) ;

                try
                {
                    kinect = KinectSensor.KinectSensors[0];
                    kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    kinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                    kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinect_AllFramesReady);
                    kinect.SkeletonStream.Enable();
                    kinect.Start();
                    colorVideo = new Texture2D(graphics.GraphicsDevice, kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight);
                    depthVideo = new Texture2D(graphics.GraphicsDevice, kinect.DepthStream.FrameWidth, kinect.DepthStream.FrameHeight);
                }
                catch (Exception e)
                {
                    isUserHasKinect = false;
                }
            
            cameraPosition = new Vector3(10.0f, 0.0f, 0.0f); // kamera od boku
            //cameraPosition = new Vector3(0.0f, 5.0f, 10.0f);  // kamera pokazuj¹ca tak, jak ma byæ w grze finalnie
            mainMenu = new MainMenu(graphics,this);
            mainMenu.IsGameInMenuMode = false;
            kinectPlayer = new KinectPlayer(Content,new Vector3(FirstPlatformPosition + (PlatformRow.rowLength/2)*spaceBetweenPlatforms,platformGroundLevel,BeginningOfBoardPositionZ));
            kinectPlayer.SetPlatformRadius(platformRadius);
            kinectPlayer.SetPlatformToPlatformMoveTime(SpeedOfPlatforms, spaceBetweenRows, (float)(this.TargetElapsedTime.TotalMilliseconds),spaceBetweenPlatforms);
            platformList = new List<Platform>();
            platformGenerator=new PlatformCollection();


                rowFromGenerator[2] = true;



            //this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 10.0f); // zmniejszenie Update'u do 10/s

            var heroArrangement = new ObjectData3D
                                      {
                                          Position = new Vector3(0.0f, 0.5f, 9.0f),
                                          Scale = new Vector3(0.5f, 0.5f, 0.5f),
                                          Rotation = new Vector3(0.0f)
                                      };
            player = new Hero(heroArrangement);

            CreatePlatforms(PlatformRow.rowLength, FirstPlatformPosition, spaceBetweenPlatforms, BeginningOfBoardPositionZ);
            
            for (int i = 0; i < PlatformCollection.lanesNumber+5; i++)
            {
                CreatePlatforms(PlatformRow.rowLength, FirstPlatformPosition, spaceBetweenPlatforms,platformList.Last().objectArrangement.Position.Z-spaceBetweenRows);
            }
            
            base.Initialize();
        }

        void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs imageFrames)
        {
            using (SkeletonFrame skeletonFrame = imageFrames.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if ((skeletonData == null) || (this.skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                }
            }

            if (skeletonData != null)
            {
                foreach (Skeleton skel in skeletonData)
                {
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        skeleton = skel;
                    }
                }
            }
            mainMenu.KinectUpdate(skeleton, new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
            kinectPlayer.KinectUpdate(skeleton);
        }


        private void DrawSkeleton(SpriteBatch spriteBatch, Vector2 resolution, Texture2D img)
        {
            if (skeleton != null)
            {
                spriteBatch.Begin();
                foreach (Joint joint in skeleton.Joints)
                {
                    Vector2 position = new Vector2((((0.5f * joint.Position.X) +0.3f) * (resolution.X)),
                        (((-0.5f * joint.Position.Y) + 0.3f) * (resolution.Y)));

                        spriteBatch.Draw(img, new Rectangle(Convert.ToInt32(position.X), Convert.ToInt32(position.Y), 50, 50), Color.Black);
                }
                spriteBatch.End();
            }
        }


        void CreatePlatforms(int platformCount, float firstPlatformPosition, float distanceBetweenPlatforms, float zDistance)
        {
            //rowFromGenerator = platformGenerator.GetLastAddedRowValues;
            

            for (int i = 0; i < platformCount; i++)
            {
                if (rowFromGenerator[i] == true)
                {
                    ObjectData3D platformArrangement = new ObjectData3D();
                    platformArrangement.Position = new Vector3(firstPlatformPosition + i * distanceBetweenPlatforms, platformGroundLevel, zDistance);
                    platformArrangement.Scale = new Vector3(0.5f);
                    platformArrangement.Rotation = new Vector3(0.0f);
                    Platform newPlatform = new Platform(platformArrangement);
                    platformList.Add(newPlatform);
                }
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /// 
        protected override void LoadContent()
        {
            logger.Trace("Load Content starts");
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            debugFont = Content.Load<SpriteFont>("myFont");
            heroModel = Content.Load<Model>(@"Models\hero");

            platformModel = Content.Load<Model>(@"Models\platforma");
            jointTexture = Content.Load<Texture2D>(@"Sprites\cursor_left_normal");

            aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height;
            mainMenu.LoadContent(Content);

            logger.Trace("Load Content ends");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void MovePlatforms()
        {
            foreach (Platform platform in platformList)
            {
                platform.MoveInAxisZ(SpeedOfPlatforms);
            }
        }



        private Stopwatch zegar = new Stopwatch();

        private void RemovePlatformsAtEnd()
        {
            if (platformList.Count>0)
            {
                if (platformList[0].objectArrangement.Position.Z > EndOfBoardPositionZ)
                {
                    if (!zegar.IsRunning)
                    {
                        zegar.Reset();
                        zegar.Start();
                    }
                    else
                    {
                        zegar.Stop();
                    }

                    platformList.RemoveAt(0);
                    platformGenerator.UpdatePlatforms();
                    CreatePlatforms(PlatformRow.rowLength, FirstPlatformPosition, spaceBetweenPlatforms,platformList.Last().objectArrangement.Position.Z - spaceBetweenRows);
                }
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        bool change = true;
        private Stopwatch btnTimer = new Stopwatch();

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.P)) { UnloadContent(); Exit(); }
            var keyState = Keyboard.GetState();

            #region test buttons

            if (btnTimer.IsRunning)
            {
                if (btnTimer.Elapsed.TotalMilliseconds > 500 )
                {
                    change = true;
                    btnTimer.Reset();
                }
            }



            if (Keyboard.GetState().IsKeyDown(Keys.J))
            {
                if (change)
                {
                    speedLevelFactor -= 0.1f;
                    SpeedOfPlatforms = 0.1f * speedLevelFactor;
                    kinectPlayer.SetPlatformToPlatformMoveTime(SpeedOfPlatforms, spaceBetweenRows, (float)(this.TargetElapsedTime.TotalMilliseconds),spaceBetweenPlatforms);
                    change = false;
                    btnTimer.Start();
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                if (change)
                {   
                    speedLevelFactor += 0.1f;
                    SpeedOfPlatforms = 0.1f * speedLevelFactor;
                    kinectPlayer.SetPlatformToPlatformMoveTime(SpeedOfPlatforms, spaceBetweenRows, (float)(this.TargetElapsedTime.TotalMilliseconds),spaceBetweenPlatforms);
                     
                    //MovePlatforms();
                    //RemovePlatformsAtEnd();
                    change = false;
                    btnTimer.Start();

                }
            }
            #endregion





            switch (mainMenu.IsGameInMenuMode)
            {
                case false:
                    kinectPlayer.Update(platformList,spaceBetweenRows);
                    MovePlatforms();
                    RemovePlatformsAtEnd();
                break;
                    
                case true:
                    if (!isUserHasKinect)
                    {
                        mainMenu.KinectUpdate(null, new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                    }
                break;
                     
            }
         
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch(mainMenu.IsGameInMenuMode)
            {
                case true:
                    mainMenu.Draw(spriteBatch,debugFont);
                    break;
                case false:
                    for (int i = platformList.Count-1; i >= 0; i--)
                    {
                        platformList[i].Draw(aspectRatio, cameraPosition, platformModel);
                    }
                    kinectPlayer.Draw(spriteBatch, debugFont, aspectRatio, cameraPosition);
                    break;
            }

            drawDebufInfo(spriteBatch, debugFont);
            base.Draw(gameTime);
        }

        private void drawDebufInfo(SpriteBatch spritebatch, SpriteFont debugFont)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(debugFont, "kinectPlayer.isBehind :" + kinectPlayer.isBehind.ToString(), new Vector2(100, 10), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(debugFont, "platformList.First().Z : "+ platformList.First().objectArrangement.Position.Z.ToString(), new Vector2(100, 30), Color.Red);
            spriteBatch.DrawString(debugFont, "zegar.ElapsedMS : " + zegar.Elapsed.TotalMilliseconds.ToString(), new Vector2(100, 50), Color.Red);
            
            spriteBatch.DrawString(debugFont, "kinectPlayer.timeAmount : " + kinectPlayer.timeAmount.ToString(), new Vector2(100, 90), Color.Red);
            spriteBatch.DrawString(debugFont, "kinectPlayer.distance : " + kinectPlayer.distance.ToString(), new Vector2(100, 110), Color.Red);
            spriteBatch.DrawString(debugFont, "kinectPlayer.CurrentStance : " + kinectPlayer.currentStance.ToString(), new Vector2(100, 130), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(debugFont, "kinectPlayer.idleShoulderHeight : " + kinectPlayer.idleShoulderHeight.ToString(), new Vector2(100, 150), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(debugFont, "kinectPlayer.timer : " + kinectPlayer.timer.ToString(), new Vector2(100, 170), Color.Red);
            spriteBatch.DrawString(debugFont, "kinectPlayer.tdistance : " + kinectPlayer.tdistance.ToString(), new Vector2(100, 190), Color.Red);
            spriteBatch.DrawString(debugFont, "kinectPlayer.platformTime : " + kinectPlayer.rowToRowIdleMoveTime.ToString(), new Vector2(100, 210), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(debugFont, "kinectPlayer.gravity :" + kinectPlayer.idleJumpGravity.ToString(), new Vector2(100, 230), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(debugFont, "kinectPlayer.verticalVelocity :" + kinectPlayer.verticalVelocity.ToString(), new Vector2(100, 250), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(debugFont, "kinectPlayer.horizontalVelocity :" + kinectPlayer.horizontalVelocity.ToString(), new Vector2(100, 270), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(debugFont, "kinectPlayer.platformRadius :" + kinectPlayer.platformRadius.ToString(), new Vector2(100, 290), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(debugFont, "kinectPlayer.radiusToIdleJump :" + kinectPlayer.radiusToIdleJump.ToString(), new Vector2(100, 310), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(debugFont, kinectPlayer.currentStance.ToString(), new Vector2(50, 450), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);

            spriteBatch.End();
        }


    }
}
