using Microsoft.Xna.Framework.Content;

namespace RolePlayingGame.GearObjects
{
    public class GearDrop
    {
        /// <summary>
        /// The content name of the gear.
        /// </summary>
        private string _gearName;

        /// <summary>
        /// The content name of the gear.
        /// </summary>
        public string GearName
        {
            get => _gearName;
            set => _gearName = value;
        }

        /// <summary>
        /// The percentage chance that the gear will drop, from 0 to 100.
        /// </summary>
        private int _dropPercentage;

        /// <summary>
        /// The percentage chance that the gear will drop, from 0 to 100.
        /// </summary>
        public int DropPercentage
        {
            get => _dropPercentage;
            set => _dropPercentage = value > 100 ? 100 : (value < 0 ? 0 : value);
        }

        /// <summary>
        /// Read a GearDrop object from the content pipeline.
        /// </summary>
        public class GearDropReader : ContentTypeReader<GearDrop>
        {
            protected override GearDrop Read(ContentReader input, GearDrop existingInstance)
            {
                GearDrop gearDrop = existingInstance;
                if (gearDrop == null)
                {
                    gearDrop = new GearDrop();
                }

                gearDrop.GearName = input.ReadString();
                gearDrop.DropPercentage = input.ReadInt32();

                return gearDrop;
            }
        }
    }
}
