using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Media;
using System.Linq.Expressions;
using Microsoft.Xna.Framework.Audio;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Rybactwo
{
    public class Game1 : Game
    {
        static Point gameResolution = new(320, 180);
        bool showDebug;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D tileset;
        Texture2D character;
        Texture2D pointingCrosshair;
        Texture2D actionCrosshair;
        Texture2D hud;

        SpriteFont gameFont;

        RenderTarget2D renderTarget;
        Rectangle renderTargetDestination;

        Map[] mapa;
        Player player;
        Npc npc1;

        MouseState mState;                              // Get info from mouse
        Vector2 position;
        int userLeftClicks = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //TargetElapsedTime = TimeSpan.FromSeconds(1d / 30d); // 30fps
            //IsMouseVisible = true;
            showDebug = false;
        }

        protected override void LoadContent()
        {
            mapa = new Map[3];
            for (int i = 0; i < 3; i++)
            {
                mapa[i] = new Map(i);
            }
            player = new Player();
            npc1 = new Npc();

            spriteBatch = new SpriteBatch(GraphicsDevice);

            graphics.PreferredBackBufferWidth = gameResolution.X;
            graphics.PreferredBackBufferHeight = gameResolution.Y;

            graphics.ApplyChanges();

            //Content.Load<SoundEffect>("abc").Play();
            //Loading Sprites
            tileset = Content.Load<Texture2D>("tileset");
            character = Content.Load<Texture2D>("NPC1");
            //pointingCrosshair = Content.Load<Texture2D>("cursorselect");
            //actionCrosshair = Content.Load<Texture2D>("cursorgrabbing");
            gameFont = Content.Load<SpriteFont>("font");
            hud = Content.Load<Texture2D>("hud");

            Texture2D[] misc = new Texture2D[1];

            misc[0] = new Texture2D(GraphicsDevice, 1, 1);
            misc[0].SetData(new Color[] { Color.Red });

            player.Load(spriteBatch, character, misc);
            npc1.Load(spriteBatch, character);
            for (int i = 0; i < 3; i++)
            {
                mapa[i].Load(tileset, spriteBatch);
            }

            renderTarget = new RenderTarget2D(GraphicsDevice, gameResolution.X, gameResolution.Y);
            renderTargetDestination = GetRenderTargetDestination(gameResolution, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            this.ToggleFullScreen();
        }

        protected override void Update(GameTime gameTime)
        {
            // Game update code
            double dTime = gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState kstate = Keyboard.GetState();
            Vector2 shift = new(0);
            Vector2 shift2 = new(0);
            float speed = 60;

            if (kstate.IsKeyDown(Keys.F1))
            {
                showDebug = true;
            }
            if (kstate.IsKeyDown(Keys.F2))
            {
                showDebug = false;
            }

            if (kstate.IsKeyDown(Keys.Escape))
            {
                ToggleFullScreen();
            }
            if (kstate.IsKeyDown(Keys.LeftShift))
            {
                speed = (float)(speed * 1.4);
            }
            if (kstate.IsKeyDown(Keys.W))
            {
                shift.Y -= (speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                player.direction = 0;
            }
            if (kstate.IsKeyDown(Keys.S))
            {
                shift.Y += (speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                player.direction = 1;
            }
            if (kstate.IsKeyDown(Keys.A))
            {
                shift.X -= (speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                player.direction = 2;
            }
            if (kstate.IsKeyDown(Keys.D))
            {
                shift.X += (speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                player.direction = 3;
            }

            for (int i = 0; i < 3; i++)
            {
                shift2 = mapa[i].Move(shift);
            }

            if (kstate.IsKeyDown(Keys.Space))
            {
                player.ClickAction(true);
            }
            else
            {
                player.ClickAction(false);
            }

            npc1.Tick(dTime);
            npc1.MapMove(shift2);
            player.Tick(dTime);

            // Mouse usage

            mState = Mouse.GetState();                                      // Returns mouse state (every frame)
            position.X = mState.X;
            position.Y = mState.Y;

            if (mState.LeftButton == ButtonState.Pressed)                   // 
            {
                userLeftClicks++;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Aqua);
            spriteBatch.Begin();

            for (int i = 0; i < 3; i++)
            {
                mapa[i].Draw();
            }
            player.DrawAll();
            npc1.DrawAll();
            spriteBatch.Draw(
                            hud,
                            new Vector2(0,0),
                            new Rectangle(0, 0, 320, 180),
                            Color.White);

            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Aqua);
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null, null, null, Matrix.CreateScale(1, 1, 0));
            spriteBatch.Draw(renderTarget, renderTargetDestination, Color.White);
            //spriteBatch.Draw(pointingCrosshair, position, Color.White);
            if (showDebug)
            {

                spriteBatch.DrawString(gameFont, userLeftClicks.ToString(), new Vector2(10, 10), Color.Black);
                spriteBatch.DrawString(gameFont,
                    "BLOCK SHIFT: " + ((int)mapa[0].shiftBlock.X) + " , " + ((int)mapa[0].shiftBlock.Y),
                    new Vector2(10, 30), Color.Black);
                spriteBatch.DrawString(gameFont,
                    "MAP SHIFT: " + ((int)mapa[0].shiftMap.X).ToString() + ", " + ((int)mapa[0].shiftMap.Y).ToString(),
                    new Vector2(10, 50), Color.Black);
                spriteBatch.DrawString(gameFont, "CAST: " + player.cast.ToString(), new Vector2(10, 70), Color.Black);
                spriteBatch.DrawString(gameFont,
                    "FPS: " + (1 / gameTime.ElapsedGameTime.TotalSeconds).ToString(),
                    new Vector2(10, 90), Color.Black);

            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ToggleFullScreen()
        {
            if (!graphics.IsFullScreen)
            {
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                graphics.PreferredBackBufferWidth = gameResolution.X;
                graphics.PreferredBackBufferHeight = gameResolution.Y;
            }
            graphics.IsFullScreen = !graphics.IsFullScreen;
            graphics.ApplyChanges();

            renderTargetDestination = GetRenderTargetDestination(gameResolution, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }
        static Rectangle GetRenderTargetDestination(Point resolution, int preferredBackBufferWidth, int preferredBackBufferHeight)
        {
            float resolutionRatio = (float)resolution.X / resolution.Y;
            float screenRatio;
            Point bounds = new(preferredBackBufferWidth, preferredBackBufferHeight);
            screenRatio = (float)bounds.X / bounds.Y;
            float scale;
            Rectangle rectangle = new();

            if (resolutionRatio < screenRatio)
                scale = (float)bounds.Y / resolution.Y;
            else if (resolutionRatio > screenRatio)
                scale = (float)bounds.X / resolution.X;
            else
            {
                // Resolution and window/screen share aspect ratio
                rectangle.Size = bounds;
                return rectangle;
            }
            rectangle.Width = (int)(resolution.X * scale);
            rectangle.Height = (int)(resolution.Y * scale);
            return CenterRectangle(new Rectangle(Point.Zero, bounds), rectangle);
        }
        static Rectangle CenterRectangle(Rectangle outerRectangle, Rectangle innerRectangle)
        {
            Point delta = outerRectangle.Center - innerRectangle.Center;
            innerRectangle.Offset(delta);
            return innerRectangle;
        }
    }
}