using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        bool moveOnlyOnceRight;
        bool moveOnlyOnceLeft;
        KinectPlayer kinectPlayer;

        // The aspect ratio determines how to scale 3d to 2d projection.
        float aspectRatio;

        // The array that determines in which column the platform must be drawn
        private bool[] rowFromGenerator = new bool[PlatformRow.rowLength];

        //The constants that define range of board
        const float EndOfBoardPositionZ = 13.0f;
        const float BeginningOfBoardPositionZ = -26.0f;

        static int speedLevelFactor = 5;
        static float SpeedOfPlatforms = 0.1f * speedLevelFactor;

        int counterForNextRowAppearence = 0;
        float DistanceBetweenPlatforms = 4.0f;
        int PlatformCount = 5;
        float FirstPlatformPosition = -8.0f;

        private const float safeRangeForJump = 0.5f;

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
            
            cameraPosition = new Vector3(0.0f, 5.0f, 10.0f);
            moveOnlyOnceRight = true;
            moveOnlyOnceLeft = true;
            mainMenu = new MainMenu(graphics,this);
            mainMenu.IsGameInMenuMode = false;
            kinectPlayer = new KinectPlayer(Content,new Vector3(DistanceBetweenPlatforms,0,2.55f));

            platformList = new List<Platform>();
            platformGenerator=new PlatformCollection();
            var heroArrangement = new ObjectData3D
                                      {
                                          Position = new Vector3(0.0f, 0.5f, 9.0f),
                                          Scale = new Vector3(0.5f, 0.5f, 0.5f),
                                          Rotation = new Vector3(0.0f)
                                      };
            player = new Hero(heroArrangement);

            CreatePlatforms(PlatformCount, FirstPlatformPosition, DistanceBetweenPlatforms);

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


        void CreatePlatforms(int platformCount, float firstPlatformPosition, float distanceBetweenPlatforms)
        {
            platformGenerator.UpdatePlatforms();
            rowFromGenerator = platformGenerator.GetLastAddedRowValues;

            for (int i = 0; i < platformCount; i++)
            {
                if (rowFromGenerator[i] == true)
                {
                    ObjectData3D platformArrangement = new ObjectData3D();
                    platformArrangement.Position = new Vector3(firstPlatformPosition + i * distanceBetweenPlatforms, 0.0f, BeginningOfBoardPositionZ);
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

        static int tajmer = 0;
        bool runOnce = true;
        

        private void MovePlatforms()
        {
            foreach (Platform platform in platformList)
            {
                platform.MoveInAxisZ(SpeedOfPlatforms);
            }
        }

        private void RemovePlatformsAtEnd()
        {
            if (platformList.Count>0)
            {
                if (platformList[0].objectArrangement.Position.Z > EndOfBoardPositionZ)
                {
                    platformList.RemoveAt(0);
                }
            }
        }



        private bool IsPlayerCanJump()
        {
            foreach (Platform platform in platformList)
            {
                if (platform.objectArrangement.Position.Z < player.objectArrangement.Position.Z + safeRangeForJump
                    && platform.objectArrangement.Position.Z > player.objectArrangement.Position.Z - safeRangeForJump)
                {
                    return true;
                }
                return false;
            }
            return false;
        }


        private void AddNewPlatforms()
        {
            counterForNextRowAppearence++;
            if (counterForNextRowAppearence == 60 / speedLevelFactor)
            {
                CreatePlatforms(PlatformCount, FirstPlatformPosition, DistanceBetweenPlatforms);
                counterForNextRowAppearence = 0;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.P)) { UnloadContent(); Exit(); }
            var keyState = Keyboard.GetState();

            //kinectPlayer.Update(SpeedOfPlatforms,SpeedOfPlatforms, gravity);
            
            switch(mainMenu.IsGameInMenuMode)
            {
                case false:
                MovePlatforms();
                AddNewPlatforms();
                RemovePlatformsAtEnd();
                break;
                    
                case true:
                if (!isUserHasKinect)
                {
                    mainMenu.KinectUpdate(null, new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                }
                break;
                     
            }
            
                #region player controls
                bool playerCanJump = IsPlayerCanJump();

                if (keyState.IsKeyDown(Keys.Right))
                {
                    if (moveOnlyOnceRight && playerCanJump)
                    {
                        if (player.CurrentPlatformPosition < 4)
                        {
                            player.MoveRight();
                        }
                        moveOnlyOnceRight = false;
                        playerCanJump = false;
                    }
                }

                if (keyState.IsKeyUp(Keys.Right))
                {
                    moveOnlyOnceRight = true;
                }


                if (keyState.IsKeyDown(Keys.Left))
                {
                    if (moveOnlyOnceLeft && playerCanJump)
                    {
                        if (player.CurrentPlatformPosition > 0)
                        {
                            player.MoveLeft();
                        }

                        moveOnlyOnceLeft = false;
                        playerCanJump = false;
                    }
                }

                if (keyState.IsKeyUp(Keys.Left))
                {
                    moveOnlyOnceLeft = true;
                }
                #endregion
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
                    foreach (var platform in platformList)
                    {
                        platform.Draw(aspectRatio, cameraPosition,platformModel);
                    }
                    kinectPlayer.Draw(spriteBatch, debugFont, aspectRatio, cameraPosition);
                    break;
            }
            
            base.Draw(gameTime);
        }
    }
}
