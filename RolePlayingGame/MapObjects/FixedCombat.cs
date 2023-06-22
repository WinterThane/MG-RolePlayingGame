using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Characters;
using RolePlayingGame.Engine;
using RolePlayingGame.WorldObjects;
using System.Collections.Generic;
using System.IO;

namespace RolePlayingGame.MapObjects
{
    public class FixedCombat : WorldObject
    {
        /// <summary>
        /// The content name and quantities of the monsters in this encounter.
        /// </summary>
        private List<ContentEntry<Monster>> _entriesList = new();

        /// <summary>
        /// The content name and quantities of the monsters in this encounter.
        /// </summary>
        public List<ContentEntry<Monster>> Entries
        {
            get => _entriesList;
            set => _entriesList = value;
        }

        /// <summary>
        /// Reads a FixedCombat object from the content pipeline.
        /// </summary>
        public class FixedCombatReader : ContentTypeReader<FixedCombat>
        {
            /// <summary>
            /// Reads a FixedCombat object from the content pipeline.
            /// </summary>
            protected override FixedCombat Read(ContentReader input, FixedCombat existingInstance)
            {
                FixedCombat fixedCombat = existingInstance;
                if (fixedCombat == null)
                {
                    fixedCombat = new FixedCombat();
                }

                input.ReadRawObject<WorldObject>(fixedCombat as WorldObject);
                fixedCombat.Entries.AddRange(input.ReadObject<List<ContentEntry<Monster>>>());
                foreach (ContentEntry<Monster> fixedCombatEntry in fixedCombat.Entries)
                {
                    fixedCombatEntry.Content = input.ContentManager.Load<Monster>(Path.Combine("Characters/Monsters", fixedCombatEntry.ContentName));
                }

                return fixedCombat;
            }
        }
    }
}