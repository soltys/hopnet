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
        private Texture2D gameOver;
        private int achievedScore;

        public DrawGameOver(HopnetGame hopNetGame, SpriteBatch sBatch, int score)
        {
            hopnetGame = hopNetGame;
            spriteBatch = sBatch;
            cursorPosition = GameConstants.DrawGameOverMessagePosition;
            gameOver = hopNetGame.Content.Load<Texture2D>("Sprites/GameOver");
            achievedScore = score;
        }

        public void DrawGameOverScene()
        {
            DrawGameOverMessage();
            DrawAchievedScore();

          
        }

        private void DrawGameOverMessage()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(gameOver, new Vector2(cursorPosition.X-gameOver.Width/2,cursorPosition.Y-gameOver.Height/2), null, Color.White, 0f, new Vector2(10, 10), 1f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
        private void DrawAchievedScore()
        {
            DrawPlayerScore(achievedScore);
        }
    }
}
