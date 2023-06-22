using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Directions;
using RolePlayingGame.Engine;
using System;
using System.Collections.Generic;

namespace RolePlayingGame.Animations
{
    public class AnimatingSprite : ContentObject, ICloneable
    {
        /// <summary>
        /// The content path and name of the texture for this spell animation.
        /// </summary>
        private string _textureName;

        /// <summary>
        /// The content path and name of the texture for this spell animation.
        /// </summary>
        public string TextureName
        {
            get => _textureName;
            set => _textureName = value;
        }

        /// <summary>
        /// The texture for this spell animation.
        /// </summary>
        private Texture2D _texture;

        /// <summary>
        /// The texture for this spell animation.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D Texture
        {
            get => _texture;
            set => _texture = value;
        }

        /// <summary>
        /// The dimensions of a single frame of animation.
        /// </summary>
        private Point _frameDimensions;

        /// <summary>
        /// The width of a single frame of animation.
        /// </summary>
        public Point FrameDimensions
        {
            get => _frameDimensions;
            set
            {
                _frameDimensions = value;
                _frameOrigin.X = _frameDimensions.X / 2;
                _frameOrigin.Y = _frameDimensions.Y / 2;
            }
        }

        /// <summary>
        /// The origin of the sprite, within a frame.
        /// </summary>
        private Point _frameOrigin;

        /// <summary>
        /// The number of frames in a row in this sprite.
        /// </summary>
        private int _framesPerRow;

        /// <summary>
        /// The number of frames in a row in this sprite.
        /// </summary>
        public int FramesPerRow
        {
            get => _framesPerRow;
            set => _framesPerRow = value;
        }

        /// <summary>
        /// The offset of this sprite from the position it's drawn at.
        /// </summary>
        private Vector2 _sourceOffset;

        /// <summary>
        /// The offset of this sprite from the position it's drawn at.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public Vector2 SourceOffset
        {
            get => _sourceOffset;
            set => _sourceOffset = value;
        }

        /// <summary>
        /// The animations defined for this sprite.
        /// </summary>
        private List<Animation> _animations = new();

        /// <summary>
        /// The animations defined for this sprite.
        /// </summary>
        public List<Animation> Animations
        {
            get => _animations;
            set => _animations = value;
        }

        /// <summary>
        /// Enumerate the animations on this animated sprite.
        /// </summary>
        /// <param name="animationName">The name of the animation.</param>
        /// <returns>The animation if found; null otherwise.</returns>
        public Animation this[string animationName]
        {
            get
            {
                if (string.IsNullOrEmpty(animationName))
                {
                    return null;
                }
                foreach (Animation animation in _animations)
                {
                    if (string.Compare(animation.Name, animationName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return animation;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Add the animation to the list, checking for name collisions.
        /// </summary>
        /// <returns>True if the animation was added to the list.</returns>
        public bool AddAnimation(Animation animation)
        {
            if ((animation != null) && (this[animation.Name] == null))
            {
                _animations.Add(animation);
                return true;
            }

            return false;
        }

        /// <summary>
        /// The animation currently playing back on this sprite.
        /// </summary>
        private Animation _currentAnimation = null;

        /// <summary>
        /// The current frame in the current animation.
        /// </summary>
        private int _currentFrame;

        /// <summary>
        /// The elapsed time since the last frame switch.
        /// </summary>
        private float _elapsedTime;


        /// <summary>
        /// The source rectangle of the current frame of animation.
        /// </summary>
        private Rectangle _sourceRectangle;

        /// <summary>
        /// The source rectangle of the current frame of animation.
        /// </summary>
        public Rectangle SourceRectangle => _sourceRectangle;

        /// <summary>
        /// Play the given animation on the sprite.
        /// </summary>
        /// <remarks>The given animation may be null, to clear any animation.</remarks>
        public void PlayAnimation(Animation animation)
        {
            // start the new animation, ignoring redundant Plays
            if (animation != _currentAnimation)
            {
                _currentAnimation = animation;
                ResetAnimation();
            }
        }

        /// <summary>
        /// Play an animation given by index.
        /// </summary>
        public void PlayAnimation(int index)
        {
            // check the parameter
            if ((index < 0) || (index >= _animations.Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            PlayAnimation(_animations[index]);
        }

        /// <summary>
        /// Play an animation given by name.
        /// </summary>
        public void PlayAnimation(string name)
        {
            // check the parameter
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            PlayAnimation(this[name]);
        }

        /// <summary>
        /// Play a given animation name, with the given direction suffix.
        /// </summary>
        /// <example>
        /// For example, passing "Walk" and Direction.South will play the animation
        /// named "WalkSouth".
        /// </example>
        public void PlayAnimation(string name, Direction direction)
        {
            // check the parameter
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            PlayAnimation(name + direction.ToString());
        }

        /// <summary>
        /// Reset the animation back to its starting position.
        /// </summary>
        public void ResetAnimation()
        {
            _elapsedTime = 0f;
            if (_currentAnimation != null)
            {
                _currentFrame = _currentAnimation.StartingFrame;
                // calculate the source rectangle by updating the animation
                UpdateAnimation(0f);
            }
        }

        /// <summary>
        /// Advance the current animation to the final sprite.
        /// </summary>
        public void AdvanceToEnd()
        {
            if (_currentAnimation != null)
            {
                _currentFrame = _currentAnimation.EndingFrame;
                // calculate the source rectangle by updating the animation
                UpdateAnimation(0f);
            }
        }

        /// <summary>
        /// Stop any animation playing on the sprite.
        /// </summary>
        public void StopAnimation()
        {
            _currentAnimation = null;
        }

        /// <summary>
        /// Returns true if playback on the current animation is complete, or if
        /// there is no animation at all.
        /// </summary>
        public bool IsPlaybackComplete
        {
            get
            {
                return (_currentAnimation == null) || (!_currentAnimation.IsLoop && (_currentFrame > _currentAnimation.EndingFrame));
            }
        }

        /// <summary>
        /// Update the current animation.
        /// </summary>
        public void UpdateAnimation(float elapsedSeconds)
        {
            if (IsPlaybackComplete)
            {
                return;
            }

            // loop the animation if needed
            if (_currentAnimation.IsLoop && (_currentFrame > _currentAnimation.EndingFrame))
            {
                _currentFrame = _currentAnimation.StartingFrame;
            }

            // update the source rectangle
            int column = (_currentFrame - 1) / _framesPerRow;
            _sourceRectangle = new Rectangle(
                (_currentFrame - 1 - (column * _framesPerRow)) * _frameDimensions.X,
                column * _frameDimensions.Y,
                _frameDimensions.X, _frameDimensions.Y);

            // update the elapsed time
            _elapsedTime += elapsedSeconds;

            // advance to the next frame if ready
            while (_elapsedTime * 1000f > (float)_currentAnimation.Interval)
            {
                _currentFrame++;
                _elapsedTime -= (float)_currentAnimation.Interval / 1000f;
            }
        }

        /// <summary>
        /// Draw the sprite at the given position.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch object used to draw.</param>
        /// <param name="position">The position of the sprite on-screen.</param>
        /// <param name="layerDepth">The depth at which the sprite is drawn.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float layerDepth)
        {
            Draw(spriteBatch, position, layerDepth, SpriteEffects.None);
        }

        /// <summary>
        /// Draw the sprite at the given position.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch object used to draw.</param>
        /// <param name="position">The position of the sprite on-screen.</param>
        /// <param name="layerDepth">The depth at which the sprite is drawn.</param>
        /// <param name="spriteEffect">The sprite-effect applied.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float layerDepth, SpriteEffects spriteEffect)
        {
            // check the parameters
            if (spriteBatch == null)
            {
                throw new ArgumentNullException("spriteBatch");
            }

            if (_texture != null)
            {
                spriteBatch.Draw(_texture, position, _sourceRectangle, Color.White, 0f, _sourceOffset, 1f, spriteEffect, MathHelper.Clamp(layerDepth, 0f, 1f));
            }
        }

        /// <summary>
        /// Read an AnimatingSprite object from the content pipeline.
        /// </summary>
        public class AnimatingSpriteReader : ContentTypeReader<AnimatingSprite>
        {
            /// <summary>
            /// Read an AnimatingSprite object from the content pipeline.
            /// </summary>
            protected override AnimatingSprite Read(ContentReader input, AnimatingSprite existingInstance)
            {
                AnimatingSprite animatingSprite = existingInstance;
                if (animatingSprite == null)
                {
                    animatingSprite = new AnimatingSprite();
                }

                animatingSprite.AssetName = input.AssetName;

                animatingSprite.TextureName = input.ReadString();
                animatingSprite.Texture = input.ContentManager.Load<Texture2D>(System.IO.Path.Combine("Textures", animatingSprite.TextureName));
                animatingSprite.FrameDimensions = input.ReadObject<Point>();
                animatingSprite.FramesPerRow = input.ReadInt32();
                animatingSprite.SourceOffset = input.ReadObject<Vector2>();
                animatingSprite.Animations.AddRange(input.ReadObject<List<Animation>>());

                return animatingSprite;
            }
        }

        /// <summary>
        /// Creates a clone of this object.
        /// </summary>
        public object Clone()
        {
            AnimatingSprite animatingSprite = new();

            animatingSprite._animations.AddRange(_animations);
            animatingSprite._currentAnimation = _currentAnimation;
            animatingSprite._currentFrame = _currentFrame;
            animatingSprite._elapsedTime = _elapsedTime;
            animatingSprite._frameDimensions = _frameDimensions;
            animatingSprite._frameOrigin = _frameOrigin;
            animatingSprite._framesPerRow = _framesPerRow;
            animatingSprite._sourceOffset = _sourceOffset;
            animatingSprite._sourceRectangle = _sourceRectangle;
            animatingSprite._texture = _texture;
            animatingSprite._textureName = _textureName;

            return animatingSprite;
        }
    }
}
