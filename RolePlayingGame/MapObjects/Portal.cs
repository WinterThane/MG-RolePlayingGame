using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Engine;

namespace RolePlayingGame.MapObjects
{
    public class Portal : ContentObject
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        private Point _landingMapPosition;
        public Point LandingMapPosition
        {
            get => _landingMapPosition;
            set => _landingMapPosition = value;
        }

        private string _destinationMapContentName;
        public string DestinationMapContentName
        {
            get => _destinationMapContentName;
            set => _destinationMapContentName = value;
        }

        private string _destinationMapPortalName;
        public string DestinationMapPortalName
        {
            get => _destinationMapPortalName;
            set => _destinationMapPortalName = value;
        }

        public class PortalReader : ContentTypeReader<Portal>
        {
            protected override Portal Read(ContentReader input, Portal existingInstance)
            {
                Portal portal = existingInstance;
                if (portal == null)
                {
                    portal = new Portal();
                }

                portal.AssetName = input.AssetName;

                portal.Name = input.ReadString();

                portal.LandingMapPosition = input.ReadObject<Point>();
                portal.DestinationMapContentName = input.ReadString();
                portal.DestinationMapPortalName = input.ReadString();

                return portal;
            }
        }
    }
}
