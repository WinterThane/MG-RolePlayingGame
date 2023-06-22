using Microsoft.Xna.Framework.Content;
using RolePlayingGame.GearObjects;
using System.Collections.Generic;

namespace RolePlayingGame.MapObjects
{
    public class StoreCategory
    {
        /// <summary>
        /// The display name of this store category.
        /// </summary>
        private string _name;

        /// <summary>
        /// The display name of this store category.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// The content names for the gear available in this category.
        /// </summary>
        private List<string> _availableContentNamesList = new();

        /// <summary>
        /// The content names for the gear available in this category.
        /// </summary>
        public List<string> AvailableContentNamesLst
        {
            get => _availableContentNamesList;
            set => _availableContentNamesList = value;
        }

        /// <summary>
        /// The gear available in this category.
        /// </summary>
        private List<Gear> _availableGearList = new List<Gear>();

        /// <summary>
        /// The gear available in this category.
        /// </summary>
        [ContentSerializerIgnore]
        public List<Gear> AvailableGearList
        {
            get => _availableGearList;
            set => _availableGearList = value;
        }

        /// <summary>
        /// Reads a StoreCategory object from the content pipeline.
        /// </summary>
        public class StoreCategoryReader : ContentTypeReader<StoreCategory>
        {
            /// <summary>
            /// Reads a StoreCategory object from the content pipeline.
            /// </summary>
            protected override StoreCategory Read(ContentReader input, StoreCategory existingInstance)
            {
                StoreCategory storeCategory = existingInstance;
                if (storeCategory == null)
                {
                    storeCategory = new StoreCategory();
                }

                storeCategory.Name = input.ReadString();
                storeCategory.AvailableContentNamesLst.AddRange(input.ReadObject<List<string>>());

                // populate the gear list based on the content names
                foreach (string gearName in storeCategory.AvailableContentNamesLst)
                {
                    storeCategory.AvailableGearList.Add(input.ContentManager.Load<Gear>(System.IO.Path.Combine("Gear", gearName)));
                }

                return storeCategory;
            }
        }
    }
}
