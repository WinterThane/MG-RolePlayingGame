using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Engine;
using RolePlayingGame.MapObjects;

namespace RolePlayingGame.WorldObjects
{
    public class WorldEntry<T> : MapEntry<T> where T : ContentObject
    {
        /// <summary>
        /// The name of the map where the content is added.
        /// </summary>
        private string mapContentName;

        /// <summary>
        /// The name of the map where the content is added.
        /// </summary>
        public string MapContentName
        {
            get { return mapContentName; }
            set { mapContentName = value; }
        }

        /// <summary>
        /// Reads a WorldEntry object from the content pipeline.
        /// </summary>
        public class WorldEntryReader : ContentTypeReader<WorldEntry<T>>
        {
            /// <summary>
            /// Reads a WorldEntry object from the content pipeline.
            /// </summary>
            protected override WorldEntry<T> Read(ContentReader input,
                WorldEntry<T> existingInstance)
            {
                WorldEntry<T> desc = existingInstance;
                if (desc == null)
                {
                    desc = new WorldEntry<T>();
                }

                input.ReadRawObject<MapEntry<T>>(desc as MapEntry<T>);
                desc.MapContentName = input.ReadString();

                return desc;
            }
        }
    }
}
