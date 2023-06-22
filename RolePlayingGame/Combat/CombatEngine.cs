using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Animations;
using RolePlayingGame.Audio;
using RolePlayingGame.Characters;
using RolePlayingGame.Data;
using RolePlayingGame.Engine;
using RolePlayingGame.GearObjects;
using RolePlayingGame.MapObjects;
using RolePlayingGame.ScreensManager.Screens.GameScreens;
using RolePlayingGame.ScreensManager;
using RolePlayingGame.SessionObjects;
using RolePlayingGame.TextFonts;
using System;
using System.Collections.Generic;
using System.IO;
using RolePlayingGame.InputsManager;
using Microsoft.Xna.Framework.Input;
using RolePlayingGame.Combat.Actions;

namespace RolePlayingGame.Combat
{
    public class CombatEngine
    {
        /// <summary>
        /// The singleton of the combat engine.
        /// </summary>
        private static CombatEngine _singleton = null;

        /// <summary>
        /// Check to see if there is a combat going on, and throw an exception if not.
        /// </summary>
        private static void CheckSingleton()
        {
            if (_singleton == null)
            {
                throw new InvalidOperationException(
                    "There is no active combat at this time.");
            }
        }

        /// <summary>
        /// If true, the combat engine is active and the user is in combat.
        /// </summary>
        public static bool IsActive => _singleton != null;

        /// <summary>
        /// If true, it is currently the players' turn.
        /// </summary>
        private bool _isPlayersTurn;

        /// <summary>
        /// If true, it is currently the players' turn.
        /// </summary>
        public static bool IsPlayersTurn
        {
            get
            {
                CheckSingleton();
                return _singleton._isPlayersTurn;
            }
        }

        /// <summary>
        /// The fixed combat used to generate this fight, if any.
        /// </summary>
        /// <remarks>
        /// Used for rewards.  Null means it was a random fight with no special rewards.
        /// </remarks>
        private MapEntry<FixedCombat> _fixedCombatEntry;

        /// <summary>
        /// The fixed combat used to generate this fight, if any.
        /// </summary>
        /// <remarks>
        /// Used for rewards.  Null means it was a random fight with no special rewards.
        /// </remarks>
        public static MapEntry<FixedCombat> FixedCombatEntry => _singleton?._fixedCombatEntry;

        /// <summary>
        /// The players involved in the current combat.
        /// </summary>
        private List<CombatantPlayer> _playersList = null;

        /// <summary>
        /// The players involved in the current combat.
        /// </summary>
        public static List<CombatantPlayer> Players
        {
            get
            {
                CheckSingleton();
                return _singleton._playersList;
            }
        }

        private int _highlightedPlayer;

        /// <summary>
        /// The positions of the players on screen.
        /// </summary>
        private static readonly Vector2[] PlayerPositions = new Vector2[5]
        {
            new Vector2(850f, 345f),
            new Vector2(980f, 260f),
            new Vector2(940f, 440f),
            new Vector2(1100f, 200f),
            new Vector2(1100f, 490f)
        };

        /// <summary>
        /// Start the given player's combat turn.
        /// </summary>
        private void BeginPlayerTurn(CombatantPlayer player)
        {
            // check the parameter
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            // set the highlight sprite
            _highlightedCombatant = player;
            _primaryTargetedCombatant = null;
            _secondaryTargetedCombatants.Clear();

            Session.Hud.ActionText = "Choose an Action";
        }

        /// <summary>
        /// Begin the players' turn in this combat round.
        /// </summary>
        private void BeginPlayersTurn()
        {
            // set the player-turn
            _isPlayersTurn = true;

            // reset each player for the next combat turn
            foreach (CombatantPlayer player in _playersList)
            {
                // reset the animation of living players
                if (!player.IsDeadOrDying)
                {
                    player.State = Character.CharacterState.Idle;
                }
                // reset the turn-taken flag
                player.IsTurnTaken = false;
                // clear the combat action
                player.CombatAction = null;
                // advance each player
                player.AdvanceRound();
            }

            // set the action text on the HUD
            Session.Hud.ActionText = "Your Party's Turn";

            // find the first player who is alive
            _highlightedPlayer = 0;
            CombatantPlayer firstPlayer = _playersList[_highlightedPlayer];
            while (firstPlayer.IsTurnTaken || firstPlayer.IsDeadOrDying)
            {
                _highlightedPlayer = (_highlightedPlayer + 1) % _playersList.Count;
                firstPlayer = _playersList[_highlightedPlayer];
            }

            // start the first player's turn
            BeginPlayerTurn(firstPlayer);
        }

        /// <summary>
        /// Check for whether all players have taken their turn.
        /// </summary>
        private bool IsPlayersTurnComplete
        {
            get
            {
                return _playersList.TrueForAll(delegate (CombatantPlayer player)
                {
                    return player.IsTurnTaken || player.IsDeadOrDying;
                });
            }
        }

        /// <summary>
        /// Check for whether the players have been wiped out and defeated.
        /// </summary>
        private bool ArePlayersDefeated
        {
            get
            {
                return _playersList.TrueForAll(delegate (CombatantPlayer player)
                {
                    return player.State == Character.CharacterState.Dead;
                });
            }
        }

        /// <summary>
        /// Retrieves the first living player, if any.
        /// </summary>
        private CombatantPlayer FirstPlayerTarget
        {
            get
            {
                // if there are no living players, then this is moot
                if (ArePlayersDefeated)
                {
                    return null;
                }

                int playerIndex = 0;
                while ((playerIndex < _playersList.Count) && _playersList[playerIndex].IsDeadOrDying)
                {
                    playerIndex++;
                }
                return _playersList[playerIndex];
            }
        }

        /// <summary>
        /// The monsters involved in the current combat.
        /// </summary>
        private List<CombatantMonster> _monsters = null;

        /// <summary>
        /// The monsters involved in the current combat.
        /// </summary>
        public static List<CombatantMonster> Monsters
        {
            get
            {
                CheckSingleton();
                return _singleton._monsters;
            }
        }

        /// <summary>
        /// The positions of the monsters on the screen.
        /// </summary>
        private static readonly Vector2[] MonsterPositions = new Vector2[5]
        {
            new Vector2(480f, 345f),
            new Vector2(345f, 260f),
            new Vector2(370f, 440f),
            new Vector2(225f, 200f),
            new Vector2(225f, 490f)
        };

        /// <summary>
        /// Start the given player's combat turn.
        /// </summary>
        private void BeginMonsterTurn(CombatantMonster monster)
        {
            // if it's null, find a random living monster who has yet to take their turn
            if (monster == null)
            {
                // don't bother if all monsters have finished
                if (IsMonstersTurnComplete)
                {
                    return;
                }
                // pick random living monsters who haven't taken their turn
                do
                {
                    monster = _monsters[Session.Random.Next(_monsters.Count)];
                }
                while (monster.IsTurnTaken || monster.IsDeadOrDying);
            }

            // set the highlight sprite
            _highlightedCombatant = monster;
            _primaryTargetedCombatant = null;
            _secondaryTargetedCombatants.Clear();

            // choose the action immediate
            monster.CombatAction = monster.ArtificialIntelligence.ChooseAction();
        }

        /// <summary>
        /// Begin the monsters' turn in this combat round.
        /// </summary>
        private void BeginMonstersTurn()
        {
            // set the monster-turn
            _isPlayersTurn = false;

            // reset each monster for the next combat turn
            foreach (CombatantMonster monster in _monsters)
            {
                // reset the animations back to idle
                monster.Character.State = Character.CharacterState.Idle;
                // reset the turn-taken flag
                monster.IsTurnTaken = false;
                // clear the combat action
                monster.CombatAction = null;
                // advance the combatants
                monster.AdvanceRound();
            }

            // set the action text on the HUD
            Session.Hud.ActionText = "Enemy Party's Turn";

            // start a Session.Random monster's turn
            BeginMonsterTurn(null);
        }

        /// <summary>
        /// Check for whether all monsters have taken their turn.
        /// </summary>
        private bool IsMonstersTurnComplete
        {
            get
            {
                return _monsters.TrueForAll(delegate (CombatantMonster monster)
                {
                    return monster.IsTurnTaken || monster.IsDeadOrDying;
                });
            }
        }

        /// <summary>
        /// Check for whether the monsters have been wiped out and defeated.
        /// </summary>
        private bool AreMonstersDefeated
        {
            get
            {
                return _monsters.TrueForAll(delegate (CombatantMonster monster)
                {
                    return monster.State == Character.CharacterState.Dead;
                });
            }
        }

        /// <summary>
        /// Retrieves the first living monster, if any.
        /// </summary>
        private CombatantMonster FirstMonsterTarget
        {
            get
            {
                // if there are no living monsters, then this is moot
                if (AreMonstersDefeated)
                {
                    return null;
                }

                int monsterIndex = 0;
                while ((monsterIndex < _monsters.Count) &&
                    _monsters[monsterIndex].IsDeadOrDying)
                {
                    monsterIndex++;
                }
                return _monsters[monsterIndex];
            }
        }

        /// <summary>
        /// The currently highlighted player, if any.
        /// </summary>
        private Combatant _highlightedCombatant;

        /// <summary>
        /// The currently highlighted player, if any.
        /// </summary>
        public static Combatant HighlightedCombatant
        {
            get
            {
                CheckSingleton();
                return _singleton._highlightedCombatant;
            }
        }

        /// <summary>
        /// The current primary target, if any.
        /// </summary>
        private Combatant _primaryTargetedCombatant;

        /// <summary>
        /// The current primary target, if any.
        /// </summary>
        public static Combatant PrimaryTargetedCombatant
        {
            get
            {
                CheckSingleton();
                return _singleton._primaryTargetedCombatant;
            }
        }

        /// <summary>
        /// The current secondary targets, if any.
        /// </summary>
        private List<Combatant> _secondaryTargetedCombatants = new();

        /// <summary>
        /// The current secondary targets, if any.
        /// </summary>
        public static List<Combatant> SecondaryTargetedCombatants
        {
            get
            {
                CheckSingleton();
                return _singleton._secondaryTargetedCombatants;
            }
        }

        /// <summary>
        /// Retrieves the first living enemy, if any.
        /// </summary>
        public static Combatant FirstEnemyTarget
        {
            get
            {
                CheckSingleton();

                if (IsPlayersTurn)
                {
                    return _singleton.FirstMonsterTarget;
                }
                else
                {
                    return _singleton.FirstPlayerTarget;
                }
            }
        }

        /// <summary>
        /// Retrieves the first living ally, if any.
        /// </summary>
        public static Combatant FirstAllyTarget
        {
            get
            {
                CheckSingleton();

                if (IsPlayersTurn)
                {
                    return _singleton.FirstPlayerTarget;
                }
                else
                {
                    return _singleton.FirstMonsterTarget;
                }
            }
        }

        /// <summary>
        /// Set the primary and any secondary targets.
        /// </summary>
        /// <param name="primaryTarget">The desired primary target.</param>
        /// <param name="adjacentTargets">
        /// The number of simultaneous, adjacent targets affected by this spell.
        /// </param>
        private void SetTargets(Combatant primaryTarget, int adjacentTargets)
        {
            // set the primary target
            _primaryTargetedCombatant = primaryTarget;

            // set any secondary targets
            _secondaryTargetedCombatants.Clear();
            if ((primaryTarget != null) && (adjacentTargets > 0))
            {
                // find out which side is targeted
                bool isPlayerTarget = primaryTarget is CombatantPlayer;
                // find the index
                int primaryTargetIndex = 0;
                if (isPlayerTarget)
                {
                    primaryTargetIndex = _playersList.FindIndex(delegate (CombatantPlayer player)
                    {
                        return (player == primaryTarget);
                    });
                }
                else
                {
                    primaryTargetIndex = _monsters.FindIndex(delegate (CombatantMonster monster)
                    {
                        return (monster == primaryTarget);
                    });
                }

                // add the surrounding indices
                for (int i = 1; i <= adjacentTargets; i++)
                {
                    int leftIndex = primaryTargetIndex - i;
                    if (leftIndex >= 0)
                    {
                        _secondaryTargetedCombatants.Add(isPlayerTarget ? _playersList[leftIndex] : _monsters[leftIndex]);
                    }

                    int rightIndex = primaryTargetIndex + i;
                    if (rightIndex < (isPlayerTarget ? _playersList.Count : _monsters.Count))
                    {
                        _secondaryTargetedCombatants.Add(isPlayerTarget ? _playersList[rightIndex] : _monsters[rightIndex]);
                    }
                }
            }
        }

        /// <summary>
        /// A combat effect sprite, typically used for damage or healing numbers.
        /// </summary>
        private class CombatEffect
        {
            /// <summary>
            /// The starting position of the effect on the screen.
            /// </summary>
            public Vector2 OriginalPosition;


            /// <summary>
            /// The current position of the effect on the screen.
            /// </summary>
            protected Vector2 _position;

            /// <summary>
            /// The current position of the effect on the screen.
            /// </summary>
            public Vector2 Position => _position;

            /// <summary>
            /// The text that appears on top of the effect.
            /// </summary>
            protected string _text = string.Empty;

            /// <summary>
            /// The text that appears on top of the effect.
            /// </summary>
            public string Text
            {
                get => _text;
                set
                {
                    _text = value;
                    // recalculate the origin
                    if (string.IsNullOrEmpty(_text))
                    {
                        _textOrigin = Vector2.Zero;
                    }
                    else
                    {
                        Vector2 textSize = Fonts.DamageFont.MeasureString(_text);
                        _textOrigin = new Vector2((float)Math.Ceiling(textSize.X / 2f), (float)Math.Ceiling(textSize.Y / 2f));
                    }
                }
            }

            /// <summary>
            /// The drawing origin of the text on the effect.
            /// </summary>
            private Vector2 _textOrigin = Vector2.Zero;

            /// <summary>
            /// The speed at which the effect rises on the screen.
            /// </summary>
            const int _risePerSecond = 100;

            /// <summary>
            /// The amount which the effect rises on the screen.
            /// </summary>
            const int _riseMaximum = 80;

            /// <summary>
            /// The amount which the effect has already risen on the screen.
            /// </summary>
            public float Rise = 0;

            /// <summary>
            /// If true, the effect has finished rising.
            /// </summary>
            private bool _isRiseComplete = false;

            /// <summary>
            /// If true, the effect has finished rising.
            /// </summary>
            public bool IsRiseComplete => _isRiseComplete;

            /// <summary>
            /// Updates the combat effect.
            /// </summary>
            /// <param name="elapsedSeconds">
            /// The number of seconds elapsed since the last update.
            /// </param>
            public virtual void Update(float elapsedSeconds)
            {
                if (!_isRiseComplete)
                {
                    Rise += _risePerSecond * elapsedSeconds;
                    if (Rise > _riseMaximum)
                    {
                        Rise = _riseMaximum;
                        _isRiseComplete = true;
                    }
                    _position = new Vector2(OriginalPosition.X, OriginalPosition.Y - Rise);
                }
            }

            /// <summary>
            /// Draw the combat effect.
            /// </summary>
            /// <param name="spriteBatch">The SpriteBatch used to draw.</param>
            /// <param name="texture">The texture for the effect.</param>
            public virtual void Draw(SpriteBatch spriteBatch, Texture2D texture)
            {
                // check the parameter
                if (spriteBatch == null)
                {
                    return;
                }
                // draw the texture
                if (texture != null)
                {
                    spriteBatch.Draw(texture, _position, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0.3f * (float)Rise / 200f);
                }
                // draw the text
                if (!string.IsNullOrEmpty(Text))
                {
                    spriteBatch.DrawString(Fonts.DamageFont, _text, _position, Color.White, 0f, new Vector2(_textOrigin.X, _textOrigin.Y), 1f, SpriteEffects.None, 0.2f * (float)Rise / 200f);
                }
            }
        }

        /// <summary>
        /// The sprite texture for all damage combat effects.
        /// </summary>
        private Texture2D _damageCombatEffectTexture;

        /// <summary>
        /// All current damage combat effects.
        /// </summary>
        private List<CombatEffect> _damageCombatEffects = new();

        /// <summary>
        /// Adds a new damage combat effect to the scene.
        /// </summary>
        /// <param name="position">The position that the effect starts at.</param>
        /// <param name="damage">The damage statistics.</param>
        public static void AddNewDamageEffects(Vector2 position, StatisticsValue damage)
        {
            int startingRise = 0;

            CheckSingleton();

            if (damage.HealthPoints != 0)
            {
                CombatEffect combatEffect = new()
                {
                    OriginalPosition = position,
                    Text = "HP\n" + damage.HealthPoints.ToString(),
                    Rise = startingRise
                };
                startingRise -= 5;
                _singleton._damageCombatEffects.Add(combatEffect);
            }

            if (damage.MagicPoints != 0)
            {
                CombatEffect combatEffect = new()
                {
                    OriginalPosition = position,
                    Text = "MP\n" + damage.MagicPoints.ToString(),
                    Rise = startingRise
                };
                startingRise -= 5;
                _singleton._damageCombatEffects.Add(combatEffect);
            }

            if (damage.PhysicalOffense != 0)
            {
                CombatEffect combatEffect = new()
                {
                    OriginalPosition = position,
                    Text = "PO\n" + damage.PhysicalOffense.ToString(),
                    Rise = startingRise
                };
                startingRise -= 5;
                _singleton._damageCombatEffects.Add(combatEffect);
            }

            if (damage.PhysicalDefense != 0)
            {
                CombatEffect combatEffect = new()
                {
                    OriginalPosition = position,
                    Text = "PD\n" + damage.PhysicalDefense.ToString(),
                    Rise = startingRise
                };
                startingRise -= 5;
                _singleton._damageCombatEffects.Add(combatEffect);
            }

            if (damage.MagicalOffense != 0)
            {
                CombatEffect combatEffect = new()
                {
                    OriginalPosition = position,
                    Text = "MO\n" + damage.MagicalOffense.ToString(),
                    Rise = startingRise
                };
                startingRise -= 5;
                _singleton._damageCombatEffects.Add(combatEffect);
            }

            if (damage.MagicalDefense != 0)
            {
                CombatEffect combatEffect = new()
                {
                    OriginalPosition = position,
                    Text = "MD\n" + damage.MagicalDefense.ToString(),
                    Rise = startingRise
                };
                startingRise -= 5;
                _singleton._damageCombatEffects.Add(combatEffect);
            }
        }

        /// <summary>
        /// The sprite texture for all healing combat effects.
        /// </summary>
        private Texture2D _healingCombatEffectTexture;


        /// <summary>
        /// All current healing combat effects.
        /// </summary>
        private List<CombatEffect> _healingCombatEffects = new();

        /// <summary>
        /// Adds a new healing combat effect to the scene.
        /// </summary>
        /// <param name="position">The position that the effect starts at.</param>
        /// <param name="damage">The healing statistics.</param>
        public static void AddNewHealingEffects(Vector2 position, StatisticsValue healing)
        {
            int startingRise = 0;

            CheckSingleton();

            if (healing.HealthPoints != 0)
            {
                CombatEffect combatEffect = new()
                {
                    OriginalPosition = position,
                    Text = "HP\n" + healing.HealthPoints.ToString(),
                    Rise = startingRise
                };
                startingRise -= 5;
                _singleton._healingCombatEffects.Add(combatEffect);
            }

            if (healing.MagicPoints != 0)
            {
                CombatEffect combatEffect = new()
                {
                    OriginalPosition = position,
                    Text = "MP\n" + healing.MagicPoints.ToString(),
                    Rise = startingRise
                };
                startingRise -= 5;
                _singleton._healingCombatEffects.Add(combatEffect);
            }

            if (healing.PhysicalOffense != 0)
            {
                CombatEffect combatEffect = new()
                {
                    OriginalPosition = position,
                    Text = "PO\n" + healing.PhysicalOffense.ToString(),
                    Rise = startingRise
                };
                startingRise -= 5;
                _singleton._healingCombatEffects.Add(combatEffect);
            }

            if (healing.PhysicalDefense != 0)
            {
                CombatEffect combatEffect = new()
                {
                    OriginalPosition = position,
                    Text = "PD\n" + healing.PhysicalDefense.ToString(),
                    Rise = startingRise
                };
                startingRise -= 5;
                _singleton._healingCombatEffects.Add(combatEffect);
            }

            if (healing.MagicalOffense != 0)
            {
                CombatEffect combatEffect = new()
                {
                    OriginalPosition = position,
                    Text = "MO\n" + healing.MagicalOffense.ToString(),
                    Rise = startingRise
                };
                startingRise -= 5;
                _singleton._healingCombatEffects.Add(combatEffect);
            }

            if (healing.MagicalDefense != 0)
            {
                CombatEffect combatEffect = new()
                {
                    OriginalPosition = position,
                    Text = "MD\n" + healing.MagicalDefense.ToString(),
                    Rise = startingRise
                };
                startingRise -= 5;
                _singleton._healingCombatEffects.Add(combatEffect);
            }
        }

        /// <summary>
        /// Load the graphics data for the combat effect sprites.
        /// </summary>
        private void CreateCombatEffectSprites()
        {
            ContentManager content = Session.ScreenManager.Game.Content;

            _damageCombatEffectTexture = content.Load<Texture2D>("Textures/Combat/DamageIcon");
            _healingCombatEffectTexture = content.Load<Texture2D>("Textures/Combat/HealingIcon");
        }

        /// <summary>
        /// Draw all combat effect sprites.
        /// </summary>
        private void DrawCombatEffects(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            SpriteBatch spriteBatch = Session.ScreenManager.SpriteBatch;

            // update all effects
            foreach (CombatEffect combatEffect in _damageCombatEffects)
            {
                combatEffect.Update(elapsedSeconds);
            }
            foreach (CombatEffect combatEffect in _healingCombatEffects)
            {
                combatEffect.Update(elapsedSeconds);
            }

            // draw the damage effects
            if (_damageCombatEffectTexture != null)
            {
                foreach (CombatEffect combatEffect in _damageCombatEffects)
                {
                    combatEffect.Draw(spriteBatch, _damageCombatEffectTexture);
                }
            }

            // draw the healing effects
            if (_healingCombatEffectTexture != null)
            {
                foreach (CombatEffect combatEffect in _healingCombatEffects)
                {
                    combatEffect.Draw(spriteBatch, _healingCombatEffectTexture);
                }
            }

            // remove all complete effects
            Predicate<CombatEffect> removeCompleteEffects = delegate (CombatEffect combatEffect)
            {
                return combatEffect.IsRiseComplete;
            };
            _damageCombatEffects.RemoveAll(removeCompleteEffects);
            _healingCombatEffects.RemoveAll(removeCompleteEffects);
        }

        /// <summary>
        /// The animating sprite that draws over the highlighted character.
        /// </summary>
        private AnimatingSprite _highlightForegroundSprite = new AnimatingSprite();

        /// <summary>
        /// The animating sprite that draws behind the highlighted character.
        /// </summary>
        private AnimatingSprite _highlightBackgroundSprite = new AnimatingSprite();

        /// <summary>
        /// The animating sprite that draws behind the primary target character.
        /// </summary>
        private AnimatingSprite _primaryTargetSprite = new AnimatingSprite();

        /// <summary>
        /// The animating sprite that draws behind any secondary target characters.
        /// </summary>
        private AnimatingSprite _secondaryTargetSprite = new AnimatingSprite();

        /// <summary>
        /// Create the selection sprite objects.
        /// </summary>
        private void CreateSelectionSprites()
        {
            ContentManager content = Session.ScreenManager.Game.Content;

            Point frameDimensions = new Point(76, 58);
            _highlightForegroundSprite.FramesPerRow = 6;
            _highlightForegroundSprite.FrameDimensions = frameDimensions;
            _highlightForegroundSprite.AddAnimation(new Animation("Selection", 1, 4, 100, true));
            _highlightForegroundSprite.PlayAnimation(0);
            _highlightForegroundSprite.SourceOffset = new Vector2(frameDimensions.X / 2f, 40f);
            _highlightForegroundSprite.Texture = content.Load<Texture2D>("Textures/Combat/TilesheetSprangles");

            frameDimensions = new Point(102, 54);
            _highlightBackgroundSprite.FramesPerRow = 4;
            _highlightBackgroundSprite.FrameDimensions = frameDimensions;
            _highlightBackgroundSprite.AddAnimation(new Animation("Selection", 1, 4, 100, true));
            _highlightBackgroundSprite.PlayAnimation(0);
            _highlightBackgroundSprite.SourceOffset = new Vector2(frameDimensions.X / 2f, frameDimensions.Y / 2f);
            _highlightBackgroundSprite.Texture = content.Load<Texture2D>("Textures/Combat/CharSelectionRing");

            _primaryTargetSprite.FramesPerRow = 4;
            _primaryTargetSprite.FrameDimensions = frameDimensions;
            _primaryTargetSprite.AddAnimation(new Animation("Selection", 1, 4, 100, true));
            _primaryTargetSprite.PlayAnimation(0);
            _primaryTargetSprite.SourceOffset = new Vector2(frameDimensions.X / 2f, frameDimensions.Y / 2f);
            _primaryTargetSprite.Texture = content.Load<Texture2D>("Textures/Combat/Target1SelectionRing");

            _secondaryTargetSprite.FramesPerRow = 4;
            _secondaryTargetSprite.FrameDimensions = frameDimensions;
            _secondaryTargetSprite.AddAnimation(new Animation("Selection", 1, 4, 100, true));
            _secondaryTargetSprite.PlayAnimation(0);
            _secondaryTargetSprite.SourceOffset = new Vector2(frameDimensions.X / 2f, frameDimensions.Y / 2f);
            _secondaryTargetSprite.Texture = content.Load<Texture2D>("Textures/Combat/Target2SelectionRing");
        }

        /// <summary>
        /// Draw the highlight sprites.
        /// </summary>
        private void DrawSelectionSprites(GameTime gameTime)
        {
            SpriteBatch spriteBatch = Session.ScreenManager.SpriteBatch;
            Viewport viewport = Session.ScreenManager.GraphicsDevice.Viewport;

            // update the animations
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _highlightForegroundSprite.UpdateAnimation(elapsedSeconds);
            _highlightBackgroundSprite.UpdateAnimation(elapsedSeconds);
            _primaryTargetSprite.UpdateAnimation(elapsedSeconds);
            _secondaryTargetSprite.UpdateAnimation(elapsedSeconds);

            // draw the highlighted-player sprite, if any
            if (_highlightedCombatant != null)
            {
                _highlightBackgroundSprite.Draw(spriteBatch, _highlightedCombatant.Position, 1f - (_highlightedCombatant.Position.Y - 1) / viewport.Height);
                _highlightForegroundSprite.Draw(spriteBatch, _highlightedCombatant.Position, 1f - (_highlightedCombatant.Position.Y + 1) / viewport.Height);
            }

            // draw the primary target sprite and name, if any
            if (_primaryTargetedCombatant != null)
            {
                _primaryTargetSprite.Draw(spriteBatch, _primaryTargetedCombatant.Position, 1f - (_primaryTargetedCombatant.Position.Y - 1) / viewport.Height);
                if (_primaryTargetedCombatant.Character is Monster)
                {
                    Fonts.DrawCenteredText(spriteBatch, Fonts.DamageFont,
#if DEBUG
                    _primaryTargetedCombatant.Character.Name + "\n" +
                    _primaryTargetedCombatant.Statistics.HealthPoints + "/" +
                    _primaryTargetedCombatant.Character.CharacterStatistics.HealthPoints,
#else
                        primaryTargetedCombatant.Character.Name,
#endif
                        _primaryTargetedCombatant.Position + new Vector2(0f, 42f), Color.White);
                }
            }

            // draw the secondary target sprites on live enemies, if any
            foreach (Combatant combatant in _secondaryTargetedCombatants)
            {
                if (combatant.IsDeadOrDying)
                {
                    continue;
                }
                _secondaryTargetSprite.Draw(spriteBatch, combatant.Position, 1f - (combatant.Position.Y - 1) / viewport.Height);
                if (combatant.Character is Monster)
                {
                    Fonts.DrawCenteredText(spriteBatch, Fonts.DamageFont,
#if DEBUG
                        combatant.Character.Name + "\n" +
                        combatant.Statistics.HealthPoints + "/" +
                        combatant.Character.CharacterStatistics.HealthPoints,
#else
                        combatant.Character.Name,
#endif
                        combatant.Position + new Vector2(0f, 42f), Color.White);
                }
            }
        }

        /// <summary>
        /// Varieties of delays that are interspersed throughout the combat flow.
        /// </summary>
        private enum DelayType
        {
            /// <summary>
            /// No delay at this time.
            /// </summary>
            NoDelay,

            /// <summary>
            /// Delay at the start of combat.
            /// </summary>
            StartCombat,

            /// <summary>
            /// Delay when one side turn's ends before the other side begins.
            /// </summary>
            EndRound,

            /// <summary>
            /// Delay at the end of a character's turn before the next one begins.
            /// </summary>
            EndCharacterTurn,

            /// <summary>
            /// Delay before a flee is attempted.
            /// </summary>
            FleeAttempt,

            /// <summary>
            /// Delay when the party has fled from combat before combat ends.
            /// </summary>
            FleeSuccessful,
        }

        /// <summary>
        /// The current delay, if any (otherwise NoDelay).
        /// </summary>
        private DelayType _delayType = DelayType.NoDelay;

        /// <summary>
        /// Returns true if the combat engine is delaying for any reason.
        /// </summary>
        public static bool IsDelaying => _singleton != null && _singleton._delayType != DelayType.NoDelay;

        /// <summary>
        /// The duration for all kinds of delays, in milliseconds.
        /// </summary>
        private const int _totalDelay = 1000;

        /// <summary>
        /// The duration of the delay so far.
        /// </summary>
        private int _currentDelay = 0;

        /// <summary>
        /// Update any delays in the combat system.
        /// </summary>
        /// <remarks>
        /// This function may cause combat to end, setting the singleton to null.
        /// </remarks>
        private void UpdateDelay(int elapsedMilliseconds)
        {
            if (_delayType == DelayType.NoDelay)
            {
                return;
            }

            // increment the delay
            _currentDelay += elapsedMilliseconds;

            // if the delay is ongoing, then we're done
            if (_currentDelay < _totalDelay)
            {
                return;
            }
            _currentDelay = 0;

            // the delay has ended, so the operation implied by the DelayType happens
            switch (_delayType)
            {
                case DelayType.StartCombat:
                    // determine who goes first and start combat
                    int whoseTurn = Session.Random.Next(2);
                    if (whoseTurn == 0)
                    {
                        BeginPlayersTurn();
                    }
                    else
                    {
                        BeginMonstersTurn();
                    }
                    _delayType = DelayType.NoDelay;
                    break;

                case DelayType.EndCharacterTurn:
                    if (IsPlayersTurn)
                    {
                        // check to see if the players' turn is complete
                        if (IsPlayersTurnComplete)
                        {
                            _delayType = DelayType.EndRound;
                            break;
                        }
                        // find the next player
                        int highlightedIndex = _playersList.FindIndex(delegate (CombatantPlayer player)
                        {
                            return (player ==
                                _highlightedCombatant as CombatantPlayer);
                        });
                        int nextIndex = (highlightedIndex + 1) % _playersList.Count;
                        while (_playersList[nextIndex].IsDeadOrDying || _playersList[nextIndex].IsTurnTaken)
                        {
                            nextIndex = (nextIndex + 1) % _playersList.Count;
                        }
                        BeginPlayerTurn(_playersList[nextIndex]);
                    }
                    else
                    {
                        // check to see if the monsters' turn is complete
                        if (IsMonstersTurnComplete)
                        {
                            _delayType = DelayType.EndRound;
                            break;
                        }
                        // find the next monster
                        BeginMonsterTurn(null);
                    }
                    _delayType = DelayType.NoDelay;
                    break;

                case DelayType.EndRound:
                    // check for turn completion
                    if (IsPlayersTurn && IsPlayersTurnComplete)
                    {
                        BeginMonstersTurn();
                    }
                    else if (!IsPlayersTurn && IsMonstersTurnComplete)
                    {
                        BeginPlayersTurn();
                    }
                    _delayType = DelayType.NoDelay;
                    break;

                case DelayType.FleeAttempt:
                    if (_fleeThreshold <= 0)
                    {
                        _delayType = DelayType.EndCharacterTurn;
                        Session.Hud.ActionText = "This Fight Cannot Be Escaped...";
                        if (_highlightedCombatant != null)
                        {
                            _highlightedCombatant.IsTurnTaken = true;
                        }
                    }
                    else if (CalculateFleeAttempt())
                    {
                        _delayType = DelayType.FleeSuccessful;
                        Session.Hud.ActionText = "Your Party Has Fled!";
                    }
                    else
                    {
                        _delayType = DelayType.EndCharacterTurn;
                        Session.Hud.ActionText = "Your Party Failed to Escape!";
                        if (_highlightedCombatant != null)
                        {
                            _highlightedCombatant.IsTurnTaken = true;
                        }
                    }
                    break;

                case DelayType.FleeSuccessful:
                    EndCombat(CombatEndingState.Fled);
                    _delayType = DelayType.NoDelay;
                    break;
            }
        }

        /// <summary>
        /// Generates a list of CombatantPlayer objects from the party members.
        /// </summary>
        private static List<CombatantPlayer> GenerateCombatantsFromParty()
        {
            List<CombatantPlayer> generatedPlayers = new();

            foreach (Player player in Session.Party.Players)
            {
                if (generatedPlayers.Count <= PlayerPositions.Length)
                {
                    generatedPlayers.Add(new CombatantPlayer(player));
                }
            }

            return generatedPlayers;
        }

        /// <summary>
        /// Start a new combat from the given FixedCombat object.
        /// </summary>
        public static void StartNewCombat(MapEntry<FixedCombat> fixedCombatEntry)
        {
            // check the parameter
            if (fixedCombatEntry == null)
            {
                throw new ArgumentNullException("fixedCombatEntry");
            }
            FixedCombat fixedCombat = fixedCombatEntry.Content;
            if (fixedCombat == null)
            {
                throw new ArgumentException("fixedCombatEntry has no content.");
            }

            // generate the monster combatant list
            List<CombatantMonster> generatedMonsters = new();
            foreach (ContentEntry<Monster> entry in fixedCombat.Entries)
            {
                for (int i = 0; i < entry.Count; i++)
                {
                    generatedMonsters.Add(new CombatantMonster(entry.Content));
                }
            }

            // randomize the list of monsters
            List<CombatantMonster> randomizedMonsters = new();
            while ((generatedMonsters.Count > 0) && (randomizedMonsters.Count <= MonsterPositions.Length))
            {
                int index = Session.Random.Next(generatedMonsters.Count);
                randomizedMonsters.Add(generatedMonsters[index]);
                generatedMonsters.RemoveAt(index);
            }

            // start the combat
            StartNewCombat(GenerateCombatantsFromParty(), randomizedMonsters, 0);
            _singleton._fixedCombatEntry = fixedCombatEntry;
        }

        /// <summary>
        /// Start a new combat from the given RandomCombat object.
        /// </summary>
        public static void StartNewCombat(RandomCombat randomCombat)
        {
            // check the parameter
            if (randomCombat == null)
            {
                throw new ArgumentNullException("randomCombat");
            }

            // determine how many monsters will be in the combat
            int monsterCount = randomCombat.MonsterCountRange.GenerateValue(Session.Random);

            // determine the total probability
            int totalWeight = 0;
            foreach (var entry in randomCombat.EntriesList)
            {
                totalWeight += entry.Weight;
            }

            // generate each monster
            List<CombatantMonster> generatedMonsters = new();
            for (int i = 0; i < monsterCount; i++)
            {
                int monsterChoice = Session.Random.Next(totalWeight);
                foreach (var entry in randomCombat.EntriesList)
                {
                    if (monsterChoice < entry.Weight)
                    {
                        generatedMonsters.Add(new CombatantMonster(entry.Content));
                        break;
                    }
                    else
                    {
                        monsterChoice -= entry.Weight;
                    }
                }
            }

            // randomize the list of monsters
            List<CombatantMonster> randomizedMonsters = new();
            while ((generatedMonsters.Count > 0) && (randomizedMonsters.Count <= MonsterPositions.Length))
            {
                int index = Session.Random.Next(generatedMonsters.Count);
                randomizedMonsters.Add(generatedMonsters[index]);
                generatedMonsters.RemoveAt(index);
            }

            // start the combat
            StartNewCombat(GenerateCombatantsFromParty(), randomizedMonsters, randomCombat.FleeProbability);
        }

        /// <summary>
        /// Start a new combat between the party and a group of monsters.
        /// </summary>
        /// <param name="players">The player combatants.</param>
        /// <param name="monsters">The monster combatants.</param>
        /// <param name="fleeThreshold">The odds of success when fleeing.</param>
        private static void StartNewCombat(List<CombatantPlayer> players,
            List<CombatantMonster> monsters, int fleeThreshold)
        {
            // check if we are already in combat
            if (_singleton != null)
            {
                throw new InvalidOperationException(
                    "There can only be one combat at a time.");
            }

            // create the new CombatEngine object
            _singleton = new CombatEngine(players, monsters, fleeThreshold);
        }



        /// <summary>
        /// Construct a new CombatEngine object.
        /// </summary>
        /// <param name="players">The player combatants.</param>
        /// <param name="monsters">The monster combatants.</param>
        /// <param name="fleeThreshold">The odds of success when fleeing.</param>
        private CombatEngine(List<CombatantPlayer> players, List<CombatantMonster> monsters, int fleeThreshold)
        {
            // check the parameters
            if ((players == null) || (players.Count <= 0) || (players.Count > PlayerPositions.Length))
            {
                throw new ArgumentException("players");
            }
            if ((monsters == null) || (monsters.Count <= 0) || (monsters.Count > MonsterPositions.Length))
            {
                throw new ArgumentException("monsters");
            }

            // assign the parameters
            _playersList = players;
            _monsters = monsters;
            this._fleeThreshold = fleeThreshold;

            // assign positions
            for (int i = 0; i < players.Count; i++)
            {
                if (i >= PlayerPositions.Length)
                {
                    break;
                }
                players[i].Position = players[i].OriginalPosition = PlayerPositions[i];
            }
            for (int i = 0; i < monsters.Count; i++)
            {
                if (i >= MonsterPositions.Length)
                {
                    break;
                }
                monsters[i].Position = monsters[i].OriginalPosition = MonsterPositions[i];
            }

            // sort the monsters by the y coordinates, descending
            monsters.Sort(delegate (CombatantMonster monster1, CombatantMonster monster2)
            {
                return monster2.OriginalPosition.Y.CompareTo(monster1.OriginalPosition.Y);
            });

            // create the selection sprites
            CreateSelectionSprites();

            // create the combat effect sprites
            CreateCombatEffectSprites();

            // start the first combat turn after a delay
            _delayType = DelayType.StartCombat;

            // start the combat music
            AudioManager.PushMusic(TileEngine.Map.CombatMusicCueName);
        }

        public static void AttemptFlee()
        {
            CheckSingleton();

            if (!IsPlayersTurn)
            {
                throw new InvalidOperationException("Only the players may flee.");
            }

            _singleton._delayType = DelayType.FleeAttempt;
            Session.Hud.ActionText = "Attempting to Escape...";
        }

        /// <summary>
        /// The odds of being able to flee this combat, from 0 to 100.
        /// </summary>
        private int _fleeThreshold = 0;

        /// <summary>
        /// Calculate an attempted escape from the combat.
        /// </summary>
        /// <returns>If true, the escape succeeds.</returns>
        private bool CalculateFleeAttempt()
        {
            return Session.Random.Next(100) < _fleeThreshold;
        }

        /// <summary>
        /// End the combat
        /// </summary>
        /// <param name="combatEndState"></param>
        private void EndCombat(CombatEndingState combatEndingState)
        {
            // go back to the non-combat music
            AudioManager.PopMusic();

            switch (combatEndingState)
            {
                case CombatEndingState.Victory:
                    int experienceReward = 0;
                    int goldReward = 0;
                    List<Gear> gearRewards = new();
                    List<string> gearRewardNames = new();
                    // calculate the rewards from the monsters
                    foreach (CombatantMonster combatantMonster in _monsters)
                    {
                        Monster monster = combatantMonster.Monster;
                        Session.Party.AddMonsterKill(monster);
                        experienceReward += monster.CalculateExperienceReward(Session.Random);
                        goldReward += monster.CalculateGoldReward(Session.Random);
                        gearRewardNames.AddRange(monster.CalculateGearDrop(Session.Random));
                    }
                    foreach (string gearRewardName in gearRewardNames)
                    {
                        gearRewards.Add(Session.ScreenManager.Game.Content.Load<Gear>(Path.Combine("Gear", gearRewardName)));
                    }
                    // add the reward screen
                    Session.ScreenManager.AddScreen(new RewardsScreen(RewardsScreen.RewardScreenMode.Combat, experienceReward, goldReward, gearRewards));
                    // remove the fixed combat entry, if this wasn't a random fight
                    if (FixedCombatEntry != null)
                    {
                        Session.RemoveFixedCombat(FixedCombatEntry);
                    }
                    break;

                case CombatEndingState.Loss: // game over
                    ScreenManager screenManager = Session.ScreenManager;
                    // end the session
                    Session.EndSession();
                    // add the game-over screen
                    screenManager.AddScreen(new GameOverScreen());
                    break;

                case CombatEndingState.Fled:
                    break;
            }

            // clear the singleton
            _singleton = null;
        }

        /// <summary>
        /// Ensure that there is no combat happening right now.
        /// </summary>
        public static void ClearCombat()
        {
            // clear the singleton
            if (_singleton != null)
            {
                _singleton = null;
            }
        }

        /// <summary>
        /// Update the combat engine for this frame.
        /// </summary>
        public static void Update(GameTime gameTime)
        {
            // if there is no active combat, then there's nothing to update
            // -- this will be called every frame, so there should be no exception for 
            //    calling this method outside of combat
            if (_singleton == null)
            {
                return;
            }

            // update the singleton
            _singleton.UpdateCombatEngine(gameTime);
        }

        /// <summary>
        /// Update the combat engine for this frame.
        /// </summary>
        private void UpdateCombatEngine(GameTime gameTime)
        {
            // check for the end of combat
            if (ArePlayersDefeated)
            {
                EndCombat(CombatEndingState.Loss);
                return;
            }
            else if (AreMonstersDefeated)
            {
                EndCombat(CombatEndingState.Victory);
                return;
            }

            // update the target selections
            if ((_highlightedCombatant != null) && (_highlightedCombatant.CombatAction != null))
            {
                SetTargets(_highlightedCombatant.CombatAction.Target, _highlightedCombatant.CombatAction.AdjacentTargets);
            }

            // update the delay
            UpdateDelay(gameTime.ElapsedGameTime.Milliseconds);
            // UpdateDelay might cause combat to end due to a successful escape,
            // which will set the singleton to null.
            if (_singleton == null)
            {
                return;
            }

            // update the players
            foreach (CombatantPlayer player in _playersList)
            {
                player.Update(gameTime);
            }

            // update the monsters
            foreach (CombatantMonster monster in _monsters)
            {
                monster.Update(gameTime);
            }

            // check for completion of the highlighted combatant
            if ((_delayType == DelayType.NoDelay) &&
                (_highlightedCombatant != null) && _highlightedCombatant.IsTurnTaken)
            {
                _delayType = DelayType.EndCharacterTurn;
            }

            // handle any player input
            HandleInput();
        }


        /// <summary>
        /// Handle player input that affects the combat engine.
        /// </summary>
        private void HandleInput()
        {
            // only accept input during the players' turn
            // -- exit game, etc. is handled by GameplayScreen
            if (!IsPlayersTurn || IsPlayersTurnComplete || (_highlightedCombatant == null))
            {
                return;
            }

#if DEBUG
            // cheat key
            if (InputManager.IsGamePadRightShoulderTriggered() || InputManager.IsKeyTriggered(Keys.W))
            {
                EndCombat(CombatEndingState.Victory);
                return;
            }
#endif
            // handle input while choosing an action
            if (_highlightedCombatant.CombatAction != null)
            {
                // skip if its turn is over or the action is already going
                if (_highlightedCombatant.IsTurnTaken || (_highlightedCombatant.CombatAction.Stage != CombatAction.CombatActionStage.NotStarted))
                {
                    return;
                }

                // back out of the action
                if (InputManager.IsActionTriggered(InputManager.Action.Back))
                {
                    _highlightedCombatant.CombatAction = null;
                    SetTargets(null, 0);
                    return;
                }

                // start the action
                if (InputManager.IsActionTriggered(InputManager.Action.Ok))
                {
                    _highlightedCombatant.CombatAction.Start();
                    return;
                }

                // go to the next target
                if (InputManager.IsActionTriggered(InputManager.Action.TargetUp))
                {
                    // cycle through monsters or party members
                    if (_highlightedCombatant.CombatAction.IsOffensive)
                    {
                        // find the index of the current target
                        int newIndex = _monsters.FindIndex(delegate (CombatantMonster monster)
                        {
                            return _primaryTargetedCombatant == monster;
                        });
                        // find the next living target
                        do
                        {
                            newIndex = (newIndex + 1) % _monsters.Count;
                        }
                        while (_monsters[newIndex].IsDeadOrDying);
                        // set the new target
                        _highlightedCombatant.CombatAction.Target = _monsters[newIndex];
                    }
                    else
                    {
                        // find the index of the current target
                        int newIndex = _playersList.FindIndex(delegate (CombatantPlayer player)
                        {
                            return _primaryTargetedCombatant == player;
                        });
                        // find the next active, living target
                        do
                        {
                            newIndex = (newIndex + 1) % _playersList.Count;
                        }
                        while (_playersList[newIndex].IsDeadOrDying);
                        // set the new target
                        _highlightedCombatant.CombatAction.Target = _playersList[newIndex];
                    }
                    return;
                }
                // go to the previous target
                else if (InputManager.IsActionTriggered(InputManager.Action.TargetDown))
                {
                    // cycle through monsters or party members
                    if (_highlightedCombatant.CombatAction.IsOffensive)
                    {
                        // find the index of the current target
                        int newIndex = _monsters.FindIndex(delegate (CombatantMonster monster)
                        {
                            return (_primaryTargetedCombatant == monster);
                        });
                        // find the previous active, living target
                        do
                        {
                            newIndex--;
                            while (newIndex < 0)
                            {
                                newIndex += _monsters.Count;
                            }
                        }
                        while (_monsters[newIndex].IsDeadOrDying);
                        // set the new target
                        _highlightedCombatant.CombatAction.Target = _monsters[newIndex];
                    }
                    else
                    {
                        // find the index of the current target
                        int newIndex = _playersList.FindIndex(delegate (CombatantPlayer player)
                        {
                            return (_primaryTargetedCombatant == player);
                        });
                        // find the previous living target
                        do
                        {
                            newIndex--;
                            while (newIndex < 0)
                            {
                                newIndex += _playersList.Count;
                            }
                        }
                        while (_playersList[newIndex].IsDeadOrDying);
                        // set the new target
                        _highlightedCombatant.CombatAction.Target = _playersList[newIndex];
                    }
                    return;
                }
            }
            else // choosing which character will act
            {
                // move to the previous living character
                if (InputManager.IsActionTriggered(InputManager.Action.ActiveCharacterLeft))
                {
                    int newHighlightedPlayer = _highlightedPlayer;
                    do
                    {
                        newHighlightedPlayer--;
                        while (newHighlightedPlayer < 0)
                        {
                            newHighlightedPlayer += _playersList.Count;
                        }
                    }
                    while (_playersList[newHighlightedPlayer].IsDeadOrDying || _playersList[newHighlightedPlayer].IsTurnTaken);
                    if (newHighlightedPlayer != _highlightedPlayer)
                    {
                        _highlightedPlayer = newHighlightedPlayer;
                        BeginPlayerTurn(_playersList[_highlightedPlayer]);
                    }
                    return;
                }
                // move to the next living character
                else if (InputManager.IsActionTriggered(InputManager.Action.ActiveCharacterRight))
                {
                    int newHighlightedPlayer = _highlightedPlayer;
                    do
                    {
                        newHighlightedPlayer = (newHighlightedPlayer + 1) % _playersList.Count;
                    }
                    while (_playersList[newHighlightedPlayer].IsDeadOrDying || _playersList[newHighlightedPlayer].IsTurnTaken);
                    if (newHighlightedPlayer != _highlightedPlayer)
                    {
                        _highlightedPlayer = newHighlightedPlayer;
                        BeginPlayerTurn(_playersList[_highlightedPlayer]);
                    }
                    return;
                }
                Session.Hud.UpdateActionsMenu();
            }
        }

        /// <summary>
        /// Draw the combat for this frame.
        /// </summary>
        public static void Draw(GameTime gameTime)
        {
            // if there is no active combat, then there's nothing to draw
            // -- this will be called every frame, so there should be no exception for 
            //    calling this method outside of combat
            if (_singleton == null)
            {
                return;
            }

            // update the singleton
            _singleton.DrawCombatEngine(gameTime);
        }

        /// <summary>
        /// Draw the combat for this frame.
        /// </summary>
        private void DrawCombatEngine(GameTime gameTime)
        {
            // draw the players
            foreach (CombatantPlayer player in _playersList)
            {
                player.Draw(gameTime);
            }

            // draw the monsters
            foreach (CombatantMonster monster in _monsters)
            {
                monster.Draw(gameTime);
            }

            // draw the selection animations
            DrawSelectionSprites(gameTime);

            // draw the combat effects
            DrawCombatEffects(gameTime);
        }
    }
}
