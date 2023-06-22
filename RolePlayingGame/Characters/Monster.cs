using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Content;
using RolePlayingGame.GearObjects;

namespace RolePlayingGame.Characters
{
    public class Monster : FightingCharacter
    {
        /// <summary>
        /// The chance that this monster will defend instead of attack.
        /// </summary>
        private int _defendPercentage;

        /// <summary>
        /// The chance that this monster will defend instead of attack.
        /// </summary>
        public int DefendPercentage
        {
            get => _defendPercentage;
            set => _defendPercentage = value > 100 ? 100 : (value < 0 ? 0 : value);
        }

        /// <summary>
        /// The possible gear drops from this monster.
        /// </summary>
        private List<GearDrop> _gearDrops = new();

        /// <summary>
        /// The possible gear drops from this monster.
        /// </summary>
        public List<GearDrop> GearDrops
        {
            get => _gearDrops;
            set => _gearDrops = value;
        }

        public int CalculateGoldReward(Random random) => CharacterClass.BaseGoldValue * CharacterLevel;

        public int CalculateExperienceReward(Random random) => CharacterClass.BaseExperienceValue * CharacterLevel;

        public List<string> CalculateGearDrop(Random random)
        {
            List<string> gearRewards = new();

            Random useRandom = random;
            if (useRandom == null)
            {
                useRandom = new Random();
            }

            foreach (GearDrop gearDrop in GearDrops)
            {
                if (random.Next(100) < gearDrop.DropPercentage)
                {
                    gearRewards.Add(gearDrop.GearName);
                }
            }

            return gearRewards;
        }

        /// <summary>
        /// Reads a Monster object from the content pipeline.
        /// </summary>
        public class MonsterReader : ContentTypeReader<Monster>
        {
            protected override Monster Read(ContentReader input, Monster existingInstance)
            {
                Monster monster = existingInstance;
                if (monster == null)
                {
                    monster = new Monster();
                }

                input.ReadRawObject<FightingCharacter>(monster as FightingCharacter);

                monster.DefendPercentage = input.ReadInt32();
                monster.GearDrops.AddRange(input.ReadObject<List<GearDrop>>());

                return monster;
            }
        }
    }
}
