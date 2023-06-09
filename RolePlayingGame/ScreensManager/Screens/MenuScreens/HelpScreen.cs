﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.InputsManager;
using RolePlayingGame.TextFonts;
using System.Collections.Generic;

namespace RolePlayingGame.ScreensManager.Screens.MenuScreens
{
    public class HelpScreen : GameScreen
    {
        private Texture2D backgroundTexture;

        private Texture2D plankTexture;
        private Vector2 plankPosition;
        private Vector2 titlePosition;

        private string helpText = @"Welcome, hero!  You must meet new comrades, earn necessary
                                    experience, gold, spells, and the equipment required to challenge
                                    and defeat the evil Tamar, who resides in his lair, known as the
                                    Unspoken Tower.  Be wary!  The Unspoken Tower is filled with
                                    monstrosities that only the most hardened of heroes could possibly 
                                    face.  Good luck!";

        private List<string> textLines;

        private Texture2D scrollUpTexture;
        private readonly Vector2 scrollUpPosition = new(980, 200);
        private Texture2D scrollDownTexture;
        private readonly Vector2 scrollDownPosition = new(980, 460);

        private Texture2D lineBorderTexture;
        private readonly Vector2 linePosition = new(200, 570);

        private Texture2D backTexture;
        private readonly Vector2 backPosition = new(225, 610);

        private int startIndex;
        private const int maxLineDisplay = 3;

        public HelpScreen() : base()
        {
            textLines = Fonts.BreakTextIntoList(helpText, Fonts.DescriptionFont, 590);
        }

        /// <summary>
        /// Loads the graphics content for this screen
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            ContentManager content = ScreenManager.Game.Content;

            backgroundTexture = content.Load<Texture2D>("Textures/MainMenu/MainMenu");
            plankTexture = content.Load<Texture2D>("Textures/MainMenu/MainMenuPlank03");
            backTexture = content.Load<Texture2D>("Textures/Buttons/BButton");
            scrollUpTexture = content.Load<Texture2D>("Textures/GameScreens/ScrollUp");
            scrollDownTexture = content.Load<Texture2D>("Textures/GameScreens/ScrollDown");
            lineBorderTexture = content.Load<Texture2D>("Textures/GameScreens/LineBorder");

            plankPosition.X = backgroundTexture.Width / 2 - plankTexture.Width / 2;
            plankPosition.Y = 60;

            titlePosition.X = plankPosition.X + (plankTexture.Width - Fonts.HeaderFont.MeasureString("Help").X) / 2;
            titlePosition.Y = plankPosition.Y + (plankTexture.Height - Fonts.HeaderFont.MeasureString("Help").Y) / 2;
        }

        /// <summary>
        /// Handles user input.
        /// </summary>
        public override void HandleInput()
        {
            // exits the screen
            if (InputManager.IsActionTriggered(InputManager.Action.Back))
            {
                ExitScreen();
                return;
            }
            // scroll down
            else if (InputManager.IsActionTriggered(InputManager.Action.CursorDown))
            {
                // Traverse down the help text
                if (startIndex + maxLineDisplay < textLines.Count)
                {
                    startIndex += 1;
                }
            }
            // scroll up
            else if (InputManager.IsActionTriggered(InputManager.Action.CursorUp))
            {
                // Traverse up the help text
                if (startIndex > 0)
                {
                    startIndex -= 1;
                }
            }
        }

        /// <summary>
        /// Draws the help screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
            spriteBatch.Draw(plankTexture, plankPosition, Color.White);
            spriteBatch.Draw(backTexture, backPosition, Color.White);

            spriteBatch.Draw(lineBorderTexture, linePosition, Color.White);
            spriteBatch.DrawString(Fonts.ButtonNamesFont, "Back", new Vector2(backPosition.X + 55, backPosition.Y + 5), Color.White);

            spriteBatch.Draw(scrollUpTexture, scrollUpPosition, Color.White);
            spriteBatch.Draw(scrollDownTexture, scrollDownPosition, Color.White);

            spriteBatch.DrawString(Fonts.HeaderFont, "Help", titlePosition, Fonts.TitleColor);

            for (int i = 0; i < maxLineDisplay; i++)
            {
                spriteBatch.DrawString(Fonts.DescriptionFont, textLines[startIndex + i], new Vector2(360, 200 + (Fonts.DescriptionFont.LineSpacing + 10) * i), Color.Black);
            }

            spriteBatch.End();
        }
    }
}
