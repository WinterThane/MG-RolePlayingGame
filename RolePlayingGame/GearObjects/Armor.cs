using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Data;

namespace RolePlayingGame.GearObjects
{
    public class Armor : Equipment
    {
        /// <summary>
        /// Slots that a piece of armor may fill on a character.
        /// </summary>
        /// <remarks>Only one piece may fill a slot at the same time.</remarks>
        public enum ArmorSlot
        {
            Helmet,
            Shield,
            Torso,
            Boots,
        };


        /// <summary>
        /// The slot that this armor fills.
        /// </summary>
        private ArmorSlot _slot;

        /// <summary>
        /// The slot that this armor fills.
        /// </summary>
        public ArmorSlot Slot
        {
            get => _slot;
            set => _slot = value;
        }

        /// <summary>
        /// Builds and returns a string describing the power of this armor.
        /// </summary>
        public override string GetPowerText()
        {
            return "Weapon Defense: " + OwnerHealthDefenseRange.ToString() + "\nMagic Defense: " + OwnerMagicDefenseRange.ToString();
        }

        /// <summary>
        /// The range of health defense provided by this armor to its owner.
        /// </summary>
        private Int32Range _ownerHealthDefenseRange;

        /// <summary>
        /// The range of health defense provided by this armor to its owner.
        /// </summary>
        public Int32Range OwnerHealthDefenseRange
        {
            get => _ownerHealthDefenseRange;
            set => _ownerHealthDefenseRange = value;
        }

        /// <summary>
        /// The range of magic defense provided by this armor to its owner.
        /// </summary>
        private Int32Range _ownerMagicDefenseRange;

        /// <summary>
        /// The range of magic defense provided by this armor to its owner.
        /// </summary>
        public Int32Range OwnerMagicDefenseRange
        {
            get => _ownerMagicDefenseRange;
            set => _ownerMagicDefenseRange = value;
        }

        /// <summary>
        /// Read the Weapon type from the content pipeline.
        /// </summary>
        public class ArmorReader : ContentTypeReader<Armor>
        {
            protected override Armor Read(ContentReader input, Armor existingInstance)
            {
                Armor armor = existingInstance;

                if (armor == null)
                {
                    armor = new Armor();
                }

                // read the gear settings
                input.ReadRawObject<Equipment>(armor as Equipment);

                // read armor settings
                armor.Slot = (ArmorSlot)input.ReadInt32();
                armor.OwnerHealthDefenseRange = input.ReadObject<Int32Range>();
                armor.OwnerMagicDefenseRange = input.ReadObject<Int32Range>();

                return armor;
            }
        }
    }
}
