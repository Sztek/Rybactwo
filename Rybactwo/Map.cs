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
        public int[,,] tileMap;
        public bool[,] collisionMap;
        private static Point size = new(16, 10);
        private static Point center = new(10, 6);
        private static byte warstwy = 3;
        private const byte block = 16;
        private Texture2D texture;
        private SpriteBatch spritebatch;
        public Vector2 shiftBlock = new(0);
        public Point shiftMap = new(123, 360);
        private bool[] wall = new bool[8];
        public bool[] direction;
        private int sizeMapX;
        private int sizeMapY;
        private int tileMapWidth;
        private double moveTime;
        //private readonly SoundPlayer soundPlayer;

        public Map()
        {
            sizeMapX = 400;
            sizeMapY = 600;
            tileMapWidth = 103;
            tileMap = new int[warstwy, sizeMapX, sizeMapY];
            collisionMap = new bool[sizeMapX, sizeMapY];
            direction = new bool[4];
            moveTime = 0;

            //Im schizo

            //string filePath = "1.wav";

            //string filePath = Path.Combine("..\\Content", "1.wav");
            //SoundPlayer soundPlayer = new(filePath);
            
            for (int i = 0; i < sizeMapX; i++)
            {
                for (int j = 0; j < sizeMapY; j++)
                {
                    for (int k = 0; k < warstwy; k++)
                    {
                        tileMap[k, i, j] = -1;
                        collisionMap[i, j] = false;
                    }
                }
            }
            SelectMap(0);
        }

        public void Load(Texture2D texture, SpriteBatch spritebatch)
        {
            this.texture = texture;
            this.spritebatch = spritebatch;
        }

        public void SelectMap(int id)
        {
            switch (id)
            {
                case 0:
                    LoadMapFromFile("prolog_teren.csv", 0);
                    LoadMapFromFile("prolog_obiekty.csv", 1);
                    LoadMapFromFile("prolog_bibeloty.csv", 2);
                    break;
                default: break;
            }
        }

        public void Tick(double dTime)
        {
            byte directionCount = 0;
            foreach (bool x in direction) { if (x) { directionCount++; } }
            moveTime += dTime;
            double tickTime = 60;
            if (directionCount == 1) { tickTime = 1 / 64; }
            else if (directionCount == 2) { tickTime = (1.2 / 64); }
            if (moveTime >= tickTime)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (direction[i])
                    {
                        switch (i)
                        {
                            case 0:
                                Move(new Vector2(0, -1));
                                break;
                            case 1:
                                Move(new Vector2(0, 1));
                                break;
                            case 2:
                                Move(new Vector2(-1, 0));
                                break;
                            case 3:
                                Move(new Vector2(1, 0));
                                break;
                        }
                    }
                }
                moveTime = 0;
            }  
        }

        public void Draw()
        {
            Vector2 place;
            for (int i = 0; i < 8; i++) wall[i] = false;
            for (int i = 0; i < size.X; i++)
            {
                for (int j = 0; j < size.Y; j++)
                {
                    for (int k = 0; k < warstwy; k++)
                    {
                        place = new Vector2(i * 16 + 8 + (int)shiftBlock.X, j * 16 + 8 + (int)shiftBlock.Y);
                        spritebatch.Draw(
                            texture,
                            place,
                            GetTile(tileMap[k, i + shiftMap.X, j + shiftMap.Y]),
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
        }

        public Vector2 Move(Vector2 shift)
        {
            if (shift.X != 0 || shift.Y != 0)
            {
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

                if (shiftBlock.Y > 0)
                {
                    shiftBlock.Y -= block;
                    shiftMap.Y--;
                }

                if (shiftBlock.Y < -block)
                {
                    shiftBlock.Y += block;
                    shiftMap.Y++;
                }

                if (shiftBlock.X > 0)
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

        private void LoadMapFromFile(string file, int w)
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
                        tileMap[w, j, i] = int.Parse(values[j]);
                        switch (tileMap[w, j, i])
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
