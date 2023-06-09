﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Animations;
using RolePlayingGame.Data;

namespace RolePlayingGame.GearObjects
{
    public class Weapon : Equipment
    {
        /// <summary>
        /// Builds and returns a string describing the power of this weapon.
        /// </summary>
        public override string GetPowerText()
        {
            return "Weapon Attack: " + TargetDamageRange.ToString();
        }

        /// <summary>
        /// The range of health damage applied by this weapon to its target.
        /// </summary>
        /// <remarks>Damage range values are positive, and will be subtracted.</remarks>
        private Int32Range _targetDamageRange;

        /// <summary>
        /// The range of health damage applied by this weapon to its target.
        /// </summary>
        /// <remarks>Damage range values are positive, and will be subtracted.</remarks>
        public Int32Range TargetDamageRange
        {
            get => _targetDamageRange;
            set => _targetDamageRange = value;
        }

        /// <summary>
        /// The name of the sound effect cue played when the weapon is swung.
        /// </summary>
        private string _swingCueName;

        /// <summary>
        /// The name of the sound effect cue played when the weapon is swung.
        /// </summary>
        public string SwingCueName
        {
            get => _swingCueName;
            set => _swingCueName = value;
        }

        /// <summary>
        /// The name of the sound effect cue played when the weapon hits its target.
        /// </summary>
        private string _hitCueName;

        /// <summary>
        /// The name of the sound effect cue played when the weapon hits its target.
        /// </summary>
        public string HitCueName
        {
            get => _hitCueName;
            set => _hitCueName = value;
        }

        /// <summary>
        /// The name of the sound effect cue played when the weapon is blocked.
        /// </summary>
        private string _blockCueName;

        /// <summary>
        /// The name of the sound effect cue played when the weapon is blocked.
        /// </summary>
        public string BlockCueName
        {
            get => _blockCueName;
            set => _blockCueName = value;
        }

        /// <summary>
        /// The overlay sprite for this weapon.
        /// </summary>
        private AnimatingSprite _overlay;

        /// <summary>
        /// The overlay sprite for this weapon.
        /// </summary>
        public AnimatingSprite Overlay
        {
            get => _overlay;
            set => _overlay = value;
        }

        /// <summary>
        /// Read the Weapon type from the content pipeline.
        /// </summary>
        public class WeaponReader : ContentTypeReader<Weapon>
        {
            /// <summary>
            /// Read the Weapon type from the content pipeline.
            /// </summary>
            protected override Weapon Read(ContentReader input, Weapon existingInstance)
            {
                Weapon weapon = existingInstance;

                if (weapon == null)
                {
                    weapon = new Weapon();
                }

                // read the gear settings
                input.ReadRawObject<Equipment>(weapon as Equipment);

                // read the weapon settings
                weapon.TargetDamageRange = input.ReadObject<Int32Range>();
                weapon.SwingCueName = input.ReadString();
                weapon.HitCueName = input.ReadString();
                weapon.BlockCueName = input.ReadString();
                weapon.Overlay = input.ReadObject<AnimatingSprite>();
                weapon.Overlay.SourceOffset = new Vector2(weapon.Overlay.FrameDimensions.X / 2, weapon.Overlay.FrameDimensions.Y);

                return weapon;
            }
        }
    }
}
