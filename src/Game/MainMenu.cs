using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Game
{
    class MainMenu
    {
        private readonly bool isGameInMenu=true;
        private float screenWidth;
        private float screenHeight;
        private const int defaultScreenWidth = 1280;
        private const int defaultScreenHeight = 720;
        private float horizontalScale=0;
        private float verticalScale=0;
        private int horizontalSpaceFromLeft=40;
        private int verticalSpaceBetweenButtons = 40;
        private int blinkingTextureNumber = 2;

        public const int defaultButtonWidth=300;
        public const int defaultButtonHeight=150;

        private enum CurrentTexture : int { Normal, WithBorder };

        public Sprite []newGameSprite;
        private int newGameTextureType=(int)CurrentTexture.WithBorder;

        public Sprite []scoresSprite;
        private int scoresTextureType = (int)CurrentTexture.WithBorder;

        public Sprite []exitSprite;
        private int exitTextureType = (int)CurrentTexture.WithBorder;

        public Sprite backgroundSprite;

        public bool IsGameInMenuMode
        {
            get { return isGameInMenu; }
        }


        public MainMenu(GraphicsDeviceManager graphics)
        {
            screenWidth = (float)graphics.PreferredBackBufferWidth;
            screenHeight = (float)graphics.PreferredBackBufferHeight;

            horizontalScale = (screenWidth / defaultScreenWidth);
            verticalScale = (screenHeight / defaultScreenHeight);

            backgroundSprite=new Sprite();
            newGameSprite = new Sprite[blinkingTextureNumber];
            scoresSprite = new Sprite[blinkingTextureNumber];
            exitSprite = new Sprite[blinkingTextureNumber];

            for (int i = 0; i < blinkingTextureNumber; i++)
            {
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
            backgroundSprite.Rectangle = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }


        public void Update(KinectPlayer player)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundSprite.Texture, backgroundSprite.Rectangle, Color.White);
            spriteBatch.Draw(newGameSprite[newGameTextureType].Texture,newGameSprite[newGameTextureType].Rectangle, Color.White);
            spriteBatch.Draw(scoresSprite[scoresTextureType].Texture, scoresSprite[scoresTextureType].Rectangle, Color.White);
            spriteBatch.Draw(exitSprite[exitTextureType].Texture, exitSprite[exitTextureType].Rectangle, Color.White);
            spriteBatch.End();
        }
    }
}
