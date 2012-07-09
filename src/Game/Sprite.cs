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
        private Rectangle rectangle;
        private Texture2D texture;

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
        #endregion

        public Sprite()
        {
            rectangle = new Rectangle();
        }

        public void LoadSprite(ContentManager contentManager, string assetName)
        {
            texture = contentManager.Load<Texture2D>(assetName);
        }

    }
}
