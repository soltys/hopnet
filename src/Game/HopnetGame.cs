using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLog;
using Microsoft.Kinect;

namespace Game
{
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
        private Camera camera;
        private Texture2D backgroundTexture;

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
            LoadConfigFile();
            graphics.PreferredBackBufferWidth = GameConstants.HorizontalGameResolution;
            graphics.PreferredBackBufferHeight = GameConstants.VerticalGameResolution;

            graphics.IsFullScreen = GameConstants.IsGameInFullScreen;
            IsMouseVisible = GameConstants.IsMouseVisible;
            graphics.ApplyChanges();

            kinectData = new KinectData();
            camera = new Camera(graphics);


            if (kinectData.IsKinectConnected)
            {
                try
                {
                    kinectData.KinectSensor.AllFramesReady += KinectAllFramesReady;
                    kinectData.KinectSensor.SkeletonStream.Enable();
                    kinectData.KinectSensor.Start();
                }
                catch
                {
                    
                }
            }

            mainMenu = new MainMenu(this) {IsGameInMenuMode = true};
            kinectPlayer = new KinectPlayer(new Vector3(GameConstants.FirstPlatformPosition + (GameConstants.RowLength/2)*GameConstants.SpaceBetweenPlatforms,GameConstants.PlatformGroundLevel,GameConstants.BeginningOfBoardPositionZ));
            
          
            platformList = new List<Platform>();
            platformGenerator=new PlatformCollection();

            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / GameConstants.GameUpdatesPerSecond);

            PreparePlatformsForNewGame();

            base.Initialize();
        }

        private void NewGameSettingsReset()
        {
            kinectPlayer.SetJumpSafeZone();
            kinectPlayer.SetPlatformToPlatformMoveTime(GameConstants.SpeedOfPlatformsOneUpdate, GameConstants.SpaceBetweenRows, (float)(TargetElapsedTime.TotalMilliseconds), GameConstants.SpaceBetweenPlatforms);
        }

        private void PreparePlatformsForNewGame()
        {
            platformList.Clear();
            var newGamePlatforms = new bool[GameConstants.RowLength];

            newGamePlatforms[GameConstants.RowLength/2] = true;

            CreatePlatforms(GameConstants.RowLength, GameConstants.FirstPlatformPosition, GameConstants.SpaceBetweenPlatforms, GameConstants.BeginningOfBoardPositionZ -0.9f*GameConstants.PlatformRadius,newGamePlatforms);

            for (int i = 0; i < GameConstants.LanesNumber; i++)
            {
                CreatePlatforms(GameConstants.RowLength, GameConstants.FirstPlatformPosition, GameConstants.SpaceBetweenPlatforms, platformList.Last().objectArrangement.Position.Z - GameConstants.SpaceBetweenRows,newGamePlatforms);
            }
        }


        private void DrawBackground(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0.9f);
            spriteBatch.End();
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
                    mainMenu.KinectUpdate(kinectData);
                    break;
                case false:
                    kinectPlayer.KinectUpdate(kinectData);
                    break;
            }
             
        }



        void CreatePlatforms(int platformCount, float firstPlatformPosition, float distanceBetweenPlatforms, float zDistance, bool[] platformSettings)
        {
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
                                                      Scale = new Vector3(2f),
                                                      Rotation = new Vector3(0.0f,180f,0.0f)
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


        private void LoadConfigFile()
        {
            var gameConfigData = GameConfigFile.Load();

            if (gameConfigData != null)
            {
                GameConstants.HorizontalGameResolution = gameConfigData.resolutionData.width;
                GameConstants.VerticalGameResolution = gameConfigData.resolutionData.height;
                GameConstants.IsGameInFullScreen = gameConfigData.fullscreenEnabled;
            }

        }

        protected override void LoadContent()
        {
            logger.Trace("Load Content starts");
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            debugFont = Content.Load<SpriteFont>(@"myFont");
            Content.Load<Model>(@"Models\hero");
            backgroundTexture = Content.Load<Texture2D>(@"Sprites\cosmos");
            platformModel = Content.Load<Model>(@"Models\platforma");
            Content.Load<Texture2D>(@"Sprites\cursor_left_normal");
            kinectPlayer.LoadContent(Content);
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




        private void RotatePlatforms()
        {
            for(int i=0; i<platformList.Count;i++)
            {
                platformList[i].Rotate();
            }
        }

        
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.P)) { UnloadContent(); Exit(); }

            switch (mainMenu.IsGameInMenuMode)
            {
                case false:
                    kinectPlayer.Update(platformList, camera);
                    if (!kinectData.IsKinectConnected)
                    {
                        kinectPlayer.DebugInputUpdate(platformList);
                    }
                    kinectPlayer.DebugInputUpdate(platformList);
                    if (kinectPlayer.currentStance != GameConstants.PlayerStance.GameStartCountDown && 
                        kinectPlayer.currentStance != GameConstants.PlayerStance.GameEnded && 
                        kinectPlayer.currentStance != GameConstants.PlayerStance.GameSettingsSetup)
                    {
                        RotatePlatforms();
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
                                mainMenu.highScores.SaveToFile();

                                kinectPlayer.NewGameDataReset();
                                mainMenu.IsGameInMenuMode = true;
                                mainMenu.MenuState = GameConstants.MenuState.AfterGameLoss;
                                PreparePlatformsForNewGame();
                                break;
                            case GameConstants.PlayerStance.GameSettingsSetup:
                                NewGameSettingsReset();
                                kinectPlayer.lastStance = kinectPlayer.currentStance;
                                kinectPlayer.currentStance = GameConstants.PlayerStance.GameStartCountDown;
                                break;
                        }
                    }
                break;
                case true:
                if (!kinectData.IsKinectConnected)
                {
                    mainMenu.DebugInputUpdate();
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
            DrawBackground(spriteBatch);

            switch(mainMenu.IsGameInMenuMode)
            {
                case true:
                    mainMenu.Draw(spriteBatch,debugFont);
                    break;
                case false:
                    
                    /*
                    for (int i = platformList.Count-1; i >= 0; i--)
                    {
                        platformList[i].Draw(camera, platformModel);
                    }
                     */

                    foreach (var platform in platformList)
                    {
                        platform.Draw(camera,platformModel);
                    }

                    kinectPlayer.Draw(spriteBatch, debugFont,camera);
                    break;
            }
            
            base.Draw(gameTime);
        }

        


    }
}
