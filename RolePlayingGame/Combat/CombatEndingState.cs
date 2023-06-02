namespace RolePlayingGame.Combat
{
    enum CombatEndingState
    {
        /// <summary>
        /// All of the monsters died in combat.
        /// </summary>
        Victory,

        /// <summary>
        /// The party successfully fled from combat.
        /// </summary>
        Fled,

        /// <summary>
        /// All of the players died in combat.
        /// </summary>
        Loss,
    }
}
