﻿using RolePlayingGame.Data;

namespace RolePlayingGame.Combat.Actions
{
    public class DefendCombatAction : CombatAction
    {
        /// <summary>
        /// Returns true if the action is offensive, targeting the opponents.
        /// </summary>
        public override bool IsOffensive => false;

        /// <summary>
        /// Returns true if this action requires a target.
        /// </summary>
        public override bool IsTargetNeeded => false;

        /// <summary>
        /// Starts a new combat stage.  Called right after the stage changes.
        /// </summary>
        /// <remarks>The stage never changes into NotStarted.</remarks>
        protected override void StartStage()
        {
            switch (stage)
            {
                case CombatActionStage.Preparing: // called from Start()
                    Combatant.CombatSprite.PlayAnimation("Defend");
                    break;

                case CombatActionStage.Executing:
                    Combatant.CombatEffects.AddStatistics(new StatisticsValue(0, 0, 0, Combatant.Character.CharacterStatistics.PhysicalDefense, 0, Combatant.Character.CharacterStatistics.MagicalDefense), 1);
                    break;
            }
        }

        /// <summary>
        /// The heuristic used to compare actions of this type to similar ones.
        /// </summary>
        public override int Heuristic => 0;

        /// <summary>
        /// Constructs a new DefendCombatAction object.
        /// </summary>
        /// <param name="character">The character performing the action.</param>
        public DefendCombatAction(Combatant combatant) : base(combatant) { }
    }
}
