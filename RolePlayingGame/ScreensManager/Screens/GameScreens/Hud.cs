using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Characters;
using RolePlayingGame.Combat;
using RolePlayingGame.Combat.Actions;
using RolePlayingGame.GearObjects;
using RolePlayingGame.InputsManager;
using RolePlayingGame.SessionObjects;
using RolePlayingGame.Spells;
using RolePlayingGame.TextFonts;
using System;

namespace RolePlayingGame.ScreensManager.Screens.GameScreens
{
    public class Hud
    {
        private ScreenManager _screenManager;

        public const int HudHeight = 183;

        private Texture2D _backgroundHudTexture;
        private Texture2D _topHudTexture;
        private Texture2D _combatPopupTexture;
        private Texture2D _activeCharInfoTexture;
        private Texture2D _inActiveCharInfoTexture;
        private Texture2D _cantUseCharInfoTexture;
        private Texture2D _selectionBracketTexture;
        private Texture2D _menuTexture;
        private Texture2D _statsTexture;
        private Texture2D _deadPortraitTexture;
        private Texture2D _charSelFadeLeftTexture;
        private Texture2D _charSelFadeRightTexture;
        private Texture2D _charSelArrowLeftTexture;
        private Texture2D _charSelArrowRightTexture;
        private Texture2D _actionTexture;
        private Texture2D _yButtonTexture;
        private Texture2D _startButtonTexture;

        private Vector2 _topHudPosition = new(353f, 30f);
        private Vector2 _charSelLeftPosition = new(70f, 600f);
        private Vector2 _charSelRightPosition = new(1170f, 600f);
        private Vector2 _yButtonPosition = new(0f, 560f + 20f);
        private Vector2 _startButtonPosition = new(0f, 560f + 35f);
        private Vector2 _yTextPosition = new(0f, 560f + 70f);
        private Vector2 _startTextPosition = new(0f, 560f + 70f);
        private Vector2 _actionTextPosition = new(640f, 55f);
        private Vector2 _backgroundHudPosition = new(0f, 525f);
        private Vector2 _portraitPosition = new(640f, 55f);
        private Vector2 _startingInfoPosition = new(0f, 550f);
        private Vector2 _namePosition;
        private Vector2 _levelPosition;
        private Vector2 _detailPosition;

        private readonly Color _activeNameColor = new(200, 200, 200);
        private readonly Color _inActiveNameColor = new(100, 100, 100);
        private readonly Color _nonSelColor = new(86, 26, 5);
        private readonly Color _selColor = new(229, 206, 144);

        /// <summary>
        /// The text that is shown in the action bar at the top of the combat screen.
        /// </summary>
        private string _actionText = string.Empty;

        /// <summary>
        /// The text that is shown in the action bar at the top of the combat screen.
        /// </summary>
        public string ActionText
        {
            get => _actionText;
            set => _actionText = value;
        }

        /// <summary>
        /// Creates a new Hud object using the given ScreenManager.
        /// </summary>
        public Hud(ScreenManager screenManager)
        {
            // check the parameter
            if (screenManager == null)
            {
                throw new ArgumentNullException("screenManager");
            }
            _screenManager = screenManager;
        }

        /// <summary>
        /// Load the graphics content from the content manager.
        /// </summary>
        public void LoadContent()
        {
            ContentManager content = _screenManager.Game.Content;

            _backgroundHudTexture = content.Load<Texture2D>("Textures/HUD/HudBkgd");
            _topHudTexture = content.Load<Texture2D>("Textures/HUD/CombatStateInfoStrip");
            _activeCharInfoTexture = content.Load<Texture2D>("Textures/HUD/PlankActive");
            _inActiveCharInfoTexture = content.Load<Texture2D>("Textures/HUD/PlankInActive");
            _cantUseCharInfoTexture = content.Load<Texture2D>("Textures/HUD/PlankCantUse");
            _selectionBracketTexture = content.Load<Texture2D>("Textures/HUD/SelectionBrackets");
            _deadPortraitTexture = content.Load<Texture2D>("Textures/Characters/Portraits/Tombstone");
            _combatPopupTexture = content.Load<Texture2D>("Textures/HUD/CombatPopup");
            _charSelFadeLeftTexture = content.Load<Texture2D>("Textures/Buttons/CharSelectFadeLeft");
            _charSelFadeRightTexture = content.Load<Texture2D>("Textures/Buttons/CharSelectFadeRight");
            _charSelArrowLeftTexture = content.Load<Texture2D>("Textures/Buttons/CharSelectHlLeft");
            _charSelArrowRightTexture = content.Load<Texture2D>("Textures/Buttons/CharSelectHlRight");
            _actionTexture = content.Load<Texture2D>("Textures/HUD/HudSelectButton");
            _yButtonTexture = content.Load<Texture2D>("Textures/Buttons/YButton");
            _startButtonTexture = content.Load<Texture2D>("Textures/Buttons/StartButton");
            _menuTexture = content.Load<Texture2D>("Textures/HUD/Menu");
            _statsTexture = content.Load<Texture2D>("Textures/HUD/Stats");
        }

        /// <summary>
        /// Draw the screen.
        /// </summary>
        public void Draw()
        {
            SpriteBatch spriteBatch = _screenManager.SpriteBatch;
            spriteBatch.Begin();

            _startingInfoPosition.X = 640f;
            _startingInfoPosition.X -= Session.Party.Players.Count / 2 * 200f;

            if (Session.Party.Players.Count % 2 != 0)
            {
                _startingInfoPosition.X -= 100f;
            }

            spriteBatch.Draw(_backgroundHudTexture, _backgroundHudPosition, Color.White);

            if (CombatEngine.IsActive)
            {
                DrawForCombat();
            }
            else
            {
                DrawForNonCombat();
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Draws HUD for Combat Mode
        /// </summary>
        private void DrawForCombat()
        {
            SpriteBatch spriteBatch = _screenManager.SpriteBatch;
            Vector2 position = _startingInfoPosition;

            foreach (CombatantPlayer combatantPlayer in CombatEngine.Players)
            {
                DrawCombatPlayerDetails(combatantPlayer, position);
                position.X += _activeCharInfoTexture.Width - 6f;
            }

            _charSelLeftPosition.X = _startingInfoPosition.X - 5f - _charSelArrowLeftTexture.Width;
            _charSelRightPosition.X = position.X + 5f;
            // Draw character Selection Arrows
            if (CombatEngine.IsPlayersTurn)
            {
                spriteBatch.Draw(_charSelArrowLeftTexture, _charSelLeftPosition, Color.White);
                spriteBatch.Draw(_charSelArrowRightTexture, _charSelRightPosition, Color.White);
            }
            else
            {
                spriteBatch.Draw(_charSelFadeLeftTexture, _charSelLeftPosition, Color.White);
                spriteBatch.Draw(_charSelFadeRightTexture, _charSelRightPosition, Color.White);
            }

            if (_actionText.Length > 0)
            {
                spriteBatch.Draw(_topHudTexture, _topHudPosition, Color.White);
                // Draw Action Text
                Fonts.DrawCenteredText(spriteBatch, Fonts.PlayerStatisticsFont, _actionText, _actionTextPosition, Color.Black);
            }
        }

        /// <summary>
        /// Draws HUD for non Combat Mode
        /// </summary>
        private void DrawForNonCombat()
        {
            SpriteBatch spriteBatch = _screenManager.SpriteBatch;
            Vector2 position = _startingInfoPosition;

            foreach (Player player in Session.Party.Players)
            {
                DrawNonCombatPlayerDetails(player, position);
                position.X += _inActiveCharInfoTexture.Width - 6f;
            }

            _yTextPosition.X = position.X + 5f;
            _yButtonPosition.X = position.X + 9f;

            // Draw Select Button
            spriteBatch.Draw(_statsTexture, _yTextPosition, Color.White);
            spriteBatch.Draw(_yButtonTexture, _yButtonPosition, Color.White);

            _startTextPosition.X = _startingInfoPosition.X - _startButtonTexture.Width - 25f;
            _startButtonPosition.X = _startingInfoPosition.X - _startButtonTexture.Width - 10f;

            // Draw Back Button
            spriteBatch.Draw(_menuTexture, _startTextPosition, Color.White);
            spriteBatch.Draw(_startButtonTexture, _startButtonPosition, Color.White);
        }

        enum PlankState
        {
            Active,
            InActive,
            CantUse,
        }

        /// <summary>
        /// Draws Player Details
        /// </summary>
        /// <param name="playerIndex">Index of player details to draw</param>
        /// <param name="position">Position where to draw</param>
        private void DrawCombatPlayerDetails(CombatantPlayer player, Vector2 position)
        {
            SpriteBatch spriteBatch = _screenManager.SpriteBatch;

            PlankState plankState;
            bool isPortraitActive = false;
            bool isCharDead = false;
            Color color;

            _portraitPosition.X = position.X + 7f;
            _portraitPosition.Y = position.Y + 7f;

            _namePosition.X = position.X + 84f;
            _namePosition.Y = position.Y + 12f;

            _levelPosition.X = position.X + 84f;
            _levelPosition.Y = position.Y + 39f;

            _detailPosition.X = position.X + 25f;
            _detailPosition.Y = position.Y + 66f;

            position.X -= 2;
            position.Y -= 4;

            if (player.IsTurnTaken)
            {
                plankState = PlankState.CantUse;
                isPortraitActive = false;
            }
            else
            {
                plankState = PlankState.InActive;
                isPortraitActive = true;
            }

            if (((CombatEngine.HighlightedCombatant == player) && !player.IsTurnTaken) || (CombatEngine.PrimaryTargetedCombatant == player) || (CombatEngine.SecondaryTargetedCombatants.Contains(player)))
            {
                plankState = PlankState.Active;
            }

            if (player.IsDeadOrDying)
            {
                isCharDead = true;
                isPortraitActive = false;
                plankState = PlankState.CantUse;
            }

            // Draw Info Slab
            if (plankState == PlankState.Active)
            {
                color = _activeNameColor;
                spriteBatch.Draw(_activeCharInfoTexture, position, Color.White);

                // Draw Brackets
                if ((CombatEngine.HighlightedCombatant == player) && !player.IsTurnTaken)
                {
                    spriteBatch.Draw(_selectionBracketTexture, position, Color.White);
                }

                if (isPortraitActive && (CombatEngine.HighlightedCombatant == player) && (CombatEngine.HighlightedCombatant.CombatAction == null) && !CombatEngine.IsDelaying)
                {
                    position.X += _activeCharInfoTexture.Width / 2;
                    position.X -= _combatPopupTexture.Width / 2;
                    position.Y -= _combatPopupTexture.Height;
                    // Draw Action
                    DrawActionsMenu(position);
                }
            }
            else if (plankState == PlankState.InActive)
            {
                color = _inActiveNameColor;
                spriteBatch.Draw(_inActiveCharInfoTexture, position, Color.White);
            }
            else
            {
                color = Color.Black;
                spriteBatch.Draw(_cantUseCharInfoTexture, position, Color.White);
            }

            if (isCharDead)
            {
                spriteBatch.Draw(_deadPortraitTexture, _portraitPosition, Color.White);
            }
            else
            {
                // Draw Player Portrait
                DrawPortrait(player.Player, _portraitPosition, plankState);
            }

            // Draw Player Name
            spriteBatch.DrawString(Fonts.PlayerStatisticsFont, player.Player.Name, _namePosition, color);
            color = Color.Black;
            // Draw Player Details
            spriteBatch.DrawString(Fonts.HudDetailFont, "Lvl: " + player.Player.CharacterLevel, _levelPosition, color);
            spriteBatch.DrawString(Fonts.HudDetailFont, "HP: " + player.Statistics.HealthPoints + "/" + player.Player.CharacterStatistics.HealthPoints, _detailPosition, color);

            _detailPosition.Y += 30f;
            spriteBatch.DrawString(Fonts.HudDetailFont, "MP: " + player.Statistics.MagicPoints + "/" + player.Player.CharacterStatistics.MagicPoints, _detailPosition, color);
        }

        /// <summary>
        /// Draws Player Details
        /// </summary>
        /// <param name="playerIndex">Index of player details to draw</param>
        /// <param name="position">Position where to draw</param>
        private void DrawNonCombatPlayerDetails(Player player, Vector2 position)
        {
            SpriteBatch spriteBatch = _screenManager.SpriteBatch;

            PlankState plankState;
            bool isCharDead = false;
            Color color;

            _portraitPosition.X = position.X + 7f;
            _portraitPosition.Y = position.Y + 7f;

            _namePosition.X = position.X + 84f;
            _namePosition.Y = position.Y + 12f;

            _levelPosition.X = position.X + 84f;
            _levelPosition.Y = position.Y + 39f;

            _detailPosition.X = position.X + 25f;
            _detailPosition.Y = position.Y + 66f;

            position.X -= 2;
            position.Y -= 4;

            plankState = PlankState.Active;

            // Draw Info Slab
            if (plankState == PlankState.Active)
            {
                color = _activeNameColor;
                spriteBatch.Draw(_activeCharInfoTexture, position, Color.White);
            }
            else if (plankState == PlankState.InActive)
            {
                color = _inActiveNameColor;
                spriteBatch.Draw(_inActiveCharInfoTexture, position, Color.White);
            }
            else
            {
                color = Color.Black;
                spriteBatch.Draw(_cantUseCharInfoTexture, position, Color.White);
            }

            if (isCharDead)
            {
                spriteBatch.Draw(_deadPortraitTexture, _portraitPosition, Color.White);
            }
            else
            {
                // Draw Player Portrait
                DrawPortrait(player, _portraitPosition, plankState);
            }

            // Draw Player Name
            spriteBatch.DrawString(Fonts.PlayerStatisticsFont, player.Name, _namePosition, color);

            color = Color.Black;
            // Draw Player Details
            spriteBatch.DrawString(Fonts.HudDetailFont, "Lvl: " + player.CharacterLevel, _levelPosition, color);
            spriteBatch.DrawString(Fonts.HudDetailFont, "HP: " + player.CurrentStatistics.HealthPoints + "/" + player.CharacterStatistics.HealthPoints, _detailPosition, color);

            _detailPosition.Y += 30f;
            spriteBatch.DrawString(Fonts.HudDetailFont, "MP: " + player.CurrentStatistics.MagicPoints + "/" + player.CharacterStatistics.MagicPoints, _detailPosition, color);
        }

        /// <summary>
        /// Draw the portrait of the given player at the given position.
        /// </summary>
        private void DrawPortrait(Player player, Vector2 position, PlankState plankState)
        {
            switch (plankState)
            {
                case PlankState.Active:
                    _screenManager.SpriteBatch.Draw(player.ActivePortraitTexture, position, Color.White);
                    break;
                case PlankState.InActive:
                    _screenManager.SpriteBatch.Draw(player.InactivePortraitTexture, position, Color.White);
                    break;
                case PlankState.CantUse:
                    _screenManager.SpriteBatch.Draw(player.UnselectablePortraitTexture, position, Color.White);
                    break;
            }
        }

        /// <summary>
        /// The list of entries in the combat action menu.
        /// </summary>
        private string[] _actionList = new string[5]
        {
            "Attack",
            "Spell",
            "Item",
            "Defend",
            "Flee",
        };

        /// <summary>
        /// The currently highlighted item.
        /// </summary>
        private int _highlightedAction = 0;

        /// <summary>
        /// Handle user input to the actions menu.
        /// </summary>
        public void UpdateActionsMenu()
        {
            // cursor up
            if (InputManager.IsActionTriggered(InputManager.Action.CursorUp))
            {
                if (_highlightedAction > 0)
                {
                    _highlightedAction--;
                }
                return;
            }
            // cursor down
            if (InputManager.IsActionTriggered(InputManager.Action.CursorDown))
            {
                if (_highlightedAction < _actionList.Length - 1)
                {
                    _highlightedAction++;
                }
                return;
            }
            // select an action
            if (InputManager.IsActionTriggered(InputManager.Action.Ok))
            {
                switch (_actionList[_highlightedAction])
                {
                    case "Attack":
                        {
                            ActionText = "Performing a Melee Attack";
                            CombatEngine.HighlightedCombatant.CombatAction = new MeleeCombatAction(CombatEngine.HighlightedCombatant)
                            {
                                Target = CombatEngine.FirstEnemyTarget
                            };
                        }
                        break;

                    case "Spell":
                        {
                            SpellbookScreen spellbookScreen = new(CombatEngine.HighlightedCombatant.Character, CombatEngine.HighlightedCombatant.Statistics);
                            spellbookScreen.SpellSelected += new SpellbookScreen.SpellSelectedHandler(spellbookScreen_SpellSelected);
                            Session.ScreenManager.AddScreen(spellbookScreen);
                        }
                        break;

                    case "Item":
                        {
                            InventoryScreen inventoryScreen = new(true);
                            inventoryScreen.GearSelected += new InventoryScreen.GearSelectedHandler(inventoryScreen_GearSelected);
                            Session.ScreenManager.AddScreen(inventoryScreen);
                        }
                        break;

                    case "Defend":
                        {
                            ActionText = "Defending";
                            CombatEngine.HighlightedCombatant.CombatAction = new DefendCombatAction(CombatEngine.HighlightedCombatant);
                            CombatEngine.HighlightedCombatant.CombatAction.Start();
                        }
                        break;

                    case "Flee":
                        CombatEngine.AttemptFlee();
                        break;
                }
                return;
            }
        }

        /// <summary>
        /// Recieves the spell from the Spellbook screen and casts it.
        /// </summary>
        void spellbookScreen_SpellSelected(Spell spell)
        {
            if (spell != null)
            {
                ActionText = "Casting " + spell.Name;
                CombatEngine.HighlightedCombatant.CombatAction = new SpellCombatAction(CombatEngine.HighlightedCombatant, spell);
                if (spell.IsOffensive)
                {
                    CombatEngine.HighlightedCombatant.CombatAction.Target = CombatEngine.FirstEnemyTarget;
                }
                else
                {
                    CombatEngine.HighlightedCombatant.CombatAction.Target = CombatEngine.HighlightedCombatant;
                }
            }
        }

        /// <summary>
        /// Receives the item back from the Inventory screen and uses it.
        /// </summary>
        void inventoryScreen_GearSelected(Gear gear)
        {
            Item item = gear as Item;
            if (item != null)
            {
                ActionText = "Using " + item.Name;
                CombatEngine.HighlightedCombatant.CombatAction = new ItemCombatAction(CombatEngine.HighlightedCombatant, item);
                if (item.IsOffensive)
                {
                    CombatEngine.HighlightedCombatant.CombatAction.Target = CombatEngine.FirstEnemyTarget;
                }
                else
                {
                    CombatEngine.HighlightedCombatant.CombatAction.Target = CombatEngine.HighlightedCombatant;
                }
            }
        }

        /// <summary>
        /// Draws the combat action menu.
        /// </summary>
        /// <param name="position">The position of the menu.</param>
        private void DrawActionsMenu(Vector2 position)
        {
            ActionText = "Choose an Action";

            SpriteBatch spriteBatch = _screenManager.SpriteBatch;

            Vector2 arrowPosition;
            float height = 25f;

            spriteBatch.Draw(_combatPopupTexture, position, Color.White);

            position.Y += 21f;
            arrowPosition = position;

            arrowPosition.X += 10f;
            arrowPosition.Y += 2f;
            arrowPosition.Y += height * _highlightedAction;
            spriteBatch.Draw(_actionTexture, arrowPosition, Color.White);

            position.Y += 4f;
            position.X += 50f;

            // Draw Action Text
            for (int i = 0; i < _actionList.Length; i++)
            {
                spriteBatch.DrawString(Fonts.GearInfoFont, _actionList[i], position, i == _highlightedAction ? _selColor : _nonSelColor);
                position.Y += height;
            }
        }
    }
}
