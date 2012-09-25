using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    class DrawHighScore : DrawScore
    {
        private int currentPlace;

        private HighScores highScores;
        private Score currentScore;

        public DrawHighScore(HopnetGame HopnetGame, HighScores HighScores, SpriteBatch SpriteBatch)
        {
            hopnetGame = HopnetGame;
            highScores = HighScores;
            spriteBatch = SpriteBatch;
            currentPlace = 1;
            cursorPosition = new Vector2(GameConstants.DrawHighScoreLeftMargin, GameConstants.DrawHighScoreTopMargin);
        }

        public void DrawHighScores()
        {
            foreach (var score in highScores)
            {
                currentScore = score;
                DrawPlace();
                DrawTime();
                DrawEmptySpaces(5);
                base.DrawPlayerScore(currentScore.Points);
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
        private void DrawEmptySpaces(int NumberOfSpaces)
        {
            for (int i = 0; i < NumberOfSpaces; i++)
            {
                MoveCursorRight();
            }
        }





    }
}
