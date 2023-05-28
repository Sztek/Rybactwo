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
        Color colorDebug;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D tileset;
        Texture2D character;
        Texture2D crosshair;
        Texture2D hud;

        SpriteFont gameFont;

        RenderTarget2D renderTarget;
        Rectangle renderTargetDestination;
        Vector2 scale;

        Map mapa;
        Player player;
        Npc npc1;

        MouseState mState;                              // Get info from mouse
        Vector2 position;
        KeyboardState prevState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 120d);
            IsMouseVisible = false;
            showDebug = true;
            colorDebug = Color.White;
        }
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            
            mapa = new Map();
            player = new Player();
            npc1 = new Npc();

            spriteBatch = new SpriteBatch(GraphicsDevice);

            graphics.PreferredBackBufferWidth = gameResolution.X;
            graphics.PreferredBackBufferHeight = gameResolution.Y;

            graphics.ApplyChanges();

            //Loading Sprites
            tileset = Content.Load<Texture2D>("tileset");
            character = Content.Load<Texture2D>("NPC1");
            //pointingCrosshair = Content.Load<Texture2D>("cursorselect");
            //actionCrosshair = Content.Load<Texture2D>("cursorgrabbing");
            gameFont = Content.Load<SpriteFont>("font");
            crosshair = Content.Load<Texture2D>("kursor");
            hud = Content.Load<Texture2D>("hud");

            Texture2D[] misc = new Texture2D[2];

            misc[0] = new Texture2D(GraphicsDevice, 1, 1);
            misc[0].SetData(new Color[] { Color.Red });
            misc[1] = Content.Load<Texture2D>("splawik");

            player.Load(spriteBatch, character, misc);
            npc1.Load(spriteBatch, character);
            mapa.Load(tileset, spriteBatch);

            renderTarget = new RenderTarget2D(GraphicsDevice, gameResolution.X, gameResolution.Y);
            renderTargetDestination = GetRenderTargetDestination(
                gameResolution, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            this.ToggleFullScreen();
        }

        protected override void Update(GameTime gameTime)
        {
            double dTime = gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState kState = Keyboard.GetState();
            Vector2 shift = new(0);

            if (kState.IsKeyDown(Keys.F1) && prevState.IsKeyUp(Keys.F1))
            {
                if (showDebug) { showDebug = false; }
                else { showDebug = true; }
            }

            if (prevState.IsKeyUp(Keys.Escape) && kState.IsKeyDown(Keys.Escape))
            {
                ToggleFullScreen();
            }
            
            if (kState.IsKeyDown(Keys.LeftShift))
            {
                mapa.speed = 64;
            }
            else { mapa.speed = 48; }

            for (int i = 0; i < 4; i++) { mapa.direction[i] = false; }
            if (kState.IsKeyDown(Keys.W))
            {
                mapa.direction[0] = true;
                player.direction = 0;
            }
            if (kState.IsKeyDown(Keys.S))
            {
                mapa.direction[1] = true;
                player.direction = 1;
            }
            if (kState.IsKeyDown(Keys.A))
            {
                mapa.direction[2] = true;
                player.direction = 2;
            }
            if (kState.IsKeyDown(Keys.D))
            {
                mapa.direction[3] = true;
                player.direction = 3;
            }
            mapa.Tick(dTime);
            //shift2 = mapa.Move(shift);

            if (kState.IsKeyDown(Keys.Space))
            {
                player.ClickAction(true);
            }
            else
            {
                player.ClickAction(false);
            }

            //npc1.Tick(dTime);
            //npc1.MapMove(shift2);
            player.Tick(dTime);

            // Mouse usage
            
            mState = Mouse.GetState();                                      // Returns mouse state (every frame)
            position.X = mState.X;
            position.Y = mState.Y;

            prevState = kState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Aqua);
            spriteBatch.Begin();

            mapa.Draw();
            player.DrawAll();
            //npc1.DrawAll();
            spriteBatch.Draw(
                            hud,
                            new Vector2(0,0),
                            new Rectangle(0, 0, 320, 180),
                            Color.White);
            spriteBatch.Draw(crosshair,
                            new Vector2((int)(position.X / scale.X), (int)(position.Y / scale.Y)),
                            new Rectangle(0, 0, 4, 4), Color.White);

            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Aqua);
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null, null, null, Matrix.CreateScale(1, 1, 0));
            spriteBatch.Draw(renderTarget, renderTargetDestination, Color.White);

            if (showDebug)
            {
                int line = -10;
                int next = 20;
                spriteBatch.DrawString(gameFont,
                    "BLOCK SHIFT: " + ((int)mapa.shiftBlock.X) + " , " + ((int)mapa.shiftBlock.Y),
                    new Vector2(10, line += next), colorDebug);
                spriteBatch.DrawString(gameFont,
                    "MAP SHIFT: " + ((int)mapa.shiftMap.X).ToString() + ", " + ((int)mapa.shiftMap.Y).ToString(),
                    new Vector2(10, line += next), colorDebug);
                spriteBatch.DrawString(gameFont,
                    "CAST: " + player.cast.ToString(),
                    new Vector2(10, line += next), colorDebug);
                spriteBatch.DrawString(gameFont,
                    "FPS: " + (gameTime.ElapsedGameTime.TotalSeconds).ToString(),
                    new Vector2(10, line += next), colorDebug);
                spriteBatch.DrawString(gameFont,
                    "SPEED: " + (mapa.tickTime).ToString(),
                    new Vector2(10, line += next), colorDebug);

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
                scale = new Vector2(
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / gameResolution.X,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / gameResolution.Y);
            }
            else
            {
                graphics.PreferredBackBufferWidth = gameResolution.X;
                graphics.PreferredBackBufferHeight = gameResolution.Y;
                scale = new Vector2(1, 1);
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