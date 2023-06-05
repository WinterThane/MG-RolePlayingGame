using Microsoft.Xna.Framework.Content;
using System.Xml.Serialization;

namespace RolePlayingGame.Engine
{
    public class ContentEntry<T> where T : ContentObject
    {
        private string contentName;
        [ContentSerializer(Optional = true)]
        public string ContentName
        {
            get { return contentName; }
            set { contentName = value; }
        }

        private T content;
        [ContentSerializerIgnore]
        [XmlIgnore]
        public T Content
        {
            get { return content; }
            set { content = value; }
        }

        private int count = 1;
        [ContentSerializer(Optional = true)]
        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        public class ContentEntryReader : ContentTypeReader<ContentEntry<T>>
        {
            /// <summary>
            /// Reads a ContentEntry object from the content pipeline.
            /// </summary>
            protected override ContentEntry<T> Read(ContentReader input, ContentEntry<T> existingInstance)
            {
                ContentEntry<T> member = existingInstance;
                if (member == null)
                {
                    member = new ContentEntry<T>();
                }

                member.ContentName = input.ReadString();
                member.Count = input.ReadInt32();

                return member;
            }
        }
    }
}
