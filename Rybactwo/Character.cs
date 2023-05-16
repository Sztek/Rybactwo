using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Rybactwo
{
    internal abstract class Character
    {
        protected Texture2D texture;
        protected SpriteBatch spritebatch;
        protected Vector2 position;
        protected Vector2 shiftMap;
        public void DrawCharacter()
        {
            if (position.X - shiftMap.X > -17 && (int)position.Y - (int)shiftMap.Y > -17 &&
                position.X - shiftMap.X < 321 && (int)position.Y - (int)shiftMap.Y < 181)
            {
                spritebatch.Draw(
                            texture,
                            new Vector2((int)position.X - (int)shiftMap.X, (int)position.Y - (int)shiftMap.Y),
                            new Rectangle(0, 8, 16, 16),
                            Color.White);
            }
        }

        public void Load(SpriteBatch spritebatch, Texture2D txt, Texture2D[] misc = null)
        {
            texture = txt;
            this.spritebatch = spritebatch;
            if (misc != null) { LoadMisc(misc); }
        }

        protected abstract void LoadMisc(Texture2D[] misc);

        public abstract void DrawAll();

        public abstract void Tick(double dTime);
    }
}
