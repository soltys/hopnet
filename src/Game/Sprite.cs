using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    public class Sprite 
    {
        public Rectangle rectangle;
        private Texture2D texture;
        private Vector2 position;
        #region accessors
        public Rectangle Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        #endregion

        public Sprite()
        {
            position = new Vector2();
            rectangle = new Rectangle();
        }

        public void LoadSprite(ContentManager contentManager, string assetName)
        {
            texture = contentManager.Load<Texture2D>(assetName);
        }

        public void DrawByVector(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, position, null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        public void DrawByRectangle(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, rectangle, Color.White);
            spriteBatch.End();
        }



    }
}
