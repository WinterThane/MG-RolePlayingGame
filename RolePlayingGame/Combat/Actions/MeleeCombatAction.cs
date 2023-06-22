using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Audio;
using RolePlayingGame.Data;
using RolePlayingGame.GearObjects;
using RolePlayingGame.SessionObjects;
using System;

namespace RolePlayingGame.Combat.Actions
{
    public class MeleeCombatAction : CombatAction
    {
        /// <summary>
        /// Returns true if the action is offensive, targeting the opponents.
        /// </summary>
        public override bool IsOffensive => true;

        /// <summary>
        /// Returns true if this action requires a target.
        /// </summary>
        public override bool IsTargetNeeded => true;

        /// <summary>
        /// The speed at which the advancing character moves, in units per second.
        /// </summary>
        private const float _advanceSpeed = 300f;

        /// <summary>
        /// The offset from the advance destination to the target position
        /// </summary>
        private static readonly Vector2 _advanceOffset = new Vector2(85f, 0f);

        /// <summary>
        /// The direction of the advancement.
        /// </summary>
        private Vector2 _advanceDirection;

        /// <summary>
        /// The distance covered so far by the advance/return action
        /// </summary>
        private float _advanceDistanceCovered = 0f;

        /// <summary>
        /// The total distance between the original combatant position and the target.
        /// </summary>
        private float _totalAdvanceDistance;

        /// <summary>
        /// Starts a new combat stage.  Called right after the stage changes.
        /// </summary>
        /// <remarks>The stage never changes into NotStarted.</remarks>
        protected override void StartStage()
        {
            switch (stage)
            {
                case CombatActionStage.Preparing: // called from Start()
                    {
                        // play the animation
                        _combatant.CombatSprite.PlayAnimation("Idle");
                    }
                    break;

                case CombatActionStage.Advancing:
                    {
                        // play the animation
                        _combatant.CombatSprite.PlayAnimation("Walk");
                        // calculate the advancing destination
                        if (Target.Position.X > Combatant.Position.X)
                        {
                            _advanceDirection = Target.Position - Combatant.OriginalPosition - _advanceOffset;
                        }
                        else
                        {
                            _advanceDirection = Target.Position - Combatant.OriginalPosition + _advanceOffset;
                        }
                        _totalAdvanceDistance = _advanceDirection.Length();
                        _advanceDirection.Normalize();
                        _advanceDistanceCovered = 0f;
                    }
                    break;

                case CombatActionStage.Executing:
                    {
                        // play the animation
                        _combatant.CombatSprite.PlayAnimation("Attack");
                        // play the audio
                        Weapon weapon = _combatant.Character.GetEquippedWeapon();
                        if (weapon != null)
                        {
                            AudioManager.PlayCue(weapon.SwingCueName);
                        }
                        else
                        {
                            AudioManager.PlayCue("StaffSwing");
                        }
                    }
                    break;

                case CombatActionStage.Returning:
                    {
                        // play the animation
                        _combatant.CombatSprite.PlayAnimation("Walk");
                        // calculate the damage
                        Int32Range damageRange = _combatant.Character.TargetDamageRange + _combatant.Statistics.PhysicalOffense;
                        Int32Range defenseRange = Target.Character.HealthDefenseRange + Target.Statistics.PhysicalDefense;
                        int damage = Math.Max(0, damageRange.GenerateValue(Session.Random) - defenseRange.GenerateValue(Session.Random));
                        // apply the damage
                        if (damage > 0)
                        {
                            // play the audio
                            Weapon weapon = _combatant.Character.GetEquippedWeapon();
                            if (weapon != null)
                            {
                                AudioManager.PlayCue(weapon.HitCueName);
                            }
                            else
                            {
                                AudioManager.PlayCue("StaffHit");
                            }
                            // damage the target
                            Target.DamageHealth(damage, 0);
                            if (weapon != null && weapon.Overlay != null)
                            {
                                weapon.Overlay.PlayAnimation(0);
                                weapon.Overlay.ResetAnimation();
                            }
                        }
                    }
                    break;

                case CombatActionStage.Finishing:
                    {
                        // play the animation
                        _combatant.CombatSprite.PlayAnimation("Idle");
                    }
                    break;

                case CombatActionStage.Complete:
                    {
                        // play the animation
                        _combatant.CombatSprite.PlayAnimation("Idle");
                    }
                    break;
            }
        }

        /// <summary>
        /// Update the action for the current stage.
        /// </summary>
        /// <remarks>
        /// This function is guaranteed to be called at least once per stage.
        /// </remarks>
        protected override void UpdateCurrentStage(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (stage)
            {
                case CombatActionStage.Advancing:
                    {
                        // move to the destination
                        if (_advanceDistanceCovered < _totalAdvanceDistance)
                        {
                            _advanceDistanceCovered = Math.Min(_advanceDistanceCovered + _advanceSpeed * elapsedSeconds, _totalAdvanceDistance);
                        }
                        // update the combatant's position
                        _combatant.Position = _combatant.OriginalPosition + _advanceDirection * _advanceDistanceCovered;
                    }
                    break;

                case CombatActionStage.Returning:
                    {
                        // move to the destination
                        if (_advanceDistanceCovered > 0f)
                        {
                            _advanceDistanceCovered -= _advanceSpeed * elapsedSeconds;
                        }
                        _combatant.Position = _combatant.OriginalPosition + _advanceDirection * _advanceDistanceCovered;
                    }
                    break;
            }
        }

        /// <summary>
        /// Returns true if the combat action is ready to proceed to the next stage.
        /// </summary>
        protected override bool IsReadyForNextStage
        {
            get
            {
                switch (stage)
                {
                    case CombatActionStage.Preparing: // ready to advance?
                        return true;

                    case CombatActionStage.Advancing: // ready to execute?
                        if (_advanceDistanceCovered >= _totalAdvanceDistance)
                        {
                            _advanceDistanceCovered = _totalAdvanceDistance;
                            _combatant.Position = _combatant.OriginalPosition + _advanceDirection * _totalAdvanceDistance;
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    case CombatActionStage.Executing: // ready to return?
                        return _combatant.CombatSprite.IsPlaybackComplete;

                    case CombatActionStage.Returning: // ready to finish?
                        if (_advanceDistanceCovered <= 0f)
                        {
                            _advanceDistanceCovered = 0f;
                            _combatant.Position = _combatant.OriginalPosition;
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    case CombatActionStage.Finishing: // ready to complete?
                        return true;
                }

                // fall through to the base behavior
                return base.IsReadyForNextStage;
            }
        }

        /// <summary>
        /// The heuristic used to compare actions of this type to similar ones.
        /// </summary>
        public override int Heuristic => _combatant.Character.TargetDamageRange.Average;

        /// <summary>
        /// Constructs a new MeleeCombatAction object.
        /// </summary>
        /// <param name="character">The character performing the action.</param>
        public MeleeCombatAction(Combatant combatant) : base(combatant) { }

        /// <summary>
        /// Updates the action over time.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // update the weapon animation
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Weapon weapon = Combatant.Character.GetEquippedWeapon();
            if (weapon != null && weapon.Overlay != null)
            {
                weapon.Overlay.UpdateAnimation(elapsedSeconds);
            }

            // update the action
            base.Update(gameTime);
        }

        /// <summary>
        /// Draw any elements of the action that are independent of the character.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw the weapon overlay (typically blood)
            Weapon weapon = Combatant.Character.GetEquippedWeapon();
            if (weapon != null && weapon.Overlay != null && !weapon.Overlay.IsPlaybackComplete)
            {
                weapon.Overlay.Draw(spriteBatch, Target.Position, 0f);
            }

            base.Draw(gameTime, spriteBatch);
        }
    }
}
