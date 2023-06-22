using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace RolePlayingGame.GameData
{
    public class GameStartDescription
    {
        /// <summary>
        /// The content name of the  map for a new game.
        /// </summary>
        private string _mapContentName;

        /// <summary>
        /// The content name of the  map for a new game.
        /// </summary>
        public string MapContentName
        {
            get => _mapContentName;
            set => _mapContentName = value;
        }

        /// <summary>
        /// The content names of the players in the party from the beginning.
        /// </summary>
        private List<string> _playerContentNames = new();

        /// <summary>
        /// The content names of the players in the party from the beginning.
        /// </summary>
        public List<string> PlayerContentNames
        {
            get => _playerContentNames;
            set => _playerContentNames = value;
        }

        /// <summary>
        /// The quest line in action when the game starts.
        /// </summary>
        /// <remarks>The first quest will be started before the world is shown.</remarks>
        private string _questLineContentName;

        /// <summary>
        /// The quest line in action when the game starts.
        /// </summary>
        /// <remarks>The first quest will be started before the world is shown.</remarks>
        [ContentSerializer(Optional = true)]
        public string QuestLineContentName
        {
            get => _questLineContentName;
            set => _questLineContentName = value;
        }

        /// <summary>
        /// Read a GameStartDescription object from the content pipeline.
        /// </summary>
        public class GameStartDescriptionReader : ContentTypeReader<GameStartDescription>
        {
            protected override GameStartDescription Read(ContentReader input, GameStartDescription existingInstance)
            {
                GameStartDescription desc = existingInstance;
                if (desc == null)
                {
                    desc = new GameStartDescription();
                }

                desc.MapContentName = input.ReadString();
                desc.PlayerContentNames.AddRange(input.ReadObject<List<string>>());
                desc.QuestLineContentName = input.ReadString();

                return desc;
            }
        }
    }
}
