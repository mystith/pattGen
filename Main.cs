using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace pattGen
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D plain;
        Effect mainEffect;
        Pattern pattern; // r/t /////
        float detail; // q/w
        float zoom; // a/s
        float multiplier; // z/x
        float panSpeed; // v/b /////
        Vector2 pan; // arrow keys
        SpriteFont mainFont;
        MouseState prevMouseState;
        bool showGUI; // toggle with y

        KeyboardState prevKeyboardState;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                HardwareModeSwitch = false,
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferWidth = 768,
                PreferredBackBufferHeight = 768
            };

            IsMouseVisible = true;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            pattern = Pattern.BasicDist;
            zoom = 1f;
            detail = 1f;
            pan = new Vector2(0, 0);
            showGUI = true;
            panSpeed = 1;
            multiplier = 1;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mainEffect = Content.Load<Effect>("mainEffect");
            SetProperty("ZOOM", zoom);
            SetProperty("MULTI", multiplier);
            SetProperty("DETAIL", detail);

            float w = graphics.PreferredBackBufferWidth;
            float h = graphics.PreferredBackBufferHeight;
            SetProperty("WIDTH", w);
            SetProperty("HEIGHT", h);

            float hw = w / 2f;
            float hh = h / 2f;
            SetProperty("MAX_DIST", (float)System.Math.Sqrt(hw * hw + hh * hh));

            mainFont = Content.Load<SpriteFont>("mainFont");

            plain = new Texture2D(GraphicsDevice, 1, 1);
            plain.SetData(new Color[] { Color.White });
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if ((mouseState.X != prevMouseState.X || mouseState.Y != prevMouseState.Y) && prevMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Pressed)
            {
                pan.X -= mouseState.X - prevMouseState.X;
                pan.Y -= mouseState.Y - prevMouseState.Y;

                SetProperty("X_PAN", pan.X);
                SetProperty("Y_PAN", pan.Y);
            }

            if(mouseState.ScrollWheelValue != prevMouseState.ScrollWheelValue)
            {
                zoom *= (float)System.Math.Pow(1.1f, (mouseState.ScrollWheelValue - prevMouseState.ScrollWheelValue) / 100f);
                SetProperty("ZOOM", zoom);
            }
            //bool shiftMod = state.IsKeyDown(Keys.LeftShift);
            //bool ctrlMod = state.IsKeyDown(Keys.LeftControl);
            //bool altMod = state.IsKeyDown(Keys.LeftAlt);

            //DETAIL
            if (keyState.IsKeyDown(Keys.Q))
            {
                detail /= 1.05f;
                SetProperty("DETAIL", detail);
            }
            else if (keyState.IsKeyDown(Keys.W))
            {
                detail *= 1.05f;
                SetProperty("DETAIL", detail);
            }

            //ZOOM
            if (keyState.IsKeyDown(Keys.A))
            {
                zoom /= 1.1f;
                SetProperty("ZOOM", zoom);
            }
            else if (keyState.IsKeyDown(Keys.S))
            {
                zoom *= 1.1f;
                SetProperty("ZOOM", zoom);
            }

            //PAN
            if (keyState.IsKeyDown(Keys.Up))
            {
                pan.Y -= panSpeed;
                SetProperty("Y_PAN", pan.Y);
            }
            else if (keyState.IsKeyDown(Keys.Down))
            {
                pan.Y += panSpeed;
                SetProperty("Y_PAN", pan.Y);
            }

            if (keyState.IsKeyDown(Keys.Left))
            {
                pan.X -= panSpeed;
                SetProperty("X_PAN", pan.X);
            }
            else if (keyState.IsKeyDown(Keys.Right))
            {
                pan.X += panSpeed;
                SetProperty("X_PAN", pan.X);
            }

            //MULTI
            if (keyState.IsKeyDown(Keys.Z))
            {
                multiplier /= 1.05f;
                SetProperty("MULTI", multiplier);
            }
            else if (keyState.IsKeyDown(Keys.X))
            {
                multiplier *= 1.05f;
                SetProperty("MULTI", multiplier);
            }

            if (multiplier <= 0) multiplier = 0.0001f;
            //panSpeed
            if (keyState.IsKeyDown(Keys.V))
            {
                panSpeed -= 1;
            }
            else if (keyState.IsKeyDown(Keys.B))
            {
                panSpeed += 1;
            }

            //pattern
            if (keyState.IsKeyDown(Keys.R) && prevKeyboardState.IsKeyUp(Keys.R))
            {
                switch (pattern)
                {
                    case Pattern.BasicDist: pattern = Pattern.Sine; break;
                    case Pattern.LoopedDist: pattern = Pattern.BasicDist; break;
                    case Pattern.Sine: pattern = Pattern.LoopedDist; break;
                }
            }
            else if (keyState.IsKeyDown(Keys.T) && prevKeyboardState.IsKeyUp(Keys.T))
            {
                switch (pattern)
                {
                    case Pattern.BasicDist: pattern = Pattern.LoopedDist; break;
                    case Pattern.LoopedDist: pattern = Pattern.Sine; break;
                    case Pattern.Sine: pattern = Pattern.BasicDist; break;
                }
            }

            if(keyState.IsKeyDown(Keys.Y) && prevKeyboardState.IsKeyUp(Keys.Y))
            {
                if (showGUI) showGUI = false;
                else showGUI = true;
            }

            prevMouseState = mouseState;
            prevKeyboardState = keyState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            mainEffect.CurrentTechnique.Passes[pattern.ToString()].Apply();
            spriteBatch.Draw(plain, new Rectangle(0, 0, 768, 768), Color.White);

            spriteBatch.End();

            if (showGUI)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(plain, new Rectangle(6, 6, 206, 124), Color.FromNonPremultiplied(0, 0, 0, 150));
                spriteBatch.DrawString(mainFont, $"(r/t) Pattern: {pattern.ToString()}", new Vector2(10, 10), Color.White);
                spriteBatch.DrawString(mainFont, $"(q/w) Detail: {detail}", new Vector2(10, 23), Color.White);
                spriteBatch.DrawString(mainFont, $"(a/s) Zoom: {zoom}", new Vector2(10, 36), Color.White);
                spriteBatch.DrawString(mainFont, $"(z/x) Multiplier: {multiplier}x", new Vector2(10, 49), Color.White);
                spriteBatch.DrawString(mainFont, $"(v/b) Pan Speed: {panSpeed}", new Vector2(10, 62), Color.White);
                spriteBatch.DrawString(mainFont, $"(arrow keys) Pan X: {pan.X}", new Vector2(10, 75), Color.White);
                spriteBatch.DrawString(mainFont, $"(arrow keys) Pan Y: {pan.Y}", new Vector2(10, 88), Color.White);
                spriteBatch.DrawString(mainFont, "Toggle GUI with Y", new Vector2(10, 114), Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public void SetProperty(string key, float value)
        {
            mainEffect.Parameters[key].SetValue(value);
        }

        public void SetProperty(string key, int value)
        {
            mainEffect.Parameters[key].SetValue(value);
        }
    }
}
