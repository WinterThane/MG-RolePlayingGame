using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Audio;
using RolePlayingGame.Data;
using RolePlayingGame.SessionObjects;
using RolePlayingGame.Spells;
using System;

namespace RolePlayingGame.Combat.Actions
{
    public class SpellCombatAction : CombatAction
    {
        /// <summary>
        /// Returns true if the action is offensive, targeting the opponents.
        /// </summary>
        public override bool IsOffensive => Spell.IsOffensive;

        /// <summary>
        /// Returns true if the character can use this action.
        /// </summary>
        public override bool IsCharacterValidUser => Spell.MagicPointCost <= Combatant.Statistics.MagicPoints;

        /// <summary>
        /// Returns true if this action requires a target.
        /// </summary>
        public override bool IsTargetNeeded => true;

        /// <summary>
        /// The spell used in this action.
        /// </summary>
        private Spell _spell;

        /// <summary>
        /// The spell used in this action.
        /// </summary>
        public Spell Spell => _spell;

        /// <summary>
        /// The current position of the spell sprite.
        /// </summary>
        private Vector2 _spellSpritePosition;

        /// <summary>
        /// Apply the action's spell to the given target.
        /// </summary>
        /// <returns>True if there was any effect on the target.</returns>
        private bool ApplySpell(Combatant spellTarget)
        {
            StatisticsValue effectStatistics = CalculateSpellDamage(_combatant, _spell);
            if (_spell.IsOffensive)
            {
                // calculate the defense
                Int32Range defenseRange = spellTarget.Character.MagicDefenseRange + spellTarget.Statistics.MagicalDefense;
                Int32 defense = defenseRange.GenerateValue(Session.Random);
                // subtract the defense
                effectStatistics -= new StatisticsValue(defense, defense, defense, defense, defense, defense);
                // make sure that this only contains damage
                effectStatistics.ApplyMinimum(new StatisticsValue());
                // damage the target
                spellTarget.Damage(effectStatistics, _spell.TargetDuration);
            }
            else
            {
                // make sure that this only contains healing
                effectStatistics.ApplyMinimum(new StatisticsValue());
                // heal the target
                spellTarget.Heal(effectStatistics, _spell.TargetDuration);
            }

            return !effectStatistics.IsZero;
        }

        /// <summary>
        /// The speed at which the projectile moves, in units per second.
        /// </summary>
        private const float _projectileSpeed = 600f;

        /// <summary>
        /// The direction of the projectile.
        /// </summary>
        private Vector2 _projectileDirection;

        /// <summary>
        /// The distance covered so far by the projectile.
        /// </summary>
        private float _projectileDistanceCovered = 0f;

        /// <summary>
        /// The total distance between the original combatant position and the target.
        /// </summary>
        private float _totalProjectileDistance;

        /// <summary>
        /// The sprite effect on the projectile, if any.
        /// </summary>
        private SpriteEffects _projectileSpriteEffect = SpriteEffects.None;

        /// <summary>
        /// The sound effect cue for the traveling projectile.
        /// </summary>
        private Cue _projectileCue;

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
                        // play the animations
                        _combatant.CombatSprite.PlayAnimation("SpellCast");
                        _spellSpritePosition = Combatant.Position;
                        _spell.SpellSprite.PlayAnimation("Creation");
                        // remove the magic points
                        Combatant.PayCostForSpell(_spell);
                    }
                    break;

                case CombatActionStage.Advancing:
                    {
                        // play the animations
                        _spell.SpellSprite.PlayAnimation("Traveling");
                        // calculate the projectile destination
                        _projectileDirection = Target.Position - Combatant.OriginalPosition;
                        _totalProjectileDistance = _projectileDirection.Length();
                        _projectileDirection.Normalize();
                        _projectileDistanceCovered = 0f;
                        // determine if the projectile is flipped
                        if (Target.Position.X > Combatant.Position.X)
                        {
                            _projectileSpriteEffect = SpriteEffects.FlipHorizontally;
                        }
                        else
                        {
                            _projectileSpriteEffect = SpriteEffects.None;
                        }
                        // get the projectile's cue and play it
                        _projectileCue = AudioManager.GetCue(_spell.TravelingCueName);
                        if (_projectileCue != null)
                        {
                            _projectileCue.Play();
                        }
                    }
                    break;

                case CombatActionStage.Executing:
                    {
                        // play the animation
                        _spell.SpellSprite.PlayAnimation("Impact");
                        // stop the projectile sound effect
                        if (_projectileCue != null)
                        {
                            _projectileCue.Stop(AudioStopOptions.Immediate);
                        }
                        // apply the spell effect to the primary target
                        bool damagedAnyone = ApplySpell(Target);
                        // apply the spell to the secondary targets
                        foreach (Combatant targetCombatant in CombatEngine.SecondaryTargetedCombatants)
                        {
                            // skip dead or dying targets
                            if (targetCombatant.IsDeadOrDying)
                            {
                                continue;
                            }
                            // apply the spell
                            damagedAnyone |= ApplySpell(targetCombatant);
                        }
                        // play the impact sound effect
                        if (damagedAnyone)
                        {
                            AudioManager.PlayCue(_spell.ImpactCueName);
                            if (_spell.Overlay != null)
                            {
                                _spell.Overlay.PlayAnimation(0);
                                _spell.Overlay.ResetAnimation();
                            }
                        }
                    }
                    break;

                case CombatActionStage.Returning:
                    // play the animation
                    _combatant.CombatSprite.PlayAnimation("Idle");
                    break;

                case CombatActionStage.Finishing:
                    // play the animation
                    _combatant.CombatSprite.PlayAnimation("Idle");
                    break;

                case CombatActionStage.Complete:
                    // play the animation
                    _combatant.CombatSprite.PlayAnimation("Idle");
                    // make sure that the overlay has stopped
                    _spell.Overlay.StopAnimation();
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
            switch (stage)
            {
                case CombatActionStage.Advancing:
                    if (_projectileDistanceCovered < _totalProjectileDistance)
                    {
                        _projectileDistanceCovered += _projectileSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    _spellSpritePosition = _combatant.OriginalPosition + _projectileDirection * _projectileDistanceCovered;
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
                        return (_combatant.CombatSprite.IsPlaybackComplete && _spell.SpellSprite.IsPlaybackComplete);

                    case CombatActionStage.Advancing: // ready to execute?
                        if (_spell.SpellSprite.IsPlaybackComplete || (_projectileDistanceCovered >= _totalProjectileDistance))
                        {
                            _projectileDistanceCovered = _totalProjectileDistance;
                            return true;
                        }
                        return false;

                    case CombatActionStage.Executing: // ready to return?
                        return _spell.SpellSprite.IsPlaybackComplete;
                }

                // fall through to the base behavior
                return base.IsReadyForNextStage;
            }
        }

        /// <summary>
        /// The heuristic used to compare actions of this type to similar ones.
        /// </summary>
        public override int Heuristic => Combatant.Statistics.MagicalOffense + Spell.TargetEffectRange.HealthPointsRange.Average;

        /// <summary>
        /// Constructs a new SpellCombatAction object.
        /// </summary>
        /// <param name="character">The combatant performing the action.</param>
        public SpellCombatAction(Combatant combatant, Spell spell) : base(combatant)
        {
            // check the parameter
            if (spell == null)
            {
                throw new ArgumentNullException("spell");
            }

            // assign the parameter
            _spell = spell;
            _adjacentTargets = _spell.AdjacentTargets;
        }

        /// <summary>
        /// Start executing the combat action.
        /// </summary>
        public override void Start()
        {
            // play the creation sound effect
            AudioManager.PlayCue(_spell.CreatingCueName);

            base.Start();
        }

        /// <summary>
        /// Updates the action over time.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // update the animations
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _spell.SpellSprite.UpdateAnimation(elapsedSeconds);
            if (_spell.Overlay != null)
            {
                _spell.Overlay.UpdateAnimation(elapsedSeconds);
                if (!_spell.Overlay.IsPlaybackComplete && Target.CombatSprite.IsPlaybackComplete)
                {
                    _spell.Overlay.StopAnimation();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw any elements of the action that are independent of the character.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw the spell projectile
            if (!_spell.SpellSprite.IsPlaybackComplete)
            {
                if (stage == CombatActionStage.Advancing)
                {
                    _spell.SpellSprite.Draw(spriteBatch, _spellSpritePosition, 0f, _projectileSpriteEffect);
                }
                else
                {
                    _spell.SpellSprite.Draw(spriteBatch, _spellSpritePosition, 0f);
                }
            }

            // draw the spell overlay
            if (!_spell.Overlay.IsPlaybackComplete)
            {
                _spell.Overlay.Draw(spriteBatch, Target.Position, 0f);
            }

            base.Draw(gameTime, spriteBatch);
        }

        /// <summary>
        /// Calculate the spell damage done by the given combatant and spell.
        /// </summary>
        public static StatisticsValue CalculateSpellDamage(Combatant combatant,
            Spell spell)
        {
            // check the parameters
            if (combatant == null)
            {
                throw new ArgumentNullException("combatant");
            }
            if (spell == null)
            {
                throw new ArgumentNullException("spell");
            }

            // get the magical offense from the character's class, gear, and bonuses
            // -- note that this includes stat buffs
            int magicalOffense = combatant.Statistics.MagicalOffense;

            // add the magical offense to the spell
            StatisticsValue damage = spell.TargetEffectRange.GenerateValue(Session.Random);
            damage.HealthPoints += (damage.HealthPoints != 0) ? magicalOffense : 0;
            damage.MagicPoints += (damage.MagicPoints != 0) ? magicalOffense : 0;
            damage.PhysicalOffense += (damage.PhysicalOffense != 0) ? magicalOffense : 0;
            damage.PhysicalDefense += (damage.PhysicalDefense != 0) ? magicalOffense : 0;
            damage.MagicalOffense += (damage.MagicalOffense != 0) ? magicalOffense : 0;
            damage.MagicalDefense += (damage.MagicalDefense != 0) ? magicalOffense : 0;

            // add in the spell damage
            return damage;
        }
    }
}
