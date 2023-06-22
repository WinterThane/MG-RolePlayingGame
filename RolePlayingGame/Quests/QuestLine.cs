using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Engine;
using System;
using System.Collections.Generic;
using System.IO;

namespace RolePlayingGame.Quests
{
    public class QuestLine : ContentObject, ICloneable
    {
        /// <summary>
        /// The name of the quest line.
        /// </summary>
        private string _name;

        /// <summary>
        /// The name of the quest line.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// An ordered list of content names of quests that will be presented in order.
        /// </summary>
        private List<string> _questContentNamesList = new();

        /// <summary>
        /// An ordered list of content names of quests that will be presented in order.
        /// </summary>
        public List<string> QuestContentNamesList
        {
            get => _questContentNamesList;
            set => _questContentNamesList = value;
        }

        /// <summary>
        /// An ordered list of quests that will be presented in order.
        /// </summary>
        private List<Quest> _questsList = new();

        /// <summary>
        /// An ordered list of quests that will be presented in order.
        /// </summary>
        [ContentSerializerIgnore]
        public List<Quest> QuestsList => _questsList;

        /// <summary>
        /// Reads a QuestLine object from the content pipeline.
        /// </summary>
        public class QuestLineReader : ContentTypeReader<QuestLine>
        {
            /// <summary>
            /// Reads a QuestLine object from the content pipeline.
            /// </summary>
            protected override QuestLine Read(ContentReader input, QuestLine existingInstance)
            {
                QuestLine questLine = existingInstance;
                if (questLine == null)
                {
                    questLine = new QuestLine();
                }

                questLine.AssetName = input.AssetName;
                questLine.Name = input.ReadString();
                questLine.QuestContentNamesList.AddRange(input.ReadObject<List<string>>());

                foreach (string contentName in questLine.QuestContentNamesList)
                {
                    questLine._questsList.Add(input.ContentManager.Load<Quest>(Path.Combine("Quests", contentName)));

                }

                return questLine;
            }
        }

        public object Clone()
        {
            QuestLine questLine = new()
            {
                AssetName = AssetName,
                _name = _name
            };
            questLine._questContentNamesList.AddRange(_questContentNamesList);

            foreach (Quest quest in _questsList)
            {
                questLine._questsList.Add(quest.Clone() as Quest);
            }

            return questLine;
        }
    }
}
