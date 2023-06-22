using RolePlayingGame.Characters;
using RolePlayingGame.MapObjects;
using System;

namespace RolePlayingGame.ScreensManager.Screens.GameScreens
{
    public abstract class NpcScreen<T> : DialogueScreen where T : Character
    {
        protected MapEntry<T> _mapEntry = null;
        protected Character _character = null;

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
            _mapEntry = mapEntry;
            _character = mapEntry.Content;

            if (_character == null)
            {
                throw new ArgumentNullException("NpcScreen requires a MapEntry with a character.");
            }
            TitleText = _character.Name;
        }
    }
}
