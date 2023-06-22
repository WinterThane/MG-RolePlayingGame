using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Animations;
using RolePlayingGame.Characters;
using RolePlayingGame.Engine;
using RolePlayingGame.GearObjects;
using RolePlayingGame.MapObjects;
using RolePlayingGame.WorldObjects;
using System;
using System.Collections.Generic;
using System.IO;

namespace RolePlayingGame.Quests
{
    public class Quest : ContentObject, ICloneable
    {
        /// <summary>
        /// The possible stages of a quest.
        /// </summary>
        public enum QuestStage
        {
            NotStarted,
            InProgress,
            RequirementsMet,
            Completed
        };

        /// <summary>
        /// The current stage of this quest.
        /// </summary>
        private QuestStage _stage = QuestStage.NotStarted;

        /// <summary>
        /// The current stage of this quest.
        /// </summary>
        [ContentSerializerIgnore]
        public QuestStage Stage
        {
            get => _stage;
            set => _stage = value;
        }

        /// <summary>
        /// The name of the quest.
        /// </summary>
        private string _name;

        /// <summary>
        /// The name of the quest.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// A description of the quest.
        /// </summary>
        private string _description;

        /// <summary>
        /// A description of the quest.
        /// </summary>
        public string Description
        {
            get => _description;
            set => _description = value;
        }

        /// <summary>
        /// A message describing the objective of the quest, 
        /// presented when the player receives the quest.
        /// </summary>
        private string _objectiveMessage;

        /// <summary>
        /// A message describing the objective of the quest, 
        /// presented when the player receives the quest.
        /// </summary>
        public string ObjectiveMessage
        {
            get => _objectiveMessage;
            set => _objectiveMessage = value;
        }

        /// <summary>
        /// A message announcing the completion of the quest, 
        /// presented when the player reaches the goals of the quest.
        /// </summary>
        private string _completionMessage;

        public string CompletionMessage
        {
            get => _completionMessage;
            set => _completionMessage = value;
        }

        /// <summary>
        /// The gear that the player must have to finish the quest.
        /// </summary>
        private List<QuestRequirement<Gear>> _gearRequirementsList = new();

        /// <summary>
        /// The gear that the player must have to finish the quest.
        /// </summary>
        public List<QuestRequirement<Gear>> GearRequirementsList
        {
            get => _gearRequirementsList;
            set => _gearRequirementsList = value;
        }

        /// <summary>
        /// The monsters that must be killed to finish the quest.
        /// </summary>
        private List<QuestRequirement<Monster>> _monsterRequirementsList = new();

        /// <summary>
        /// The monsters that must be killed to finish the quest.
        /// </summary>
        public List<QuestRequirement<Monster>> MonsterRequirementsList
        {
            get => _monsterRequirementsList;
            set => _monsterRequirementsList = value;
        }

        /// <summary>
        /// Returns true if all requirements for this quest have been met.
        /// </summary>
        public bool AreRequirementsMet
        {
            get
            {
                foreach (QuestRequirement<Gear> gearRequirement in _gearRequirementsList)
                {
                    if (gearRequirement.CompletedCount < gearRequirement.Count)
                    {
                        return false;
                    }
                }
                foreach (QuestRequirement<Monster> monsterRequirement in _monsterRequirementsList)
                {
                    if (monsterRequirement.CompletedCount < monsterRequirement.Count)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// The fixed combat encounters added to the world when this quest is active.
        /// </summary>
        private List<WorldEntry<FixedCombat>> _fixedCombatEntriesList = new();

        /// <summary>
        /// The fixed combat encounters added to the world when this quest is active.
        /// </summary>
        public List<WorldEntry<FixedCombat>> FixedCombatEntriesList
        {
            get => _fixedCombatEntriesList;
            set => _fixedCombatEntriesList = value;
        }

        /// <summary>
        /// The chests added to thew orld when this quest is active.
        /// </summary>
        private List<WorldEntry<Chest>> _chestEntriesList = new();

        /// <summary>
        /// The chests added to thew orld when this quest is active.
        /// </summary>
        public List<WorldEntry<Chest>> ChestEntriesList
        {
            get => _chestEntriesList;
            set => _chestEntriesList = value;
        }

        /// <summary>
        /// The map with the destination Npc, if any.
        /// </summary>
        private string _destinationMapContentName;

        /// <summary>
        /// The map with the destination Npc, if any.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public string DestinationMapContentName
        {
            get => _destinationMapContentName;
            set => _destinationMapContentName = value;
        }

        /// <summary>
        /// The Npc that the party must visit to finish the quest, if any.
        /// </summary>
        private string _destinationNpcContentName;

        /// <summary>
        /// The Npc that the party must visit to finish the quest, if any.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public string DestinationNpcContentName
        {
            get => _destinationNpcContentName;
            set => _destinationNpcContentName = value;
        }

        /// <summary>
        /// The message shown when the party is eligible to complete the quest, if any.
        /// </summary>
        private string _destinationObjectiveMessage;

        /// <summary>
        /// The message shown when the party is eligible to complete the quest, if any.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public string DestinationObjectiveMessage
        {
            get => _destinationObjectiveMessage;
            set => _destinationObjectiveMessage = value;
        }

        /// <summary>
        /// The number of experience points given to each party member as a reward.
        /// </summary>
        private int _experienceReward;

        /// <summary>
        /// The number of experience points given to each party member as a reward.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public int ExperienceReward
        {
            get => _experienceReward;
            set => _experienceReward = value;
        }

        /// <summary>
        /// The amount of gold given to the party as a reward.
        /// </summary>
        private int _goldReward;

        /// <summary>
        /// The amount of gold given to the party as a reward.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public int GoldReward
        {
            get => _goldReward;
            set => _goldReward = value;
        }

        /// <summary>
        /// The content names of the gear given to the party as a reward.
        /// </summary>
        private List<string> _gearRewardContentNamesList = new();

        /// <summary>
        /// The content names of the gear given to the party as a reward.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public List<string> GearRewardContentNamesList
        {
            get => _gearRewardContentNamesList;
            set => _gearRewardContentNamesList = value;
        }

        /// <summary>
        /// The gear given to the party as a reward.
        /// </summary>
        private List<Gear> _gearRewardsList = new();

        /// <summary>
        /// The gear given to the party as a reward.
        /// </summary>
        [ContentSerializerIgnore]
        public List<Gear> GearRewardsList
        {
            get => _gearRewardsList;
            set => _gearRewardsList = value;
        }

        /// <summary>
        /// Reads a Quest object from the content pipeline.
        /// </summary>
        public class QuestReader : ContentTypeReader<Quest>
        {
            /// <summary>
            /// Reads a Quest object from the content pipeline.
            /// </summary>
            protected override Quest Read(ContentReader input, Quest existingInstance)
            {
                Quest quest = existingInstance;
                if (quest == null)
                {
                    quest = new Quest();
                }

                quest.AssetName = input.AssetName;

                quest.Name = input.ReadString();
                quest.Description = input.ReadString();
                quest.ObjectiveMessage = input.ReadString();
                quest.CompletionMessage = input.ReadString();

                quest.GearRequirementsList.AddRange(input.ReadObject<List<QuestRequirement<Gear>>>());
                quest.MonsterRequirementsList.AddRange(input.ReadObject<List<QuestRequirement<Monster>>>());

                // load the fixed combat entries
                Random random = new();
                quest.FixedCombatEntriesList.AddRange(input.ReadObject<List<WorldEntry<FixedCombat>>>());
                foreach (WorldEntry<FixedCombat> fixedCombatEntry in quest.FixedCombatEntriesList)
                {
                    fixedCombatEntry.Content = input.ContentManager.Load<FixedCombat>(Path.Combine("Maps/FixedCombats", fixedCombatEntry.ContentName));
                    // clone the map sprite in the entry, as there may be many entries
                    // per FixedCombat
                    fixedCombatEntry.MapSprite = fixedCombatEntry.Content.Entries[0].Content.MapSprite.Clone() as AnimatingSprite;
                    // play the idle animation
                    fixedCombatEntry.MapSprite.PlayAnimation("Idle", fixedCombatEntry.Direction);
                    // advance in a random amount so the animations aren't synchronized
                    fixedCombatEntry.MapSprite.UpdateAnimation(4f * (float)random.NextDouble());
                }

                quest.ChestEntriesList.AddRange(input.ReadObject<List<WorldEntry<Chest>>>());
                foreach (WorldEntry<Chest> chestEntry in quest.ChestEntriesList)
                {
                    chestEntry.Content = input.ContentManager.Load<Chest>(Path.Combine("Maps/Chests", chestEntry.ContentName)).Clone() as Chest;
                }

                quest.DestinationMapContentName = input.ReadString();
                quest.DestinationNpcContentName = input.ReadString();
                quest.DestinationObjectiveMessage = input.ReadString();

                quest._experienceReward = input.ReadInt32();
                quest._goldReward = input.ReadInt32();

                quest.GearRewardContentNamesList.AddRange(input.ReadObject<List<string>>());
                foreach (string contentName in quest.GearRewardContentNamesList)
                {
                    quest.GearRewardsList.Add(input.ContentManager.Load<Gear>(Path.Combine("Gear", contentName)));
                }

                return quest;
            }
        }

        public object Clone()
        {
            Quest quest = new()
            {
                AssetName = AssetName
            };

            foreach (WorldEntry<Chest> chestEntry in _chestEntriesList)
            {
                WorldEntry<Chest> worldEntry = new()
                {
                    Content = chestEntry.Content.Clone() as Chest,
                    ContentName = chestEntry.ContentName,
                    Count = chestEntry.Count,
                    Direction = chestEntry.Direction,
                    MapContentName = chestEntry.MapContentName,
                    MapPosition = chestEntry.MapPosition
                };
                quest._chestEntriesList.Add(worldEntry);
            }
            quest._completionMessage = _completionMessage;
            quest._description = _description;
            quest._destinationMapContentName = _destinationMapContentName;
            quest._destinationNpcContentName = _destinationNpcContentName;
            quest._destinationObjectiveMessage = _destinationObjectiveMessage;
            quest._experienceReward = _experienceReward;
            quest._fixedCombatEntriesList.AddRange(_fixedCombatEntriesList);
            quest._gearRequirementsList.AddRange(_gearRequirementsList);
            quest._gearRewardContentNamesList.AddRange(_gearRewardContentNamesList);
            quest._gearRewardsList.AddRange(_gearRewardsList);
            quest._goldReward = _goldReward;
            quest._monsterRequirementsList.AddRange(_monsterRequirementsList);
            quest._name = _name;
            quest._objectiveMessage = _objectiveMessage;
            quest._stage = _stage;

            return quest;
        }
    }
}
