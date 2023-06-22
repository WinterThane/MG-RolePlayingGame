using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Characters;
using RolePlayingGame.Data;
using RolePlayingGame.Engine;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace RolePlayingGame.MapObjects
{
    public class RandomCombat
    {
        /// <summary>
        /// The chance of a random combat starting with each step, from 1 to 100.
        /// </summary>
        private int _combatProbability;

        /// <summary>
        /// The chance of a random combat starting with each step, from 1 to 100.
        /// </summary>
        public int CombatProbability
        {
            get => _combatProbability;
            set => _combatProbability = value;
        }

        /// <summary>
        /// The chance of a successful escape from a random combat, from 1 to 100.
        /// </summary>
        private int _fleeProbability;

        /// <summary>
        /// The chance of a successful escape from a random combat, from 1 to 100.
        /// </summary>
        public int FleeProbability
        {
            get => _fleeProbability;
            set => _fleeProbability = value;
        }

        /// <summary>
        /// The range of possible quantities of monsters in the random encounter.
        /// </summary>
        private Int32Range _monsterCountRange;

        /// <summary>
        /// The range of possible quantities of monsters in the random encounter.
        /// </summary>
        public Int32Range MonsterCountRange
        {
            get => _monsterCountRange;
            set => _monsterCountRange = value;
        }

        /// <summary>
        /// The monsters that might be in the random encounter, 
        /// along with quantity and weight.
        /// </summary>
        private List<WeightedContentEntry<Monster>> _entriesList = new();

        /// <summary>
        /// The monsters that might be in the random encounter, 
        /// along with quantity and weight.
        /// </summary>
        public List<WeightedContentEntry<Monster>> EntriesList
        {
            get => _entriesList;
            set => _entriesList = value;
        }

        /// <summary>
        /// Reads a RandomCombat object from the content pipeline.
        /// </summary>
        public class RandomCombatReader : ContentTypeReader<RandomCombat>
        {
            protected override RandomCombat Read(ContentReader input, RandomCombat existingInstance)
            {
                RandomCombat randomCombat = existingInstance;
                if (randomCombat == null)
                {
                    randomCombat = new RandomCombat();
                }

                randomCombat.CombatProbability = input.ReadInt32();
                randomCombat.FleeProbability = input.ReadInt32();
                randomCombat.MonsterCountRange = input.ReadObject<Int32Range>();
                randomCombat.EntriesList.AddRange(input.ReadObject<List<WeightedContentEntry<Monster>>>());
                foreach (ContentEntry<Monster> randomCombatEntry in randomCombat.EntriesList)
                {
                    randomCombatEntry.Content = input.ContentManager.Load<Monster>(Path.Combine("Characters/Monsters", randomCombatEntry.ContentName));
                }

                return randomCombat;
            }
        }
    }
}
