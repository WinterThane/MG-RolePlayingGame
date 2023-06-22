using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Audio;
using RolePlayingGame.Characters;
using RolePlayingGame.Data;
using RolePlayingGame.InputsManager;
using RolePlayingGame.MapObjects;
using RolePlayingGame.SessionObjects;
using RolePlayingGame.TextFonts;
using System;
using System.Collections.Generic;

namespace RolePlayingGame.ScreensManager.Screens.GameScreens
{
    public class InnScreen : GameScreen
    {
        private Inn inn;

        private Texture2D _backgroundTexture;
        private Texture2D _plankTexture;
        private Texture2D _selectIconTexture;
        private Texture2D _backIconTexture;
        private Texture2D _highlightTexture;
        private Texture2D _arrowTexture;
        private Texture2D _conversationTexture;
        private Texture2D _fadeTexture;
        private Texture2D _goldIcon;

        private readonly Vector2 _stayPosition = new(620f, 250f);
        private readonly Vector2 _leavePosition = new (620f, 300f);
        private readonly Vector2 _costPosition = new (470, 450);
        private readonly Vector2 _informationPosition = new (470, 490);
        private readonly Vector2 _selectIconPosition = new (1150, 640);
        private readonly Vector2 _backIconPosition = new (80, 640);
        private readonly Vector2 _goldStringPosition = new (565, 648);
        private readonly Vector2 _stayArrowPosition = new (520f, 234f);
        private readonly Vector2 _leaveArrowPosition = new (520f, 284f);
        private readonly Vector2 _stayHighlightPosition = new (180f, 230f);
        private readonly Vector2 _leaveHighlightPosition = new (180f, 280f);
        private readonly Vector2 _innKeeperPosition = new (290, 370);
        private readonly Vector2 _conversationStripPosition = new (210f, 405f);
        private readonly Vector2 _goldIconPosition = new (490, 640);

        private Vector2 _plankPosition;
        private Vector2 _backgroundPosition;
        private Vector2 _namePosition;
        private Vector2 _selectTextPosition;
        private Vector2 _backTextPosition;
        private Rectangle _screenRectangle;

        private List<string> _welcomeMessage;
        private List<string> _serviceRenderedMessage;
        private List<string> _noGoldMessage;
        private List<string> _currentDialogue;
        private const int _maxWidth = 570;
        private const int _maxLines = 3;

        private string _costString;
        private readonly string _stayString = "Stay";
        private readonly string _leaveString = "Leave";
        private readonly string _selectString = "Select";
        private readonly string _backString = "Leave";

        private int _selectionMark;
        private int _endIndex;

        /// <summary>
        /// Creates a new InnScreen object.
        /// </summary>
        public InnScreen(Inn inn) : base()
        {
            // check the parameter
            if (inn == null)
            {
                throw new ArgumentNullException("inn");
            }

            IsPopup = true;
            this.inn = inn;

            _welcomeMessage = Fonts.BreakTextIntoList(inn.WelcomeMessage, Fonts.DescriptionFont, _maxWidth);
            _serviceRenderedMessage = Fonts.BreakTextIntoList(inn.PaidMessage, Fonts.DescriptionFont, _maxWidth);
            _noGoldMessage = Fonts.BreakTextIntoList(inn.NotEnoughGoldMessage, Fonts.DescriptionFont, _maxWidth);

            _selectionMark = 1;
            ChangeDialogue(_welcomeMessage);
        }

        /// <summary>
        /// Load the graphics content
        /// </summary>
        /// <param name="batch">SpriteBatch object</param>
        /// <param name="screenWidth">Width of screen</param>
        /// <param name="screenHeight">Height of the screen</param>
        public override void LoadContent()
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            ContentManager content = ScreenManager.Game.Content;

            _backgroundTexture = content.Load<Texture2D>("Textures/GameScreens/GameScreenBkgd");
            _plankTexture = content.Load<Texture2D>("Textures/MainMenu/MainMenuPlank03");
            _selectIconTexture = content.Load<Texture2D>("Textures/Buttons/AButton");
            _backIconTexture = content.Load<Texture2D>("Textures/Buttons/BButton");
            _highlightTexture = content.Load<Texture2D>("Textures/GameScreens/HighlightLarge");
            _arrowTexture = content.Load<Texture2D>("Textures/GameScreens/SelectionArrow");
            _conversationTexture = content.Load<Texture2D>("Textures/GameScreens/ConversationStrip");
            _goldIcon = content.Load<Texture2D>("Textures/GameScreens/GoldIcon");
            _fadeTexture = content.Load<Texture2D>("Textures/GameScreens/FadeScreen");

            _screenRectangle = new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);
            _plankPosition = new Vector2((viewport.Width - _plankTexture.Width) / 2, 67f);
            _backgroundPosition = new Vector2((viewport.Width - _backgroundTexture.Width) / 2, (viewport.Height - _backgroundTexture.Height) / 2);
            _namePosition = new Vector2((viewport.Width - Fonts.HeaderFont.MeasureString(inn.Name).X) / 2, 90f);

            _selectTextPosition = _selectIconPosition;
            _selectTextPosition.X -= Fonts.ButtonNamesFont.MeasureString(_selectString).X + 10;
            _selectTextPosition.Y += 5;

            _backTextPosition = _backIconPosition;
            _backTextPosition.X += _backIconTexture.Width + 10;
            _backTextPosition.Y += 5;
        }

        /// <summary>
        /// Handle user input.
        /// </summary>
        public override void HandleInput()
        {
            // exit the screen
            if (InputManager.IsActionTriggered(InputManager.Action.Back))
            {
                ExitScreen();
                return;
            }
            // move the cursor up
            else if (InputManager.IsActionTriggered(InputManager.Action.CursorUp))
            {
                if (_selectionMark == 2)
                {
                    _selectionMark = 1;
                }
            }
            // move the cursor down
            else if (InputManager.IsActionTriggered(InputManager.Action.CursorDown))
            {
                if (_selectionMark == 1)
                {
                    _selectionMark = 2;
                }
            }
            // select an option
            else if (InputManager.IsActionTriggered(InputManager.Action.Ok))
            {
                if (_selectionMark == 1)
                {
                    int partyCharge = GetChargeForParty(Session.Party);
                    if (Session.Party.PartyGold >= partyCharge)
                    {
                        AudioManager.PlayCue("Money");
                        Session.Party.PartyGold -= partyCharge;
                        _selectionMark = 2;
                        ChangeDialogue(_serviceRenderedMessage);
                        HealParty(Session.Party);
                    }
                    else
                    {
                        _selectionMark = 2;
                        ChangeDialogue(_noGoldMessage);
                    }
                }
                else
                {
                    ExitScreen();
                    return;
                }
            }
        }

        /// <summary>
        /// Change the current dialogue.
        /// </summary>
        private void ChangeDialogue(List<string> newDialogue)
        {
            _currentDialogue = newDialogue;
            _endIndex = _maxLines;
            if (_endIndex > _currentDialogue.Count)
            {
                _endIndex = _currentDialogue.Count;
            }
        }


        /// <summary>
        /// Calculate the charge for the party's stay at the inn.
        /// </summary>
        private int GetChargeForParty(Party party)
        {
            // check the parameter 
            if (party == null)
            {
                throw new ArgumentNullException("party");
            }

            return inn.ChargePerPlayer * party.Players.Count;
        }

        /// <summary>
        /// Heal the party back to their correct values for level + gear.
        /// </summary>
        private void HealParty(Party party)
        {
            // check the parameter 
            if (party == null)
            {
                throw new ArgumentNullException("party");
            }

            // reset the statistics for each player
            foreach (Player player in party.Players)
            {
                player.StatisticsModifiers = new StatisticsValue();
            }
        }

        /// <summary>
        /// Draw the screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 dialogPosition = _informationPosition;

            spriteBatch.Begin();

            // Draw fade screen
            spriteBatch.Draw(_fadeTexture, _screenRectangle, Color.White);

            // Draw the background
            spriteBatch.Draw(_backgroundTexture, _backgroundPosition, Color.White);
            // Draw the wooden plank
            spriteBatch.Draw(_plankTexture, _plankPosition, Color.White);
            // Draw the select icon
            spriteBatch.Draw(_selectIconTexture, _selectIconPosition, Color.White);
            // Draw the back icon
            spriteBatch.Draw(_backIconTexture, _backIconPosition, Color.White);
            // Draw the inn name on the wooden plank
            spriteBatch.DrawString(Fonts.HeaderFont, inn.Name, _namePosition, Fonts.DisplayColor);

            // Draw the stay and leave option texts based on the current selection
            if (_selectionMark == 1)
            {
                spriteBatch.Draw(_highlightTexture, _stayHighlightPosition, Color.White);
                spriteBatch.Draw(_arrowTexture, _stayArrowPosition, Color.White);
                spriteBatch.DrawString(Fonts.GearInfoFont, _stayString, _stayPosition, Fonts.HighlightColor);
                spriteBatch.DrawString(Fonts.GearInfoFont, _leaveString, _leavePosition, Fonts.DisplayColor);
            }
            else
            {
                spriteBatch.Draw(_highlightTexture, _leaveHighlightPosition, Color.White);
                spriteBatch.Draw(_arrowTexture, _leaveArrowPosition, Color.White);
                spriteBatch.DrawString(Fonts.GearInfoFont, _stayString, _stayPosition, Fonts.DisplayColor);
                spriteBatch.DrawString(Fonts.GearInfoFont, _leaveString, _leavePosition, Fonts.HighlightColor);
            }
            // Draw the amount of gold
            spriteBatch.DrawString(Fonts.ButtonNamesFont, Fonts.GetGoldString(Session.Party.PartyGold), _goldStringPosition, Color.White);
            // Draw the select button text
            spriteBatch.DrawString(Fonts.ButtonNamesFont, _selectString, _selectTextPosition, Color.White);
            // Draw the back button text
            spriteBatch.DrawString(Fonts.ButtonNamesFont, _backString, _backTextPosition, Color.White);

            // Draw Conversation Strip
            spriteBatch.Draw(_conversationTexture, _conversationStripPosition, Color.White);

            // Draw Shop Keeper
            spriteBatch.Draw(inn.ShopkeeperTexture, _innKeeperPosition, Color.White);
            // Draw the cost to stay
            _costString = "Cost: " + GetChargeForParty(Session.Party) + " Gold";
            spriteBatch.DrawString(Fonts.DescriptionFont, _costString, _costPosition, Color.DarkRed);
            // Draw the innkeeper dialog
            for (int i = 0; i < _endIndex; i++)
            {
                spriteBatch.DrawString(Fonts.DescriptionFont, _currentDialogue[i], dialogPosition, Color.Black);
                dialogPosition.Y += Fonts.DescriptionFont.LineSpacing;
            }

            // Draw Gold Icon
            spriteBatch.Draw(_goldIcon, _goldIconPosition, Color.White);

            spriteBatch.End();
        }
    }
}
