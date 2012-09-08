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
        private Logger logger = LogManager.GetCurrentClassLogger();
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private MainMenu mainMenu;
        private List<Platform> platformList;
        private Model platformModel;
        private PlatformCollection platformGenerator;
        private SpriteFont debugFont;
        private KinectPlayer kinectPlayer;
        private KinectData kinectData;
        private float aspectRatio;
        private Camera camera;

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
            graphics.PreferredBackBufferWidth = GameConstants.HorizontalGameResolution;
            graphics.PreferredBackBufferHeight = GameConstants.VerticalGameResolution;
            aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height;

            graphics.IsFullScreen = GameConstants.IsGameInFullScreen;
            IsMouseVisible = true;
            graphics.ApplyChanges();

            kinectData = new KinectData();
            camera = new Camera(graphics);


            if (kinectData.IsKinectConnected)
            {
                kinectData.KinectSensor.AllFramesReady += KinectAllFramesReady;
                kinectData.KinectSensor.SkeletonStream.Enable();
                kinectData.KinectSensor.Start();
            }

            mainMenu = new MainMenu(this) {IsGameInMenuMode = true};
            kinectPlayer = new KinectPlayer(Content,new Vector3(GameConstants.FirstPlatformPosition + (GameConstants.RowLength/2)*GameConstants.SpaceBetweenPlatforms,GameConstants.PlatformGroundLevel,GameConstants.BeginningOfBoardPositionZ));
            kinectPlayer.LoadContent(Content);
            kinectPlayer.SetPlatformRadius(GameConstants.PlatformRadius);
            kinectPlayer.SetPlatformToPlatformMoveTime(GameConstants.SpeedOfPlatformsOneUpdate, GameConstants.SpaceBetweenRows, (float)(TargetElapsedTime.TotalMilliseconds), GameConstants.SpaceBetweenPlatforms);
            platformList = new List<Platform>();
            platformGenerator=new PlatformCollection();


            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / GameConstants.GameUpdatesPerSecond); // zmniejszenie Update'u do 10/s

            PreparePlatformsForNewGame();



            base.Initialize();
        }

        private void PreparePlatformsForNewGame()
        {
            var newGamePlatforms = new bool[GameConstants.RowLength];

            newGamePlatforms[GameConstants.RowLength/2] = true;

            CreatePlatforms(GameConstants.RowLength, GameConstants.FirstPlatformPosition, GameConstants.SpaceBetweenPlatforms, GameConstants.BeginningOfBoardPositionZ-GameConstants.PlatformRadius,newGamePlatforms);

            for (int i = 0; i < GameConstants.LanesNumber; i++)
            {
                CreatePlatforms(GameConstants.RowLength, GameConstants.FirstPlatformPosition, GameConstants.SpaceBetweenPlatforms, platformList.Last().objectArrangement.Position.Z - GameConstants.SpaceBetweenRows,newGamePlatforms);
            }
        }


        void KinectAllFramesReady(object sender, AllFramesReadyEventArgs imageFrames)
        {
            using (var skeletonFrame = imageFrames.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if ((kinectData.SkeletonData == null) || (kinectData.SkeletonData.Length!= skeletonFrame.SkeletonArrayLength))
                    {
                        kinectData.SkeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }
                    skeletonFrame.CopySkeletonDataTo(kinectData.SkeletonData);
                }
            }

            if (kinectData.SkeletonData != null)
            {
                foreach (var skel in kinectData.SkeletonData)
                {
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        kinectData.Skeleton = skel;
                    }
                }
            }
            
            kinectData.CalculatePersonShoulderHeight();
            switch (mainMenu.IsGameInMenuMode)
            {
                case true:
                    mainMenu.KinectUpdate(kinectData, new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                    break;
                case false:
                    kinectPlayer.KinectUpdate(kinectData);
                    break;
            }
             
        }



        void CreatePlatforms(int platformCount, float firstPlatformPosition, float distanceBetweenPlatforms, float zDistance, bool[] platformSettings)
        {
            //rowFromGenerator = platformGenerator.GetLastAddedRowValues;
            

            for (int i = 0; i < platformCount; i++)
            {
                if (platformSettings != null && platformSettings[i])
                {
                    var platformArrangement = new ObjectData3D
                                                  {
                                                      Position =
                                                          new Vector3(
                                                          firstPlatformPosition + i*distanceBetweenPlatforms,
                                                          GameConstants.PlatformGroundLevel, zDistance),
                                                      Scale = new Vector3(0.5f),
                                                      Rotation = new Vector3(0.0f)
                                                  };
                    var newPlatform = new Platform(platformArrangement);
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
            Content.Load<Model>(@"Models\hero");

            platformModel = Content.Load<Model>(@"Models\platforma");
            Content.Load<Texture2D>(@"Sprites\cursor_left_normal");

            
            mainMenu.LoadContent(Content);

            logger.Trace("Load Content ends");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        private void MovePlatforms()
        {
            foreach (var platform in platformList)
            {
                platform.MoveInAxisZ(GameConstants.SpeedOfPlatformsOneUpdate);
            }
        }

        private int ctr = 0;
        private void RemovePlatformsAtEnd()
        {
            if (platformList.Count>0)
            {
                if (platformList[0].objectArrangement.Position.Z > GameConstants.EndOfBoardPositionZ)
                {
                    platformList.RemoveAt(0);
                    platformGenerator.UpdatePlatforms();
                    CreatePlatforms(GameConstants.RowLength, GameConstants.FirstPlatformPosition, GameConstants.SpaceBetweenPlatforms, platformList.Last().objectArrangement.Position.Z - GameConstants.SpaceBetweenRows,platformGenerator.GetLastAddedRowValues);
                }
            }
        }

        
        protected override void Update(GameTime gameTime)
        {
            

            if (Keyboard.GetState().IsKeyDown(Keys.P)) { UnloadContent(); Exit(); }

            
            switch (mainMenu.IsGameInMenuMode)
            {
                case false:
                    
                    kinectPlayer.Update(platformList, camera);
                    if (kinectPlayer.currentStance != GameConstants.PlayerStance.GameStartCountDown && kinectPlayer.currentStance != GameConstants.PlayerStance.GameEnded)
                    {
                        ++kinectPlayer.ScoreInCurrentGame;
                        MovePlatforms();
                        RemovePlatformsAtEnd();
                    }
                    else
                    {
                        switch (kinectPlayer.currentStance)
                        {
                            case GameConstants.PlayerStance.GameEnded:
                                mainMenu.scoreInCurrentGame = kinectPlayer.ScoreInCurrentGame;

                                mainMenu.highScores.Add(new Score(kinectPlayer.ScoreInCurrentGame));
                                mainMenu.highScores.Save();

                                kinectPlayer.NewGameDataReset();
                                camera.Position = new Vector3( GameConstants.FirstPlatformPosition + (GameConstants.RowLength / 2) * GameConstants.SpaceBetweenPlatforms,camera.Position.Y,camera.Position.Z);
                                camera.LookAtPoint = new Vector3(GameConstants.FirstPlatformPosition + (GameConstants.RowLength / 2) * GameConstants.SpaceBetweenPlatforms,camera.LookAtPoint.Y,camera.LookAtPoint.Z);
                                mainMenu.IsGameInMenuMode = true;
                                mainMenu.MenuState = GameConstants.MenuState.AfterGameLoss;// zrobic nowy stan - w ktorym do wyboru jest nowa gra lub wyjscie do menu
                                platformList.Clear();
                                PreparePlatformsForNewGame();
                                break;
                        }
                    }
                break;
                case true:
                    mainMenu.Update();
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
                        platformList[i].Draw(aspectRatio, camera, platformModel);
                    }
                    kinectPlayer.Draw(spriteBatch, debugFont, aspectRatio, camera);
                    DrawDebugInfo(spriteBatch,debugFont);
                    break;
            }
            
            base.Draw(gameTime);
        }

        private void DrawDebugInfo(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Points :" + kinectPlayer.ScoreInCurrentGame.ToString(), new Vector2(100, 10), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            /* 
            spriteBatch.DrawString(font, "kinectPlayer.isBehind :" + kinectPlayer.isFirstPlatformBehindPlayer.ToString(), new Vector2(100, 10), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "platformList.First().Z : "+ platformList.First().objectArrangement.Position.Z.ToString(), new Vector2(100, 30), Color.Red);
            spriteBatch.DrawString(font, "ElapsedMS : " + kinectPlayer.timeLeftToJump.Elapsed.TotalMilliseconds.ToString(), new Vector2(100, 50), Color.Red);
            
            spriteBatch.DrawString(font, "kinectPlayer.timeAmount : " + kinectPlayer.timeAmount.ToString(), new Vector2(100, 90), Color.Red);
            spriteBatch.DrawString(font, "kinectPlayer.distance : " + kinectPlayer.distance.ToString(), new Vector2(100, 110), Color.Red);
            spriteBatch.DrawString(font, "kinectPlayer.CurrentStance : " + kinectPlayer.currentStance.ToString(), new Vector2(100, 130), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            
            spriteBatch.DrawString(font, "kinectPlayer.timer : " + kinectPlayer.timer.ToString(), new Vector2(100, 170), Color.Red);
            spriteBatch.DrawString(font, "kinectPlayer.tdistance : " + kinectPlayer.tdistance.ToString(), new Vector2(100, 190), Color.Red);
            spriteBatch.DrawString(font, "kinectPlayer.platformTime : " + kinectPlayer.rowToRowIdleMoveTime.ToString(), new Vector2(100, 210), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "kinectPlayer.gravity :" + kinectPlayer.idleJumpGravity.ToString(), new Vector2(100, 230), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "kinectPlayer.verticalVelocity :" + kinectPlayer.verticalVelocity.ToString(), new Vector2(100, 250), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "kinectPlayer.horizontalVelocity :" + kinectPlayer.horizontalVelocity.ToString(), new Vector2(100, 270), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "kinectPlayer.platformRadius :" + kinectPlayer.platformRadius.ToString(), new Vector2(100, 290), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "kinectPlayer.radiusToIdleJump :" + kinectPlayer.radiusToIdleJump.ToString(), new Vector2(100, 310), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            */
            //spriteBatch.DrawString(font, kinectPlayer.idleJumpExpectedFunctionCalls.ToString(), new Vector2(50, 350), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, kinectPlayer.currentStance.ToString(), new Vector2(50, 450), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            /*
            spriteBatch.DrawString(font, kinectPlayer.verticalVelocity.ToString(), new Vector2(50, 150), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, kinectPlayer.timeCounter.ToString(), new Vector2(50, 250), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, kinectPlayer.currentJumpTime.ToString(), new Vector2(50, 320), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, kinectPlayer.idleJumpTime.ToString(), new Vector2(50, 390), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
             */
            //spriteBatch.DrawString(font, kinectPlayer.timeAmount.ToString(), new Vector2(50, 550), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            
            spriteBatch.End();
        }


    }
}
