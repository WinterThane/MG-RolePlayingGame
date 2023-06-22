using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Engine;
using RolePlayingGame.GearObjects;
using RolePlayingGame.WorldObjects;
using System;
using System.Collections.Generic;

namespace RolePlayingGame.MapObjects
{
    public class Chest : WorldObject, ICloneable
    {
        private int _gold = 0;
        /// <summary>
        /// The amount of gold in the chest.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public int Gold
        {
            get => _gold;
            set => _gold = value;
        }

        private List<ContentEntry<Gear>> _entries = new();
        /// <summary>
        /// The gear in the chest, along with quantities.
        /// </summary>
        public List<ContentEntry<Gear>> Entries
        {
            get => _entries;
            set => _entries = value;
        }

        /// <summary>
        /// Array accessor for the chest's contents.
        /// </summary>
        public ContentEntry<Gear> this[int index] => _entries[index];

        /// <summary>
        /// Returns true if the chest is empty.
        /// </summary>
        public bool IsEmpty => (_gold <= 0) && (_entries.Count <= 0);

        private string _textureName;
        /// <summary>
        /// The content name of the texture for this chest.
        /// </summary>
        public string TextureName
        {
            get => _textureName;
            set => _textureName = value;
        }

        private Texture2D _texture;
        /// <summary>
        /// The texture for this chest.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D Texture
        {
            get => _texture;
            set => _texture = value;
        }

        /// <summary>
        /// Reads a Chest object from the content pipeline.
        /// </summary>
        public class ChestReader : ContentTypeReader<Chest>
        {
            protected override Chest Read(ContentReader input, Chest existingInstance)
            {
                Chest chest = existingInstance;
                if (chest == null)
                {
                    chest = new Chest();
                }

                input.ReadRawObject<WorldObject>(chest as WorldObject);

                chest.Gold = input.ReadInt32();

                chest.Entries.AddRange(input.ReadObject<List<ContentEntry<Gear>>>());
                foreach (ContentEntry<Gear> contentEntry in chest.Entries)
                {
                    contentEntry.Content = input.ContentManager.Load<Gear>(System.IO.Path.Combine("Gear", contentEntry.ContentName));
                }

                chest.TextureName = input.ReadString();
                chest.Texture = input.ContentManager.Load<Texture2D>(System.IO.Path.Combine("Textures/Chests", chest.TextureName));

                return chest;
            }
        }

        /// <summary>
        /// Clone implementation for chest copies.
        /// </summary>
        /// <remarks>
        /// The game has to handle chests that have had some contents removed
        /// without modifying the original chest (and all chests that come after).
        /// </remarks>
        public object Clone()
        {
            // create the new chest
            Chest chest = new()
            {
                Gold = Gold,
                Name = Name,
                Texture = Texture,
                TextureName = TextureName,
                // recreate the list and entries, as counts may have changed
                _entries = new List<ContentEntry<Gear>>()
            };
            foreach (ContentEntry<Gear> originalEntry in Entries)
            {
                ContentEntry<Gear> newEntry = new()
                {
                    Count = originalEntry.Count,
                    ContentName = originalEntry.ContentName,
                    Content = originalEntry.Content
                };
                chest.Entries.Add(newEntry);
            }

            return chest;
        }
    }
}
