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
        private bool isGameInMenu=true;
        private float screenWidth;
        private float screenHeight;
        private const int defaultScreenWidth = 1280;
        private const int defaultScreenHeight = 720;
        private float horizontalScale=0;
        private float verticalScale=0;
        private int horizontalSpaceFromLeft=100;
        private int verticalSpaceBetweenButtons = 20;
        private int blinkingTextureNumber = 2;

        public const int defaultButtonWidth=420;
        public const int defaultButtonHeight=210;

        private enum Texture : int { Normal=0, WithBorder=1 };
        private enum Hand : int { Left = 0, Right = 1 };
        private enum State : int { InMainMenu = 0, InNewGame = 1, InScores = 2, OnExit = 3, Playing = 4}
        private enum ButtonSelect : int { Scores = 0, Exit = 1, ScoresGoBack = 2, None = 3, NewGame = 4 }


        public Sprite []newGameSprite;
        private int newGameTextureType=(int)Texture.Normal;

        public Sprite []scoresSprite;
        public Sprite []scoresBackSprite;
        private int scoresTextureType = (int)Texture.Normal;
        private int scoresBackTextureType = (int)Texture.Normal;

        public Sprite []exitSprite;
        private int exitTextureType = (int)Texture.Normal;

        public Sprite backgroundSprite;
        
        public const int cursorRadius = 64;



        public Sprite[,] handSprite;


        private int[] handTextureType;



        public const int onFocusDelay = 100000;
        private int onFocusDelayCounter = 0;



        private State state = State.InMainMenu;

        private Vector2 []kinectHandPosition;


        //private float kinectSensitivityMultiplier = 0.8f;

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
        private void ChangeButtonTexture(bool []cursorState,  ref int texture)
        {
            if (cursorState.Length != 2) { throw new ArgumentOutOfRangeException(); }

            if (cursorState[(int)Hand.Left] & cursorState[(int)Hand.Right])
            {
                texture = (int)Texture.WithBorder;
            }
            else
            {
                texture = (int)Texture.Normal;
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

        private ButtonSelect CheckButtonSelect()
        {
            ButtonSelect buttonState = ButtonSelect.None;
            bool []isCursorInsideButton = new bool[2];
            bool []cursorState = new bool[2];

            switch(state)
            {
                case State.InMainMenu:
                            isCursorInsideButton[(int)Hand.Left] = IsCursorInButtonArea(newGameSprite[newGameTextureType].Rectangle, handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].Rectangle);
                            isCursorInsideButton[(int)Hand.Right] = IsCursorInButtonArea(newGameSprite[newGameTextureType].Rectangle, handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].Rectangle);
                            #region cursor over new game button

                            if (isCursorInsideButton[(int)Hand.Left]) { cursorState[(int)Hand.Left] = isCursorInsideButton[(int)Hand.Left]; }
                            if (isCursorInsideButton[(int)Hand.Right]) { cursorState[(int)Hand.Right] = isCursorInsideButton[(int)Hand.Right]; }
                            
                            ChangeButtonTexture(isCursorInsideButton, ref newGameTextureType);
                            if (IsButtonSelected(isCursorInsideButton))
                            {
                                    if (IsCanChangeState(isCursorInsideButton))
                                    {
                                        buttonState = ButtonSelect.NewGame;
                                    }
                            }
                            #endregion

                            isCursorInsideButton[(int)Hand.Left] = IsCursorInButtonArea(scoresSprite[scoresTextureType].Rectangle, handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].Rectangle);
                            isCursorInsideButton[(int)Hand.Right] = IsCursorInButtonArea(scoresSprite[scoresTextureType].Rectangle, handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].Rectangle);
                            #region cursor over scores button

                            if (isCursorInsideButton[(int)Hand.Left]) { cursorState[(int)Hand.Left] = isCursorInsideButton[(int)Hand.Left]; }
                            if (isCursorInsideButton[(int)Hand.Right]) { cursorState[(int)Hand.Right] = isCursorInsideButton[(int)Hand.Right]; }

                            ChangeButtonTexture(isCursorInsideButton, ref scoresTextureType);
                            if (IsButtonSelected(isCursorInsideButton))
                            {
                                    if (IsCanChangeState(isCursorInsideButton))
                                    {
                                        buttonState = ButtonSelect.Scores;
                                    }
                            }
                            #endregion
                            
                            isCursorInsideButton[(int)Hand.Left] = IsCursorInButtonArea(exitSprite[exitTextureType].Rectangle, handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].Rectangle);
                            isCursorInsideButton[(int)Hand.Right] = IsCursorInButtonArea(exitSprite[exitTextureType].Rectangle, handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].Rectangle);
                            #region cursor over exit button
                            if (isCursorInsideButton[(int)Hand.Left]) { cursorState[(int)Hand.Left] = isCursorInsideButton[(int)Hand.Left]; }
                            if (isCursorInsideButton[(int)Hand.Right]) { cursorState[(int)Hand.Right] = isCursorInsideButton[(int)Hand.Right]; }

                            ChangeButtonTexture(isCursorInsideButton, ref exitTextureType);
                            if (IsButtonSelected(isCursorInsideButton))
                            {
                                    if (IsCanChangeState(isCursorInsideButton))
                                    {
                                        buttonState = ButtonSelect.Exit;
                                    }
                            }
                            #endregion
                    break;

                case State.InScores:
                            isCursorInsideButton[(int)Hand.Left] = IsCursorInButtonArea(scoresBackSprite[scoresBackTextureType].Rectangle, handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].Rectangle);
                            isCursorInsideButton[(int)Hand.Right] = IsCursorInButtonArea(scoresBackSprite[scoresBackTextureType].Rectangle, handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].Rectangle);
                            #region cursor over scoresBack button
                            if (isCursorInsideButton[(int)Hand.Left]) { cursorState[(int)Hand.Left] = isCursorInsideButton[(int)Hand.Left]; }
                            if (isCursorInsideButton[(int)Hand.Right]) { cursorState[(int)Hand.Right] = isCursorInsideButton[(int)Hand.Right]; }

                            if (IsButtonSelected(isCursorInsideButton))
                            {
                                    ChangeButtonTexture(isCursorInsideButton, ref scoresBackTextureType);
                                    if (IsCanChangeState(isCursorInsideButton))
                                    {
                                        return ButtonSelect.ScoresGoBack;
                                    }
                            }
                            #endregion
                    break;

                case State.OnExit:
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



        public MainMenu(GraphicsDeviceManager graphics)
        {
            handTextureType = new int[2];
            handTextureType[(int)Hand.Left] = (int)Texture.Normal;
            handTextureType[(int)Hand.Right] = (int)Texture.Normal;

            kinectHandPosition = new Vector2[2];
            kinectHandPosition[(int)Hand.Left] = Vector2.Zero;
            kinectHandPosition[(int)Hand.Right] = Vector2.Zero;

            handSprite = new Sprite[2,blinkingTextureNumber];

            screenWidth = (float)graphics.PreferredBackBufferWidth;
            screenHeight = (float)graphics.PreferredBackBufferHeight;

            horizontalScale = (screenWidth / defaultScreenWidth);
            verticalScale = (screenHeight / defaultScreenHeight);


            backgroundSprite=new Sprite();
            newGameSprite = new Sprite[blinkingTextureNumber];
            scoresSprite = new Sprite[blinkingTextureNumber];
            scoresBackSprite = new Sprite[blinkingTextureNumber];
            exitSprite = new Sprite[blinkingTextureNumber];

            #region initialization of every sprite's properties
            for (int i = 0; i < blinkingTextureNumber; i++)
            {
                scoresBackSprite[i] = new Sprite();

                scoresBackSprite[i].Rectangle = new Rectangle((int)(horizontalSpaceFromLeft*horizontalScale),
    verticalSpaceBetweenButtons,
    (int)(defaultButtonWidth*horizontalScale),
    (int)(defaultButtonHeight*verticalScale));

                handSprite[(int)Hand.Left, i] = new Sprite();
                handSprite[(int)Hand.Right, i] = new Sprite();

                handSprite[(int)Hand.Left, i].Position = new Vector2(screenWidth / 2, screenHeight / 2);
                handSprite[(int)Hand.Right, i].Position = new Vector2(screenWidth / 2, screenHeight / 2);

                handSprite[(int)Hand.Left, i].Rectangle = new Rectangle((int)(screenWidth / 2 - (cursorRadius / 2) * horizontalScale),
                    (int)(screenHeight / 2 - (cursorRadius / 2) * verticalScale), (int)(cursorRadius * horizontalScale), (int)(cursorRadius * verticalScale));

                handSprite[(int)Hand.Right, i].Rectangle = new Rectangle((int)(screenWidth / 2 - (cursorRadius / 2) * horizontalScale),
                    (int)(screenHeight / 2 - (cursorRadius / 2) * verticalScale), (int)(cursorRadius * horizontalScale), (int)(cursorRadius * verticalScale));

                newGameSprite[i] = new Sprite();
                newGameSprite[i].Rectangle = new Rectangle(
                    (int)(horizontalSpaceFromLeft * horizontalScale), 
                    (int)(verticalSpaceBetweenButtons*verticalScale), (int)(defaultButtonWidth*horizontalScale), 
                    (int)(defaultButtonHeight*verticalScale));

                scoresSprite[i] = new Sprite();
                scoresSprite[i].Rectangle = new Rectangle(
                    (int)(horizontalSpaceFromLeft * horizontalScale),
                    (int)((2 * verticalSpaceBetweenButtons + defaultButtonHeight) * (verticalScale)), 
                    (int)(defaultButtonWidth * horizontalScale), (int)(defaultButtonHeight * verticalScale));

                exitSprite[i] = new Sprite();
                exitSprite[i].Rectangle = new Rectangle(
                    (int)(horizontalSpaceFromLeft * horizontalScale), 
                    (int)((3 * verticalSpaceBetweenButtons + 2 * defaultButtonHeight) * (verticalScale)), 
                    (int)(defaultButtonWidth * horizontalScale), (int)(defaultButtonHeight * verticalScale));
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

        public void Update(object sender, Skeleton kinectData)
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
                    kinectHandPosition[(int)Hand.Left].X = 0.8f * screenWidth;
                    kinectHandPosition[(int)Hand.Left].Y = 0.5f * screenHeight;
                    kinectHandPosition[(int)Hand.Right].X = 0.8f * screenWidth;
                    kinectHandPosition[(int)Hand.Right].Y = 0.5f * screenHeight;
                }

                switch (CheckButtonSelect())
                {
                    case ButtonSelect.Scores:
                        state = State.InScores;
                        break;

                    case ButtonSelect.Exit:
                        state = State.OnExit;
                        break;

                    case ButtonSelect.ScoresGoBack:
                        state = State.InMainMenu;
                        break;

                    case ButtonSelect.NewGame:
                        isGameInMenu = false;
                        state = State.Playing;
                        break;
                }

                handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].rectangle.X = (int)kinectHandPosition[(int)Hand.Left].X;
                handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].rectangle.Y = (int)kinectHandPosition[(int)Hand.Left].Y;

                handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].rectangle.X = (int)kinectHandPosition[(int)Hand.Right].X;
                handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].rectangle.Y = (int)kinectHandPosition[(int)Hand.Right].Y;
            }
        }

        public void Draw(SpriteBatch spriteBatch,SpriteFont font)
        {
            backgroundSprite.DrawByRectangle(spriteBatch);
            
            switch(state)
            {
                case (int)State.InMainMenu:
                    {
                        newGameSprite[newGameTextureType].DrawByRectangle(spriteBatch);
                        scoresSprite[scoresTextureType].DrawByRectangle(spriteBatch);
                        exitSprite[exitTextureType].DrawByRectangle(spriteBatch);
                        break;
                    }
                case State.InScores:
                    {
                        spriteBatch.Begin();
                        spriteBatch.DrawString(font, "DZIALA!", new Vector2(600, 250), Color.Red);
                        spriteBatch.End();
                        scoresBackSprite[scoresBackTextureType].DrawByRectangle(spriteBatch);
                        break;
                    }
            }
            handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].DrawByRectangle(spriteBatch);
            handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].DrawByRectangle(spriteBatch);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "state : " + state.ToString(), new Vector2(600,200), Color.Red);
            spriteBatch.DrawString(font, onFocusDelayCounter.ToString(), new Vector2(600, 300), Color.Red);
            spriteBatch.End();

        }
    }
}


