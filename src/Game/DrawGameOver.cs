using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Game
{
    class DrawGameOver : DrawScore
    {
        private int achievedScore;

        public DrawGameOver(HopnetGame hopNetGame, SpriteBatch sBatch, int score)
        {
            hopnetGame = hopNetGame;
            spriteBatch = sBatch;
            achievedScore = score;
            scale = 1;

            cursorPosition = new Vector2(GameConstants.DrawGameOverScorePosition.X - (int)(2*GameConstants.DrawHighScoreCharWidth*(scale/0.5)),GameConstants.DrawGameOverScorePosition.Y);
        }

        public void DrawGameOverScene()
        {
            DrawAchievedScore();
        }

     
        private void DrawAchievedScore()
        {
            DrawPlayerScore(achievedScore);
        }
    }
}
