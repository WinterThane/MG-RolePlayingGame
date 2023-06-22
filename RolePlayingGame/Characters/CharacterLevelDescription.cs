using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Spells;
using System.Collections.Generic;

namespace RolePlayingGame.Characters
{
    public class CharacterLevelDescription
    {
        /// <summary>
        /// The amount of additional experience necessary to achieve this level.
        /// </summary>
        private int _experiencePoints;

        /// <summary>
        /// The amount of additional experience necessary to achieve this level.
        /// </summary>
        public int ExperiencePoints
        {
            get => _experiencePoints;
            set => _experiencePoints = value;
        }

        /// <summary>
        /// The content names of the spells given to the character 
        /// when it reaches this level.
        /// </summary>
        private List<string> _spellContentNames = new();

        /// <summary>
        /// The content names of the spells given to the character 
        /// when it reaches this level.
        /// </summary>
        public List<string> SpellContentNames
        {
            get => _spellContentNames;
            set => _spellContentNames = value;
        }

        /// <summary>
        /// Spells given to the character when it reaches this level.
        /// </summary>
        private List<Spell> _spells = new();

        /// <summary>
        /// Spells given to the character when it reaches this level.
        /// </summary>
        [ContentSerializerIgnore]
        public List<Spell> Spells
        {
            get => _spells;
            set => _spells = value;
        }

        /// <summary>
        /// Read a CharacterLevelDescription object from the content pipeline.
        /// </summary>
        public class CharacterLevelDescriptionReader : ContentTypeReader<CharacterLevelDescription>
        {
            /// <summary>
            /// Read a CharacterLevelDescription object from the content pipeline.
            /// </summary>
            protected override CharacterLevelDescription Read(ContentReader input, CharacterLevelDescription existingInstance)
            {
                CharacterLevelDescription desc = existingInstance;
                if (desc == null)
                {
                    desc = new CharacterLevelDescription();
                }

                desc.ExperiencePoints = input.ReadInt32();
                desc.SpellContentNames.AddRange(input.ReadObject<List<string>>());

                // load all of the spells immediately
                foreach (string spellContentName in desc.SpellContentNames)
                {
                    desc._spells.Add(input.ContentManager.Load<Spell>(System.IO.Path.Combine("Spells", spellContentName)));
                }

                return desc;
            }
        }
    }
}
