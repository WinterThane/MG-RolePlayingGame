using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Characters;
using RolePlayingGame.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace RolePlayingGame.GearObjects
{
    public abstract class Gear : ContentObject
    {
        private string _name;
        /// <summary>
        /// The name of this gear.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        private string _description;
        /// <summary>
        /// The long description of this gear.
        /// </summary>
        public string Description
        {
            get => _description;
            set => _description = value;
        }

        /// <summary>
        /// Builds and returns a string describing the power of this gear.
        /// </summary>
        public virtual string GetPowerText()
        {
            return string.Empty;
        }

        /// <summary>
        /// The value of this gear.
        /// </summary>
        /// <remarks>If the value is less than zero, it cannot be sold.</remarks>
        private int _goldValue;

        /// <summary>
        /// The value of this gear.
        /// </summary>
        /// <remarks>If the value is less than zero, it cannot be sold.</remarks>
        public int GoldValue
        {
            get => _goldValue;
            set => _goldValue = value;
        }

        /// <summary>
        /// If true, the gear can be dropped.  If false, it cannot ever be dropped.
        /// </summary>
        private bool _isDroppable;

        /// <summary>
        /// If true, the gear can be dropped.  If false, it cannot ever be dropped.
        /// </summary>
        public bool IsDroppable
        {
            get => _isDroppable;
            set => _isDroppable = value;
        }

        /// <summary>
        /// The minimum character level required to equip or use this gear.
        /// </summary>
        private int _minimumCharacterLevel;

        /// <summary>
        /// The minimum character level required to equip or use this gear.
        /// </summary>
        public int MinimumCharacterLevel
        {
            get => _minimumCharacterLevel;
            set => _minimumCharacterLevel = value;
        }

        /// <summary>
        /// The list of the names of all supported classes.
        /// </summary>
        /// <remarks>Class names are compared case-insensitive.</remarks>
        private List<string> _supportedClasses = new();

        /// <summary>
        /// The list of the names of all supported classes.
        /// </summary>
        /// <remarks>Class names are compared case-insensitive.</remarks>
        public List<string> SupportedClasses => _supportedClasses;

        /// <summary>
        /// Check the restrictions on this object against the provided character.
        /// </summary>
        /// <returns>True if the gear could be used, false otherwise.</returns>
        public virtual bool CheckRestrictions(FightingCharacter fightingCharacter)
        {
            if (fightingCharacter == null)
            {
                throw new ArgumentNullException("fightingCharacter");
            }

            return (fightingCharacter.CharacterLevel >= MinimumCharacterLevel) && ((SupportedClasses.Count <= 0) || SupportedClasses.Contains(fightingCharacter.CharacterClass.Name));
        }

        /// <summary>
        /// Builds a string describing the restrictions on this piece of gear.
        /// </summary>
        public virtual string GetRestrictionsText()
        {
            StringBuilder sb = new();

            // add the minimum character level, if any
            if (MinimumCharacterLevel > 0)
            {
                sb.Append("Level - ");
                sb.Append(MinimumCharacterLevel.ToString());
                sb.Append("; ");
            }

            // add the classes
            if (SupportedClasses.Count > 0)
            {
                sb.Append("Class - ");
                bool firstClass = true;
                foreach (string className in SupportedClasses)
                {
                    if (firstClass)
                    {
                        firstClass = false;
                    }
                    else
                    {
                        sb.Append(",");
                    }
                    sb.Append(className);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// The content path and name of the icon for this gear.
        /// </summary>
        private string _iconTextureName;

        /// <summary>
        /// The content path and name of the icon for this gear.
        /// </summary>
        public string IconTextureName
        {
            get => _iconTextureName;
            set => _iconTextureName = value;
        }

        /// <summary>
        /// The icon texture for this gear.
        /// </summary>
        private Texture2D _iconTexture;

        /// <summary>
        /// The icon texture for this gear.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D IconTexture => _iconTexture;

        /// <summary>
        /// Draw the icon for this gear.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch object to use when drawing.</param>
        /// <param name="position">The position of the icon on the screen.</param>
        public virtual void DrawIcon(SpriteBatch spriteBatch, Vector2 position)
        {
            // check the parameters
            if (spriteBatch == null)
            {
                throw new ArgumentNullException("spriteBatch");
            }

            // draw the icon, if we there is a texture for it
            if (_iconTexture != null)
            {
                spriteBatch.Draw(_iconTexture, position, Color.White);
            }
        }

        /// <summary>
        /// Draw the description for this gear in the space provided.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch object to use when drawing.</param>
        /// <param name="spriteFont">The font that the text is drawn with.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="position">The position of the text on the screen.</param>
        /// <param name="maximumCharactersPerLine">
        /// The maximum length of a single line of text.
        /// </param>
        /// <param name="maximumLines">The maximum number of lines to draw.</param>
        public virtual void DrawDescription(SpriteBatch spriteBatch, SpriteFont spriteFont, Color color, Vector2 position, int maximumCharactersPerLine, int maximumLines)
        {
            // check the parameters
            if (spriteBatch == null)
            {
                throw new ArgumentNullException("spriteBatch");
            }
            if (spriteFont == null)
            {
                throw new ArgumentNullException("spriteFont");
            }
            if (maximumLines <= 0)
            {
                throw new ArgumentOutOfRangeException("maximumLines");
            }
            if (maximumCharactersPerLine <= 0)
            {
                throw new ArgumentOutOfRangeException("maximumCharactersPerLine");
            }

            // if the string is trivial, then this is really easy
            if (string.IsNullOrEmpty(_description))
            {
                return;
            }

            // if the text is short enough to fit on one line, then this is still easy
            if (_description.Length < maximumCharactersPerLine)
            {
                spriteBatch.DrawString(spriteFont, _description, position, color);
                return;
            }

            // construct a new string with carriage returns
            StringBuilder stringBuilder = new(_description);
            int currentLine = 0;
            int newLineIndex = 0;
            while (((_description.Length - newLineIndex) > maximumCharactersPerLine) && (currentLine < maximumLines))
            {
                _description.IndexOf(' ', 0);
                int nextIndex = newLineIndex;
                while (nextIndex < maximumCharactersPerLine)
                {
                    newLineIndex = nextIndex;
                    nextIndex = _description.IndexOf(' ', newLineIndex + 1);
                }
                stringBuilder.Replace(' ', '\n', newLineIndex, 1);
                currentLine++;
            }

            // draw the string
            spriteBatch.DrawString(spriteFont, stringBuilder.ToString(), position, color);
        }

        /// <summary>
        /// Reads a Gear object from the content pipeline.
        /// </summary>
        public class GearReader : ContentTypeReader<Gear>
        {
            /// <summary>
            /// Reads a Gear object from the content pipeline.
            /// </summary>
            protected override Gear Read(ContentReader input, Gear existingInstance)
            {
                Gear gear = existingInstance;
                if (gear == null)
                {
                    throw new ArgumentException("Unable to create new Gear objects.");
                }

                gear.AssetName = input.AssetName;

                // read gear settings
                gear.Name = input.ReadString();
                gear.Description = input.ReadString();
                gear.GoldValue = input.ReadInt32();
                gear.IsDroppable = input.ReadBoolean();
                gear.MinimumCharacterLevel = input.ReadInt32();
                gear.SupportedClasses.AddRange(input.ReadObject<List<string>>());
                gear.IconTextureName = input.ReadString();
                gear._iconTexture = input.ContentManager.Load<Texture2D>(System.IO.Path.Combine("Textures/Gear", gear.IconTextureName));

                return gear;
            }
        }
    }
}
