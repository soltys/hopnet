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

        List<Platform> platformList;
        Model platformModel;
        PlatformCollection platformGenerator;
        Hero heroModel;
        SpriteFont debugFont;
        Vector3 cameraPosition;
        bool moveOnlyOnceRight;
        bool moveOnlyOnceLeft;

        // The aspect ratio determines how to scale 3d to 2d projection.
        float aspectRatio;

        // The array that determines in which column the platform must be drawn
        private bool[] rowFromGenerator = new bool[PlatformRow.RowLength];

        //The constants that define range of board
        const float EndOfBoardPositionZ = 13.0f;
        const float BeginningOfBoardPositionZ = -26.0f;

        static int factorOfAcceleration = 4;
        static float SpeedOfPlatforms = 0.1f * factorOfAcceleration;
        int counterForNextRowAppearence = 0;


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
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 1000;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            cameraPosition = new Vector3(0.0f, 5.0f, 10.0f);
            moveOnlyOnceRight = true;
            moveOnlyOnceLeft = true;
            platformList = new List<Platform>();
            platformGenerator=new PlatformCollection();
            var heroArrangement = new ObjectArrangementIn3D
                                      {
                                          Position = new Vector3(0.0f, 0.5f, 9.0f),
                                          Scale = new Vector3(0.5f, 0.5f, 0.5f),
                                          Rotation = new Vector3(0.0f)
                                      };
            heroModel = new Hero(heroArrangement);

            CreatePlatforms(5, -8.0f, 4.0f);

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
                    platformList.Add(new Platform(platformArrangement, Content));
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
            heroModel.Mesh = Content.Load<Model>(@"Models\hero");

            platformModel = Content.Load<Model>(@"Models\platforma");

            foreach (var platform in platformList)
            {
                platform.Mesh = platformModel;
            }


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
            for (int i = 0; i < platformList.Count; i++)
            {
                if (platformList[i].ObjectArrangement.Position.Z > EndOfBoardPositionZ)
                {
                    platformList.RemoveAt(i);
                }
            }
        }



        private bool IsPlayerCanJump()
        {
            foreach (Platform platform in platformList)
            {
                if (platform.ObjectArrangement.Position.Z < heroModel.ObjectArrangement.Position.Z + safeRangeForJump
                    && platform.ObjectArrangement.Position.Z > heroModel.ObjectArrangement.Position.Z - safeRangeForJump)
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
            if (counterForNextRowAppearence == 60 / factorOfAcceleration)
            {
                CreatePlatforms(5, -8.0f, 4.0f);
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
                        if (heroModel.CurrentPlatformPosition < 4)
                        {
                            heroModel.MoveRight();
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
                        if (heroModel.CurrentPlatformPosition > 0)
                        {
                            heroModel.MoveLeft();
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
                platform.Draw(aspectRatio, cameraPosition);
            }
            heroModel.Draw(aspectRatio, cameraPosition);

            spriteBatch.Begin();
            spriteBatch.DrawString(debugFont, "PlayerPos:" + heroModel.ObjectArrangement.Position.ToString(), new Vector2(0, 100), Color.White);
            spriteBatch.DrawString(debugFont, "CurrentPlatformPos:" + heroModel.CurrentPlatformPosition.ToString(), new Vector2(0, 120), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
