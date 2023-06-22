using Microsoft.Xna.Framework;
using RolePlayingGame.Animations;
using RolePlayingGame.Characters;
using RolePlayingGame.Data;
using RolePlayingGame.Spells;
using System;
using static RolePlayingGame.Characters.Character;

namespace RolePlayingGame.Combat
{
    public class CombatantPlayer : Combatant
    {
        /// <summary>
        /// The Player object encapsulated by this object.
        /// </summary>
        private Player _player;

        /// <summary>
        /// The Player object encapsulated by this object.
        /// </summary>
        public Player Player => _player;

        /// <summary>
        /// The character encapsulated by this combatant.
        /// </summary>
        public override FightingCharacter Character => _player;

        /// <summary>
        /// The current state of this combatant.
        /// </summary>
        public override CharacterState State
        {
            get => _player.State;
            set
            {
                if (value == _player.State)
                {
                    return;
                }
                _player.State = value;
                switch (_player.State)
                {
                    case CharacterState.Idle:
                        CombatSprite.PlayAnimation("Idle");
                        break;

                    case CharacterState.Hit:
                        CombatSprite.PlayAnimation("Hit");
                        break;

                    case CharacterState.Dying:
                        _player.StatisticsModifiers.HealthPoints =
                            -1 * _player.CharacterStatistics.HealthPoints;
                        CombatSprite.PlayAnimation("Die");
                        break;
                }
            }
        }

        /// <summary>
        /// Accessor for the combat sprite for this combatant.
        /// </summary>
        public override AnimatingSprite CombatSprite => _player.CombatSprite;

        /// <summary>
        /// The current statistics of this combatant.
        /// </summary>
        public override StatisticsValue Statistics => _player.CurrentStatistics + CombatEffects.TotalStatistics;

        /// <summary>
        /// Heals the combatant by the given amount.
        /// </summary>
        public override void Heal(StatisticsValue healingStatistics, int duration)
        {
            if (duration > 0)
            {
                CombatEffects.AddStatistics(healingStatistics, duration);
            }
            else
            {
                _player.StatisticsModifiers += healingStatistics;
                _player.StatisticsModifiers.ApplyMaximum(new StatisticsValue());
            }
            base.Heal(healingStatistics, duration);
        }

        /// <summary>
        /// Damages the combatant by the given amount.
        /// </summary>
        public override void Damage(StatisticsValue damageStatistics, int duration)
        {
            if (duration > 0)
            {
                CombatEffects.AddStatistics(new StatisticsValue() - damageStatistics,
                    duration);
            }
            else
            {
                _player.StatisticsModifiers -= damageStatistics;
                _player.StatisticsModifiers.ApplyMaximum(new StatisticsValue());
            }
            base.Damage(damageStatistics, duration);
        }

        /// <summary>
        /// Pay the cost for the given spell.
        /// </summary>
        /// <returns>True if the cost could be paid (and therefore was paid).</returns>
        public override bool PayCostForSpell(Spell spell)
        {
            // check the parameter.
            if (spell == null)
            {
                throw new ArgumentNullException("spell");
            }

            // check the requirements
            if (Statistics.MagicPoints < spell.MagicPointCost)
            {
                return false;
            }

            // reduce the player's magic points by the spell's cost
            _player.StatisticsModifiers.MagicPoints -= spell.MagicPointCost;

            return true;
        }

        /// <summary>
        /// Construct a new CombatantPlayer object containing the given player.
        /// </summary>
        public CombatantPlayer(Player player) : base()
        {
            // check the parameter
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            // assign the parameters
            _player = player;

            // if the player starts dead, make sure the sprite is already "dead"
            if (IsDeadOrDying)
            {
                if (Statistics.HealthPoints > 0)
                {
                    State = CharacterState.Idle;
                }
                else
                {
                    CombatSprite.PlayAnimation("Die");
                    CombatSprite.AdvanceToEnd();
                }
            }
            else
            {
                State = CharacterState.Idle;
                CombatSprite.PlayAnimation("Idle");
            }
        }

        /// <summary>
        /// Update the player for this frame.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
