using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Media;
//using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using Microsoft.Xna.Framework.Media;

namespace Rybactwo
{
    internal class Map
    {
        public int[,] tileMap;
        public bool[,] collisionMap;
        static Point size = new(22, 13);
        static Point center = new(10, 6);
        const byte block = 16;
        Texture2D texture;
        SpriteBatch spritebatch;
        public Vector2 shiftBlock = new(0);
        public Point shiftMap = new(123, 360);
        bool[] wall = new bool[8];
        private int sizeMapX;
        private int sizeMapY;
        private int tileMapWidth;
        //private readonly SoundPlayer soundPlayer;

        public Map(int warstwa)
        {
            sizeMapX = 400;
            sizeMapY = 600;
            tileMapWidth = 103;
            tileMap = new int[sizeMapX, sizeMapY];
            collisionMap = new bool[sizeMapX, sizeMapY];

            //Im schizo

            //string filePath = "1.wav";

            //string filePath = Path.Combine("..\\Content", "1.wav");
            //SoundPlayer soundPlayer = new(filePath);

            for (int i = 0; i < sizeMapX; i++)
            {
                for (int j = 0; j < sizeMapY; j++)
                {
                    tileMap[i, j] = -1;
                    collisionMap[i, j] = false;
                }
            }
            switch (warstwa)
            {
                case 0:
                    LoadMapFromFile("prolog_teren.csv");
                    break;
                case 1:
                    LoadMapFromFile("prolog_obiekty.csv");
                    break;
                case 2:
                    LoadMapFromFile("prolog_bibeloty.csv");
                    break;
            }
        }

        public void Load(Texture2D texture, SpriteBatch spritebatch)
        {
            this.texture = texture;
            this.spritebatch = spritebatch;
        }

        public void Draw()
        {
            Vector2 place;
            for (int i = 0; i < 8; i++) wall[i] = false;
            for (int i = 0; i < size.X; i++)
            {
                for (int j = 0; j < size.Y; j++)
                {
                    place = new Vector2(i * 16 - 16 + (int)shiftBlock.X, j * 16 - 16 + (int)shiftBlock.Y);
                    spritebatch.Draw(
                        texture,
                        place,
                        GetTile(tileMap[i + shiftMap.X, j + shiftMap.Y]),
                        Color.White);

                    if (place.Y == 80 - block)
                    {
                        if (place.X < 144 && place.X > 144 - 16)
                        {
                            wall[0] = collisionMap[i + shiftMap.X, j + shiftMap.Y];
                        }
                        if (place.X >= 144 && place.X < 144 + 16)
                        {
                            wall[1] = collisionMap[i + shiftMap.X, j + shiftMap.Y];
                        }
                    } //collision up
                    if (place.Y == 80 + block)
                    {
                        if (place.X < 144 && place.X > 144 - 16)
                        {
                            wall[2] = collisionMap[i + shiftMap.X, j + shiftMap.Y];
                        }
                        if (place.X >= 144 && place.X < 144 + 16)
                        {
                            wall[3] = collisionMap[i + shiftMap.X, j + shiftMap.Y];
                        }
                    } //collision down
                    if (place.X == 144 - block)
                    {
                        if (place.Y < 80 && place.Y > 80 - 16)
                        {
                            wall[4] = collisionMap[i + shiftMap.X, j + shiftMap.Y];
                        }
                        if (place.Y >= 80 && place.Y < 80 + 16)
                        {
                            wall[5] = collisionMap[i + shiftMap.X, j + shiftMap.Y];
                        }
                    } //collision left
                    if (place.X == 144 + block)
                    {
                        if (place.Y < 80 && place.Y > 80 - 16)
                        {
                            wall[6] = collisionMap[i + shiftMap.X, j + shiftMap.Y];
                        }
                        if (place.Y >= 80 && place.Y < 80 + 16)
                        {
                            wall[7] = collisionMap[i + shiftMap.X, j + shiftMap.Y];
                        }
                    } //collision right
                }
            }
        }

        public Vector2 Move(Vector2 shift)
        {
            if (shift.X != 0 || shift.Y != 0)
            {
                bool[] collision = new bool[4];
                for (int i = 0; i < 4; i++) collision[i] = false;
                if (shift.X != 0 && shift.Y != 0)
                {
                    shift.X /= (float)1.41;
                    shift.Y /= (float)1.41;
                }
                if ((wall[0] == true || wall[1] == true) && shift.Y < 0)
                {
                    shift.Y = 0;
                    //                 soundPlayer.Play();
                }
                if ((wall[2] == true || wall[3] == true) && shift.Y > 0)
                {
                    shift.Y = 0;
                    //                 soundPlayer.Play();
                }
                if ((wall[4] == true || wall[5] == true) && shift.X < 0)
                {
                    shift.X = 0;
                    //                 soundPlayer.Play();
                }
                if ((wall[6] == true || wall[7] == true) && shift.X > 0)
                {
                    shift.X = 0;
                    //                soundPlayer.Play();
                }

                shiftBlock -= shift;

                if (shiftBlock.Y > block)
                {
                    shiftBlock.Y -= block;
                    shiftMap.Y--;
                }

                if (shiftBlock.Y < -block)
                {
                    shiftBlock.Y += block;
                    shiftMap.Y++;
                }

                if (shiftBlock.X > block)
                {
                    shiftBlock.X -= block;
                    shiftMap.X--;
                }

                if (shiftBlock.X < -block)
                {
                    shiftBlock.X += block;
                    shiftMap.X++;
                }

                if (shiftMap.X < 0)
                {
                    shiftMap.X = 0;
                }

                if (shiftMap.Y < 0)
                {
                    shiftMap.Y = 0;
                }
            }
            return new Vector2(shiftMap.X * 16 - shiftBlock.X, shiftMap.Y * 16 - shiftBlock.Y);
        }

        private void LoadMapFromFile(string file)
        {
            using (var reader = new StreamReader(file))
            {
                int i = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    for (int j = 0; j < values.Length; j++)
                    {
                        tileMap[j, i] = int.Parse(values[j]);
                        switch (tileMap[j, i])
                        {
                            case 1:
                                collisionMap[j, i] = false;
                                break;
                            case 5:
                                collisionMap[j, i] = false;
                                break;
                            case 6:
                                collisionMap[j, i] = false;
                                break;
                            default:
                                collisionMap[j, i] = false;
                                break;
                        }
                    }
                    i++;
                }
            }
        }

        private Rectangle GetTile(int id)
        {
            if (id >= 0)
                return new Rectangle((id - tileMapWidth * (id / tileMapWidth)) * block, id / tileMapWidth * block, block, block);
            return new Rectangle(4 * block, 7 * block, block, block);
            
        }
    }
}
