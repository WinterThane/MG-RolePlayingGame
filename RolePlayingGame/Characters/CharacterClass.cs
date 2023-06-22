using RolePlayingGame.Engine;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Data;
using RolePlayingGame.Spells;

namespace RolePlayingGame.Characters
{
    public class CharacterClass : ContentObject
    {
        /// <summary>
        /// The name of the character class.
        /// </summary>
        private string _name;

        /// <summary>
        /// The name of the character class.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// The initial statistics of characters that use this class.
        /// </summary>
        private StatisticsValue _initialStatistics = new();

        /// <summary>
        /// The initial statistics of characters that use this class.
        /// </summary>
        public StatisticsValue InitialStatistics
        {
            get => _initialStatistics;
            set => _initialStatistics = value;
        }

        /// <summary>
        /// Statistics changes for leveling up characters that use this class.
        /// </summary>
        private CharacterLevelingStatistics _levelingStatistics;

        /// <summary>
        /// Statistics changes for leveling up characters that use this class.
        /// </summary>
        public CharacterLevelingStatistics LevelingStatistics
        {
            get => _levelingStatistics;
            set => _levelingStatistics = value;
        }

        /// <summary>
        /// Entries of the requirements and rewards for each level of this class.
        /// </summary>
        private List<CharacterLevelDescription> _levelEntries = new();

        /// <summary>
        /// Entries of the requirements and rewards for each level of this class.
        /// </summary>
        public List<CharacterLevelDescription> LevelEntries
        {
            get => _levelEntries;
            set => _levelEntries = value;
        }

        /// <summary>
        /// Calculate the statistics of a character of this class and the given level.
        /// </summary>
        public StatisticsValue GetStatisticsForLevel(int characterLevel)
        {
            // check the parameter
            if (characterLevel <= 0)
            {
                throw new ArgumentOutOfRangeException("characterLevel");
            }

            // start with the initial statistics
            StatisticsValue output = _initialStatistics;

            // add each level of leveling statistics
            for (int i = 1; i < characterLevel; i++)
            {
                if ((_levelingStatistics.LevelsPerHealthPointsIncrease > 0) && ((i % _levelingStatistics.LevelsPerHealthPointsIncrease) == 0))
                {
                    output.HealthPoints += _levelingStatistics.HealthPointsIncrease;
                }
                if ((_levelingStatistics.LevelsPerMagicPointsIncrease > 0) && ((i % _levelingStatistics.LevelsPerMagicPointsIncrease) == 0))
                {
                    output.MagicPoints += _levelingStatistics.MagicPointsIncrease;
                }
                if ((_levelingStatistics.LevelsPerPhysicalOffenseIncrease > 0) && ((i % _levelingStatistics.LevelsPerPhysicalOffenseIncrease) == 0))
                {
                    output.PhysicalOffense += _levelingStatistics.PhysicalOffenseIncrease;
                }
                if ((_levelingStatistics.LevelsPerPhysicalDefenseIncrease > 0) && ((i % _levelingStatistics.LevelsPerPhysicalDefenseIncrease) == 0))
                {
                    output.PhysicalDefense += _levelingStatistics.PhysicalDefenseIncrease;
                }
                if ((_levelingStatistics.LevelsPerMagicalOffenseIncrease > 0) && ((i % _levelingStatistics.LevelsPerMagicalOffenseIncrease) == 0))
                {
                    output.MagicalOffense += _levelingStatistics.MagicalOffenseIncrease;
                }
                if ((_levelingStatistics.LevelsPerMagicalDefenseIncrease > 0) && ((i % _levelingStatistics.LevelsPerMagicalDefenseIncrease) == 0))
                {
                    output.MagicalDefense += _levelingStatistics.MagicalDefenseIncrease;
                }
            }

            return output;
        }

        /// <summary>
        /// Build a list of all spells available to a character 
        /// of this class and the given level.
        /// </summary>
        public List<Spell> GetAllSpellsForLevel(int characterLevel)
        {
            // check the parameter
            if (characterLevel <= 0)
            {
                throw new ArgumentOutOfRangeException("characterLevel");
            }

            // go through each level and add the spells to the output list
            List<Spell> spells = new();

            for (int i = 0; i < characterLevel; i++)
            {
                if (i >= _levelEntries.Count)
                {
                    break;
                }

                // add new spells, and level up existing ones
                foreach (Spell spell in _levelEntries[i].Spells)
                {
                    Spell existingSpell = spells.Find(delegate (Spell testSpell)
                    {
                        return spell.AssetName == testSpell.AssetName;
                    });

                    if (existingSpell == null)
                    {
                        spells.Add(spell.Clone() as Spell);
                    }
                    else
                    {
                        existingSpell.Level++;
                    }
                }
            }

            return spells;
        }

        /// <summary>
        /// The base experience value of Npcs of this character class.
        /// </summary>
        /// <remarks>Used for calculating combat rewards.</remarks>
        private int _baseExperienceValue;

        /// <summary>
        /// The base experience value of Npcs of this character class.
        /// </summary>
        /// <remarks>Used for calculating combat rewards.</remarks>
        public int BaseExperienceValue
        {
            get => _baseExperienceValue;
            set => _baseExperienceValue = value;
        }

        /// <summary>
        /// The base gold value of Npcs of this character class.
        /// </summary>
        /// <remarks>Used for calculating combat rewards.</remarks>
        private int _baseGoldValue;

        /// <summary>
        /// The base gold value of Npcs of this character class.
        /// </summary>
        /// <remarks>Used for calculating combat rewards.</remarks>
        public int BaseGoldValue
        {
            get { return _baseGoldValue; }
            set { _baseGoldValue = value; }
        }

        /// <summary>
        /// Reads a CharacterClass object from the content pipeline.
        /// </summary>
        public class CharacterClassReader : ContentTypeReader<CharacterClass>
        {
            /// <summary>
            /// Reads a CharacterClass object from the content pipeline.
            /// </summary>
            protected override CharacterClass Read(ContentReader input, CharacterClass existingInstance)
            {
                CharacterClass characterClass = existingInstance;
                if (characterClass == null)
                {
                    characterClass = new CharacterClass();
                }

                characterClass.AssetName = input.AssetName;

                characterClass.Name = input.ReadString();
                characterClass.InitialStatistics = input.ReadObject<StatisticsValue>();
                characterClass.LevelingStatistics = input.ReadObject<CharacterLevelingStatistics>();
                characterClass.LevelEntries.AddRange(input.ReadObject<List<CharacterLevelDescription>>());
                characterClass.BaseExperienceValue = input.ReadInt32();
                characterClass.BaseGoldValue = input.ReadInt32();

                return characterClass;
            }
        }
    }
}
