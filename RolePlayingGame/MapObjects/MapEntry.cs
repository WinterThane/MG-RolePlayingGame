using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Animations;
using RolePlayingGame.Directions;
using RolePlayingGame.Engine;

namespace RolePlayingGame.MapObjects
{
    public class MapEntry<T> : ContentEntry<T> where T : ContentObject
    {
        private Point _mapPosition;
        public Point MapPosition
        {
            get => _mapPosition;
            set => _mapPosition = value;
        }

        private Direction _direction;
        [ContentSerializer(Optional = true)]
        public Direction Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public override bool Equals(object obj)
        {
            MapEntry<T> mapEntry = obj as MapEntry<T>;
            return (mapEntry != null) && (mapEntry.Content == Content) && (mapEntry._mapPosition == _mapPosition) && (mapEntry.Direction == Direction);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private AnimatingSprite _mapSprite;
        [ContentSerializer(Optional = true)]
        public AnimatingSprite MapSprite
        {
            get => _mapSprite;
            set => _mapSprite = value;
        }

        public class MapEntryReader : ContentTypeReader<MapEntry<T>>
        {
            /// <summary>
            /// Read a MapEntry object from the content pipeline.
            /// </summary>
            protected override MapEntry<T> Read(ContentReader input, MapEntry<T> existingInstance)
            {
                MapEntry<T> desc = existingInstance;
                if (desc == null)
                {
                    desc = new MapEntry<T>();
                }

                input.ReadRawObject<ContentEntry<T>>(desc as ContentEntry<T>);
                desc.MapPosition = input.ReadObject<Point>();
                desc.Direction = (Direction)input.ReadInt32();

                return desc;
            }
        }
    }
}
