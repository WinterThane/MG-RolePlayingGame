﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RolePlayingGame.TextFonts
{
    public static class Fonts
    {
        private static SpriteFont headerFont;
        public static SpriteFont HeaderFont
        {
            get { return headerFont; }
        }


        private static SpriteFont playerNameFont;
        public static SpriteFont PlayerNameFont
        {
            get { return playerNameFont; }
        }


        private static SpriteFont debugFont;
        public static SpriteFont DebugFont
        {
            get { return debugFont; }
        }


        private static SpriteFont buttonNamesFont;
        public static SpriteFont ButtonNamesFont
        {
            get { return buttonNamesFont; }
        }


        private static SpriteFont descriptionFont;
        public static SpriteFont DescriptionFont
        {
            get { return descriptionFont; }
        }


        private static SpriteFont gearInfoFont;
        public static SpriteFont GearInfoFont
        {
            get { return gearInfoFont; }
        }


        private static SpriteFont damageFont;
        public static SpriteFont DamageFont
        {
            get { return damageFont; }
        }


        private static SpriteFont playerStatisticsFont;
        public static SpriteFont PlayerStatisticsFont
        {
            get { return playerStatisticsFont; }
        }


        private static SpriteFont hudDetailFont;
        public static SpriteFont HudDetailFont
        {
            get { return hudDetailFont; }
        }


        private static SpriteFont captionFont;
        public static SpriteFont CaptionFont
        {
            get { return captionFont; }
        }

        public static readonly Color CountColor = new(79, 24, 44);
        public static readonly Color TitleColor = new(59, 18, 6);
        public static readonly Color CaptionColor = new(228, 168, 57);
        public static readonly Color HighlightColor = new(223, 206, 148);
        public static readonly Color DisplayColor = new(68, 32, 19);
        public static readonly Color DescriptionColor = new(0, 0, 0);
        public static readonly Color RestrictionColor = new(0, 0, 0);
        public static readonly Color ModifierColor = new(0, 0, 0);
        public static readonly Color MenuSelectedColor = new(248, 218, 127);

        /// <summary>
        /// Load the fonts from the content pipeline.
        /// </summary>
        public static void LoadContent(ContentManager contentManager)
        {
            // check the parameters
            if (contentManager == null)
            {
                throw new ArgumentNullException("contentManager");
            }

            // load each font from the content pipeline
            buttonNamesFont = contentManager.Load<SpriteFont>("Fonts/ButtonNamesFont");
            captionFont = contentManager.Load<SpriteFont>("Fonts/ButtonNamesFont");
            damageFont = contentManager.Load<SpriteFont>("Fonts/ButtonNamesFont");
            debugFont = contentManager.Load<SpriteFont>("Fonts/ButtonNamesFont");
            descriptionFont = contentManager.Load<SpriteFont>("Fonts/ButtonNamesFont");
            gearInfoFont = contentManager.Load<SpriteFont>("Fonts/ButtonNamesFont");
            headerFont = contentManager.Load<SpriteFont>("Fonts/ButtonNamesFont");
            hudDetailFont = contentManager.Load<SpriteFont>("Fonts/ButtonNamesFont");
            playerNameFont = contentManager.Load<SpriteFont>("Fonts/ButtonNamesFont");
            playerStatisticsFont = contentManager.Load<SpriteFont>("Fonts/ButtonNamesFont");
        }


        /// <summary>
        /// Release all references to the fonts.
        /// </summary>
        public static void UnloadContent()
        {
            buttonNamesFont = null;
            captionFont = null;
            damageFont = null;
            debugFont = null;
            descriptionFont = null;
            gearInfoFont = null;
            headerFont = null;
            hudDetailFont = null;
            playerNameFont = null;
            playerStatisticsFont = null;
        }

        /// <summary>
        /// Adds newline characters to a string so that it fits within a certain size.
        /// </summary>
        /// <param name="text">The text to be modified.</param>
        /// <param name="maximumCharactersPerLine">
        /// The maximum length of a single line of text.
        /// </param>
        /// <param name="maximumLines">The maximum number of lines to draw.</param>
        /// <returns>The new string, with newline characters if needed.</returns>
        public static string BreakTextIntoLines(string text, int maximumCharactersPerLine, int maximumLines)
        {
            if (maximumLines <= 0)
            {
                throw new ArgumentOutOfRangeException("maximumLines");
            }
            if (maximumCharactersPerLine <= 0)
            {
                throw new ArgumentOutOfRangeException("maximumCharactersPerLine");
            }

            // if the string is trivial, then this is really easy
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // if the text is short enough to fit on one line, then this is still easy
            if (text.Length < maximumCharactersPerLine)
            {
                return text;
            }

            // construct a new string with carriage returns
            StringBuilder stringBuilder = new(text);
            int currentLine = 0;
            int newLineIndex = 0;

            while (((text.Length - newLineIndex) > maximumCharactersPerLine) && (currentLine < maximumLines))
            {
                text.IndexOf(' ', 0);
                int nextIndex = newLineIndex;
                while ((nextIndex >= 0) && (nextIndex < maximumCharactersPerLine))
                {
                    newLineIndex = nextIndex;
                    nextIndex = text.IndexOf(' ', newLineIndex + 1);
                }
                stringBuilder.Replace(' ', '\n', newLineIndex, 1);
                currentLine++;
            }

            return stringBuilder.ToString();
        }


        /// <summary>
        /// Adds new-line characters to a string to make it fit.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="maximumCharactersPerLine">
        /// The maximum length of a single line of text.
        /// </param>
        public static string BreakTextIntoLines(string text, int maximumCharactersPerLine)
        {
            // check the parameters
            if (maximumCharactersPerLine <= 0)
            {
                throw new ArgumentOutOfRangeException("maximumCharactersPerLine");
            }

            // if the string is trivial, then this is really easy
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // if the text is short enough to fit on one line, then this is still easy
            if (text.Length < maximumCharactersPerLine)
            {
                return text;
            }

            // construct a new string with carriage returns
            StringBuilder stringBuilder = new(text);
            int currentLine = 0;
            int newLineIndex = 0;

            while (((text.Length - newLineIndex) > maximumCharactersPerLine))
            {
                text.IndexOf(' ', 0);
                int nextIndex = newLineIndex;
                while ((nextIndex >= 0) && (nextIndex < maximumCharactersPerLine))
                {
                    newLineIndex = nextIndex;
                    nextIndex = text.IndexOf(' ', newLineIndex + 1);
                }
                stringBuilder.Replace(' ', '\n', newLineIndex, 1);
                currentLine++;
            }

            return stringBuilder.ToString();
        }


        /// <summary>
        /// Break text up into separate lines to make it fit.
        /// </summary>
        /// <param name="text">The text to be broken up.</param>
        /// <param name="font">The font used ot measure the width of the text.</param>
        /// <param name="rowWidth">The maximum width of each line, in pixels.</param>
        public static List<string> BreakTextIntoList(string text, SpriteFont font, int rowWidth)
        {
            // check parameters
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }
            if (rowWidth <= 0)
            {
                throw new ArgumentOutOfRangeException("rowWidth");
            }

            // create the list
            List<string> lines = new();

            // check for trivial text
            if (string.IsNullOrEmpty("text"))
            {
                lines.Add(string.Empty);
                return lines;
            }

            // check for text that fits on a single line
            if (font.MeasureString(text).X <= rowWidth)
            {
                lines.Add(text);
                return lines;
            }

            // break the text up into words
            string[] words = text.Split(' ');

            // add words until they go over the length
            int currentWord = 0;
            while (currentWord < words.Length)
            {
                int wordsThisLine = 0;
                string line = string.Empty;
                while (currentWord < words.Length)
                {
                    string testLine = line;
                    if (testLine.Length < 1)
                    {
                        testLine += words[currentWord];
                    }
                    else if ((testLine[testLine.Length - 1] == '.') || (testLine[testLine.Length - 1] == '?') || (testLine[testLine.Length - 1] == '!'))
                    {
                        testLine += "  " + words[currentWord];
                    }
                    else
                    {
                        testLine += " " + words[currentWord];
                    }
                    if ((wordsThisLine > 0) && (font.MeasureString(testLine).X > rowWidth))
                    {
                        break;
                    }
                    line = testLine;
                    wordsThisLine++;
                    currentWord++;
                }
                lines.Add(line);
            }
            return lines;
        }


        /// <summary>
        /// Returns a properly-formatted gold-quantity string.
        /// </summary>
        public static string GetGoldString(int gold)
        {
            return string.Format("{0:n0}", gold);
        }

        /// <summary>
        /// Draws text centered at particular position.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch object used to draw.</param>
        /// <param name="font">The font used to draw the text.</param>
        /// <param name="text">The text to be drawn</param>
        /// <param name="position">The center position of the text.</param>
        /// <param name="color">The color of the text.</param>
        public static void DrawCenteredText(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color)
        {
            // check the parameters
            if (spriteBatch == null)
            {
                throw new ArgumentNullException("spriteBatch");
            }
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            // check for trivial text
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            // calculate the centered position
            Vector2 textSize = font.MeasureString(text);
            Vector2 centeredPosition = new(position.X - (int)textSize.X / 2, position.Y - (int)textSize.Y / 2);

            // draw the string
            spriteBatch.DrawString(font, text, centeredPosition, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f - position.Y / 720f);
        }
    }
}
