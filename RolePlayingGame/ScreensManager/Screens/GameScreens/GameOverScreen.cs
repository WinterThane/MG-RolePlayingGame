using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Audio;
using RolePlayingGame.InputsManager;
using RolePlayingGame.ScreensManager.Screens.MenuScreens;
using RolePlayingGame.TextFonts;
using System;

namespace RolePlayingGame.ScreensManager.Screens.GameScreens
{
    public class GameOverScreen : GameScreen
    {
        private Texture2D _backTexture;
        private Texture2D _selectIconTexture;
        private Texture2D _fadeTexture;
        private Vector2 _backgroundPosition;
        private Vector2 _titlePosition;
        private Vector2 _gameOverPosition;
        private Vector2 _selectPosition;
        private Vector2 _selectIconPosition;

        private readonly string _titleString = "Game Over";
        private readonly string _gameOverString = "The party has been defeated.";
        private readonly string _selectString = "Continue";

        /// <summary>
        /// Create a new GameOverScreen object.
        /// </summary>
        public GameOverScreen() : base()
        {
            AudioManager.PushMusic("LoseTheme");
            Exiting += new EventHandler(GameOverScreen_Exiting);
        }

        void GameOverScreen_Exiting(object sender, EventArgs e)
        {
            AudioManager.PopMusic();
        }

        /// <summary>
        /// Load the graphics data from the content manager.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            _fadeTexture = content.Load<Texture2D>("Textures/GameScreens/FadeScreen");
            _backTexture = content.Load<Texture2D>("Textures/GameScreens/PopupScreen");
            _selectIconTexture = content.Load<Texture2D>("Textures/Buttons/AButton");

            _backgroundPosition.X = (viewport.Width - _backTexture.Width) / 2;
            _backgroundPosition.Y = (viewport.Height - _backTexture.Height) / 2;

            _titlePosition.X = (viewport.Width - Fonts.HeaderFont.MeasureString(_titleString).X) / 2;
            _titlePosition.Y = _backgroundPosition.Y + 70f;

            _gameOverPosition.X = (viewport.Width - Fonts.ButtonNamesFont.MeasureString(_titleString).X) / 2;
            _gameOverPosition.Y = _backgroundPosition.Y + _backTexture.Height / 2;

            _selectIconPosition.X = viewport.Width / 2 + 260;
            _selectIconPosition.Y = _backgroundPosition.Y + 530f;
            _selectPosition.X = _selectIconPosition.X - Fonts.ButtonNamesFont.MeasureString(_selectString).X - 10f;
            _selectPosition.Y = _backgroundPosition.Y + 530f;
        }

        /// <summary>
        /// Handles user input.
        /// </summary>
        public override void HandleInput()
        {
            if (InputManager.IsActionTriggered(InputManager.Action.Ok) || InputManager.IsActionTriggered(InputManager.Action.Back))
            {
                ExitScreen();
                ScreenManager.AddScreen(new MainMenuScreen());
                return;
            }
        }

        /// <summary>
        /// Draws the screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            // Draw fading screen
            spriteBatch.Draw(_fadeTexture, new Rectangle(0, 0, 1280, 720), Color.White);

            // Draw popup texture
            spriteBatch.Draw(_backTexture, _backgroundPosition, Color.White);

            // Draw title
            spriteBatch.DrawString(Fonts.HeaderFont, _titleString, _titlePosition, Fonts.TitleColor);

            // Draw Gameover text
            spriteBatch.DrawString(Fonts.ButtonNamesFont, _gameOverString, _gameOverPosition, Fonts.CountColor);

            // Draw select button
            spriteBatch.DrawString(Fonts.ButtonNamesFont, _selectString, _selectPosition, Color.White);
            spriteBatch.Draw(_selectIconTexture, _selectIconPosition, Color.White);

            spriteBatch.End();
        }
    }
}
