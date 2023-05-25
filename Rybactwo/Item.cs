using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rybactwo
{
    internal class Item
    {
        private Texture2D[] textures;
        private Texture2D texture;
        private Vector2 position;
        private readonly Vector2 start;
        private Vector2 move;
        private bool isVisable;
        private int ticksToEnd;
        private int rectMove;
        private int rectMoveSpeed;

        public Item(Vector2 playerPosition)
        {
            start = playerPosition;
            position = start;
            move = new Vector2(0);
            rectMove = 0;
            rectMoveSpeed = 1;
            ticksToEnd = 0;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (isVisable)
            {
                spritebatch.Draw(
                            this.texture,
                            new Vector2((int)position.X, (int)position.Y),
                            new Rectangle(0 + rectMove*16, 0, 16, 16),
                            Color.White);
            }
        }

        public void Use(int id, double power, byte direction)
        {
            isVisable = true;
            texture = textures[id];
            switch (id)
            {
                case 1:
                    ticksToEnd = (int)(power * 60 * 2);
                    rectMoveSpeed = ticksToEnd / 4;
                    if (rectMoveSpeed <= 0) { rectMoveSpeed = 1; }
                    switch (direction)
                    {
                        case 0:
                            move = new Vector2(0, (float)(-4 * 16 * power / 60));
                            break;
                        case 1:
                            move = new Vector2(0, (float)(4 * 16 * power / 60));
                            break;
                        case 2:
                            move = new Vector2((float)(-4 * 16 * power / 60), 0);
                            break;
                        case 3:
                            move = new Vector2((float)(4 * 16 * power / 60), 0);
                            break;
                        default: break;
                    }

                    break;
                case 0:
                    break;
                default: break;
            }
        }

        public bool Tick()
        {
            position += move;
            if (ticksToEnd <= 0)
            {
                isVisable = false;
                ticksToEnd = 0;
                rectMove = 0;
                move = new Vector2(0);
                position = start;
                return false;
            }
            else
            {
                ticksToEnd--;
                if (ticksToEnd % rectMoveSpeed == 0) { rectMove += 1; }
                return true;
            }
        }
        public void Load(Texture2D[] misc)
        {
            isVisable = false;
            textures = misc;

        }
    }
}
