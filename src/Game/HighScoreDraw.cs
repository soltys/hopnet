using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    class HighScoreDraw
    {
        private Vector2 CursorPosition { get; set; }
        private Texture2D digitTexture;
        private StringBuilder lokalizacja;
        private int currentPlace;
        private SpriteBatch spriteBatch;
        private HopnetGame hopnetGame;
        private HighScores highScores;
        private Score currentScore;

        public HighScoreDraw(HopnetGame hopnetGame, HighScores highScores, SpriteBatch spriteBatch)
        {
            this.hopnetGame = hopnetGame;
            this.highScores = highScores;
            this.spriteBatch = spriteBatch;
            currentPlace = 1;
            CursorPosition = new Vector2(GameConstants.HighScoreDrawingLeftMargin, GameConstants.HighScoreDrawingTopMargin);
        }

        public HighScoreDraw(HopnetGame hopnetGame, SpriteBatch spriteBatch, Vector2 WhereToBegin)
        {
            this.hopnetGame = hopnetGame;
            this.spriteBatch = spriteBatch;
            CursorPosition = WhereToBegin;
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


        private void MoveCursorRight()
        {
            CursorPosition = new Vector2(CursorPosition.X + GameConstants.HighScoreDrawingCharWidth, CursorPosition.Y);
        }
        private void NewLine()
        {
            CursorPosition = new Vector2(GameConstants.HighScoreDrawingLeftMargin, CursorPosition.Y+GameConstants.HighScoreDrawingNewlineHeight);
        }

        public void DrawHighScores()
        {
            foreach (var score in highScores)
            {
                currentScore = score;
                DrawPlace();
                DrawTime();
                DrawScore();
                ++currentPlace;
                NewLine();
            }
        }

        private void DrawPlace()
        {
            DrawOneChar(currentPlace);
            MoveCursorRight();
            DrawOneChar(10);  // kropka po numerze miejsca
            MoveCursorRight();
        }
        private void DrawTime()
        {
            foreach ( var digit in currentScore.Time.ToString() )
            {
                MoveCursorRight();

                int digitToWrite;
                if (digit.ToString() == ":")
                {
                    DrawOneChar(11);  // dwukropek w godzinach
                }
                else if (digit.ToString() == "-")
                {
                    DrawOneChar(10); // kropka rozdziela rok, miesiac, dzien
                }
                else if (int.TryParse(digit.ToString(), out digitToWrite))
                {
                    DrawOneChar(digitToWrite);
                }

            }
        }
        private void DrawScore()
        {
            for (int i = 0; i < 5; i++)  // petla dodajaca kilka spacji miedzy czasem wyniku i samym wynikiem
            {
                MoveCursorRight();
            }

            foreach (var digit in currentScore.Points.ToString())
            {
                MoveCursorRight();
                int digitToWrite;
                int.TryParse(digit.ToString(), out digitToWrite);
                DrawOneChar(digitToWrite);
            }
        }

        private void DrawOneChar(int whatToDraw)
        {
            PrepareTexture(whatToDraw);
            spriteBatch.Begin();
            spriteBatch.Draw(digitTexture, CursorPosition, null, Color.White, 0f, new Vector2(10, 10), 0.5f, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        private void PrepareTexture(int textureNumber)
        {
            lokalizacja = new StringBuilder("Sprites/Numbers/");
            if (textureNumber < 10)  // cyfry sa reprezentowane przez [0-9]
            {
                lokalizacja.Append(textureNumber);
            }
            else  // kropka to 10, a dwukropek to 11
            {
                lokalizacja.Append((textureNumber == 10 ? "kropka" : "dwukropek"));
            }
            digitTexture = this.hopnetGame.Content.Load<Texture2D>(lokalizacja.ToString());
        }
    }
}
