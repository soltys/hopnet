using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Kinect;


namespace Game
{
    class MainMenu
    {
        private HopnetGame hopNetGame;
        private bool isGameInMenu=true;
        private float screenWidth;
        private float screenHeight;
        private const int defaultScreenWidth = 1280;
        private const int defaultScreenHeight = 720;
        private float hScale=0;
        private float vScale=0;
        private int hSpaceFromLeft=100;
        private int vSpaceBetweenButtons = 20;
        private int textureNumber = 2;

        public const int defaultBtnWidth=420;
        public const int defaultBtnHeight=210;

        private enum Texture : int { Normal=0, WithBorder=1 };
        private enum Hand : int { Left = 0, Right = 1 };
        private enum State : int { InMainMenu = 0, InNewGame = 1, InScores = 2, OnExit = 3, Playing = 4, OnDifficultySelect = 5, ExitConfirmed = 6 }
        private enum ButtonSelect : int { Scores = 0, Exit = 1, GoBack = 2, None = 3, NewGame = 4, EasyDifficulty = 5, MediumDifficulty = 6, HardDifficulty = 7, ConfirmExit = 8 }
        private enum GameDifficulty : int { Easy = 1, Medium = 2, Hard = 3 }
        private int selectedDifficulty = (int)GameDifficulty.Easy;
        private ButtonSelect lastButton = ButtonSelect.None;

        private State state = State.InMainMenu;

        private Sprite[] newGameSprite;
        private int newGameTextureType=(int)Texture.Normal;

        private Sprite[] scoresSprite;
        private int scoresTextureType = (int)Texture.Normal;

        private Sprite[] goBackSprite;
        private int goBackTextureType = (int)Texture.Normal;

        private Sprite[] exitSprite;
        private int exitTextureType = (int)Texture.Normal;

        private Sprite backgroundSprite;

        private Sprite selectDifficultyText;

        private Sprite[] easyDifficulty;
        private int easyDifficultyTextureType=(int)Texture.Normal;

        private Sprite[] mediumDifficulty;
        private int mediumDifficultyTextureType=(int)Texture.Normal;

        private Sprite[] hardDifficulty;
        private int hardDifficultyTextureType=(int)Texture.Normal;

        private Sprite[,] handSprite;
        private int[] handTextureType;

        private Sprite timeoutProgressBar;
        private const int buttonTimeDelay = 100;
        private int timeCounter = 0;
        private int timerStepSize = 0;

        private Sprite []confirmExit;
        private int confirmExitTextureType = (int)Texture.Normal;

        private Vector2[] kinectHandPosition;
        private bool[] cursorState;
        private const int cursorRadius = 64;

        public bool IsGameInMenuMode
        {
            get { return isGameInMenu; }
        }
        private Vector2 GetTextureCenter(Rectangle rectangle)
        {
            return new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2);
        }
        private void ChangeCursorTexture(bool []cursorState)
        {
            if(cursorState.Length!=2){throw new ArgumentOutOfRangeException();}

            switch (cursorState[(int)Hand.Left])
            {
                case true:
                    handTextureType[(int)Hand.Left] = (int)Texture.WithBorder;
                    break;
                case false:
                    handTextureType[(int)Hand.Left] = (int)Texture.Normal;
                    break;
            }

            switch (cursorState[(int)Hand.Right])
            {
                case true:
                    handTextureType[(int)Hand.Right] = (int)Texture.WithBorder;
                    break;
                case false:
                    handTextureType[(int)Hand.Right] = (int)Texture.Normal;
                    break;
            }
        }
        private void ChangeButtonTexture(bool []cursorState,  ref int spriteTexture)
        {
            if (cursorState.Length != 2) { throw new ArgumentOutOfRangeException(); }

            if (cursorState[(int)Hand.Left] & cursorState[(int)Hand.Right])
            {
                spriteTexture = (int)Texture.WithBorder;
            }
            else
            {
                spriteTexture = (int)Texture.Normal;
            }
        }
        private bool IsCanChangeState(bool[] cursorState)
        {
            if (cursorState.Length != 2) { throw new ArgumentOutOfRangeException(); }
            if(cursorState[(int)Hand.Left] & cursorState[(int)Hand.Right] & AreHandsTogether())
            {
                return true;
            }
            return false;
        }
        private bool IsButtonSelected(bool[] cursorState)
        {
            if (cursorState.Length != 2) { throw new ArgumentOutOfRangeException(); }

            if (cursorState[(int)Hand.Left] & cursorState[(int)Hand.Right])
            {
                return true;
            }

            return false;
        }

        private ButtonSelect CheckCurrentButton(Rectangle spriteRectangle, ref int spriteTexture, ButtonSelect newState, ref ButtonSelect lastState)
        {
            bool[] isCursorInsideButton = new bool[2];
            ChangeButtonTexture(isCursorInsideButton, ref spriteTexture);
            if (lastState == ButtonSelect.None)
            {
                isCursorInsideButton[(int)Hand.Left] = IsCursorInButtonArea(spriteRectangle, handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].Rectangle);
                isCursorInsideButton[(int)Hand.Right] = IsCursorInButtonArea(spriteRectangle, handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].Rectangle);

                if (isCursorInsideButton[(int)Hand.Left]) { cursorState[(int)Hand.Left] = isCursorInsideButton[(int)Hand.Left]; }
                if (isCursorInsideButton[(int)Hand.Right]) { cursorState[(int)Hand.Right] = isCursorInsideButton[(int)Hand.Right]; }

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

        private ButtonSelect CheckButtonSelect()
        {
            ButtonSelect buttonState = ButtonSelect.None;

            cursorState[(int)Hand.Left] = false;
            cursorState[(int)Hand.Right] = false;

            bool []isCursorInsideButton = new bool[2];

            switch(state)
            {
                case State.InMainMenu:
                    buttonState = CheckCurrentButton(newGameSprite[newGameTextureType].Rectangle, ref newGameTextureType, ButtonSelect.NewGame, ref buttonState);
                    buttonState = CheckCurrentButton(scoresSprite[scoresTextureType].Rectangle, ref scoresTextureType, ButtonSelect.Scores, ref buttonState);
                    buttonState = CheckCurrentButton(exitSprite[exitTextureType].Rectangle, ref exitTextureType, ButtonSelect.Exit, ref buttonState);
                    break;

                case State.InScores:
                    buttonState = CheckCurrentButton(goBackSprite[goBackTextureType].Rectangle, ref goBackTextureType, ButtonSelect.GoBack, ref buttonState);
                    break;

                case State.OnDifficultySelect:
                    buttonState = CheckCurrentButton(easyDifficulty[easyDifficultyTextureType].Rectangle, ref easyDifficultyTextureType, ButtonSelect.EasyDifficulty, ref buttonState);
                    buttonState = CheckCurrentButton(mediumDifficulty[mediumDifficultyTextureType].Rectangle, ref mediumDifficultyTextureType, ButtonSelect.MediumDifficulty, ref buttonState);
                    buttonState = CheckCurrentButton(hardDifficulty[hardDifficultyTextureType].Rectangle, ref hardDifficultyTextureType, ButtonSelect.HardDifficulty, ref buttonState);
                    buttonState = CheckCurrentButton(goBackSprite[goBackTextureType].Rectangle, ref goBackTextureType, ButtonSelect.GoBack, ref buttonState);
                    break;

                case State.OnExit:
                    buttonState = CheckCurrentButton(goBackSprite[goBackTextureType].Rectangle, ref goBackTextureType, ButtonSelect.GoBack, ref buttonState);
                    buttonState = CheckCurrentButton(confirmExit[confirmExitTextureType].Rectangle, ref confirmExitTextureType, ButtonSelect.ConfirmExit, ref buttonState);
                    break;
                case State.ExitConfirmed:
                    hopNetGame.Exit();
                    break;
            }
            ChangeCursorTexture(cursorState);
            return buttonState;
        }
        
        private bool IsCursorInButtonArea(Rectangle buttonRectangle,Rectangle cursor)
        {
            Vector2 recangleMiddlePos = GetTextureCenter(buttonRectangle);
            Vector2 cursorMiddlePos = GetTextureCenter(cursor);

            int horizontalDistance = (int)Math.Sqrt(Math.Pow((double)(recangleMiddlePos.X - cursorMiddlePos.X), 2));
            int verticallDistance = (int)Math.Sqrt(Math.Pow((double)(recangleMiddlePos.Y - cursorMiddlePos.Y), 2));

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
            hopNetGame = hopnetGame;
            handTextureType = new int[2];
            handTextureType[(int)Hand.Left] = (int)Texture.Normal;
            handTextureType[(int)Hand.Right] = (int)Texture.Normal;

            cursorState = new bool[2];

            kinectHandPosition = new Vector2[2];
            kinectHandPosition[(int)Hand.Left] = Vector2.Zero;
            kinectHandPosition[(int)Hand.Right] = Vector2.Zero;

            handSprite = new Sprite[2,textureNumber];

            screenWidth = (float)graphics.PreferredBackBufferWidth;
            screenHeight = (float)graphics.PreferredBackBufferHeight;

            timerStepSize = (int)(screenWidth) / buttonTimeDelay;
            hScale = (screenWidth / defaultScreenWidth);
            vScale = (screenHeight / defaultScreenHeight);

            backgroundSprite=new Sprite();
            timeoutProgressBar = new Sprite();
            timeoutProgressBar.rectangle = new Rectangle(0, 0, 0, 30);
            newGameSprite = new Sprite[textureNumber];
            scoresSprite = new Sprite[textureNumber];
            goBackSprite = new Sprite[textureNumber];
            exitSprite = new Sprite[textureNumber];
            easyDifficulty = new Sprite[textureNumber];
            mediumDifficulty = new Sprite[textureNumber];
            hardDifficulty = new Sprite[textureNumber];
            confirmExit = new Sprite[textureNumber];

            #region initialization of every sprite's properties
            for (int i = 0; i < textureNumber; i++)
            {
                confirmExit[i] = new Sprite();
                confirmExit[i].Rectangle = new Rectangle(
                    (int)(screenWidth / 2 - defaultBtnWidth * hScale / 2),
                    (int)(screenHeight / 2 - defaultBtnHeight * vScale / 2),
                    (int)(defaultBtnWidth * hScale), (int)(defaultBtnHeight * vScale));


                goBackSprite[i] = new Sprite();
                goBackSprite[i].Rectangle = new Rectangle((int)(hSpaceFromLeft * hScale),
                (int)((3 * vSpaceBetweenButtons + 2 * defaultBtnHeight) * (vScale)),
                (int)(defaultBtnWidth/2 * hScale),
                (int)(defaultBtnHeight/2 * vScale));

                easyDifficulty[i] = new Sprite();
                easyDifficulty[i].Rectangle = new Rectangle(
                    (int)(screenWidth / 2 - defaultBtnWidth * hScale / 2),
                    (int)(vSpaceBetweenButtons * vScale), (int)(defaultBtnWidth * hScale),
                    (int)(defaultBtnHeight * vScale));

                mediumDifficulty[i] = new Sprite();
                mediumDifficulty[i].Rectangle = new Rectangle(
                    (int)(screenWidth / 2 - defaultBtnWidth * hScale / 2),
                    (int)((2 * vSpaceBetweenButtons + defaultBtnHeight) * (vScale)),
                    (int)(defaultBtnWidth * hScale), (int)(defaultBtnHeight * vScale));

                hardDifficulty[i] = new Sprite();
                hardDifficulty[i].Rectangle = new Rectangle(
                    (int)(screenWidth / 2 - defaultBtnWidth * hScale / 2),
                    (int)((3 * vSpaceBetweenButtons + 2 * defaultBtnHeight) * (vScale)),
                    (int)(defaultBtnWidth * hScale), (int)(defaultBtnHeight * vScale));

                handSprite[(int)Hand.Left, i] = new Sprite();
                handSprite[(int)Hand.Right, i] = new Sprite();

                handSprite[(int)Hand.Left, i].Position = new Vector2(screenWidth / 2, screenHeight / 2);
                handSprite[(int)Hand.Right, i].Position = new Vector2(screenWidth / 2, screenHeight / 2);

                handSprite[(int)Hand.Left, i].Rectangle = new Rectangle((int)(screenWidth / 2 - (cursorRadius / 2) * hScale),
                    (int)(screenHeight / 2 - (cursorRadius / 2) * vScale), (int)(cursorRadius * hScale), (int)(cursorRadius * vScale));

                handSprite[(int)Hand.Right, i].Rectangle = new Rectangle((int)(screenWidth / 2 - (cursorRadius / 2) * hScale),
                    (int)(screenHeight / 2 - (cursorRadius / 2) * vScale), (int)(cursorRadius * hScale), (int)(cursorRadius * vScale));

                newGameSprite[i] = new Sprite();
                newGameSprite[i].Rectangle = new Rectangle(
                    (int)(hSpaceFromLeft * hScale), 
                    (int)(vSpaceBetweenButtons*vScale), (int)(defaultBtnWidth*hScale), 
                    (int)(defaultBtnHeight*vScale));

                scoresSprite[i] = new Sprite();
                scoresSprite[i].Rectangle = new Rectangle(
                    (int)(hSpaceFromLeft * hScale),
                    (int)((2 * vSpaceBetweenButtons + defaultBtnHeight) * (vScale)), 
                    (int)(defaultBtnWidth * hScale), (int)(defaultBtnHeight * vScale));

                exitSprite[i] = new Sprite();
                exitSprite[i].Rectangle = new Rectangle(
                    (int)(hSpaceFromLeft * hScale), 
                    (int)((3 * vSpaceBetweenButtons + 2 * defaultBtnHeight) * (vScale)), 
                    (int)(defaultBtnWidth * hScale), (int)(defaultBtnHeight * vScale));
            }
            backgroundSprite.Rectangle = new Rectangle(0, 0, (int)screenWidth, (int)screenHeight);
            #endregion
        }


        private bool AreHandsTogether()
        {
            Vector2 leftHandleMiddle = GetTextureCenter(handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].Rectangle);
            Vector2 rightHandleMiddle = GetTextureCenter(handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].Rectangle);

            double distance = Math.Sqrt((leftHandleMiddle.X - rightHandleMiddle.X) * (leftHandleMiddle.X - rightHandleMiddle.X) +
                (leftHandleMiddle.Y - rightHandleMiddle.Y) * (leftHandleMiddle.Y - rightHandleMiddle.Y));

            if (distance < 2 * cursorRadius) { return true; }

            return false;
        }

        public void Update(Skeleton kinectData, Vector2 mousePos)
        {
            if (isGameInMenu)
            {
                if (kinectData != null)
                {
                    kinectHandPosition[(int)Hand.Left].X = ((0.5f * kinectData.Joints[JointType.HandLeft].Position.X) + 0.5f) * screenWidth;
                    kinectHandPosition[(int)Hand.Left].Y = ((-0.5f * kinectData.Joints[JointType.HandLeft].Position.Y) + 0.5f) * screenHeight;
                    kinectHandPosition[(int)Hand.Right].X = ((0.5f * kinectData.Joints[JointType.HandRight].Position.X) + 0.5f) * screenWidth;
                    kinectHandPosition[(int)Hand.Right].Y = ((-0.5f * kinectData.Joints[JointType.HandRight].Position.Y) + 0.5f) * screenHeight;
                }
                else
                {
                    kinectHandPosition[(int)Hand.Left].X = mousePos.X-40;
                    kinectHandPosition[(int)Hand.Left].Y = mousePos.Y;
                    kinectHandPosition[(int)Hand.Right].X = mousePos.X + 40;
                    kinectHandPosition[(int)Hand.Right].Y = mousePos.Y;
                }

                var selectedButton = CheckButtonSelect();

                if (selectedButton != ButtonSelect.None & selectedButton==lastButton)
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
                    case ButtonSelect.Scores:
                        if (timeCounter > buttonTimeDelay) { state = State.InScores; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0; }
                        break;

                    case ButtonSelect.Exit:
                        if (timeCounter > buttonTimeDelay) { state = State.OnExit; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0; }
                        break;

                    case ButtonSelect.GoBack:
                        if (timeCounter > buttonTimeDelay) { state = State.InMainMenu; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0; }
                        break;

                    case ButtonSelect.NewGame:
                        if (timeCounter > buttonTimeDelay) { state = State.OnDifficultySelect; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0; }
                        break;

                    case ButtonSelect.EasyDifficulty:
                        selectedDifficulty = (int)GameDifficulty.Easy;
                        if (timeCounter > buttonTimeDelay) { state = State.Playing; isGameInMenu = false; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0; }
                        break;

                    case ButtonSelect.MediumDifficulty:
                        selectedDifficulty = (int)GameDifficulty.Medium;
                        if (timeCounter > buttonTimeDelay) { state = State.Playing; isGameInMenu = false; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0; }
                        break;

                    case ButtonSelect.HardDifficulty:
                        selectedDifficulty = (int)GameDifficulty.Hard;
                        if (timeCounter > buttonTimeDelay) { state = State.Playing; isGameInMenu = false; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0; }
                        break;

                    case ButtonSelect.ConfirmExit:
                        if (timeCounter > buttonTimeDelay) { state = State.ExitConfirmed; timeCounter = 0; timeoutProgressBar.rectangle.Width = 0;}
                        break;
                }

                lastButton = selectedButton;
                handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].rectangle.X = (int)kinectHandPosition[(int)Hand.Left].X;
                handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].rectangle.Y = (int)kinectHandPosition[(int)Hand.Left].Y;

                handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].rectangle.X = (int)kinectHandPosition[(int)Hand.Right].X;
                handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].rectangle.Y = (int)kinectHandPosition[(int)Hand.Right].Y;
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
                case State.InMainMenu:
                    {
                        newGameSprite[newGameTextureType].DrawByRectangle(spriteBatch);
                        scoresSprite[scoresTextureType].DrawByRectangle(spriteBatch);
                        exitSprite[exitTextureType].DrawByRectangle(spriteBatch);
                        break;
                    }
                case State.InScores:
                    {
                        goBackSprite[goBackTextureType].DrawByRectangle(spriteBatch);
                        break;
                    }
                case State.OnDifficultySelect:
                    {
                        easyDifficulty[easyDifficultyTextureType].DrawByRectangle(spriteBatch);
                        mediumDifficulty[mediumDifficultyTextureType].DrawByRectangle(spriteBatch);
                        hardDifficulty[hardDifficultyTextureType].DrawByRectangle(spriteBatch);
                        goBackSprite[goBackTextureType].DrawByRectangle(spriteBatch);
                        break;
                    }
                case State.OnExit:
                    {
                        confirmExit[confirmExitTextureType].DrawByRectangle(spriteBatch);
                        goBackSprite[goBackTextureType].DrawByRectangle(spriteBatch);
                        break;
                    }
            }

            timeoutProgressBar.DrawByRectangle(spriteBatch);
            handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].DrawByRectangle(spriteBatch);
            handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].DrawByRectangle(spriteBatch);
        }
    }
}


