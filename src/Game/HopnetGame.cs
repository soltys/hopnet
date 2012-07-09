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

namespace Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class HopnetGame : Microsoft.Xna.Framework.Game
    {

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

        // The aspect ratio determines how to scale 3d to 2d projection.
        float aspectRatio;

        // The array that determines in which column the platform must be drawn
        private bool[] rowFromGenerator = new bool[PlatformRow.rowLength];

        //The constants that define range of board
        const float EndOfBoardPositionZ = 13.0f;
        const float BeginningOfBoardPositionZ = -26.0f;

        const float SpeedOfPlatforms = 1f;
        int counterForNextRowAppearence = 0;
        int timeBeforeNewPlatformAppear = 6;
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
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            IsMouseVisible = true;
            graphics.ApplyChanges();


            cameraPosition = new Vector3(0.0f, 5.0f, 10.0f);
            moveOnlyOnceRight = true;
            moveOnlyOnceLeft = true;
            mainMenu = new MainMenu(graphics);

            platformList = new List<Platform>();
            platformGenerator=new PlatformCollection();
            var heroArrangement = new ObjectArrangementIn3D
                                      {
                                          Position = new Vector3(0.0f, 0.5f, 9.0f),
                                          Scale = new Vector3(0.5f, 0.5f, 0.5f),
                                          Rotation = new Vector3(0.0f)
                                      };
            player = new Hero(heroArrangement);

            CreatePlatforms(PlatformCount, FirstPlatformPosition, DistanceBetweenPlatforms);

            base.Initialize();
        }

        void CreatePlatforms(int platformCount, float firstPlatformPosition, float distanceBetweenPlatforms)
        {
            platformGenerator.UpdatePlatforms();
            rowFromGenerator = platformGenerator.GetLastAddedRowValues;

            for (int i = 0; i < platformCount; i++)
            {
                if (rowFromGenerator[i] == true)
                {
                    ObjectArrangementIn3D platformArrangement = new ObjectArrangementIn3D();
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
            mainMenu.newGameSprite[0].LoadSprite(Content, @"Sprites\testsprite1");
            mainMenu.newGameSprite[1].LoadSprite(Content, @"Sprites\testsprite2");

            mainMenu.scoresSprite[0].LoadSprite(Content, @"Sprites\testsprite1");
            mainMenu.scoresSprite[1].LoadSprite(Content, @"Sprites\testsprite2");

            mainMenu.exitSprite[0].LoadSprite(Content, @"Sprites\testsprite1");
            mainMenu.exitSprite[1].LoadSprite(Content, @"Sprites\testsprite2");

            mainMenu.backgroundSprite.LoadSprite(Content, @"Sprites\testsprite1");


            aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height;


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

        private void RemovePlatformsAtEnd()
        {
            if (platformList[0].ObjectArrangement.Position.Z > EndOfBoardPositionZ)
            {
                platformList.RemoveAt(0);
            }
        }



        private bool IsPlayerCanJump()
        {
            foreach (Platform platform in platformList)
            {
                if (platform.ObjectArrangement.Position.Z < player.ObjectArrangement.Position.Z + safeRangeForJump
                    && platform.ObjectArrangement.Position.Z > player.ObjectArrangement.Position.Z - safeRangeForJump)
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
            if (counterForNextRowAppearence == timeBeforeNewPlatformAppear)
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
            if (Keyboard.GetState().IsKeyDown(Keys.P)) { Exit(); }
            var keyState = Keyboard.GetState();
                
                MovePlatforms();
                AddNewPlatforms();
                RemovePlatformsAtEnd();
                

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

            
            
            foreach (var platform in platformList)
            {
                platform.Draw(aspectRatio, cameraPosition,platformModel);
            }
            player.Draw(aspectRatio, cameraPosition,heroModel);
            
            
            spriteBatch.Begin();
            spriteBatch.DrawString(debugFont, "platforms in list: " + platformList.Count.ToString(), new Vector2(0, 80), Color.Red);
            spriteBatch.DrawString(debugFont, "PlayerPos:" + player.ObjectArrangement.Position.ToString(), new Vector2(0, 100), Color.Red);
            spriteBatch.DrawString(debugFont, "CurrentPlatformPos:" + player.CurrentPlatformPosition.ToString(), new Vector2(0, 120), Color.Red);
            spriteBatch.End();

            mainMenu.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}
