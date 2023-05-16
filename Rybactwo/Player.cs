using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rybactwo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rybactwo
{
    internal class Player : Character
    {
        private double maxCast;
        public double cast;
        private bool casting;
        public byte direction;
        public byte selectedItem;
        private Item item;
        private Texture2D castBar;
        public Player()
        {
            position = new Vector2(144-24, 80-8);
            item = new Item(position);
            maxCast = 1;
            selectedItem = 0;
            direction = 3;
        }

        public override void Tick(double dTime)
        {
            if (casting & !item.Tick())
            {
                cast += dTime;
                if (cast >= maxCast)
                {
                    casting = false;
                    cast = maxCast;
                }
            }


        }
        public void ClickAction(bool hold)
        {
            if (hold || cast > 0)
            {
                if (!casting && hold)
                {
                    cast = 0;
                }
                casting = hold;
                if (cast == maxCast)
                {
                    casting = false;

                }
            }
            if (!hold && cast > 0)
            {
                item.Use(0, cast, direction);
                cast = 0;
            }
        }
        public void Action()
        {
            switch (selectedItem)
            {
                case 0:
                    break;
                default: break;

            }
        }
        public void DrawCastBar()
        {
            spritebatch.Draw(
                            castBar,
                            new Vector2((int)position.X - (int)shiftMap.X, (int)position.Y - (int)shiftMap.Y - 4),
                            new Rectangle(0, 0, (int)(16 * cast), 4),
                            Color.White);

        }
        public override void DrawAll()
        {
            DrawCharacter();
            item.Draw(spritebatch);
            DrawCastBar();
        }

        protected override void LoadMisc(Texture2D[] misc)
        {
            castBar = misc[0];
            item.Load(misc);
        }
    }
}
