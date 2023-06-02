﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Animations;
using RolePlayingGame.Audio;
using RolePlayingGame.Characters;
using RolePlayingGame.Combat.Actions;
using RolePlayingGame.Data;
using RolePlayingGame.SessionObjects;
using RolePlayingGame.Spells;
using static RolePlayingGame.Characters.Character;

namespace RolePlayingGame.Combat
{
    public abstract class Combatant
    {
        /// <summary>
        /// The character encapsulated by this combatant.
        /// </summary>
        public abstract FightingCharacter Character
        {
            get;
        }

        /// <summary>
        /// The current state of this combatant.
        /// </summary>
        public abstract Character.CharacterState State
        {
            get;
            set;
        }


        /// <summary>
        /// Returns true if the character is dead or dying.
        /// </summary>
        public bool IsDeadOrDying
        {
            get
            {
                return ((State == CharacterState.Dying) || (State == CharacterState.Dead));
            }
        }


        /// <summary>
        /// If true, the combatant has taken their turn this round.
        /// </summary>
        private bool isTurnTaken;

        /// <summary>
        /// If true, the combatant has taken their turn this round.
        /// </summary>
        public bool IsTurnTaken
        {
            get { return isTurnTaken; }
            set { isTurnTaken = value; }
        }

        /// <summary>
        /// Accessor for the combat sprite for this combatant.
        /// </summary>
        public abstract AnimatingSprite CombatSprite
        {
            get;
        }


        /// <summary>
        /// The current position on screen for this combatant.
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// The current position on screen for this combatant.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }


        /// <summary>
        /// The original position on screen for this combatant.
        /// </summary>
        private Vector2 originalPosition;

        /// <summary>
        /// The original position on screen for this combatant.
        /// </summary>
        public Vector2 OriginalPosition
        {
            get { return originalPosition; }
            set { originalPosition = value; }
        }

        /// <summary>
        /// The current statistics of this combatant.
        /// </summary>
        public abstract StatisticsValue Statistics
        {
            get;
        }


        /// <summary>
        /// Heals the combatant's health by the given amount.
        /// </summary>
        public void HealHealth(int healthHealingAmount, int duration)
        {
            Heal(new StatisticsValue(healthHealingAmount, 0, 0, 0, 0, 0), duration);
        }


        /// <summary>
        /// Heal the combatant by the given amount.
        /// </summary>
        public virtual void Heal(StatisticsValue healingStatistics, int duration)
        {
            CombatEngine.AddNewHealingEffects(OriginalPosition, healingStatistics);
        }


        /// <summary>
        /// Damages the combatant's health by the given amount.
        /// </summary>
        public void DamageHealth(int healthDamageAmount, int duration)
        {
            Damage(new StatisticsValue(healthDamageAmount, 0, 0, 0, 0, 0), duration);
        }


        /// <summary>
        /// Damages the combatant by the given amount.
        /// </summary>
        public virtual void Damage(StatisticsValue damageStatistics, int duration)
        {
            State = CharacterState.Hit;
            CombatEngine.AddNewDamageEffects(OriginalPosition, damageStatistics);
        }


        /// <summary>
        /// Pay the cost for the given spell.
        /// </summary>
        /// <returns>True if the cost could be paid (and therefore was paid).</returns>
        public virtual bool PayCostForSpell(Spell spell) { return false; }

        /// <summary>
        /// The current combat action for this combatant.
        /// </summary>
        private CombatAction combatAction;

        /// <summary>
        /// The current combat action for this combatant.
        /// </summary>
        public CombatAction CombatAction
        {
            get { return combatAction; }
            set { combatAction = value; }
        }

        /// <summary>
        /// Statistics stack of the combat effects that are applied to this combatant.
        /// </summary>
        private StatisticsValueStack combatEffects = new StatisticsValueStack();

        /// <summary>
        /// Statistics stack of the combat effects that are applied to this combatant.
        /// </summary>
        public StatisticsValueStack CombatEffects
        {
            get { return combatEffects; }
        }

        /// <summary>
        /// Constructs a new Combatant object.
        /// </summary>
        protected Combatant() { }

        /// <summary>
        /// Update the combatant for this frame.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // update the combat action
            if (combatAction != null)
            {
                // update the combat action
                combatAction.Update(gameTime);
                // remove the combat action if it is done and set the turn-taken flag
                if (combatAction.Stage == CombatAction.CombatActionStage.Complete)
                {
                    combatAction = null;
                    isTurnTaken = true;
                }
            }


            // update the combat sprite animation
            CombatSprite.UpdateAnimation(elapsedSeconds);

            // check for death
            if (!IsDeadOrDying && (Statistics.HealthPoints <= 0))
            {
                AudioManager.PlayCue("Death");
                State = CharacterState.Dying;
            }
            // check for waking up
            else if (IsDeadOrDying && (Statistics.HealthPoints > 0))
            {
                State = CharacterState.Idle;
            }
            else if (CombatSprite.IsPlaybackComplete)
            {
                if (State == CharacterState.Hit)
                {
                    State = CharacterState.Idle;
                }
                else if (State == CharacterState.Dying)
                {
                    State = CharacterState.Dead;
                }
            }
        }


        /// <summary>
        /// Advance the combatant state for one combat round.
        /// </summary>
        public virtual void AdvanceRound()
        {
            // advance the combat effects stack
            combatEffects.Advance();
        }

        /// <summary>
        /// Draw the combatant for this frame.
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
            CombatSprite.Draw(Session.ScreenManager.SpriteBatch,
                Position, 1f - Position.Y / 720f);

            Session.ScreenManager.SpriteBatch.Draw(Character.ShadowTexture, Position,
                null, Color.White, 0f, new Vector2(Character.ShadowTexture.Width / 2,
                Character.ShadowTexture.Height / 2), 1f, SpriteEffects.None, 1f);

            // draw the combat action
            if (combatAction != null)
            {
                // update the combat action
                combatAction.Draw(gameTime, Session.ScreenManager.SpriteBatch);
            }
        }
    }
}
