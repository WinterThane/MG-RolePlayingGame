using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Engine;

namespace RolePlayingGame.Animations
{
    public class Animation : ContentObject
    {
        /// <summary>
        /// The name of the animation.
        /// </summary>
        private string _name;

        /// <summary>
        /// The name of the animation.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// The first frame of the animation.
        /// </summary>
        private int _startingFrame;

        /// <summary>
        /// The first frame of the animation.
        /// </summary>
        public int StartingFrame
        {
            get => _startingFrame;
            set => _startingFrame = value;
        }

        /// <summary>
        /// The last frame of the animation.
        /// </summary>
        private int _endingFrame;

        /// <summary>
        /// The last frame of the animation.
        /// </summary>
        public int EndingFrame
        {
            get => _endingFrame;
            set => _endingFrame = value;
        }

        /// <summary>
        /// The interval between frames of the animation.
        /// </summary>
        private int _interval;

        /// <summary>
        /// The interval between frames of the animation.
        /// </summary>
        public int Interval
        {
            get => _interval;
            set => _interval = value;
        }

        /// <summary>
        /// If true, the animation loops.
        /// </summary>
        private bool _isLoop;

        /// <summary>
        /// If true, the animation loops.
        /// </summary>
        public bool IsLoop
        {
            get => _isLoop;
            set => _isLoop = value;
        }

        /// <summary>
        /// Creates a new Animation object.
        /// </summary>
        public Animation() { }

        /// <summary>
        /// Creates a new Animation object by full specification.
        /// </summary>
        public Animation(string name, int startingFrame, int endingFrame, int interval, bool isLoop)
        {
            Name = name;
            StartingFrame = startingFrame;
            EndingFrame = endingFrame;
            Interval = interval;
            IsLoop = isLoop;
        }

        /// <summary>
        /// Read an Animation object from the content pipeline.
        /// </summary>
        public class AnimationReader : ContentTypeReader<Animation>
        {
            /// <summary>
            /// Read an Animation object from the content pipeline.
            /// </summary>
            protected override Animation Read(ContentReader input, Animation existingInstance)
            {
                Animation animation = existingInstance;
                if (animation == null)
                {
                    animation = new Animation();
                }

                animation.AssetName = input.AssetName;

                animation.Name = input.ReadString();
                animation.StartingFrame = input.ReadInt32();
                animation.EndingFrame = input.ReadInt32();
                animation.Interval = input.ReadInt32();
                animation.IsLoop = input.ReadBoolean();

                return animation;
            }
        }
    }
}
