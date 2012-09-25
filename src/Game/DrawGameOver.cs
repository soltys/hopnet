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
        private Texture2D GameOver;
        private int achievedScore;

        public DrawGameOver(HopnetGame HopnetGame, SpriteBatch SpriteBatch, int Score)
        {
            hopnetGame = HopnetGame;
            spriteBatch = SpriteBatch;
            cursorPosition = GameConstants.DrawGameOverMessagePosition;
            GameOver = hopnetGame.Content.Load<Texture2D>("Sprites/GameOver");
            achievedScore = Score;
        }

        public void DrawGameOverScene()
        {
            DrawGameOverMessage();
            DrawAchievedScore();

          
        }

        private void DrawGameOverMessage()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(GameOver, cursorPosition, null, Color.White, 0f, new Vector2(10, 10), 1f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
        private void DrawAchievedScore()
        {
            cursorPosition = GameConstants.DrawGameOverScorePosition;
            DrawPlayerScore(achievedScore);
        }
    }
}
