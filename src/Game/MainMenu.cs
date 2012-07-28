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

        private enum Texture : int { Normal, WithBorder };
        private enum Hand : int { Left = 0, Right = 1 };
        private enum State : int { InMainMenu=0, InNewGame=1, InScores=2, OnExit=3, Playing=4}
        private enum ButtonSelect : int { MainMenu = 0, Scores = 1, Exit = 3, ScoresGoBack=4, None=5 }


        public Sprite []newGameSprite;
        private int newGameTextureType=(int)Texture.WithBorder;

        public Sprite []scoresSprite;
        public Sprite scoresBackSprite;
        private int scoresTextureType = (int)Texture.WithBorder;

        public Sprite []exitSprite;
        private int exitTextureType = (int)Texture.WithBorder;

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

        /*
        private CurrentButtonSelect CheckButtonSelect()
        {

            bool areHandTogether = AreHandsTogether();
            switch(state)
            {
                case CurrentState.InMainMenu:



                    break;


                case CurrentState.InScores:



                    break;
            }
        }
        */


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
            scoresBackSprite = new Sprite();
            exitSprite = new Sprite[blinkingTextureNumber];

            #region initialization of every sprite's properties
            for (int i = 0; i < blinkingTextureNumber; i++)
            {

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
            /*scoresBackSprite.Rectangle = new Rectangle((int)(horizontalSpaceFromLeft*horizontalScale),
                verticalSpaceBetweenButtons,
                (int)(defaultButtonWidth*horizontalScale),
                (int)(defaultButtonHeight*verticalScale));*/

            scoresBackSprite.Rectangle = new Rectangle(50, 50, 300, 300);
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

        //public void Update(KinectData kinectData)
        public void Update(object sender, Skeleton kinectData)
        {
            if (isGameInMenu)
            {
                bool leftCursorState = false;
                bool rightCursorState = false;

                bool isLeftCursorInsideButton;
                bool isRightCursorInsideButton;
                bool areHandsTogether = false;

                if (kinectData != null)
                {
                    /*
                    leftHandPosition.X = ((0.5f*kinectData.skeleton.Joints[JointType.HandLeft].Position.X)+0.5f)*screenWidth;
                    leftHandPosition.Y = ((-0.5f*kinectData.skeleton.Joints[JointType.HandLeft].Position.Y)+0.5f)*screenHeight;
                    rightHandPosition.X = ((0.5f*kinectData.skeleton.Joints[JointType.HandRight].Position.X)+0.5f)*screenWidth;
                    rightHandPosition.Y = ((-0.5f*kinectData.skeleton.Joints[JointType.HandRight].Position.Y)+0.5f)*screenHeight;
                    */

                    kinectHandPosition[(int)Hand.Left].X = ((0.5f * kinectData.Joints[JointType.HandLeft].Position.X) + 0.5f) * screenWidth;
                    kinectHandPosition[(int)Hand.Left].Y = ((-0.5f * kinectData.Joints[JointType.HandLeft].Position.Y) + 0.3f) * screenHeight;
                    kinectHandPosition[(int)Hand.Right].X = ((0.5f * kinectData.Joints[JointType.HandRight].Position.X) + 0.5f) * screenWidth;
                    kinectHandPosition[(int)Hand.Right].Y = ((-0.5f * kinectData.Joints[JointType.HandRight].Position.Y) + 0.3f) * screenHeight;
                }
                else
                {
                    kinectHandPosition[(int)Hand.Left].X = 0.2f * screenWidth;
                    kinectHandPosition[(int)Hand.Left].Y = 0.5f * screenHeight;
                    kinectHandPosition[(int)Hand.Right].X = 0.8f * screenWidth;
                    kinectHandPosition[(int)Hand.Right].Y = 0.5f * screenHeight;
                }

                areHandsTogether = AreHandsTogether();
                
                switch(state)
                {
                    case (int)State.InMainMenu:
                        {
                            isLeftCursorInsideButton = IsCursorInButtonArea(newGameSprite[newGameTextureType].Rectangle, handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].Rectangle);
                            isRightCursorInsideButton = IsCursorInButtonArea(newGameSprite[newGameTextureType].Rectangle, handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].Rectangle);
                            #region cursor over new game button

                            if (isLeftCursorInsideButton) { leftCursorState = isLeftCursorInsideButton; }
                            if (isRightCursorInsideButton) { rightCursorState = isRightCursorInsideButton; }

                            if (isLeftCursorInsideButton & isRightCursorInsideButton)
                            {
                                newGameTextureType = (int)Texture.WithBorder;
                            }
                            else
                            {
                                newGameTextureType = (int)Texture.Normal;
                            }
                            if (leftCursorState & rightCursorState & areHandsTogether)
                            {
                                    isGameInMenu = false; 
                                    state = State.Playing;
                            }
                            #endregion

                            isLeftCursorInsideButton = IsCursorInButtonArea(scoresSprite[scoresTextureType].Rectangle, handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].Rectangle);
                            isRightCursorInsideButton = IsCursorInButtonArea(scoresSprite[scoresTextureType].Rectangle, handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].Rectangle);
                            #region cursor over scores button
                            if (isLeftCursorInsideButton) { leftCursorState = isLeftCursorInsideButton; }
                            if (isRightCursorInsideButton) { rightCursorState = isRightCursorInsideButton; }

                            if (isLeftCursorInsideButton & isRightCursorInsideButton)
                            {
                                scoresTextureType = (int)Texture.WithBorder;
                            }
                            else
                            {
                                scoresTextureType = (int)Texture.Normal;
                            }
                            if (leftCursorState & rightCursorState & areHandsTogether)
                            {
                                    state = State.InScores;
                            }
                            #endregion
                            /*
                            isLeftCursorInsideButton = IsCursorInButtonArea(exitSprite[exitTextureType].Rectangle, handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].Rectangle);
                            isRightCursorInsideButton = IsCursorInButtonArea(exitSprite[exitTextureType].Rectangle, handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].Rectangle);
                            #region cursor over exit button
                            if (isLeftCursorInsideButton) { leftCursorState = isLeftCursorInsideButton; }
                            if (isRightCursorInsideButton) { rightCursorState = isRightCursorInsideButton; }
                            if (isLeftCursorInsideButton & isRightCursorInsideButton)
                            {
                                exitTextureType = (int)CurrentTexture.WithBorder;
                            }
                            else
                            {
                                exitTextureType = (int)CurrentTexture.Normal;
                            }
                            if (leftCursorState & rightCursorState & areHandsTogether) { state = CurrentState.OnExit; }
                            #endregion
                            */
                        break;
                        }
                    case State.InScores:
                        isLeftCursorInsideButton = IsCursorInButtonArea(scoresBackSprite.Rectangle, handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].Rectangle);
                        isRightCursorInsideButton = IsCursorInButtonArea(scoresBackSprite.Rectangle, handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].Rectangle);
                        if (isLeftCursorInsideButton) { leftCursorState = isLeftCursorInsideButton; }
                        if (isRightCursorInsideButton) { rightCursorState = isRightCursorInsideButton; }
                        if (leftCursorState & rightCursorState & areHandsTogether)
                        {
                                state = (int)State.InMainMenu;
                        }
                        break;
                    case State.OnExit:
                        break;
                }

                #region cursor texture changer
                switch (leftCursorState)
                {
                    case true:
                        handTextureType[(int)Hand.Left] = (int)Texture.WithBorder;
                        break;
                    case false:
                        handTextureType[(int)Hand.Left] = (int)Texture.Normal;
                        break;
                }

                switch (rightCursorState)
                {
                    case true:
                        handTextureType[(int)Hand.Right] = (int)Texture.WithBorder;
                        break;
                    case false:
                        handTextureType[(int)Hand.Right] = (int)Texture.Normal;
                        break;
                }
                #endregion
                
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
                        scoresBackSprite.DrawByRectangle(spriteBatch);
                        break;
                    }
            }
            handSprite[(int)Hand.Left, handTextureType[(int)Hand.Left]].DrawByRectangle(spriteBatch);
            handSprite[(int)Hand.Right, handTextureType[(int)Hand.Right]].DrawByRectangle(spriteBatch);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "state : " + state.ToString(), new Vector2(600,200), Color.Red);
            spriteBatch.DrawString(font, State.InScores.ToString(), new Vector2(600, 250), Color.Red);
            spriteBatch.DrawString(font, onFocusDelayCounter.ToString(), new Vector2(600, 300), Color.Red);
            spriteBatch.End();

        }
    }
}


