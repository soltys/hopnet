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
        private float screenWidth;
        private float screenHeight;
        private float hScale=0;
        private float vScale=0;
        private int hSpaceFromLeft=100;
        private int vSpaceBetweenButtons = 20;

        
        private int selectedDifficulty = (int)GameConstants.GameDifficulty.Easy;

        private float heightModifier = 0f;
        private float lastHeightModifier = 4f;
        private float heightThreshold = 0.005f;
        private double heightChangeTime = 3000.0f;
        private float idleHeight = 0.4f;
        private GameConstants.ButtonSelect lastButton = GameConstants.ButtonSelect.None;
        private GameConstants.State state = GameConstants.State.InMainMenu;

        private GameConstants.State MenuState
        {
            get { return state; }
            set { state = value; }
        }

        private Sprite[] newGameSprite;
        private int newGameTextureType=(int)GameConstants.Texture.Normal;

        private Sprite[] scoresSprite;
        private int scoresTextureType = (int)GameConstants.Texture.Normal;

        private Sprite[] goBackSprite;
        private int goBackTextureType = (int)GameConstants.Texture.Normal;

        private Sprite[] exitSprite;
        private int exitTextureType = (int)GameConstants.Texture.Normal;

        private Sprite backgroundSprite;

        private Sprite selectDifficultyText;

        private Sprite[] easyDifficulty;
        private int easyDifficultyTextureType=(int)GameConstants.Texture.Normal;

        private Sprite[] mediumDifficulty;
        private int mediumDifficultyTextureType=(int)GameConstants.Texture.Normal;

        private Sprite[] hardDifficulty;
        private int hardDifficultyTextureType=(int)GameConstants.Texture.Normal;

        private Sprite[,] handSprite;
        private int[] handTextureType;

        private Sprite timeoutProgressBar;
        private const int buttonTimeDelay = 100;
        private int timeCounter = 0;
        private int timerStepSize = 0;

        private Sprite []confirmExit;
        private int confirmExitTextureType = (int)GameConstants.Texture.Normal;

        private Vector2[] kinectHandPosition;
        private bool[] cursorOnButtonState;
        private const int cursorRadius = 64;
        private Stopwatch jumpTimer;
        #endregion

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
                    handTextureType[(int)GameConstants.Hand.Left] = (int)GameConstants.Texture.WithBorder;
                    break;
                case false:
                    handTextureType[(int)GameConstants.Hand.Left] = (int)GameConstants.Texture.Normal;
                    break;
            }

            switch (cursorState[(int)GameConstants.Hand.Right])
            {
                case true:
                    handTextureType[(int)GameConstants.Hand.Right] = (int)GameConstants.Texture.WithBorder;
                    break;
                case false:
                    handTextureType[(int)GameConstants.Hand.Right] = (int)GameConstants.Texture.Normal;
                    break;
            }
        }
        private void ChangeButtonTexture(bool []cursorState,  ref int spriteTexture)
        {
            if (cursorState.Length != 2) { throw new ArgumentOutOfRangeException(); }

            if (cursorState[(int)GameConstants.Hand.Left] & cursorState[(int)GameConstants.Hand.Right])
            {
                spriteTexture = (int)GameConstants.Texture.WithBorder;
            }
            else
            {
                spriteTexture = (int)GameConstants.Texture.Normal;
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
        private GameConstants.ButtonSelect CheckCurrentButton(Rectangle spriteRectangle, ref int spriteTexture, GameConstants.ButtonSelect newState, ref GameConstants.ButtonSelect lastState)
        {
            var isCursorInsideButton = new bool[2];
            ChangeButtonTexture(isCursorInsideButton, ref spriteTexture);
            if (lastState == GameConstants.ButtonSelect.None)
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
        private GameConstants.ButtonSelect CheckButtonSelect()
        {
            var buttonState = GameConstants.ButtonSelect.None;

            cursorOnButtonState[(int)GameConstants.Hand.Left] = false;
            cursorOnButtonState[(int)GameConstants.Hand.Right] = false;

            var isCursorInsideButton = new bool[2];

            switch(state)
            {
                case GameConstants.State.InMainMenu:
                    buttonState = CheckCurrentButton(newGameSprite[newGameTextureType].Rectangle, ref newGameTextureType, GameConstants.ButtonSelect.NewGame, ref buttonState);
                    buttonState = CheckCurrentButton(scoresSprite[scoresTextureType].Rectangle, ref scoresTextureType, GameConstants.ButtonSelect.Scores, ref buttonState);
                    buttonState = CheckCurrentButton(exitSprite[exitTextureType].Rectangle, ref exitTextureType, GameConstants.ButtonSelect.Exit, ref buttonState);
                    break;

                case GameConstants.State.InScores:
                    buttonState = CheckCurrentButton(goBackSprite[goBackTextureType].Rectangle, ref goBackTextureType, GameConstants.ButtonSelect.GoBack, ref buttonState);
                    break;

                case GameConstants.State.OnDifficultySelect:
                    buttonState = CheckCurrentButton(easyDifficulty[easyDifficultyTextureType].Rectangle, ref easyDifficultyTextureType, GameConstants.ButtonSelect.EasyDifficulty, ref buttonState);
                    buttonState = CheckCurrentButton(mediumDifficulty[mediumDifficultyTextureType].Rectangle, ref mediumDifficultyTextureType, GameConstants.ButtonSelect.MediumDifficulty, ref buttonState);
                    buttonState = CheckCurrentButton(hardDifficulty[hardDifficultyTextureType].Rectangle, ref hardDifficultyTextureType, GameConstants.ButtonSelect.HardDifficulty, ref buttonState);
                    buttonState = CheckCurrentButton(goBackSprite[goBackTextureType].Rectangle, ref goBackTextureType, GameConstants.ButtonSelect.GoBack, ref buttonState);
                    break;

                case GameConstants.State.OnExit:
                    buttonState = CheckCurrentButton(goBackSprite[goBackTextureType].Rectangle, ref goBackTextureType, GameConstants.ButtonSelect.GoBack, ref buttonState);
                    buttonState = CheckCurrentButton(confirmExit[confirmExitTextureType].Rectangle, ref confirmExitTextureType, GameConstants.ButtonSelect.ConfirmExit, ref buttonState);
                    break;
                case GameConstants.State.ExitConfirmed:
                    hopNetGame.Exit();
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
        public MainMenu(GraphicsDeviceManager graphics, HopnetGame hopnetGame)
        {
            jumpTimer = new Stopwatch();
            jumpTimer.Reset();

            hopNetGame = hopnetGame;
            handTextureType = new int[2];
            handTextureType[(int)GameConstants.Hand.Left] = (int)GameConstants.Texture.Normal;
            handTextureType[(int)GameConstants.Hand.Right] = (int)GameConstants.Texture.Normal;

            cursorOnButtonState = new bool[2];

            kinectHandPosition = new Vector2[2];
            kinectHandPosition[(int)GameConstants.Hand.Left] = Vector2.Zero;
            kinectHandPosition[(int)GameConstants.Hand.Right] = Vector2.Zero;

            handSprite = new Sprite[2,GameConstants.MenuTextureNumber];

            screenWidth = graphics.PreferredBackBufferWidth;
            screenHeight = graphics.PreferredBackBufferHeight;

            timerStepSize = (int)(screenWidth) / buttonTimeDelay;
            hScale = (screenWidth / GameConstants.DefaultScreenWidth);
            vScale = (screenHeight / GameConstants.DefaultScreenHeight);

            backgroundSprite=new Sprite();
            timeoutProgressBar = new Sprite {rectangle = new Rectangle(0, 0, 0, 30)};
            newGameSprite = new Sprite[GameConstants.MenuTextureNumber];
            scoresSprite = new Sprite[GameConstants.MenuTextureNumber];
            goBackSprite = new Sprite[GameConstants.MenuTextureNumber];
            exitSprite = new Sprite[GameConstants.MenuTextureNumber];
            easyDifficulty = new Sprite[GameConstants.MenuTextureNumber];
            mediumDifficulty = new Sprite[GameConstants.MenuTextureNumber];
            hardDifficulty = new Sprite[GameConstants.MenuTextureNumber];
            confirmExit = new Sprite[GameConstants.MenuTextureNumber];

            #region initialization of every sprite's properties
            for (int i = 0; i < GameConstants.MenuTextureNumber; i++)
            {
                confirmExit[i] = new Sprite
                                     {
                                         Rectangle = new Rectangle(
                                             (int) (screenWidth/2 - GameConstants.DefaultMenuBtnWidth*hScale/2),
                                             (int) (screenHeight/2 - GameConstants.DefaultMenuBtnHeight*vScale/2),
                                             (int) (GameConstants.DefaultMenuBtnWidth*hScale), (int) (GameConstants.DefaultMenuBtnHeight*vScale))
                                     };


                goBackSprite[i] = new Sprite
                                      {
                                          Rectangle = new Rectangle((int) (hSpaceFromLeft*hScale),
                                                                    (int)
                                                                    ((3*vSpaceBetweenButtons + 2*GameConstants.DefaultMenuBtnHeight)*
                                                                     (vScale)),
                                                                    (int) (GameConstants.DefaultMenuBtnWidth/2*hScale),
                                                                    (int) (GameConstants.DefaultMenuBtnHeight/2*vScale))
                                      };

                easyDifficulty[i] = new Sprite
                                        {
                                            Rectangle = new Rectangle(
                                                (int) (screenWidth/2 - GameConstants.DefaultMenuBtnWidth*hScale/2),
                                                (int) (vSpaceBetweenButtons*vScale), (int) (GameConstants.DefaultMenuBtnWidth*hScale),
                                                (int) (GameConstants.DefaultMenuBtnHeight*vScale))
                                        };

                mediumDifficulty[i] = new Sprite
                                          {
                                              Rectangle = new Rectangle(
                                                  (int) (screenWidth/2 - GameConstants.DefaultMenuBtnWidth*hScale/2),
                                                  (int) ((2*vSpaceBetweenButtons + GameConstants.DefaultMenuBtnHeight)*(vScale)),
                                                  (int) (GameConstants.DefaultMenuBtnWidth*hScale), (int) (GameConstants.DefaultMenuBtnHeight*vScale))
                                          };

                hardDifficulty[i] = new Sprite
                                        {
                                            Rectangle = new Rectangle(
                                                (int) (screenWidth/2 - GameConstants.DefaultMenuBtnWidth*hScale/2),
                                                (int) ((3*vSpaceBetweenButtons + 2*GameConstants.DefaultMenuBtnHeight)*(vScale)),
                                                (int) (GameConstants.DefaultMenuBtnWidth*hScale), (int) (GameConstants.DefaultMenuBtnHeight*vScale))
                                        };

                handSprite[(int)GameConstants.Hand.Left, i] = new Sprite();
                handSprite[(int)GameConstants.Hand.Right, i] = new Sprite();

                handSprite[(int)GameConstants.Hand.Left, i].Position = new Vector2(screenWidth / 2, screenHeight / 2);
                handSprite[(int)GameConstants.Hand.Right, i].Position = new Vector2(screenWidth / 2, screenHeight / 2);

                handSprite[(int)GameConstants.Hand.Left, i].Rectangle = new Rectangle((int)(screenWidth / 2 - (cursorRadius / 2) * hScale),
                    (int)(screenHeight / 2 - (cursorRadius / 2) * vScale), (int)(cursorRadius * hScale), (int)(cursorRadius * vScale));

                handSprite[(int)GameConstants.Hand.Right, i].Rectangle = new Rectangle((int)(screenWidth / 2 - (cursorRadius / 2) * hScale),
                    (int)(screenHeight / 2 - (cursorRadius / 2) * vScale), (int)(cursorRadius * hScale), (int)(cursorRadius * vScale));

                newGameSprite[i] = new Sprite
                                       {
                                           Rectangle = new Rectangle(
                                               (int) (hSpaceFromLeft*hScale),
                                               (int) (vSpaceBetweenButtons*vScale), (int) (GameConstants.DefaultMenuBtnWidth*hScale),
                                               (int) (GameConstants.DefaultMenuBtnHeight*vScale))
                                       };

                scoresSprite[i] = new Sprite
                                      {
                                          Rectangle = new Rectangle(
                                              (int) (hSpaceFromLeft*hScale),
                                              (int) ((2*vSpaceBetweenButtons + GameConstants.DefaultMenuBtnHeight)*(vScale)),
                                              (int) (GameConstants.DefaultMenuBtnWidth*hScale), (int) (GameConstants.DefaultMenuBtnHeight*vScale))
                                      };

                exitSprite[i] = new Sprite
                                    {
                                        Rectangle = new Rectangle(
                                            (int) (hSpaceFromLeft*hScale),
                                            (int) ((3*vSpaceBetweenButtons + 2*GameConstants.DefaultMenuBtnHeight)*(vScale)),
                                            (int) (GameConstants.DefaultMenuBtnWidth*hScale), (int) (GameConstants.DefaultMenuBtnHeight*vScale))
                                    };
            }
            backgroundSprite.Rectangle = new Rectangle(0, 0, (int)screenWidth, (int)screenHeight);
            #endregion
        }
        private bool AreHandsTogether()
        {
            Vector2 leftHandleMiddle = GetTextureCenter(handSprite[(int)GameConstants.Hand.Left, handTextureType[(int)GameConstants.Hand.Left]].Rectangle);
            Vector2 rightHandleMiddle = GetTextureCenter(handSprite[(int)GameConstants.Hand.Right, handTextureType[(int)GameConstants.Hand.Right]].Rectangle);

            double distance = Math.Sqrt((leftHandleMiddle.X - rightHandleMiddle.X) * (leftHandleMiddle.X - rightHandleMiddle.X) +
                (leftHandleMiddle.Y - rightHandleMiddle.Y) * (leftHandleMiddle.Y - rightHandleMiddle.Y));

            if (distance < 2 * cursorRadius) { return true; }

            return false;
        }
        private void CalculateHeightPosition(Skeleton skeleton)
        {
            if (!jumpTimer.IsRunning)
            {
                lastHeightModifier = heightModifier;
            }

            heightModifier = skeleton.Joints[JointType.ShoulderCenter].Position.Y;

            if (Math.Abs(lastHeightModifier - heightModifier) > heightThreshold)
            {
                if (!jumpTimer.IsRunning)
                {
                    jumpTimer.Start();
                }
                else
                {
                    if (jumpTimer.Elapsed.TotalMilliseconds > heightChangeTime)
                    {
                        idleHeight = heightModifier;
                        jumpTimer.Reset();
                    }
                }
            }
        }

        public void KinectUpdate(Skeleton kinectData, Vector2 mousePos)
        {
            if (isGameInMenu)
            {
                if (kinectData != null)
                {
                    CalculateHeightPosition(kinectData);
                    kinectHandPosition[(int)GameConstants.Hand.Left].X = ((0.5f * kinectData.Joints[JointType.HandLeft].Position.X) + 0.5f) * screenWidth;
                    kinectHandPosition[(int)GameConstants.Hand.Left].Y = ((-0.5f * kinectData.Joints[JointType.HandLeft].Position.Y) + 0.5f + 0.3f*idleHeight) * screenHeight;
                    kinectHandPosition[(int)GameConstants.Hand.Right].X = ((0.5f * kinectData.Joints[JointType.HandRight].Position.X) + 0.5f) * screenWidth;
                    kinectHandPosition[(int)GameConstants.Hand.Right].Y = ((-0.5f * kinectData.Joints[JointType.HandRight].Position.Y) + 0.5f + 0.3f*idleHeight) * screenHeight;
                }
                else
                {
                    kinectHandPosition[(int)GameConstants.Hand.Left].X = mousePos.X-40;
                    kinectHandPosition[(int)GameConstants.Hand.Left].Y = mousePos.Y;
                    kinectHandPosition[(int)GameConstants.Hand.Right].X = mousePos.X + 40;
                    kinectHandPosition[(int)GameConstants.Hand.Right].Y = mousePos.Y;
                }

                var selectedButton = CheckButtonSelect();

                if (selectedButton != GameConstants.ButtonSelect.None & selectedButton==lastButton)
                {
                    timeCounter++;
                    timeoutProgressBar.rectangle.Width += timerStepSize;
                }
                else
                {
                    timeCounter = 0;
                    timeoutProgressBar.rectangle.Width=0;
                }

                switch (selectedButton)
                {
                    case GameConstants.ButtonSelect.Scores:
                        if (timeCounter > buttonTimeDelay) { state = GameConstants.State.InScores; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0; }
                        break;

                    case GameConstants.ButtonSelect.Exit:
                        if (timeCounter > buttonTimeDelay) { state = GameConstants.State.OnExit; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0; }
                        break;

                    case GameConstants.ButtonSelect.GoBack:
                        if (timeCounter > buttonTimeDelay) { state = GameConstants.State.InMainMenu; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0; }
                        break;

                    case GameConstants.ButtonSelect.NewGame:
                        if (timeCounter > buttonTimeDelay) { state = GameConstants.State.OnDifficultySelect; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0; }
                        break;

                    case GameConstants.ButtonSelect.EasyDifficulty:
                        selectedDifficulty = (int)GameConstants.GameDifficulty.Easy;
                        if (timeCounter > buttonTimeDelay) { state = GameConstants.State.Playing; isGameInMenu = false; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0; }
                        break;

                    case GameConstants.ButtonSelect.MediumDifficulty:
                        selectedDifficulty = (int)GameConstants.GameDifficulty.Medium;
                        if (timeCounter > buttonTimeDelay) { state = GameConstants.State.Playing; isGameInMenu = false; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0; }
                        break;

                    case GameConstants.ButtonSelect.HardDifficulty:
                        selectedDifficulty = (int)GameConstants.GameDifficulty.Hard;
                        if (timeCounter > buttonTimeDelay) { state = GameConstants.State.Playing; isGameInMenu = false; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0; }
                        break;

                    case GameConstants.ButtonSelect.ConfirmExit:
                        if (timeCounter > buttonTimeDelay) { state = GameConstants.State.ExitConfirmed; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0;}
                        break;
                }

                lastButton = selectedButton;
                handSprite[(int)GameConstants.Hand.Left, handTextureType[(int)GameConstants.Hand.Left]].rectangle.X = (int)kinectHandPosition[(int)GameConstants.Hand.Left].X;
                handSprite[(int)GameConstants.Hand.Left, handTextureType[(int)GameConstants.Hand.Left]].rectangle.Y = (int)kinectHandPosition[(int)GameConstants.Hand.Left].Y;

                handSprite[(int)GameConstants.Hand.Right, handTextureType[(int)GameConstants.Hand.Right]].rectangle.X = (int)kinectHandPosition[(int)GameConstants.Hand.Right].X;
                handSprite[(int)GameConstants.Hand.Right, handTextureType[(int)GameConstants.Hand.Right]].rectangle.Y = (int)kinectHandPosition[(int)GameConstants.Hand.Right].Y;
            }
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
            timeoutProgressBar.LoadSprite(content, @"Sprites\progress_bar");
            easyDifficulty[0].LoadSprite(content, @"Sprites\testsprite1");
            easyDifficulty[1].LoadSprite(content, @"Sprites\testsprite2");
            mediumDifficulty[0].LoadSprite(content, @"Sprites\testsprite1");
            mediumDifficulty[1].LoadSprite(content, @"Sprites\testsprite2");
            hardDifficulty[0].LoadSprite(content, @"Sprites\testsprite1");
            hardDifficulty[1].LoadSprite(content, @"Sprites\testsprite2");
            confirmExit[0].LoadSprite(content, @"Sprites\testsprite1");
            confirmExit[1].LoadSprite(content, @"Sprites\testsprite2");
        }
        public void Draw(SpriteBatch spriteBatch,SpriteFont font)
        {
            backgroundSprite.DrawByRectangle(spriteBatch);
            
            switch(state)
            {
                case GameConstants.State.InMainMenu:
                    {
                        newGameSprite[newGameTextureType].DrawByRectangle(spriteBatch);
                        scoresSprite[scoresTextureType].DrawByRectangle(spriteBatch);
                        exitSprite[exitTextureType].DrawByRectangle(spriteBatch);
                        break;
                    }
                case GameConstants.State.InScores:
                    {
                        goBackSprite[goBackTextureType].DrawByRectangle(spriteBatch);
                        break;
                    }
                case GameConstants.State.OnDifficultySelect:
                    {
                        easyDifficulty[easyDifficultyTextureType].DrawByRectangle(spriteBatch);
                        mediumDifficulty[mediumDifficultyTextureType].DrawByRectangle(spriteBatch);
                        hardDifficulty[hardDifficultyTextureType].DrawByRectangle(spriteBatch);
                        goBackSprite[goBackTextureType].DrawByRectangle(spriteBatch);
                        break;
                    }
                case GameConstants.State.OnExit:
                    {
                        confirmExit[confirmExitTextureType].DrawByRectangle(spriteBatch);
                        goBackSprite[goBackTextureType].DrawByRectangle(spriteBatch);
                        break;
                    }
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, idleHeight.ToString(), new Vector2(400, 200), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.End();
            timeoutProgressBar.DrawByRectangle(spriteBatch);
            handSprite[(int)GameConstants.Hand.Left, handTextureType[(int)GameConstants.Hand.Left]].DrawByRectangle(spriteBatch);
            handSprite[(int)GameConstants.Hand.Right, handTextureType[(int)GameConstants.Hand.Right]].DrawByRectangle(spriteBatch);
        }
    }
}


