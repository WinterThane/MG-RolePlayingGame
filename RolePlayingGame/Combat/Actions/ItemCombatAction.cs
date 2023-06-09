﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Audio;
using RolePlayingGame.Data;
using RolePlayingGame.GearObjects;
using RolePlayingGame.SessionObjects;
using System;

namespace RolePlayingGame.Combat.Actions
{
    public class ItemCombatAction : CombatAction
    {
        /// <summary>
        /// Returns true if the action is offensive, targeting the opponents.
        /// </summary>
        public override bool IsOffensive => Item.IsOffensive;

        /// <summary>
        /// Returns true if the character can use this action.
        /// </summary>
        public override bool IsCharacterValidUser => true;

        /// <summary>
        /// Returns true if this action requires a target.
        /// </summary>
        public override bool IsTargetNeeded => true;

        /// <summary>
        /// The item used in this action.
        /// </summary>
        private Item _item;

        /// <summary>
        /// The item used in this action.
        /// </summary>
        public Item Item => _item;

        /// <summary>
        /// The current position of the item sprite.
        /// </summary>
        private Vector2 itemSpritePosition;

        /// <summary>
        /// Apply the action's item to the given target.
        /// </summary>
        /// <returns>True if there was any effect on the target.</returns>
        private bool ApplyItem(Combatant itemTarget)
        {
            StatisticsValue effectStatistics = CalculateItemDamage(_combatant, _item);
            if (_item.IsOffensive)
            {
                // calculate the defense
                Int32Range defenseRange = itemTarget.Character.MagicDefenseRange + itemTarget.Statistics.MagicalDefense;
                Int32 defense = defenseRange.GenerateValue(Session.Random);
                // subtract the defense
                effectStatistics -= new StatisticsValue(defense, defense, defense, defense, defense, defense);
                // make sure that this only contains damage
                effectStatistics.ApplyMinimum(new StatisticsValue());
                // damage the target
                itemTarget.Damage(effectStatistics, _item.TargetDuration);
            }
            else
            {
                // make sure taht this only contains healing
                effectStatistics.ApplyMinimum(new StatisticsValue());
                // heal the target
                itemTarget.Heal(effectStatistics, _item.TargetDuration);
            }
            return !effectStatistics.IsZero;
        }

        /// <summary>
        /// The speed at which the projectile moves, in units per second.
        /// </summary>
        private const float _projectileSpeed = 300f;

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
                        _combatant.CombatSprite.PlayAnimation("ItemCast");
                        itemSpritePosition = Combatant.Position;
                        _item.SpellSprite.PlayAnimation("Creation");
                        Session.Party.RemoveFromInventory(_item, 1);
                    }
                    break;

                case CombatActionStage.Advancing:
                    {
                        // play the animations
                        _item.SpellSprite.PlayAnimation("Traveling");
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
                        _projectileCue = AudioManager.GetCue(_item.TravelingCueName);
                        if (_projectileCue != null)
                        {
                            _projectileCue.Play();
                        }
                    }
                    break;

                case CombatActionStage.Executing:
                    // play the animation
                    _item.SpellSprite.PlayAnimation("Impact");
                    // stop the projectile sound effect
                    if (_projectileCue != null)
                    {
                        _projectileCue.Stop(AudioStopOptions.Immediate);
                    }
                    // apply the item effect to the primary target
                    bool damagedAnyone = ApplyItem(Target);
                    // apply the item effect to the secondary targets
                    foreach (Combatant targetCombatant in CombatEngine.SecondaryTargetedCombatants)
                    {
                        // skip any dead or dying combatants
                        if (targetCombatant.IsDeadOrDying)
                        {
                            continue;
                        }
                        // apply the effect
                        damagedAnyone |= ApplyItem(targetCombatant);
                    }
                    // play the impact sound effect
                    if (damagedAnyone)
                    {
                        AudioManager.PlayCue(_item.ImpactCueName);
                        if (_item.Overlay != null)
                        {
                            _item.Overlay.PlayAnimation(0);
                            _item.Overlay.ResetAnimation();
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
                    itemSpritePosition = _combatant.OriginalPosition + _projectileDirection * _projectileDistanceCovered;
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
                        return (_combatant.CombatSprite.IsPlaybackComplete && _item.SpellSprite.IsPlaybackComplete);

                    case CombatActionStage.Advancing: // ready to execute?
                        if (_item.SpellSprite.IsPlaybackComplete || (_projectileDistanceCovered >= _totalProjectileDistance))
                        {
                            _projectileDistanceCovered = _totalProjectileDistance;
                            return true;
                        }
                        return false;

                    case CombatActionStage.Executing: // ready to return?
                        return _item.SpellSprite.IsPlaybackComplete;
                }

                // fall through to the base behavior
                return base.IsReadyForNextStage;
            }
        }

        /// <summary>
        /// The heuristic used to compare actions of this type to similar ones.
        /// </summary>
        public override int Heuristic => Item.TargetEffectRange.HealthPointsRange.Average;

        /// <summary>
        /// Constructs a new ItemCombatAction object.
        /// </summary>
        /// <param name="character">The combatant performing the action.</param>
        public ItemCombatAction(Combatant combatant, Item item) : base(combatant)
        {
            // check the parameter
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if ((item.Usage & Item.ItemUsage.Combat) == 0)
            {
                throw new ArgumentException("Combat items must have Combat usage.");
            }

            // assign the parameter
            _item = item;
            _adjacentTargets = _item.AdjacentTargets;
        }

        /// <summary>
        /// Start executing the combat action.
        /// </summary>
        public override void Start()
        {
            // play the creation sound effect
            AudioManager.PlayCue(_item.UsingCueName);

            base.Start();
        }

        /// <summary>
        /// Updates the action over time.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // update the animations
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _item.SpellSprite.UpdateAnimation(elapsedSeconds);
            if (_item.Overlay != null)
            {
                _item.Overlay.UpdateAnimation(elapsedSeconds);
                if (!_item.Overlay.IsPlaybackComplete && Target.CombatSprite.IsPlaybackComplete)
                {
                    _item.Overlay.StopAnimation();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw any elements of the action that are independent of the character.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw the item projectile
            if (!_item.SpellSprite.IsPlaybackComplete)
            {
                if (stage == CombatActionStage.Advancing)
                {
                    _item.SpellSprite.Draw(spriteBatch, itemSpritePosition, 0f,
                        _projectileSpriteEffect);
                }
                else
                {
                    _item.SpellSprite.Draw(spriteBatch, itemSpritePosition, 0f);
                }
            }

            // draw the item overlay
            if (!_item.Overlay.IsPlaybackComplete)
            {
                _item.Overlay.Draw(spriteBatch, Target.Position, 0f);
            }

            base.Draw(gameTime, spriteBatch);
        }

        /// <summary>
        /// Calculate the item damage done by the given combatant and item.
        /// </summary>
        public static StatisticsValue CalculateItemDamage(Combatant combatant, Item item)
        {
            // check the parameters
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            // generate a new effect value - no stats are involved for items
            return item.TargetEffectRange.GenerateValue(Session.Random);
        }
    }
}
