using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rybactwo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Rybactwo
{
    internal class Npc : Character
    {
        private Vector2 destination;
        private Random rnd;

        public Npc()
        {
            rnd = new Random();
            position = new Vector2(1 * 16, 1 * 16);
            shiftMap = new Vector2(0);
            destination = position;
        }

        public void MapMove(Vector2 shift)
        {
            shiftMap = shift;
        }

        public override void Tick(double dTime)
        {
            Idle();
            if (destination.X > (int)position.X)
            {
                position.X++;
            }
            else if (destination.X < (int)position.X)
            {
                position.X--;
            }
            if (destination.Y > (int)position.Y)
            {
                position.Y++;
            }
            else if (destination.Y < (int)position.Y)
            {
                position.Y--;
            }
        }

        private void Idle()
        {
            if (destination.X == (int)position.X && destination.Y == (int)destination.Y)
            {
                destination = new Vector2(rnd.Next(8) * 16, rnd.Next(8) * 16);
            }
        }
        public override void DrawAll()
        {
            DrawCharacter();
        }

        protected override void LoadMisc(Texture2D[] misc)
        {

        }
    }
}
