using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Characters;
using RolePlayingGame.Engine;
using RolePlayingGame.GearObjects;

namespace RolePlayingGame.Quests
{
    public class QuestRequirement<T> : ContentEntry<T> where T : ContentObject
    {
        /// <summary>
        /// The quantity of the content entry that has been acquired.
        /// </summary>
        private int completedCount;

        /// <summary>
        /// The quantity of the content entry that has been acquired.
        /// </summary>
        [ContentSerializerIgnore]
        public int CompletedCount
        {
            get { return completedCount; }
            set { completedCount = value; }
        }

        /// <summary>
        /// Reads a QuestRequirement object from the content pipeline.
        /// </summary>
        public class QuestRequirementReader : ContentTypeReader<QuestRequirement<T>>
        {
            /// <summary>
            /// Reads a QuestRequirement object from the content pipeline.
            /// </summary>
            protected override QuestRequirement<T> Read(ContentReader input,
                QuestRequirement<T> existingInstance)
            {
                QuestRequirement<T> requirement = existingInstance;
                if (requirement == null)
                {
                    requirement = new QuestRequirement<T>();
                }

                input.ReadRawObject<ContentEntry<T>>(requirement as ContentEntry<T>);
                if (typeof(T) == typeof(Gear))
                {
                    requirement.Content = input.ContentManager.Load<T>(
                        System.IO.Path.Combine("Gear", requirement.ContentName));
                }
                else if (typeof(T) == typeof(Monster))
                {
                    requirement.Content = input.ContentManager.Load<T>(
                        System.IO.Path.Combine(@"Characters\Monsters",
                        requirement.ContentName));
                }

                return requirement;
            }
        }
    }
}
