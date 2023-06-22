using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.WorldObjects;

namespace RolePlayingGame.MapObjects
{
    public class Inn : WorldObject
    {
        /// <summary>
        /// The amount that the party has to pay for each member to stay.
        /// </summary>
        private int _chargePerPlayer;

        /// <summary>
        /// The amount that the party has to pay for each member to stay.
        /// </summary>
        public int ChargePerPlayer
        {
            get => _chargePerPlayer;
            set => _chargePerPlayer = value;
        }

        /// <summary>
        /// The message shown when the player enters the inn.
        /// </summary>
        private string _welcomeMessage;

        /// <summary>
        /// The message shown when the player enters the inn.
        /// </summary>
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => _welcomeMessage = value;
        }

        /// <summary>
        /// The message shown when the party successfully pays to stay the night.
        /// </summary>
        private string _paidMessage;

        /// <summary>
        /// The message shown when the party successfully pays to stay the night.
        /// </summary>
        public string PaidMessage
        {
            get => _paidMessage;
            set => _paidMessage = value;
        }

        /// <summary>
        /// The message shown when the party tries to stay but lacks the funds.
        /// </summary>
        private string _notEnoughGoldMessage;

        /// <summary>
        /// The message shown when the party tries to stay but lacks the funds.
        /// </summary>
        public string NotEnoughGoldMessage
        {
            get => _notEnoughGoldMessage;
            set => _notEnoughGoldMessage = value;
        }

        /// <summary>
        /// The content name of the texture for the shopkeeper.
        /// </summary>
        private string _shopkeeperTextureName;

        /// <summary>
        /// The content name of the texture for the shopkeeper.
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
        /// Reads an Inn object from the content pipeline.
        /// </summary>
        public class InnReader : ContentTypeReader<Inn>
        {
            protected override Inn Read(ContentReader input, Inn existingInstance)
            {
                Inn inn = existingInstance;
                if (inn == null)
                {
                    inn = new Inn();
                }

                input.ReadRawObject<WorldObject>(inn as WorldObject);

                inn.ChargePerPlayer = input.ReadInt32();
                inn.WelcomeMessage = input.ReadString();
                inn.PaidMessage = input.ReadString();
                inn.NotEnoughGoldMessage = input.ReadString();
                inn.ShopkeeperTextureName = input.ReadString();
                inn._shopkeeperTexture = input.ContentManager.Load<Texture2D>(System.IO.Path.Combine("Textures/Characters/Portraits", inn.ShopkeeperTextureName));

                return inn;
            }
        }
    }
}
