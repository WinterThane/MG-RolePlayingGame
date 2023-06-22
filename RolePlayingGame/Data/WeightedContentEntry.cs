using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Engine;

namespace RolePlayingGame.Data
{
    public class WeightedContentEntry<T> : ContentEntry<T> where T : ContentObject
    {
        /// <summary>
        /// The weight of this content within the group, for statistical distribution.
        /// </summary>
        private int _weight;

        /// <summary>
        /// The weight of this content within the group, for statistical distribution.
        /// </summary>
        public int Weight
        {
            get => _weight;
            set => _weight = value;
        }

        /// <summary>
        /// Reads a WeightedContentEntry object from the content pipeline.
        /// </summary>
        public class WeightedContentEntryReader : ContentTypeReader<WeightedContentEntry<T>>
        {
            /// <summary>
            /// Reads a WeightedContentEntry object from the content pipeline.
            /// </summary>
            protected override WeightedContentEntry<T> Read(ContentReader input, WeightedContentEntry<T> existingInstance)
            {
                WeightedContentEntry<T> entry = existingInstance;
                if (entry == null)
                {
                    entry = new WeightedContentEntry<T>();
                }

                input.ReadRawObject<ContentEntry<T>>(entry as ContentEntry<T>);

                entry.Weight = input.ReadInt32();

                return entry;
            }
        }
    }
}
