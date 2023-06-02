using RolePlayingGame.Characters;
using RolePlayingGame.MapObjects;
using System;

namespace RolePlayingGame.ScreensManager.Screens.GameScreens
{
    public abstract class NpcScreen<T> : DialogueScreen where T : Character
    {
        protected MapEntry<T> mapEntry = null;
        protected Character character = null;

        /// <summary>
        /// Create a new NpcScreen object.
        /// </summary>
        /// <param name="mapEntry"></param>
        public NpcScreen(MapEntry<T> mapEntry) : base()
        {
            if (mapEntry == null)
            {
                throw new ArgumentNullException("mapEntry");
            }
            this.mapEntry = mapEntry;
            this.character = mapEntry.Content as Character;
            if (this.character == null)
            {
                throw new ArgumentNullException(
                    "NpcScreen requires a MapEntry with a character.");
            }
            TitleText = character.Name;
        }
    }
}
