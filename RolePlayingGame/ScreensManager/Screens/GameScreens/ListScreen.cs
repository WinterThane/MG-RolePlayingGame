using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.InputsManager;
using RolePlayingGame.SessionObjects;
using RolePlayingGame.TextFonts;
using System;
using System.Collections.ObjectModel;

namespace RolePlayingGame.ScreensManager.Screens.GameScreens
{
    public abstract class ListScreen<T> : GameScreen
    {
        protected readonly Vector2 _iconOffset = new(0f, 0f);
        protected readonly Vector2 _descriptionTextPosition = new(200, 550);
        private readonly Vector2 _listPositionTopPosition = new(1160, 354);
        private readonly Vector2 _listPositionBottomPosition = new(1160, 384);

        private Texture2D _backgroundTexture;
        private readonly Rectangle _backgroundDestination = new(0, 0, 1280, 720);
        private Texture2D _fadeTexture;

        private Texture2D _listTexture;
        private readonly Vector2 _listTexturePosition = new(187f, 180f);
        protected readonly Vector2 _listEntryStartPosition = new(200f, 202f);
        protected const int _listLineSpacing = 76;

        private Texture2D _plankTexture;
        private Vector2 _plankTexturePosition;
        protected string _titleText = string.Empty;

        protected Texture2D _goldTexture;
        private readonly Vector2 _goldTexturePosition = new(490f, 640f);
        protected string _goldText = string.Empty;
        private readonly Vector2 _goldTextPosition = new(565f, 648f);

        private Texture2D _highlightTexture;
        private readonly Vector2 _highlightStartPosition = new(170f, 237f);
        private Texture2D _selectionArrowTexture;
        private readonly Vector2 _selectionArrowPosition = new(135f, 245f);

        private Texture2D _leftTriggerTexture;
        private readonly Vector2 _leftTriggerTexturePosition = new(340f, 50f);
        protected string _leftTriggerText = string.Empty;

        private Texture2D _rightTriggerTexture;
        private readonly Vector2 _rightTriggerTexturePosition = new(900f, 50f);
        protected string _rightTriggerText = string.Empty;

        private Texture2D _backButtonTexture;
        private readonly Vector2 _backButtonTexturePosition = new(80f, 640f);
        protected string _backButtonText = string.Empty;
        private Vector2 _backButtonTextPosition = new(90f, 645f); // + tex width

        private Texture2D _selectButtonTexture;
        private readonly Vector2 _selectButtonTexturePosition = new(1150f, 640f);
        protected string _selectButtonText = string.Empty;

        private Texture2D _xButtonTexture;
        private readonly Vector2 _xButtonTexturePosition = new(240f, 640f);
        protected string _xButtonText = string.Empty;
        private Vector2 _xButtonTextPosition = new(250f, 645f); // + tex width

        private Texture2D _yButtonTexture;
        private readonly Vector2 _yButtonTexturePosition = new(890f, 640f);
        protected string _yButtonText = string.Empty;

        /// <summary>
        /// Get the list that this screen displays.
        /// </summary>
        /// <returns></returns>
        public abstract ReadOnlyCollection<T> GetDataList();

        /// <summary>
        /// The index of the selected entry.
        /// </summary>
        private int _selectedIndex = 0;

        /// <summary>
        /// The index of the selected entry.
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    EnsureVisible(_selectedIndex);
                }
            }
        }

        /// <summary>
        /// Ensure that the given index is visible on the screen.
        /// </summary>
        public void EnsureVisible(int index)
        {
            if (index < _startIndex)
            {
                // if it's above the current selection, set the first entry
                _startIndex = index;
            }
            if (_selectedIndex > (_endIndex - 1))
            {
                _startIndex += _selectedIndex - (_endIndex - 1);
            }
            // otherwise, it should be in the current selection already
            // -- note that the start and end indices are checked in Draw.
        }

        /// <summary>
        /// Move the current selection up one entry.
        /// </summary>
        protected virtual void MoveCursorUp()
        {
            if (SelectedIndex > 0)
            {
                SelectedIndex--;
            }
        }

        /// <summary>
        /// Move the current selection down one entry.
        /// </summary>
        protected virtual void MoveCursorDown()
        {
            SelectedIndex++;   // safety-checked in Draw()
        }

        /// <summary>
        /// Decrease the selected quantity by one.
        /// </summary>
        protected virtual void MoveCursorLeft() { }

        /// <summary>
        /// Increase the selected quantity by one.
        /// </summary>
        protected virtual void MoveCursorRight() { }

        /// <summary>
        /// The first index displayed on the screen from the list.
        /// </summary>
        private int _startIndex = 0;

        /// <summary>
        /// The first index displayed on the screen from the list.
        /// </summary>
        public int StartIndex
        {
            get => _startIndex;
            set => _startIndex = value;  // safety-checked in Draw
        }

        /// <summary>
        /// The last index displayed on the screen from the list.
        /// </summary>
        private int _endIndex = 0;

        /// <summary>
        /// The last index displayed on the screen from the list.
        /// </summary>
        public int EndIndex
        {
            get => _endIndex;
            set => _endIndex = value;    // safety-checked in Draw
        }

        /// <summary>
        /// The maximum number of list entries that the screen can show at once.
        /// </summary>
        public const int MaximumListEntries = 4;

        /// <summary>
        /// Constructs a new ListScreen object.
        /// </summary>
        public ListScreen() : base()
        {
            IsPopup = true;
        }

        /// <summary>
        /// Load the graphics content from the content manager.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            // load the background textures
            _fadeTexture = content.Load<Texture2D>("Textures/GameScreens/FadeScreen");
            _backgroundTexture = content.Load<Texture2D>("Textures/GameScreens/GameScreenBkgd");
            _listTexture = content.Load<Texture2D>("Textures/GameScreens/InfoDisplay");
            _plankTexture = content.Load<Texture2D>("Textures/MainMenu/MainMenuPlank03");
            _goldTexture = content.Load<Texture2D>("Textures/GameScreens/GoldIcon");

            // load the foreground textures
            _highlightTexture = content.Load<Texture2D>("Textures/GameScreens/HighlightLarge");
            _selectionArrowTexture = content.Load<Texture2D>("Textures/GameScreens/SelectionArrow");

            // load the trigger images
            _leftTriggerTexture = content.Load<Texture2D>("Textures/Buttons/LeftTriggerButton");
            _rightTriggerTexture = content.Load<Texture2D>("Textures/Buttons/RightTriggerButton");
            //leftQuantityArrowTexture = content.Load<Texture2D>(@"Textures\Buttons\QuantityArrowLeft");
            //rightQuantityArrowTexture = content.Load<Texture2D>(@"Textures\Buttons\QuantityArrowRight");
            _backButtonTexture = content.Load<Texture2D>("Textures/Buttons/BButton");
            _selectButtonTexture = content.Load<Texture2D>("Textures/Buttons/AButton");
            _xButtonTexture = content.Load<Texture2D>("Textures/Buttons/XButton");
            _yButtonTexture = content.Load<Texture2D>("Textures/Buttons/YButton");

            // calculate the centered positions
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            _plankTexturePosition = new Vector2(viewport.X + (viewport.Width - _plankTexture.Width) / 2f, 67f);

            // adjust positions for texture sizes
            if (_backButtonTexture != null)
            {
                _backButtonTextPosition.X += _backButtonTexture.Width;
            }
            if (_xButtonTexture != null)
            {
                _xButtonTextPosition.X += _xButtonTexture.Width;
            }

            base.LoadContent();
        }

        /// <summary>
        /// Handle user input.
        /// </summary>
        public override void HandleInput()
        {
            if (InputManager.IsActionTriggered(InputManager.Action.PageLeft))
            {
                PageScreenLeft();
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.PageRight))
            {
                PageScreenRight();
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.CursorUp))
            {
                MoveCursorUp();
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.CursorDown))
            {
                MoveCursorDown();
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.IncreaseAmount))
            {
                MoveCursorRight();
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.DecreaseAmount))
            {
                MoveCursorLeft();
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.Back))
            {
                BackTriggered();
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.Ok))
            {
                ReadOnlyCollection<T> dataList = GetDataList();
                if ((_selectedIndex >= 0) && (_selectedIndex < dataList.Count))
                {
                    SelectTriggered(dataList[_selectedIndex]);
                }
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.DropUnEquip))
            {
                ReadOnlyCollection<T> dataList = GetDataList();
                if ((_selectedIndex >= 0) && (_selectedIndex < dataList.Count))
                {
                    ButtonXPressed(dataList[_selectedIndex]);
                }
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.TakeView))
            {
                ReadOnlyCollection<T> dataList = GetDataList();
                if ((_selectedIndex >= 0) && (_selectedIndex < dataList.Count))
                {
                    ButtonYPressed(dataList[_selectedIndex]);
                }
            }
            base.HandleInput();
        }

        /// <summary>
        /// Switch to the screen to the "left" of this one in the UI, if any.
        /// </summary>
        protected virtual void PageScreenLeft() { }

        /// <summary>
        /// Switch to the screen to the "right" of this one in the UI, if any.
        /// </summary>
        protected virtual void PageScreenRight() { }

        /// <summary>
        /// Respond to the triggering of the Back action.
        /// </summary>
        protected virtual void BackTriggered()
        {
            ExitScreen();
        }

        /// <summary>
        /// Respond to the triggering of the Select action.
        /// </summary>
        protected virtual void SelectTriggered(T entry) { }

        /// <summary>
        /// Respond to the triggering of the X button (and related key).
        /// </summary>
        protected virtual void ButtonXPressed(T entry) { }

        /// <summary>
        /// Respond to the triggering of the Y button (and related key).
        /// </summary>
        protected virtual void ButtonYPressed(T entry) { }

        /// <summary>
        /// Draws the screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // get the content list
            ReadOnlyCollection<T> dataList = GetDataList();

            // turn off the buttons if the list is empty
            if (dataList.Count <= 0)
            {
                _selectButtonText = string.Empty;
                _xButtonText = string.Empty;
                _yButtonText = string.Empty;
            }

            // fix the indices for the current list size
            SelectedIndex = MathHelper.Clamp(SelectedIndex, 0, dataList.Count - 1);
            _startIndex = MathHelper.Clamp(_startIndex, 0, dataList.Count - MaximumListEntries);
            _endIndex = Math.Min(_startIndex + MaximumListEntries, dataList.Count);

            spriteBatch.Begin();

            DrawBackground();
            if (dataList.Count > 0)
            {
                DrawListPosition(SelectedIndex + 1, dataList.Count);
            }
            DrawButtons();
            DrawPartyGold();
            DrawColumnHeaders();
            DrawTitle();

            // draw each item currently shown
            Vector2 position = _listEntryStartPosition + new Vector2(0f, _listLineSpacing / 2);
            if (_startIndex >= 0)
            {
                for (int index = _startIndex; index < _endIndex; index++)
                {
                    T entry = dataList[index];
                    if (index == _selectedIndex)
                    {
                        DrawSelection(position);
                        DrawEntry(entry, position, true);
                        DrawSelectedDescription(entry);
                    }
                    else
                    {
                        DrawEntry(entry, position, false);
                    }
                    position.Y += _listLineSpacing;
                }
            }

            spriteBatch.End();
        }


        /// <summary>
        /// Draw the entry at the given position in the list.
        /// </summary>
        /// <param name="entry">The entry to draw.</param>
        /// <param name="position">The position to draw the entry at.</param>
        /// <param name="isSelected">If true, this entry is selected.</param>
        protected abstract void DrawEntry(T entry, Vector2 position, bool isSelected);

        /// <summary>
        /// Draw the selection graphics over the selected item.
        /// </summary>
        /// <param name="position"></param>
        protected virtual void DrawSelection(Vector2 position)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Draw(_highlightTexture, new Vector2(_highlightStartPosition.X, position.Y), Color.White);
            spriteBatch.Draw(_selectionArrowTexture, new Vector2(_selectionArrowPosition.X, position.Y + 10f), Color.White);
        }

        /// <summary>
        /// Draw the background of the screen.
        /// </summary>
        protected virtual void DrawBackground()
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Draw(_fadeTexture, _backgroundDestination, Color.White);
            spriteBatch.Draw(_backgroundTexture, _backgroundDestination, Color.White);
            spriteBatch.Draw(_listTexture, _listTexturePosition, Color.White);
        }

        /// <summary>
        /// Draw the current list position in the appropriate location on the screen.
        /// </summary>
        /// <param name="position">The current position in the list.</param>
        /// <param name="total">The total elements in the list.</param>
        protected virtual void DrawListPosition(int position, int total)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // draw the top number - the current position in the list
            string listPositionTopText = position.ToString();
            Vector2 drawPosition = _listPositionTopPosition;
            drawPosition.X -= (float)Math.Ceiling(Fonts.GearInfoFont.MeasureString(listPositionTopText).X / 2);
            spriteBatch.DrawString(Fonts.GearInfoFont, listPositionTopText, drawPosition, Fonts.CountColor);

            // draw the bottom number - the current position in the list
            string listPositionBottomText = total.ToString();
            drawPosition = _listPositionBottomPosition;
            drawPosition.X -= (float)Math.Ceiling(Fonts.GearInfoFont.MeasureString(listPositionBottomText).X / 2);
            spriteBatch.DrawString(Fonts.GearInfoFont, listPositionBottomText, drawPosition, Fonts.CountColor);
        }


        /// <summary>
        /// Draw the party gold text.
        /// </summary>
        protected virtual void DrawPartyGold()
        {
            if (!IsActive)
            {
                return;
            }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Draw(_goldTexture, _goldTexturePosition, Color.White);
            spriteBatch.DrawString(Fonts.ButtonNamesFont, Fonts.GetGoldString(Session.Party.PartyGold), _goldTextPosition, Color.White);
        }

        /// <summary>
        /// Draw the description of the selected item.
        /// </summary>
        protected abstract void DrawSelectedDescription(T entry);

        /// <summary>
        /// Draw the column headers above the list.
        /// </summary>
        protected abstract void DrawColumnHeaders();

        /// <summary>
        /// Draw all of the buttons used by the screen.
        /// </summary>
        protected virtual void DrawButtons()
        {
            if (!IsActive)
            {
                return;
            }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // draw the left trigger texture and text
            if ((_leftTriggerTexture != null) && !string.IsNullOrEmpty(_leftTriggerText))
            {
                Vector2 position = _leftTriggerTexturePosition + new Vector2(_leftTriggerTexture.Width / 2f - (float)Math.Ceiling(Fonts.PlayerStatisticsFont.MeasureString(_leftTriggerText).X / 2f), 90f);
                spriteBatch.Draw(_leftTriggerTexture, _leftTriggerTexturePosition, Color.White);
                spriteBatch.DrawString(Fonts.PlayerStatisticsFont, _leftTriggerText, position, Color.Black);
            }

            // draw the right trigger texture and text
            if ((_rightTriggerTexture != null) && !string.IsNullOrEmpty(_rightTriggerText))
            {
                Vector2 position = _rightTriggerTexturePosition + new Vector2(_rightTriggerTexture.Width / 2f - (float)Math.Ceiling(Fonts.PlayerStatisticsFont.MeasureString(_rightTriggerText).X / 2f), 90f);
                spriteBatch.Draw(_rightTriggerTexture, _rightTriggerTexturePosition, Color.White);
                spriteBatch.DrawString(Fonts.PlayerStatisticsFont, _rightTriggerText, position, Color.Black);
            }

            // draw the left trigger texture and text
            if ((_backButtonTexture != null) && !string.IsNullOrEmpty(_backButtonText))
            {
                spriteBatch.Draw(_backButtonTexture, _backButtonTexturePosition, Color.White);
                spriteBatch.DrawString(Fonts.ButtonNamesFont, _backButtonText, _backButtonTextPosition, Color.White);
            }

            // draw the left trigger texture and text
            if ((_selectButtonTexture != null) && !string.IsNullOrEmpty(_selectButtonText))
            {
                spriteBatch.Draw(_selectButtonTexture, _selectButtonTexturePosition, Color.White);
                Vector2 position = _selectButtonTexturePosition - new Vector2(Fonts.ButtonNamesFont.MeasureString(_selectButtonText).X, 0f) + new Vector2(0f, 5f);
                spriteBatch.DrawString(Fonts.ButtonNamesFont, _selectButtonText, position, Color.White);
            }

            // draw the left trigger texture and text
            if ((_xButtonTexture != null) && !string.IsNullOrEmpty(_xButtonText))
            {
                spriteBatch.Draw(_xButtonTexture, _xButtonTexturePosition, Color.White);
                spriteBatch.DrawString(Fonts.ButtonNamesFont, _xButtonText, _xButtonTextPosition, Color.White);
            }

            // draw the left trigger texture and text
            if ((_yButtonTexture != null) && !string.IsNullOrEmpty(_yButtonText))
            {
                spriteBatch.Draw(_yButtonTexture, _yButtonTexturePosition, Color.White);
                Vector2 position = _yButtonTexturePosition - new Vector2(Fonts.ButtonNamesFont.MeasureString(_yButtonText).X, 0f) + new Vector2(0f, 5f);
                spriteBatch.DrawString(Fonts.ButtonNamesFont, _yButtonText, position, Color.White);
            }
        }

        /// <summary>
        /// Draw the title of the screen, if any.
        /// </summary>
        protected virtual void DrawTitle()
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // draw the left trigger texture and text
            if ((_plankTexture != null) && !string.IsNullOrEmpty(_titleText))
            {
                Vector2 titleTextSize = Fonts.HeaderFont.MeasureString(_titleText);
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                Vector2 position = new((float)Math.Floor(viewport.X + viewport.Width / 2 - titleTextSize.X / 2f), 90f);
                spriteBatch.Draw(_plankTexture, _plankTexturePosition, Color.White);
                spriteBatch.DrawString(Fonts.HeaderFont, _titleText, position, Fonts.TitleColor);
            }
        }
    }
}
