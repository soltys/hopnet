using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Kinect;
using System.Diagnostics;


namespace Game
{
    class MainMenu
    {
        #region properties and accessors
        private readonly HopnetGame hopNetGame;
        private bool isGameInMenu=true;
        private readonly float hScale;
        private readonly float vScale;
        public HighScores highScores;
      
        private int selectedDifficulty = (int)GameConstants.GameDifficulty.Easy;

        private float idlePersonHeight = 0.4f;
        private  GameConstants.MenuButton currentButton = GameConstants.MenuButton.None;
        private GameConstants.MenuButton lastButton = GameConstants.MenuButton.None;
        private GameConstants.MenuState state = GameConstants.MenuState.InMainMenu;

        public GameConstants.MenuState MenuState
        {
            get { return state; }
            set { state = value; }
        }

        private readonly Sprite[] newGameSprite;
        private int newGameTextureType=(int)GameConstants.TextureType.Normal;

        private readonly Sprite[] scoresSprite;
        private int scoresTextureType = (int)GameConstants.TextureType.Normal;

        private readonly Sprite[] goBackSprite;
        private int goBackTextureType = (int)GameConstants.TextureType.Normal;

        private readonly Sprite[] exitSprite;
        private int exitTextureType = (int)GameConstants.TextureType.Normal;

        private readonly Sprite backgroundSprite;

        private readonly Sprite[] easyDifficulty;
        private int easyDifficultyTextureType=(int)GameConstants.TextureType.Normal;

        private readonly Sprite[] mediumDifficulty;
        private int mediumDifficultyTextureType=(int)GameConstants.TextureType.Normal;

        private readonly Sprite[] hardDifficulty;
        private int hardDifficultyTextureType=(int)GameConstants.TextureType.Normal;

        private readonly Sprite[,] handSprite;
        private readonly int[] handTextureType;

        private readonly Sprite[] tryAgainSprite;
        private int tryAgainSpriteTextureType;

        private readonly Sprite timeoutProgressBar;
        private readonly int timerStepSizePerSecond;

        private readonly Sprite []confirmExit;
        private int confirmExitTextureType = (int)GameConstants.TextureType.Normal;

        private readonly Vector2[] kinectHandPosition;
        private readonly bool[] cursorOnButtonState;
        private readonly Stopwatch buttonTimeoutStopwatch;

        public int scoreInCurrentGame;

        #endregion


        public MainMenu(HopnetGame hopnetGame)
        {
            highScores = new HighScores();
            highScores.Load();
            buttonTimeoutStopwatch = new Stopwatch();
            buttonTimeoutStopwatch.Reset();

            hopNetGame = hopnetGame;
            handTextureType = new int[2];
            handTextureType[(int)GameConstants.Hand.Left] = (int)GameConstants.TextureType.Normal;
            handTextureType[(int)GameConstants.Hand.Right] = (int)GameConstants.TextureType.Normal;

            cursorOnButtonState = new bool[2];

            kinectHandPosition = new Vector2[2];
            kinectHandPosition[(int)GameConstants.Hand.Left] = Vector2.Zero;
            kinectHandPosition[(int)GameConstants.Hand.Right] = Vector2.Zero;

            timerStepSizePerSecond = GameConstants.HorizontalGameResolution / GameConstants.ButtonTimeDelayInSeconds;
            hScale = (GameConstants.HorizontalGameResolution/ GameConstants.DefaultHorizontalResolutionToScaleInto);
            vScale = (GameConstants.VerticalGameResolution / GameConstants.DefaultVerticalResolutionToScaleInto);

            backgroundSprite = new Sprite();
            timeoutProgressBar = new Sprite { rectangle = new Rectangle(0, 0, 0, GameConstants.VerticalGameResolution/80) };
            newGameSprite = new Sprite[GameConstants.MenuTextureNumber];
            scoresSprite = new Sprite[GameConstants.MenuTextureNumber];
            goBackSprite = new Sprite[GameConstants.MenuTextureNumber];
            exitSprite = new Sprite[GameConstants.MenuTextureNumber];
            easyDifficulty = new Sprite[GameConstants.MenuTextureNumber];
            mediumDifficulty = new Sprite[GameConstants.MenuTextureNumber];
            hardDifficulty = new Sprite[GameConstants.MenuTextureNumber];
            confirmExit = new Sprite[GameConstants.MenuTextureNumber];
            handSprite = new Sprite[2, GameConstants.MenuTextureNumber];
            tryAgainSprite = new Sprite[GameConstants.MenuTextureNumber];

            #region initialization of every sprite's properties
            for (int i = 0; i < GameConstants.MenuTextureNumber; i++)
            {
                confirmExit[i] = new Sprite
                {
                    Rectangle = new Rectangle(
                        (int)(GameConstants.HorizontalGameResolution / 2 - GameConstants.DefaultMenuBtnWidth * hScale / 2),
                        (int)(GameConstants.VerticalGameResolution / 2 - GameConstants.DefaultMenuBtnHeight * vScale / 2),
                        (int)(GameConstants.DefaultMenuBtnWidth * hScale), (int)(GameConstants.DefaultMenuBtnHeight * vScale))
                };


                tryAgainSprite[i] = new Sprite
                {
                    Rectangle = new Rectangle(
                                                (int)(GameConstants.HorizontalGameResolution/2 -GameConstants.DefaultMenuBtnWidth*hScale/2),
                                                (int)(GameConstants.VerticalGameResolution/2 -GameConstants.DefaultMenuBtnHeight*vScale/2),
                                                (int) (GameConstants.DefaultMenuBtnWidth*hScale),(int) (GameConstants.DefaultMenuBtnHeight*vScale))
                };



                goBackSprite[i] = new Sprite
                {
                    Rectangle = new Rectangle((int)(GameConstants.HorizontalSpaceFromLeft * hScale),
                                              (int)
                                              ((3 * GameConstants.VerticalSpaceBetweenButtons + 2 * GameConstants.DefaultMenuBtnHeight) *
                                               (vScale)),
                                              (int)(GameConstants.DefaultMenuBtnWidth / 2 * hScale),
                                              (int)(GameConstants.DefaultMenuBtnHeight / 2 * vScale))
                };

                easyDifficulty[i] = new Sprite
                {
                    Rectangle = new Rectangle(
                        (int)(GameConstants.HorizontalGameResolution / 2 - GameConstants.DefaultMenuBtnWidth * hScale / 2),
                        (int)(GameConstants.VerticalSpaceBetweenButtons * vScale),
                        (int)(GameConstants.DefaultMenuBtnWidth * hScale),
                        (int)(GameConstants.DefaultMenuBtnHeight * vScale))
                };

                mediumDifficulty[i] = new Sprite
                {
                    Rectangle = new Rectangle(
                        (int)(GameConstants.HorizontalGameResolution / 2 - GameConstants.DefaultMenuBtnWidth * hScale / 2),
                        (int)((2 * GameConstants.VerticalSpaceBetweenButtons + GameConstants.DefaultMenuBtnHeight) * (vScale)),
                        (int)(GameConstants.DefaultMenuBtnWidth * hScale),
                        (int)(GameConstants.DefaultMenuBtnHeight * vScale))
                };

                hardDifficulty[i] = new Sprite
                {
                    Rectangle = new Rectangle(
                        (int)(GameConstants.HorizontalGameResolution / 2 - GameConstants.DefaultMenuBtnWidth * hScale / 2),
                        (int)((3 * GameConstants.VerticalSpaceBetweenButtons + 2 * GameConstants.DefaultMenuBtnHeight) * (vScale)),
                        (int)(GameConstants.DefaultMenuBtnWidth * hScale),
                        (int)(GameConstants.DefaultMenuBtnHeight * vScale))
                };

                handSprite[(int)GameConstants.Hand.Left, i] = new Sprite();
                handSprite[(int)GameConstants.Hand.Right, i] = new Sprite();

                handSprite[(int)GameConstants.Hand.Left, i].Position = new Vector2(GameConstants.HorizontalGameResolution / 2, GameConstants.VerticalGameResolution / 2);
                handSprite[(int)GameConstants.Hand.Right, i].Position = new Vector2(GameConstants.HorizontalGameResolution / 2, GameConstants.VerticalGameResolution / 2);

                handSprite[(int)GameConstants.Hand.Left, i].Rectangle = new Rectangle((int)(GameConstants.HorizontalGameResolution / 2 - (GameConstants.HandCursorRadius / 2) * hScale),
                    (int)(GameConstants.VerticalGameResolution / 2 - (GameConstants.HandCursorRadius / 2) * vScale), (int)(GameConstants.HandCursorRadius * hScale), (int)(GameConstants.HandCursorRadius * vScale));

                handSprite[(int)GameConstants.Hand.Right, i].Rectangle = new Rectangle((int)(GameConstants.HorizontalGameResolution / 2 - (GameConstants.HandCursorRadius / 2) * hScale),
                    (int)(GameConstants.VerticalGameResolution / 2 - (GameConstants.HandCursorRadius / 2) * vScale), (int)(GameConstants.HandCursorRadius * hScale), (int)(GameConstants.HandCursorRadius * vScale));

                newGameSprite[i] = new Sprite
                {
                    Rectangle = new Rectangle(
                        (int)(GameConstants.HorizontalSpaceFromLeft * hScale),
                        (int)(GameConstants.VerticalSpaceBetweenButtons * vScale), (int)(GameConstants.DefaultMenuBtnWidth * hScale),
                        (int)(GameConstants.DefaultMenuBtnHeight * vScale))
                };

                scoresSprite[i] = new Sprite
                {
                    Rectangle = new Rectangle(
                        (int)(GameConstants.HorizontalSpaceFromLeft * hScale),
                        (int)((2 * GameConstants.VerticalSpaceBetweenButtons + GameConstants.DefaultMenuBtnHeight) * (vScale)),
                        (int)(GameConstants.DefaultMenuBtnWidth * hScale), (int)(GameConstants.DefaultMenuBtnHeight * vScale))
                };

                exitSprite[i] = new Sprite
                {
                    Rectangle = new Rectangle(
                        (int)(GameConstants.HorizontalSpaceFromLeft * hScale),
                        (int)((3 * GameConstants.VerticalSpaceBetweenButtons + 2 * GameConstants.DefaultMenuBtnHeight) * (vScale)),
                        (int)(GameConstants.DefaultMenuBtnWidth * hScale), (int)(GameConstants.DefaultMenuBtnHeight * vScale))
                };
            }
            backgroundSprite.Rectangle = new Rectangle(0, 0, GameConstants.HorizontalGameResolution, GameConstants.VerticalGameResolution);
            #endregion
        }

        public bool IsGameInMenuMode
        {
            get { return isGameInMenu; }
            set { isGameInMenu = value; }
        }

        private Vector2 GetTextureCenter(Rectangle rectangle)
        {
            return new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2);
        }

        private void ChangeCursorTexture(bool []cursorState)
        {
            if(cursorState.Length!=2){throw new ArgumentOutOfRangeException();}

            switch (cursorState[(int)GameConstants.Hand.Left])
            {
                case true:
                    handTextureType[(int)GameConstants.Hand.Left] = (int)GameConstants.TextureType.WithBorder;
                    break;
                case false:
                    handTextureType[(int)GameConstants.Hand.Left] = (int)GameConstants.TextureType.Normal;
                    break;
            }

            switch (cursorState[(int)GameConstants.Hand.Right])
            {
                case true:
                    handTextureType[(int)GameConstants.Hand.Right] = (int)GameConstants.TextureType.WithBorder;
                    break;
                case false:
                    handTextureType[(int)GameConstants.Hand.Right] = (int)GameConstants.TextureType.Normal;
                    break;
            }
        }
        private void ChangeButtonTexture(bool []cursorState,  ref int spriteTexture)
        {
            if (cursorState.Length != 2) { throw new ArgumentOutOfRangeException(); }

            if (cursorState[(int)GameConstants.Hand.Left] & cursorState[(int)GameConstants.Hand.Right])
            {
                spriteTexture = (int)GameConstants.TextureType.WithBorder;
            }
            else
            {
                spriteTexture = (int)GameConstants.TextureType.Normal;
            }
        }
        private bool IsCanChangeState(bool []cursorState)
        {
            if (cursorState.Length != 2) { throw new ArgumentOutOfRangeException(); }
            if(cursorState[(int)GameConstants.Hand.Left] & cursorState[(int)GameConstants.Hand.Right] & AreHandsTogether())
            {
                return true;
            }
            return false;
        }
        private bool IsButtonSelected(bool []cursorState)
        {
            if (cursorState.Length != 2) { throw new ArgumentOutOfRangeException(); }

            if (cursorState[(int)GameConstants.Hand.Left] & cursorState[(int)GameConstants.Hand.Right])
            {
                return true;
            }

            return false;
        }
        private GameConstants.MenuButton CheckCurrentButton(Rectangle spriteRectangle, ref int spriteTexture, GameConstants.MenuButton newState, ref GameConstants.MenuButton lastState)
        {
            var isCursorInsideButton = new bool[2];
            ChangeButtonTexture(isCursorInsideButton, ref spriteTexture);
            if (lastState == GameConstants.MenuButton.None)
            {
                isCursorInsideButton[(int)GameConstants.Hand.Left] = IsCursorInButtonArea(spriteRectangle, handSprite[(int)GameConstants.Hand.Left, handTextureType[(int)GameConstants.Hand.Left]].Rectangle);
                isCursorInsideButton[(int)GameConstants.Hand.Right] = IsCursorInButtonArea(spriteRectangle, handSprite[(int)GameConstants.Hand.Right, handTextureType[(int)GameConstants.Hand.Right]].Rectangle);

                if (isCursorInsideButton[(int)GameConstants.Hand.Left]) { cursorOnButtonState[(int)GameConstants.Hand.Left] = isCursorInsideButton[(int)GameConstants.Hand.Left]; }
                if (isCursorInsideButton[(int)GameConstants.Hand.Right]) { cursorOnButtonState[(int)GameConstants.Hand.Right] = isCursorInsideButton[(int)GameConstants.Hand.Right]; }

                if (IsButtonSelected(isCursorInsideButton))
                {
                    ChangeButtonTexture(isCursorInsideButton, ref spriteTexture);
                    if (IsCanChangeState(isCursorInsideButton))
                    {
                        return newState;
                    }
                }
            }
            return lastState;
        }
        private GameConstants.MenuButton CheckButtonSelect()
        {
            var buttonState = GameConstants.MenuButton.None;

            cursorOnButtonState[(int)GameConstants.Hand.Left] = false;
            cursorOnButtonState[(int)GameConstants.Hand.Right] = false;

            switch(state)
            {
                case GameConstants.MenuState.InMainMenu:
                    buttonState = CheckCurrentButton(newGameSprite[newGameTextureType].Rectangle, ref newGameTextureType, GameConstants.MenuButton.NewGame, ref buttonState);
                    buttonState = CheckCurrentButton(scoresSprite[scoresTextureType].Rectangle, ref scoresTextureType, GameConstants.MenuButton.Scores, ref buttonState);
                    buttonState = CheckCurrentButton(exitSprite[exitTextureType].Rectangle, ref exitTextureType, GameConstants.MenuButton.Exit, ref buttonState);
                    break;

                case GameConstants.MenuState.InScores:
                    buttonState = CheckCurrentButton(goBackSprite[goBackTextureType].Rectangle, ref goBackTextureType, GameConstants.MenuButton.GoBack, ref buttonState);
                    break;

                case GameConstants.MenuState.OnDifficultySelect:
                    buttonState = CheckCurrentButton(easyDifficulty[easyDifficultyTextureType].Rectangle, ref easyDifficultyTextureType, GameConstants.MenuButton.EasyDifficulty, ref buttonState);
                    buttonState = CheckCurrentButton(mediumDifficulty[mediumDifficultyTextureType].Rectangle, ref mediumDifficultyTextureType, GameConstants.MenuButton.MediumDifficulty, ref buttonState);
                    buttonState = CheckCurrentButton(hardDifficulty[hardDifficultyTextureType].Rectangle, ref hardDifficultyTextureType, GameConstants.MenuButton.HardDifficulty, ref buttonState);
                    buttonState = CheckCurrentButton(goBackSprite[goBackTextureType].Rectangle, ref goBackTextureType, GameConstants.MenuButton.GoBack, ref buttonState);
                    break;

                case GameConstants.MenuState.OnExit:
                    buttonState = CheckCurrentButton(goBackSprite[goBackTextureType].Rectangle, ref goBackTextureType, GameConstants.MenuButton.GoBack, ref buttonState);
                    buttonState = CheckCurrentButton(confirmExit[confirmExitTextureType].Rectangle, ref confirmExitTextureType, GameConstants.MenuButton.ConfirmExit, ref buttonState);
                    break;

                case GameConstants.MenuState.ExitConfirmed:
                    hopNetGame.Exit();
                    break;

                case GameConstants.MenuState.AfterGameLoss:
                    buttonState = CheckCurrentButton(goBackSprite[goBackTextureType].Rectangle, ref goBackTextureType, GameConstants.MenuButton.GoBack, ref buttonState);
                    buttonState = CheckCurrentButton(tryAgainSprite[tryAgainSpriteTextureType].Rectangle, ref tryAgainSpriteTextureType, GameConstants.MenuButton.PlayAgain, ref buttonState);
                    break;
            }
            ChangeCursorTexture(cursorOnButtonState);
            return buttonState;
        }      
        private bool IsCursorInButtonArea(Rectangle buttonRectangle,Rectangle cursor)
        {
            Vector2 recangleMiddlePos = GetTextureCenter(buttonRectangle);
            Vector2 cursorMiddlePos = GetTextureCenter(cursor);

            var horizontalDistance = (int)Math.Sqrt(Math.Pow(recangleMiddlePos.X - cursorMiddlePos.X, 2));
            var verticallDistance = (int)Math.Sqrt(Math.Pow(recangleMiddlePos.Y - cursorMiddlePos.Y, 2));

            if (horizontalDistance < (buttonRectangle.Width / 2 + cursor.Width / 2))
            {
                if (verticallDistance < (buttonRectangle.Height / 2 + cursor.Height / 2))
                {
                    return true;
                }
            }
            return false;
        }
        
        private bool AreHandsTogether()
        {
            Vector2 leftHandleMiddle = GetTextureCenter(handSprite[(int)GameConstants.Hand.Left, handTextureType[(int)GameConstants.Hand.Left]].Rectangle);
            Vector2 rightHandleMiddle = GetTextureCenter(handSprite[(int)GameConstants.Hand.Right, handTextureType[(int)GameConstants.Hand.Right]].Rectangle);

            double distance = Math.Sqrt((leftHandleMiddle.X - rightHandleMiddle.X) * (leftHandleMiddle.X - rightHandleMiddle.X) +
                (leftHandleMiddle.Y - rightHandleMiddle.Y) * (leftHandleMiddle.Y - rightHandleMiddle.Y));

            if (distance < 2 * GameConstants.HandCursorRadius) { return true; }

            return false;
        }



        public void KinectUpdate(KinectData kinectData, Vector2 mousePos)
        {
            if (kinectData!=null && kinectData.Skeleton!= null)
            {
                idlePersonHeight = kinectData.PersonIdleHeight;
                kinectHandPosition[(int)GameConstants.Hand.Left].X = ((0.5f * kinectData.Skeleton.Joints[JointType.HandLeft].Position.X) + 0.5f) * GameConstants.HorizontalGameResolution;
                kinectHandPosition[(int)GameConstants.Hand.Left].Y = ((-0.5f * kinectData.Skeleton.Joints[JointType.HandLeft].Position.Y) + 0.5f + 0.3f * idlePersonHeight) * GameConstants.VerticalGameResolution;
                kinectHandPosition[(int)GameConstants.Hand.Right].X = ((0.5f * kinectData.Skeleton.Joints[JointType.HandRight].Position.X) + 0.5f) * GameConstants.HorizontalGameResolution;
                kinectHandPosition[(int)GameConstants.Hand.Right].Y = ((-0.5f * kinectData.Skeleton.Joints[JointType.HandRight].Position.Y) + 0.5f + 0.3f * idlePersonHeight) * GameConstants.VerticalGameResolution;
            }
            else
            {
                kinectHandPosition[(int)GameConstants.Hand.Left].X = mousePos.X-40;
                kinectHandPosition[(int)GameConstants.Hand.Left].Y = mousePos.Y;
                kinectHandPosition[(int)GameConstants.Hand.Right].X = mousePos.X + 40;
                kinectHandPosition[(int)GameConstants.Hand.Right].Y = mousePos.Y;
            }

            currentButton = CheckButtonSelect();

            if (currentButton != GameConstants.MenuButton.None & currentButton == lastButton)
            {
                if(!buttonTimeoutStopwatch.IsRunning)
                {
                    buttonTimeoutStopwatch.Start();
                }
                else
                {
                    timeoutProgressBar.rectangle.Width = (int)(buttonTimeoutStopwatch.Elapsed.TotalSeconds*timerStepSizePerSecond);
                }
                
            }
            else
            {
                buttonTimeoutStopwatch.Reset();
                timeoutProgressBar.rectangle.Width = 0;
            }

            switch (currentButton)
            {
                case GameConstants.MenuButton.Scores:
                    if (buttonTimeoutStopwatch.Elapsed.TotalSeconds >= GameConstants.ButtonTimeDelayInSeconds) { state = GameConstants.MenuState.InScores; buttonTimeoutStopwatch.Reset(); timeoutProgressBar.rectangle.Width = 0; }
                    break;

                case GameConstants.MenuButton.Exit:
                    if (buttonTimeoutStopwatch.Elapsed.TotalSeconds >= GameConstants.ButtonTimeDelayInSeconds ) { state = GameConstants.MenuState.OnExit; buttonTimeoutStopwatch.Reset(); timeoutProgressBar.rectangle.Width = 0; }
                    break;

                case GameConstants.MenuButton.GoBack:
                    if (buttonTimeoutStopwatch.Elapsed.TotalSeconds >= GameConstants.ButtonTimeDelayInSeconds ) { state = GameConstants.MenuState.InMainMenu; buttonTimeoutStopwatch.Reset(); timeoutProgressBar.rectangle.Width = 0; }
                    break;

                case GameConstants.MenuButton.NewGame:
                    if (buttonTimeoutStopwatch.Elapsed.TotalSeconds >= GameConstants.ButtonTimeDelayInSeconds ) { state = GameConstants.MenuState.OnDifficultySelect; buttonTimeoutStopwatch.Reset(); timeoutProgressBar.rectangle.Width = 0; }
                    break;

                case GameConstants.MenuButton.EasyDifficulty:
                    selectedDifficulty = (int)GameConstants.GameDifficulty.Easy;
                    if (buttonTimeoutStopwatch.Elapsed.TotalSeconds >= GameConstants.ButtonTimeDelayInSeconds) { state = GameConstants.MenuState.Playing; isGameInMenu = false; buttonTimeoutStopwatch.Reset(); timeoutProgressBar.rectangle.Width = 0; }
                    break;

                case GameConstants.MenuButton.MediumDifficulty:
                    selectedDifficulty = (int)GameConstants.GameDifficulty.Medium;
                    if (buttonTimeoutStopwatch.Elapsed.TotalSeconds >= GameConstants.ButtonTimeDelayInSeconds) { state = GameConstants.MenuState.Playing; isGameInMenu = false; buttonTimeoutStopwatch.Reset(); timeoutProgressBar.rectangle.Width = 0; }
                    break;

                case GameConstants.MenuButton.HardDifficulty:
                    selectedDifficulty = (int)GameConstants.GameDifficulty.Hard;
                    if (buttonTimeoutStopwatch.Elapsed.TotalSeconds >= GameConstants.ButtonTimeDelayInSeconds) { state = GameConstants.MenuState.Playing; isGameInMenu = false; buttonTimeoutStopwatch.Reset(); timeoutProgressBar.rectangle.Width = 0; }
                    break;

                case GameConstants.MenuButton.ConfirmExit:
                    if (buttonTimeoutStopwatch.Elapsed.TotalSeconds >= GameConstants.ButtonTimeDelayInSeconds) { state = GameConstants.MenuState.ExitConfirmed; buttonTimeoutStopwatch.Reset(); timeoutProgressBar.rectangle.Width = 0; }
                    break;

                case  GameConstants.MenuButton.PlayAgain:
                    if (buttonTimeoutStopwatch.Elapsed.TotalSeconds >= GameConstants.ButtonTimeDelayInSeconds) { state = GameConstants.MenuState.Playing; isGameInMenu = false; buttonTimeoutStopwatch.Reset(); timeoutProgressBar.rectangle.Width = 0; }
                    break;
            }

            lastButton = currentButton;
            handSprite[(int)GameConstants.Hand.Left, handTextureType[(int)GameConstants.Hand.Left]].rectangle.X = (int)kinectHandPosition[(int)GameConstants.Hand.Left].X;
            handSprite[(int)GameConstants.Hand.Left, handTextureType[(int)GameConstants.Hand.Left]].rectangle.Y = (int)kinectHandPosition[(int)GameConstants.Hand.Left].Y;

            handSprite[(int)GameConstants.Hand.Right, handTextureType[(int)GameConstants.Hand.Right]].rectangle.X = (int)kinectHandPosition[(int)GameConstants.Hand.Right].X;
            handSprite[(int)GameConstants.Hand.Right, handTextureType[(int)GameConstants.Hand.Right]].rectangle.Y = (int)kinectHandPosition[(int)GameConstants.Hand.Right].Y;
        }

        public void LoadContent(ContentManager content)
        {
            newGameSprite[0].LoadSprite(content, @"Sprites\testsprite1");
            newGameSprite[1].LoadSprite(content, @"Sprites\testsprite2");
            scoresSprite[0].LoadSprite(content, @"Sprites\testsprite1");
            scoresSprite[1].LoadSprite(content, @"Sprites\testsprite2");
            goBackSprite[0].LoadSprite(content, @"Sprites\testsprite1");
            goBackSprite[1].LoadSprite(content, @"Sprites\testsprite2");
            exitSprite[0].LoadSprite(content, @"Sprites\testsprite1");
            exitSprite[1].LoadSprite(content, @"Sprites\testsprite2");
            backgroundSprite.LoadSprite(content, @"Sprites\testsprite1");
            handSprite[0, 0].LoadSprite(content, @"Sprites\cursor_left_normal");
            handSprite[0, 1].LoadSprite(content, @"Sprites\cursor_left_border");
            handSprite[1, 0].LoadSprite(content, @"Sprites\cursor_right_normal");
            handSprite[1, 1].LoadSprite(content, @"Sprites\cursor_right_border");
            timeoutProgressBar.LoadSprite(content, @"Sprites\menu_progressbar");
            easyDifficulty[0].LoadSprite(content, @"Sprites\testsprite1");
            easyDifficulty[1].LoadSprite(content, @"Sprites\testsprite2");
            mediumDifficulty[0].LoadSprite(content, @"Sprites\testsprite1");
            mediumDifficulty[1].LoadSprite(content, @"Sprites\testsprite2");
            hardDifficulty[0].LoadSprite(content, @"Sprites\testsprite1");
            hardDifficulty[1].LoadSprite(content, @"Sprites\testsprite2");
            confirmExit[0].LoadSprite(content, @"Sprites\testsprite1");
            confirmExit[1].LoadSprite(content, @"Sprites\testsprite2");
            tryAgainSprite[0].LoadSprite(content, @"Sprites\testsprite1");
            tryAgainSprite[1].LoadSprite(content, @"Sprites\testsprite2");
        }


        public void Update()
        {


        }




        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            switch(state)
            {
                case GameConstants.MenuState.InMainMenu:
                        backgroundSprite.DrawByRectangle(spriteBatch);
                        newGameSprite[newGameTextureType].DrawByRectangle(spriteBatch);
                        scoresSprite[scoresTextureType].DrawByRectangle(spriteBatch);
                        exitSprite[exitTextureType].DrawByRectangle(spriteBatch);
                        break;

                case GameConstants.MenuState.InScores:
                        backgroundSprite.DrawByRectangle(spriteBatch);
                        goBackSprite[goBackTextureType].DrawByRectangle(spriteBatch);
                        DrawHighScores(spriteBatch,font);
                        break;

                case GameConstants.MenuState.OnDifficultySelect:
                        backgroundSprite.DrawByRectangle(spriteBatch);
                        easyDifficulty[easyDifficultyTextureType].DrawByRectangle(spriteBatch);
                        mediumDifficulty[mediumDifficultyTextureType].DrawByRectangle(spriteBatch);
                        hardDifficulty[hardDifficultyTextureType].DrawByRectangle(spriteBatch);
                        goBackSprite[goBackTextureType].DrawByRectangle(spriteBatch);
                        break;

                case GameConstants.MenuState.OnExit:
                        backgroundSprite.DrawByRectangle(spriteBatch);
                        confirmExit[confirmExitTextureType].DrawByRectangle(spriteBatch);
                        goBackSprite[goBackTextureType].DrawByRectangle(spriteBatch);
                        break;

                case GameConstants.MenuState.AfterGameLoss:
                        spriteBatch.Begin();
                        spriteBatch.DrawString(font, "Przegrales!\n Twoj wynik to "+ scoreInCurrentGame.ToString(),
                                           new Vector2(GameConstants.HorizontalGameResolution/2,
                                                       GameConstants.VerticalGameResolution/10), Color.Red, 0,
                                           Vector2.Zero, 2, SpriteEffects.None, 1.0f);

                        spriteBatch.End();
                        tryAgainSprite[tryAgainSpriteTextureType].DrawByRectangle(spriteBatch);
                        goBackSprite[goBackTextureType].DrawByRectangle(spriteBatch);
                        break;
            }

            timeoutProgressBar.DrawByRectangle(spriteBatch);
            handSprite[(int)GameConstants.Hand.Left, handTextureType[(int)GameConstants.Hand.Left]].DrawByRectangle(spriteBatch);
            handSprite[(int)GameConstants.Hand.Right, handTextureType[(int)GameConstants.Hand.Right]].DrawByRectangle(spriteBatch);
        }
        void DrawHighScores(SpriteBatch spriteBatch, SpriteFont font)
        {
            int Place = 1;
            spriteBatch.Begin();
            foreach (var Wynik in highScores)
            {
                spriteBatch.DrawString(font, Place.ToString()+"."+Wynik.Time.ToString()+"     "+Wynik.Points.ToString(),
                                   new Vector2(250,200 + Place*20), Color.Red, 0,
                                   Vector2.Zero, 1, SpriteEffects.None, 1.0f);
                ++Place;
            }
            spriteBatch.End();
        }
    }
}


