using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace RolePlayingGame.ScreensManager
{
    public class ScreenManager : DrawableGameComponent
    {
        private List<GameScreen> ScreenList = new();
        private List<GameScreen> ScreensToUpdateList = new();

        SpriteBatch _spriteBatch;

        bool _isInitialized;
        bool _traceEnabled;

        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        public ScreenManager(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            _isInitialized = true;
        }

        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            ContentManager content = Game.Content;

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in ScreenList)
            {
                screen.LoadContent();
            }
        }

        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in ScreenList)
            {
                screen.UnloadContent();
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            ScreensToUpdateList.Clear();

            foreach (GameScreen screen in ScreenList)
            {
                ScreensToUpdateList.Add(screen);
            }                

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (ScreensToUpdateList.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = ScreensToUpdateList[ScreensToUpdateList.Count - 1];

                ScreensToUpdateList.RemoveAt(ScreensToUpdateList.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput();
                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                    {
                        coveredByOtherScreen = true;
                    }                        
                }
            }

            // Print debug trace?
            if (_traceEnabled)
            {
                TraceScreens();
            }                
        }

        void TraceScreens()
        {
            List<string> screenNames = new();

            foreach (GameScreen screen in ScreenList)
            {
                screenNames.Add(screen.GetType().Name);
            }                

#if WINDOWS
            Trace.WriteLine(string.Join(", ", screenNames.ToArray()));
#endif
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in ScreenList)
            {
                if (screen.ScreenState == ScreenState.Hidden) continue;

                screen.Draw(gameTime);
            }
        }

        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (_isInitialized)
            {
                screen.LoadContent();
            }

            ScreenList.Add(screen);
        }

        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (_isInitialized)
            {
                screen.UnloadContent();
            }

            ScreenList.Remove(screen);
            ScreensToUpdateList.Remove(screen);
        }

        public GameScreen[] GetScreens()
        {
            return ScreenList.ToArray();
        }
    }
}
