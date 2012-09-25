﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    class DrawScore
    {
        protected HopnetGame hopnetGame;
        protected Vector2 cursorPosition { get; set; }
        protected Texture2D digitTexture;
        protected StringBuilder lokalizacjaTekstur;
        protected SpriteBatch spriteBatch;

        public void SetCursorPosition(Vector2 NewCursorPosition)
        {
            cursorPosition = NewCursorPosition;
        }
        public void DrawPlayerScore(int Score)
        {
            foreach (var digit in Score.ToString())
            {
                MoveCursorRight();
                int digitToWrite;
                int.TryParse(digit.ToString(), out digitToWrite);
                DrawOneChar(digitToWrite);
            }
        }

        protected void MoveCursorRight()
        {
            cursorPosition = new Vector2(cursorPosition.X + GameConstants.DrawHighScoreCharWidth, cursorPosition.Y);
        }
        protected void NewLine()
        {
            cursorPosition = new Vector2(GameConstants.DrawHighScoreLeftMargin, cursorPosition.Y + GameConstants.DrawHighScoreNewlineHeight);
        }
        protected void PrepareTexture(int textureNumber)
        {
            lokalizacjaTekstur = new StringBuilder("Sprites/Numbers/");
            if (textureNumber < 10)  // cyfry sa reprezentowane przez [0-9]
            {
                lokalizacjaTekstur.Append(textureNumber);
            }
            else  // kropka to 10, a dwukropek to 11
            {
                lokalizacjaTekstur.Append((textureNumber == 10 ? "kropka" : "dwukropek"));
            }
            digitTexture = this.hopnetGame.Content.Load<Texture2D>(lokalizacjaTekstur.ToString());
        }
        protected void DrawOneChar(int whatToDraw)
        {
            PrepareTexture(whatToDraw);
            spriteBatch.Begin();
            spriteBatch.Draw(digitTexture, cursorPosition, null, Color.White, 0f, new Vector2(10, 10), 0.5f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
