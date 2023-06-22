using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.TextFonts;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Content;
using RolePlayingGame.InputsManager;

namespace RolePlayingGame.ScreensManager.Screens.GameScreens
{
    public class DialogueScreen : GameScreen
    {
        private Texture2D _backgroundTexture;
        private Vector2 _backgroundPosition;
        private Texture2D _fadeTexture;

        private Texture2D _selectButtonTexture;
        private Vector2 _selectPosition;
        private Vector2 _selectButtonPosition;

        private Vector2 _backPosition;
        private Texture2D _backButtonTexture;
        private Vector2 _backButtonPosition;

        private Texture2D _scrollTexture;
        private Vector2 _scrollPosition;

        private Texture2D _lineTexture;
        private Vector2 _topLinePosition;
        private Vector2 _bottomLinePosition;

        private Vector2 _titlePosition;
        private Vector2 _dialogueStartPosition;

        /// <summary>
        /// The title text shown at the top of the screen.
        /// </summary>
        private string _titleText;

        /// <summary>
        /// The title text shown at the top of the screen.
        /// </summary>
        public string TitleText
        {
            get => _titleText;
            set => _titleText = value;
        }

        /// <summary>
        /// The dialogue shown in the main portion of this dialog.
        /// </summary>
        private string _dialogueText;

        /// <summary>
        /// The dialogue shown in the main portion of this dialog, broken into lines.
        /// </summary>
        private List<string> _dialogueList = new();

        /// <summary>
        /// The dialogue shown in the main portion of this dialog.
        /// </summary>
        public string DialogueText
        {
            get => _dialogueText;
            set
            {
                // trim the new value
                string trimmedValue = value.Trim();
                // if it's a match for what we already have, then this is trivial
                if (_dialogueText == trimmedValue)
                {
                    return;
                }
                // assign the new value
                _dialogueText = trimmedValue;
                // break the text into lines
                if (string.IsNullOrEmpty(_dialogueText))
                {
                    _dialogueList.Clear();
                }
                else
                {
                    _dialogueList = Fonts.BreakTextIntoList(_dialogueText, Fonts.DescriptionFont, _maxWidth);
                }
                // set which lines ar edrawn
                _startIndex = 0;
                _endIndex = _drawMaxLines;
                if (_endIndex > _dialogueList.Count)
                {
                    _dialogueStartPosition = new Vector2(271f, 375f - ((_dialogueList.Count - _startIndex) * Fonts.DescriptionFont.LineSpacing / 2));
                    _endIndex = _dialogueList.Count;
                }
                else
                {
                    _dialogueStartPosition = new Vector2(271f, 225f);
                }
            }
        }


        /// <summary>
        /// The text shown next to the A button, if any.
        /// </summary>
        private string _selectText = "Continue";

        /// <summary>
        /// The text shown next to the A button, if any.
        /// </summary>
        public string SelectText
        {
            get => _selectText;
            set
            {
                if (_selectText != value)
                {
                    _selectText = value;
                    if (_selectButtonTexture != null)
                    {
                        _selectPosition.X = _selectButtonPosition.X -
                            Fonts.ButtonNamesFont.MeasureString(_selectText).X - 10f;
                        _selectPosition.Y = _selectButtonPosition.Y;
                    }
                }
            }
        }

        /// <summary>
        /// The text shown next to the B button, if any.
        /// </summary>
        private string _backText = "Back";

        /// <summary>
        /// The text shown next to the B button, if any.
        /// </summary>
        public string BackText
        {
            get => _backText;
            set => _backText = value;
        }

        /// <summary>
        /// Maximum width of each line in pixels
        /// </summary>
        private const int _maxWidth = 705;

        /// <summary>
        /// Starting index of the list to be displayed
        /// </summary>
        private int _startIndex = 0;

        /// <summary>
        /// Ending index of the list to be displayed
        /// </summary>
        private int _endIndex = _drawMaxLines;

        /// <summary>
        /// Maximum number of lines to draw in the screen
        /// </summary>
        private const int _drawMaxLines = 13;

        /// <summary>
        /// Construct a new DialogueScreen object.
        /// </summary>
        /// <param name="mapEntry"></param>
        public DialogueScreen()
        {
            IsPopup = true;
        }

        /// <summary>
        /// Load the graphics content
        /// </summary>
        /// <param name="batch">SpriteBatch object</param>
        /// <param name="screenWidth">Width of the screen</param>
        /// <param name="screenHeight">Height of the screen</param>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            _fadeTexture = content.Load<Texture2D>("Textures/GameScreens/FadeScreen");
            _backgroundTexture = content.Load<Texture2D>("Textures/GameScreens/PopupScreen");
            _scrollTexture = content.Load<Texture2D>("Textures/GameScreens/ScrollButtons");
            _selectButtonTexture = content.Load<Texture2D>("Textures/Buttons/AButton");
            _backButtonTexture = content.Load<Texture2D>("Textures/Buttons/BButton");
            _lineTexture = content.Load<Texture2D>("Textures/GameScreens/SeparationLine");

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            _backgroundPosition.X = (viewport.Width - _backgroundTexture.Width) / 2;
            _backgroundPosition.Y = (viewport.Height - _backgroundTexture.Height) / 2;

            _selectButtonPosition.X = viewport.Width / 2 + 260;
            _selectButtonPosition.Y = _backgroundPosition.Y + 530f;
            _selectPosition.X = _selectButtonPosition.X - Fonts.ButtonNamesFont.MeasureString(_selectText).X - 10f;
            _selectPosition.Y = _selectButtonPosition.Y;

            _backPosition.X = viewport.Width / 2 - 250f;
            _backPosition.Y = _backgroundPosition.Y + 530f;
            _backButtonPosition.X = _backPosition.X - _backButtonTexture.Width - 10;
            _backButtonPosition.Y = _backPosition.Y;

            _scrollPosition = _backgroundPosition + new Vector2(820f, 200f);

            _topLinePosition.X = (viewport.Width - _lineTexture.Width) / 2 - 30f;
            _topLinePosition.Y = 200f;

            _bottomLinePosition.X = _topLinePosition.X;
            _bottomLinePosition.Y = 550f;

            _titlePosition.X = (viewport.Width - Fonts.HeaderFont.MeasureString(_titleText).X) / 2;
            _titlePosition.Y = _backgroundPosition.Y + 70f;
        }

        /// <summary>
        /// Handles user input to the dialog.
        /// </summary>
        public override void HandleInput()
        {
            // Press Select or Bback
            if (InputManager.IsActionTriggered(InputManager.Action.Ok) || InputManager.IsActionTriggered(InputManager.Action.Back))
            {
                ExitScreen();
                return;
            }

            // Scroll up
            if (InputManager.IsActionTriggered(InputManager.Action.CursorUp))
            {
                if (_startIndex > 0)
                {
                    _startIndex--;
                    _endIndex--;
                }
            }
            // Scroll down
            else if (InputManager.IsActionTriggered(InputManager.Action.CursorDown))
            {
                if (_startIndex < _dialogueList.Count - _drawMaxLines)
                {
                    _endIndex++;
                    _startIndex++;
                }
            }
        }

        /// <summary>
        /// draws the dialog.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            Vector2 textPosition = _dialogueStartPosition;

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            // draw the fading screen
            spriteBatch.Draw(_fadeTexture, new Rectangle(0, 0, 1280, 720), Color.White);

            // draw popup background
            spriteBatch.Draw(_backgroundTexture, _backgroundPosition, Color.White);

            // draw the top line
            spriteBatch.Draw(_lineTexture, _topLinePosition, Color.White);

            // draw the bottom line
            spriteBatch.Draw(_lineTexture, _bottomLinePosition, Color.White);

            // draw scrollbar
            spriteBatch.Draw(_scrollTexture, _scrollPosition, Color.White);

            // draw title
            spriteBatch.DrawString(Fonts.HeaderFont, _titleText, _titlePosition,
                Fonts.CountColor);

            // draw the dialogue
            for (int i = _startIndex; i < _endIndex; i++)
            {
                spriteBatch.DrawString(Fonts.DescriptionFont, _dialogueList[i], textPosition, Fonts.CountColor);
                textPosition.Y += Fonts.DescriptionFont.LineSpacing;
            }

            // draw the Back button and adjoining text
            if (!string.IsNullOrEmpty(_backText))
            {
                spriteBatch.DrawString(Fonts.ButtonNamesFont, _backText, _backPosition, Color.White);
                spriteBatch.Draw(_backButtonTexture, _backButtonPosition, Color.White);
            }

            // draw the Select button and adjoining text
            if (!string.IsNullOrEmpty(_selectText))
            {
                _selectPosition.X = _selectButtonPosition.X - Fonts.ButtonNamesFont.MeasureString(_selectText).X - 10f;
                _selectPosition.Y = _selectButtonPosition.Y;
                spriteBatch.DrawString(Fonts.ButtonNamesFont, _selectText, _selectPosition, Color.White);
                spriteBatch.Draw(_selectButtonTexture, _selectButtonPosition, Color.White);
            }

            spriteBatch.End();
        }
    }
}
