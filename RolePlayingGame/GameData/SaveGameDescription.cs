using System;

namespace RolePlayingGame.GameData
{
    [Serializable]
    public class SaveGameDescription
    {
        /// <summary>
        /// The name of the save file with the game data. 
        /// </summary>
        public string FileName;

        /// <summary>
        /// The short description of how far the player has progressed in the game.
        /// </summary>
        /// <remarks>Here, it's the current quest.</remarks>
        public string ChapterName;

        /// <summary>
        /// The short description of how far the player has progressed in the game.
        /// </summary>
        /// <remarks>Here, it's the time played.</remarks>
        public string Description;
    }
}
