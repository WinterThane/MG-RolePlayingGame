using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.WorldObjects;
using System.Collections.Generic;

namespace RolePlayingGame.MapObjects
{
    public class Store : WorldObject
    {
        /// <summary>
        /// A purchasing multiplier applied to the price of all gear.
        /// </summary>
        private float _buyMultiplier;

        /// <summary>
        /// A purchasing multiplier applied to the price of all gear.
        /// </summary>
        public float BuyMultiplier
        {
            get => _buyMultiplier;
            set => _buyMultiplier = value;
        }

        /// <summary>
        /// A sell-back multiplier applied to the price of all gear.
        /// </summary>
        private float _sellMultiplier;

        /// <summary>
        /// A sell-back multiplier applied to the price of all gear.
        /// </summary>
        public float SellMultiplier
        {
            get => _sellMultiplier;
            set => _sellMultiplier = value;
        }

        /// <summary>
        /// The categories of gear in this store.
        /// </summary>
        private List<StoreCategory> _storeCategoriesList = new();

        /// <summary>
        /// The categories of gear in this store.
        /// </summary>
        public List<StoreCategory> StoreCategoriesList
        {
            get => _storeCategoriesList;
            set => _storeCategoriesList = value;
        }

        /// <summary>
        /// The message shown when the party enters the store.
        /// </summary>
        private string _welcomeMessage;

        /// <summary>
        /// The message shown when the party enters the store.
        /// </summary>
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => _welcomeMessage = value;
        }

        /// <summary>
        /// The content path and name of the texture for the shopkeeper.
        /// </summary>
        private string _shopkeeperTextureName;

        /// <summary>
        /// The content path and name of the texture for the shopkeeper.
        /// </summary>
        public string ShopkeeperTextureName
        {
            get => _shopkeeperTextureName;
            set => _shopkeeperTextureName = value;
        }

        /// <summary>
        /// The texture for the shopkeeper.
        /// </summary>
        private Texture2D _shopkeeperTexture;

        /// <summary>
        /// The texture for the shopkeeper.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D ShopkeeperTexture => _shopkeeperTexture;

        /// <summary>
        /// Reads an Store object from the content pipeline.
        /// </summary>
        public class StoreReader : ContentTypeReader<Store>
        {
            protected override Store Read(ContentReader input, Store existingInstance)
            {
                Store store = existingInstance;
                if (store == null)
                {
                    store = new Store();
                }

                input.ReadRawObject<WorldObject>(store as WorldObject);

                store.BuyMultiplier = input.ReadSingle();
                store.SellMultiplier = input.ReadSingle();
                store.StoreCategoriesList.AddRange(input.ReadObject<List<StoreCategory>>());
                store.WelcomeMessage = input.ReadString();
                store.ShopkeeperTextureName = input.ReadString();
                store._shopkeeperTexture = input.ContentManager.Load<Texture2D>(System.IO.Path.Combine("Textures/Characters/Portraits", store.ShopkeeperTextureName));

                return store;
            }
        }
    }
}
