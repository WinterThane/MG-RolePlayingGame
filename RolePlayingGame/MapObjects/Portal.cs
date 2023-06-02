using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Engine;

namespace RolePlayingGame.MapObjects
{
    public class Portal : ContentObject
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Point landingMapPosition;
        public Point LandingMapPosition
        {
            get { return landingMapPosition; }
            set { landingMapPosition = value; }
        }

        private string destinationMapContentName;
        public string DestinationMapContentName
        {
            get { return destinationMapContentName; }
            set { destinationMapContentName = value; }
        }

        private string destinationMapPortalName;
        public string DestinationMapPortalName
        {
            get { return destinationMapPortalName; }
            set { destinationMapPortalName = value; }
        }

        public class PortalReader : ContentTypeReader<Portal>
        {
            protected override Portal Read(ContentReader input,
                Portal existingInstance)
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
