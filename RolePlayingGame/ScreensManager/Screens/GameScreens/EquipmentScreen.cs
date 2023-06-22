using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Characters;
using RolePlayingGame.GearObjects;
using RolePlayingGame.SessionObjects;
using RolePlayingGame.TextFonts;
using System;
using System.Collections.ObjectModel;

namespace RolePlayingGame.ScreensManager.Screens.GameScreens
{
    public class EquipmentScreen : ListScreen<Equipment>
    {
        protected string _nameColumnText = "Name";
        private const int _nameColumnInterval = 80;

        protected string _powerColumnText = "Power (min, max)";
        private const int _powerColumnInterval = 270;

        protected string _slotColumnText = "Slot";
        private const int _slotColumnInterval = 400;

        /// <summary>
        /// The FightingCharacter object whose equipment is displayed.
        /// </summary>
        private FightingCharacter _fightingCharacter;

        /// <summary>
        /// Get the list that this screen displays.
        /// </summary>
        public override ReadOnlyCollection<Equipment> GetDataList()
        {
            return _fightingCharacter.EquippedEquipment.AsReadOnly();
        }

        /// <summary>
        /// Creates a new EquipmentScreen object for the given player.
        /// </summary>
        public EquipmentScreen(FightingCharacter fightingCharacter) : base()
        {
            // check the parameter
            if (fightingCharacter == null)
            {
                throw new ArgumentNullException("fightingCharacter");
            }
            _fightingCharacter = fightingCharacter;

            // sort the player's equipment
            _fightingCharacter.EquippedEquipment.Sort(delegate (Equipment equipment1, Equipment equipment2)
            {
                // handle null values
                if (equipment1 == null)
                {
                    return (equipment2 == null ? 0 : 1);
                }
                else if (equipment2 == null)
                {
                    return -1;
                }

                // handle weapons - they're always first in the list
                if (equipment1 is Weapon)
                {
                    return (equipment2 is Weapon ?
                        equipment1.Name.CompareTo(equipment2.Name) : -1);
                }
                else if (equipment2 is Weapon)
                {
                    return 1;
                }

                // compare armor slots
                Armor armor1 = equipment1 as Armor;
                Armor armor2 = equipment2 as Armor;
                if ((armor1 != null) && (armor2 != null))
                {
                    return armor1.Slot.CompareTo(armor2.Slot);
                }

                return 0;
            });

            // configure the menu text
            _titleText = "Equipped Gear";
            _selectButtonText = string.Empty;
            _backButtonText = "Back";
            _xButtonText = "Unequip";
            _yButtonText = string.Empty;
            _leftTriggerText = string.Empty;
            _rightTriggerText = string.Empty;
        }

        /// <summary>
        /// Respond to the triggering of the X button (and related key).
        /// </summary>
        protected override void ButtonXPressed(Equipment entry)
        {
            // remove the equipment from the player's equipped list
            _fightingCharacter.Unequip(entry);

            // add the equipment back to the party's inventory
            Session.Party.AddToInventory(entry, 1);
        }

        /// <summary>
        /// Draw the equipment at the given position in the list.
        /// </summary>
        /// <param name="contentEntry">The equipment to draw.</param>
        /// <param name="position">The position to draw the entry at.</param>
        /// <param name="isSelected">If true, this item is selected.</param>
        protected override void DrawEntry(Equipment entry, Vector2 position, bool isSelected)
        {
            // check the parameter
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 drawPosition = position;

            // draw the icon
            spriteBatch.Draw(entry.IconTexture, drawPosition + _iconOffset, Color.White);

            // draw the name
            Color color = isSelected ? Fonts.HighlightColor : Fonts.DisplayColor;
            drawPosition.Y += _listLineSpacing / 4;
            drawPosition.X += _nameColumnInterval;
            spriteBatch.DrawString(Fonts.GearInfoFont, entry.Name, drawPosition, color);

            // draw the power
            drawPosition.X += _powerColumnInterval;
            string powerText = entry.GetPowerText();
            Vector2 powerTextSize = Fonts.GearInfoFont.MeasureString(powerText);
            Vector2 powerPosition = drawPosition;
            powerPosition.Y -= (float)Math.Ceiling((powerTextSize.Y - 30f) / 2);
            spriteBatch.DrawString(Fonts.GearInfoFont, powerText, powerPosition, color);

            // draw the slot
            drawPosition.X += _slotColumnInterval;
            if (entry is Weapon)
            {
                spriteBatch.DrawString(Fonts.GearInfoFont, "Weapon", drawPosition, color);
            }
            else if (entry is Armor)
            {
                Armor armor = entry as Armor;
                spriteBatch.DrawString(Fonts.GearInfoFont, armor.Slot.ToString(), drawPosition, color);
            }

            // turn on or off the unequip button
            if (isSelected)
            {
                _xButtonText = "Unequip";
            }
        }

        /// <summary>
        /// Draw the description of the selected item.
        /// </summary>
        protected override void DrawSelectedDescription(Equipment entry)
        {
            // check the parameter
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 position = _descriptionTextPosition;

            // draw the description
            // -- it's up to the content owner to fit the description
            string text = entry.Description;
            if (!string.IsNullOrEmpty(text))
            {
                spriteBatch.DrawString(Fonts.DescriptionFont, text, position, Fonts.DescriptionColor);
                position.Y += Fonts.DescriptionFont.LineSpacing;
            }

            // draw the modifiers
            text = entry.OwnerBuffStatistics.GetModifierString();
            if (!string.IsNullOrEmpty(text))
            {
                spriteBatch.DrawString(Fonts.DescriptionFont, text, position, Fonts.DescriptionColor);
                position.Y += Fonts.DescriptionFont.LineSpacing;
            }

            // draw the restrictions
            text = entry.GetRestrictionsText();
            if (!string.IsNullOrEmpty(text))
            {
                spriteBatch.DrawString(Fonts.DescriptionFont, text, position, Fonts.DescriptionColor);
                position.Y += Fonts.DescriptionFont.LineSpacing;
            }
        }

        /// <summary>
        /// Draw the column headers above the list.
        /// </summary>
        protected override void DrawColumnHeaders()
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 position = _listEntryStartPosition;

            position.X += _nameColumnInterval;
            if (!string.IsNullOrEmpty(_nameColumnText))
            {
                spriteBatch.DrawString(Fonts.CaptionFont, _nameColumnText, position, Fonts.CaptionColor);
            }

            position.X += _powerColumnInterval;
            if (!string.IsNullOrEmpty(_powerColumnText))
            {
                spriteBatch.DrawString(Fonts.CaptionFont, _powerColumnText, position, Fonts.CaptionColor);
            }

            position.X += _slotColumnInterval;
            if (!string.IsNullOrEmpty(_slotColumnText))
            {
                spriteBatch.DrawString(Fonts.CaptionFont, _slotColumnText, position, Fonts.CaptionColor);
            }
        }
    }
}
