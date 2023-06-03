using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Audio;
using RolePlayingGame.Engine;
using RolePlayingGame.InputsManager;
using RolePlayingGame.ScreensManager;
using RolePlayingGame.ScreensManager.Screens.MenuScreens;

namespace RolePlayingGame
{
    public class RolePlayingGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private ScreenManager _screenManager;

        public RolePlayingGame()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //Components.Add(new GamerServicesComponent(this));

            // add the audio manager
            //AudioManager.Initialize(this, @"Content\Audio\RpgAudio.xgs", @"Content\Audio\Wave Bank.xwb", @"Content\Audio\Sound Bank.xsb");

            // add the screen manager
            _screenManager = new ScreenManager(this);
            Components.Add(_screenManager);
        }

        protected override void Initialize()
        {
            InputManager.Initialize();

            base.Initialize();

            TileEngine.Viewport = _graphics.GraphicsDevice.Viewport;

            _screenManager.AddScreen(new MainMenuScreen());
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            //if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            InputManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            _graphics.GraphicsDevice.Clear(Color.Transparent);
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}