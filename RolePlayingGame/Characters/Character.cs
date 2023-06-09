﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Animations;
using RolePlayingGame.Directions;
using RolePlayingGame.WorldObjects;
using System;

namespace RolePlayingGame.Characters
{
    public abstract class Character : WorldObject
    {
        /// <summary>
        /// The state of a character.
        /// </summary>
        public enum CharacterState
        {
            /// <summary>
            /// Ready to perform an action, and playing the idle animation
            /// </summary>
            Idle,

            /// <summary>
            /// Walking in the world.
            /// </summary>
            Walking,

            /// <summary>
            /// In defense mode
            /// </summary>
            Defending,

            /// <summary>
            /// Performing Dodge Animation
            /// </summary>
            Dodging,

            /// <summary>
            /// Performing Hit Animation
            /// </summary>
            Hit,

            /// <summary>
            /// Dead, but still playing the dying animation.
            /// </summary>
            Dying,

            /// <summary>
            /// Dead, with the dead animation.
            /// </summary>
            Dead,
        }

        /// <summary>
        /// The state of this character.
        /// </summary>
        private CharacterState _state = CharacterState.Idle;

        /// <summary>
        /// The state of this character.
        /// </summary>
        [ContentSerializerIgnore]
        public CharacterState State
        {
            get => _state;
            set => _state = value;
        }

        /// <summary>
        /// Returns true if the character is dead or dying.
        /// </summary>
        public bool IsDeadOrDying => (State == CharacterState.Dying) || (State == CharacterState.Dead);

        /// <summary>
        /// The position of this object on the map.
        /// </summary>
        private Point _mapPosition;

        /// <summary>
        /// The position of this object on the map.
        /// </summary>
        [ContentSerializerIgnore]
        public Point MapPosition
        {
            get => _mapPosition;
            set => _mapPosition = value;
        }

        /// <summary>
        /// The orientation of this object on the map.
        /// </summary>
        private Direction _direction;

        /// <summary>
        /// The orientation of this object on the map.
        /// </summary>
        [ContentSerializerIgnore]
        public Direction Direction
        {
            get => _direction;
            set => _direction = value;
        }

        /// <summary>
        /// The animating sprite for the map view of this character.
        /// </summary>
        private AnimatingSprite _mapSprite;

        /// <summary>
        /// The animating sprite for the map view of this character.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public AnimatingSprite MapSprite
        {
            get => _mapSprite;
            set => _mapSprite = value;
        }

        /// <summary>
        /// The animating sprite for the map view of this character as it walks.
        /// </summary>
        /// <remarks>
        /// If this object is null, then the animations are taken from MapSprite.
        /// </remarks>
        private AnimatingSprite _walkingSprite;

        /// <summary>
        /// The animating sprite for the map view of this character as it walks.
        /// </summary>
        /// <remarks>
        /// If this object is null, then the animations are taken from MapSprite.
        /// </remarks>
        [ContentSerializer(Optional = true)]
        public AnimatingSprite WalkingSprite
        {
            get => _walkingSprite;
            set => _walkingSprite = value;
        }

        /// <summary>
        /// Reset the animations for this character.
        /// </summary>
        public virtual void ResetAnimation(bool isWalking)
        {
            _state = isWalking ? CharacterState.Walking : CharacterState.Idle;
            if (_mapSprite != null)
            {
                if (isWalking && _mapSprite["Walk" + Direction.ToString()] != null)
                {
                    _mapSprite.PlayAnimation("Walk", Direction);
                }
                else
                {
                    _mapSprite.PlayAnimation("Idle", Direction);
                }
            }
            if (_walkingSprite != null)
            {
                if (isWalking && _walkingSprite["Walk" + Direction.ToString()] != null)
                {
                    _walkingSprite.PlayAnimation("Walk", Direction);
                }
                else
                {
                    _walkingSprite.PlayAnimation("Idle", Direction);
                }
            }
        }

        /// <summary>
        /// The small blob shadow that is rendered under the characters.
        /// </summary>
        private Texture2D _shadowTexture;

        /// <summary>
        /// The small blob shadow that is rendered under the characters.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D ShadowTexture
        {
            get => _shadowTexture;
            set => _shadowTexture = value;
        }

        /// <summary>
        /// The default idle-animation interval for the animating map sprite.
        /// </summary>
        private int _mapIdleAnimationInterval = 200;

        /// <summary>
        /// The default idle-animation interval for the animating map sprite.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public int MapIdleAnimationInterval
        {
            get => _mapIdleAnimationInterval;
            set => _mapIdleAnimationInterval = value;
        }

        /// <summary>
        /// Add the standard character idle animations to this character.
        /// </summary>
        private void AddStandardCharacterIdleAnimations()
        {
            if (_mapSprite != null)
            {
                _mapSprite.AddAnimation(new Animation("IdleSouth", 1, 6, MapIdleAnimationInterval, true));
                _mapSprite.AddAnimation(new Animation("IdleSouthwest", 7, 12, MapIdleAnimationInterval, true));
                _mapSprite.AddAnimation(new Animation("IdleWest", 13, 18, MapIdleAnimationInterval, true));
                _mapSprite.AddAnimation(new Animation("IdleNorthwest", 19, 24, MapIdleAnimationInterval, true));
                _mapSprite.AddAnimation(new Animation("IdleNorth", 25, 30, MapIdleAnimationInterval, true));
                _mapSprite.AddAnimation(new Animation("IdleNortheast", 31, 36, MapIdleAnimationInterval, true));
                _mapSprite.AddAnimation(new Animation("IdleEast", 37, 42, MapIdleAnimationInterval, true));
                _mapSprite.AddAnimation(new Animation("IdleSoutheast", 43, 48, MapIdleAnimationInterval, true));
            }
        }

        /// <summary>
        /// The default walk-animation interval for the animating map sprite.
        /// </summary>
        private int _mapWalkingAnimationInterval = 80;

        /// <summary>
        /// The default walk-animation interval for the animating map sprite.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public int MapWalkingAnimationInterval
        {
            get => _mapWalkingAnimationInterval;
            set => _mapWalkingAnimationInterval = value;
        }

        /// <summary>
        /// Add the standard character walk animations to this character.
        /// </summary>
        private void AddStandardCharacterWalkingAnimations()
        {
            AnimatingSprite sprite = _walkingSprite ?? _mapSprite;
            if (sprite != null)
            {
                sprite.AddAnimation(new Animation("WalkSouth", 1, 6, MapWalkingAnimationInterval, true));
                sprite.AddAnimation(new Animation("WalkSouthwest", 7, 12, MapWalkingAnimationInterval, true));
                sprite.AddAnimation(new Animation("WalkWest", 13, 18, MapWalkingAnimationInterval, true));
                sprite.AddAnimation(new Animation("WalkNorthwest", 19, 24, MapWalkingAnimationInterval, true));
                sprite.AddAnimation(new Animation("WalkNorth", 25, 30, MapWalkingAnimationInterval, true));
                sprite.AddAnimation(new Animation("WalkNortheast", 31, 36, MapWalkingAnimationInterval, true));
                sprite.AddAnimation(new Animation("WalkEast", 37, 42, MapWalkingAnimationInterval, true));
                sprite.AddAnimation(new Animation("WalkSoutheast", 43, 48, MapWalkingAnimationInterval, true));
            }
        }

        /// <summary>
        /// Reads a Character object from the content pipeline.
        /// </summary>
        public class CharacterReader : ContentTypeReader<Character>
        {
            /// <summary>
            /// Reads a Character object from the content pipeline.
            /// </summary>
            protected override Character Read(ContentReader input, Character existingInstance)
            {
                Character character = existingInstance;
                if (character == null)
                {
                    throw new ArgumentNullException("existingInstance");
                }

                input.ReadRawObject<WorldObject>(character as WorldObject);

                character.MapIdleAnimationInterval = input.ReadInt32();
                character.MapSprite = input.ReadObject<AnimatingSprite>();
                if (character.MapSprite != null)
                {
                    character.MapSprite.SourceOffset = new Vector2(character.MapSprite.SourceOffset.X - 32, character.MapSprite.SourceOffset.Y - 32);
                }
                character.AddStandardCharacterIdleAnimations();

                character.MapWalkingAnimationInterval = input.ReadInt32();
                character.WalkingSprite = input.ReadObject<AnimatingSprite>();
                if (character.WalkingSprite != null)
                {
                    character.WalkingSprite.SourceOffset = new Vector2(character.WalkingSprite.SourceOffset.X - 32, character.WalkingSprite.SourceOffset.Y - 32);
                }
                character.AddStandardCharacterWalkingAnimations();

                character.ResetAnimation(false);

                character._shadowTexture = input.ContentManager.Load<Texture2D>("Textures/Characters/CharacterShadow");

                return character;
            }
        }
    }
}
