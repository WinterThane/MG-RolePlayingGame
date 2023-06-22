using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Audio;
using RolePlayingGame.Characters;
using RolePlayingGame.InputsManager;
using RolePlayingGame.Spells;
using RolePlayingGame.TextFonts;
using System;
using System.Collections.Generic;

namespace RolePlayingGame.ScreensManager.Screens.GameScreens
{
    public class LevelUpScreen : GameScreen
    {
        private int _index;
        private List<Player> _leveledUpPlayers;
        private List<Spell> _spellList = new();

        private Texture2D _backTexture;
        private Texture2D _selectIconTexture;
        private Texture2D _portraitBackTexture;
        private Texture2D _headerTexture;
        private Texture2D _lineTexture;
        private Texture2D _scrollUpTexture;
        private Texture2D _scrollDownTexture;
        private Texture2D _fadeTexture;
        private Color _color;
        private Color _colorName = new(241, 173, 10);
        private Color _colorClass = new(207, 130, 42);
        private Color _colorText = new(76, 49, 8);

        private Vector2 _backgroundPosition;
        private Vector2 _textPosition;
        private Vector2 _levelPosition;
        private Vector2 _iconPosition;
        private Vector2 _linePosition;
        private Vector2 _selectPosition;
        private Vector2 _selectIconPosition;
        private Vector2 _screenSize;
        private Vector2 _titlePosition;
        private Vector2 _scrollUpPosition;
        private Vector2 _scrollDownPosition;
        private Vector2 _spellUpgradePosition;
        private Vector2 _portraitPosition;
        private Vector2 _playerNamePosition;
        private Vector2 _playerLvlPosition;
        private Vector2 _playerClassPosition;
        private Vector2 _topLinePosition;
        private Vector2 _playerDamagePosition;
        private Vector2 _headerPosition;
        private Vector2 _backPosition;
        private Rectangle _fadeDest;

        private readonly string _titleText = "Level Up";
        private readonly string _selectString = "Continue";

        private int _startIndex;
        private int _endIndex;
        private const int _maxLines = 3;
        private const int _lineSpacing = 74;

        /// <summary>
        /// Constructs a new LevelUpScreen object.
        /// </summary>
        /// <param name="leveledUpPlayers"></param>
        public LevelUpScreen(List<Player> leveledUpPlayers)
        {
            if ((leveledUpPlayers == null) || (leveledUpPlayers.Count <= 0))
            {
                throw new ArgumentNullException("leveledUpPlayers");
            }

            IsPopup = true;
            _leveledUpPlayers = leveledUpPlayers;

            _index = 0;

            GetSpellList();

            //AudioManager.PushMusic("LevelUp");
            Exiting += new EventHandler(LevelUpScreen_Exiting);
        }

        void LevelUpScreen_Exiting(object sender, EventArgs e)
        {
            //AudioManager.PopMusic();
        }

        /// <summary>
        /// Load the graphics content
        /// </summary>
        /// <param name="sprite">SpriteBatch</param>
        /// <param name="screenWidth">Width of the screen</param>
        /// <param name="screenHeight">Height of the screen</param>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            _backTexture = content.Load<Texture2D>("Textures/GameScreens/PopupScreen");
            _selectIconTexture = content.Load<Texture2D>("Textures/Buttons/AButton");
            _portraitBackTexture = content.Load<Texture2D>("Textures/GameScreens/PlayerSelected");
            _headerTexture = content.Load<Texture2D>("Textures/GameScreens/Caption");
            _lineTexture = content.Load<Texture2D>("Textures/GameScreens/SeparationLine");
            _scrollUpTexture = content.Load<Texture2D>("Textures/GameScreens/ScrollUp");
            _scrollDownTexture = content.Load<Texture2D>("Textures/GameScreens/ScrollDown");
            _fadeTexture = content.Load<Texture2D>("Textures/GameScreens/FadeScreen");

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            _backgroundPosition.X = (viewport.Width - _backTexture.Width) / 2;
            _backgroundPosition.Y = (viewport.Height - _backTexture.Height) / 2;

            _screenSize = new Vector2(viewport.Width, viewport.Height);
            _fadeDest = new Rectangle(0, 0, viewport.Width, viewport.Height);

            _titlePosition.X = (_screenSize.X - Fonts.HeaderFont.MeasureString(_titleText).X) / 2;
            _titlePosition.Y = _backgroundPosition.Y + _lineSpacing;

            _selectIconPosition.X = _screenSize.X / 2 + 260;
            _selectIconPosition.Y = _backgroundPosition.Y + 530f;
            _selectPosition.X = _selectIconPosition.X - Fonts.ButtonNamesFont.MeasureString(_selectString).X - 10f;
            _selectPosition.Y = _selectIconPosition.Y;

            _portraitPosition = _backgroundPosition + new Vector2(143f, 155f);
            _backPosition = _backgroundPosition + new Vector2(140f, 135f);

            _playerNamePosition = _backgroundPosition + new Vector2(230f, 160f);
            _playerClassPosition = _backgroundPosition + new Vector2(230f, 185f);
            _playerLvlPosition = _backgroundPosition + new Vector2(230f, 205f);

            _topLinePosition = _backgroundPosition + new Vector2(380f, 160f);
            _textPosition = _backgroundPosition + new Vector2(335f, 320f);
            _levelPosition = _backgroundPosition + new Vector2(540f, 320f);
            _iconPosition = _backgroundPosition + new Vector2(155f, 303f);
            _linePosition = _backgroundPosition + new Vector2(142f, 285f);

            _scrollUpPosition = _backgroundPosition + new Vector2(810f, 300f);
            _scrollDownPosition = _backgroundPosition + new Vector2(810f, 480f);

            _playerDamagePosition = _backgroundPosition + new Vector2(560f, 160f);
            _spellUpgradePosition = _backgroundPosition + new Vector2(380f, 265f);

            _headerPosition = _backgroundPosition + new Vector2(120f, 248f);
        }

        /// <summary>
        /// Handles user input.
        /// </summary>
        public override void HandleInput()
        {
            // exit without bothering to see the rest
            if (InputManager.IsActionTriggered(InputManager.Action.Back))
            {
                ExitScreen();
            }
            // advance to the next player to have leveled up
            else if (InputManager.IsActionTriggered(InputManager.Action.Ok))
            {
                if (_leveledUpPlayers.Count <= 0)
                {
                    // no players at all
                    ExitScreen();
                    return;
                }
                if (_index < _leveledUpPlayers.Count - 1)
                {
                    // move to the next player
                    _index++;
                    GetSpellList();
                }
                else
                {
                    // no more players
                    ExitScreen();
                    return;
                }
            }
            // Scroll up
            else if (InputManager.IsActionTriggered(InputManager.Action.CursorUp))
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
                if (_startIndex < _spellList.Count - _maxLines)
                {
                    _endIndex++;
                    _startIndex++;
                }
            }
        }

        /// <summary>
        /// Get the spell list
        /// </summary>
        private void GetSpellList()
        {
            _spellList.Clear();

            if ((_leveledUpPlayers.Count > 0) && (_leveledUpPlayers[_index].CharacterLevel <= _leveledUpPlayers[_index].CharacterClass.LevelEntries.Count))
            {
                List<Spell> newSpells = _leveledUpPlayers[_index].CharacterClass.LevelEntries[_leveledUpPlayers[_index].CharacterLevel - 1].Spells;
                if ((newSpells == null) || (newSpells.Count <= 0))
                {
                    _startIndex = 0;
                    _endIndex = 0;
                }
                else
                {
                    _spellList.AddRange(_leveledUpPlayers[_index].Spells);
                    _spellList.RemoveAll(delegate (Spell spell)
                    {
                        return !newSpells.Exists(delegate (Spell newSpell)
                        {
                            return spell.AssetName == newSpell.AssetName;
                        });
                    });
                    _startIndex = 0;
                    _endIndex = Math.Min(_maxLines, _spellList.Count);
                }
            }
            else
            {
                _startIndex = 0;
                _endIndex = 0;
            }
        }

        /// <summary>
        /// Draw the screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            Vector2 currentTextPosition = _textPosition;
            Vector2 currentIconPosition = _iconPosition;
            Vector2 currentLinePosition = _linePosition;
            Vector2 currentLevelPosition = _levelPosition;

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // Draw the fading screen
            spriteBatch.Draw(_fadeTexture, _fadeDest, Color.White);

            // Draw the popup background
            spriteBatch.Draw(_backTexture, _backgroundPosition, Color.White);

            // Draw the title
            spriteBatch.DrawString(Fonts.HeaderFont, _titleText, _titlePosition, Fonts.TitleColor);

            DrawPlayerStats();

            // Draw the spell upgrades caption
            spriteBatch.Draw(_headerTexture, _headerPosition, Color.White);
            spriteBatch.DrawString(Fonts.PlayerNameFont, "Spell Upgrades", _spellUpgradePosition, _colorClass);

            // Draw the horizontal separating lines
            for (int i = 0; i <= _maxLines - 1; i++)
            {
                currentLinePosition.Y += _lineSpacing;
                spriteBatch.Draw(_lineTexture, currentLinePosition, Color.White);
            }

            // Draw the spell upgrade details
            for (int i = _startIndex; i < _endIndex; i++)
            {
                // Draw the spell icon
                spriteBatch.Draw(_spellList[i].IconTexture, currentIconPosition, Color.White);

                // Draw the spell name
                spriteBatch.DrawString(Fonts.GearInfoFont, _spellList[i].Name, currentTextPosition, Fonts.CountColor);

                // Draw the spell level
                spriteBatch.DrawString(Fonts.GearInfoFont, "Spell Level " + _spellList[i].Level.ToString(), currentLevelPosition, Fonts.CountColor);

                // Increment to next line position
                currentTextPosition.Y += _lineSpacing;
                currentLevelPosition.Y += _lineSpacing;
                currentIconPosition.Y += _lineSpacing;
            }

            // Draw the scroll bars
            spriteBatch.Draw(_scrollUpTexture, _scrollUpPosition, Color.White);
            spriteBatch.Draw(_scrollDownTexture, _scrollDownPosition, Color.White);

            // Draw the select button and its corresponding text
            spriteBatch.DrawString(Fonts.ButtonNamesFont, _selectString, _selectPosition, Color.White);
            spriteBatch.Draw(_selectIconTexture, _selectIconPosition, Color.White);

            spriteBatch.End();
        }


        /// <summary>
        /// Draw the player stats here
        /// </summary>
        private void DrawPlayerStats()
        {
            Vector2 position = _topLinePosition;
            Vector2 posDamage = _playerDamagePosition;
            Player player = _leveledUpPlayers[_index];
            int level = player.CharacterLevel;
            CharacterLevelingStatistics levelingStatistics = player.CharacterClass.LevelingStatistics;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Draw the portrait            
            spriteBatch.Draw(_portraitBackTexture, _backPosition, Color.White);

            spriteBatch.Draw(player.ActivePortraitTexture, _portraitPosition, Color.White);

            // Print the character name
            spriteBatch.DrawString(Fonts.PlayerNameFont, player.Name, _playerNamePosition, _colorName);

            // Draw the Class Name
            spriteBatch.DrawString(Fonts.PlayerNameFont, player.CharacterClass.Name, _playerClassPosition, _colorClass);

            // Draw the character level
            spriteBatch.DrawString(Fonts.PlayerNameFont, "LEVEL: " + level.ToString(), _playerLvlPosition, Color.Gray);

            // Draw the character Health Points
            SetColor(levelingStatistics.LevelsPerHealthPointsIncrease == 0 ? 0 : (level % levelingStatistics.LevelsPerHealthPointsIncrease) * levelingStatistics.HealthPointsIncrease);
            spriteBatch.DrawString(Fonts.PlayerStatisticsFont, "HP: " + player.CurrentStatistics.HealthPoints + "/" + player.CharacterStatistics.HealthPoints, position, _color);

            // Draw the character Mana Points
            position.Y += Fonts.GearInfoFont.LineSpacing;
            SetColor(levelingStatistics.LevelsPerMagicPointsIncrease == 0 ? 0 : (level % levelingStatistics.LevelsPerMagicPointsIncrease) * levelingStatistics.MagicPointsIncrease);
            spriteBatch.DrawString(Fonts.PlayerStatisticsFont, "MP: " + player.CurrentStatistics.MagicPoints + "/" + player.CharacterStatistics.MagicPoints, position, _color);

            // Draw the physical offense
            SetColor(levelingStatistics.LevelsPerPhysicalOffenseIncrease == 0 ? 0 : (level % levelingStatistics.LevelsPerPhysicalOffenseIncrease) * levelingStatistics.PhysicalOffenseIncrease);
            spriteBatch.DrawString(Fonts.PlayerStatisticsFont, "PO: " + player.CurrentStatistics.PhysicalOffense, posDamage, _color);

            // Draw the physical defense
            posDamage.Y += Fonts.PlayerStatisticsFont.LineSpacing;
            SetColor(levelingStatistics.LevelsPerPhysicalDefenseIncrease == 0 ? 0 : (level % levelingStatistics.LevelsPerPhysicalDefenseIncrease) * levelingStatistics.PhysicalDefenseIncrease);
            spriteBatch.DrawString(Fonts.PlayerStatisticsFont, "PD: " + player.CurrentStatistics.PhysicalDefense, posDamage, _color);

            // Draw the Magic offense
            posDamage.Y += Fonts.PlayerStatisticsFont.LineSpacing;
            SetColor(levelingStatistics.LevelsPerMagicalOffenseIncrease == 0 ? 0 : (level % levelingStatistics.LevelsPerMagicalOffenseIncrease) * levelingStatistics.MagicalOffenseIncrease);
            spriteBatch.DrawString(Fonts.PlayerStatisticsFont, "MO: " + player.CurrentStatistics.MagicalOffense, posDamage, _color);

            // Draw the Magical defense
            posDamage.Y += Fonts.PlayerStatisticsFont.LineSpacing;
            SetColor(levelingStatistics.LevelsPerMagicalDefenseIncrease == 0 ? 0 : (level % levelingStatistics.LevelsPerMagicalDefenseIncrease) * levelingStatistics.MagicalDefenseIncrease);
            spriteBatch.DrawString(Fonts.PlayerStatisticsFont, "MD: " + player.CurrentStatistics.MagicalDefense, posDamage, _color);
        }

        /// <summary>
        /// Set the current color based on whether the value has changed.
        /// </summary>
        /// <param name="change">State of levelled up values</param>
        public void SetColor(int value)
        {
            if (value > 0)
            {
                _color = Color.Green;
            }
            else if (value < 0)
            {
                _color = Color.Red;
            }
            else
            {
                _color = _colorText;
            }
        }
    }
}
