using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Combat;
using RolePlayingGame.Engine;
using RolePlayingGame.GearObjects;
using RolePlayingGame.ScreensManager.Screens.MenuScreens;
using RolePlayingGame.SessionObjects;
using RolePlayingGame.TextFonts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RolePlayingGame.ScreensManager.Screens.GameScreens
{
    public class InventoryScreen : ListScreen<ContentEntry<Gear>>
    {
        protected string _nameColumnText = "Name";
        private const int _nameColumnInterval = 80;

        protected string _powerColumnText = "Power (min, max)";
        private const int _powerColumnInterval = 270;

        protected string _quantityColumnText = "Qty";
        private const int _quantityColumnInterval = 450;

        /// <summary>
        /// If true, the menu is only displaying items; otherwise, only equipment.
        /// </summary>
        protected bool _isItems;

        /// <summary>
        /// Retrieve the list of gear shown in this menu.
        /// </summary>
        /// <returns></returns>
        public override ReadOnlyCollection<ContentEntry<Gear>> GetDataList()
        {
            List<ContentEntry<Gear>> dataList = new List<ContentEntry<Gear>>();
            ReadOnlyCollection<ContentEntry<Gear>> inventory = Session.Party.Inventory;

            // build a new list of only the desired gear
            foreach (ContentEntry<Gear> gearEntry in inventory)
            {
                if (_isItems)
                {
                    if (gearEntry.Content is Item)
                    {
                        dataList.Add(gearEntry);
                    }
                }
                else
                {
                    if (gearEntry.Content is Equipment)
                    {
                        dataList.Add(gearEntry);
                    }
                }
            }

            // sort the list by name
            dataList.Sort(delegate (ContentEntry<Gear> gearEntry1, ContentEntry<Gear> gearEntry2)
            {
                // handle null values
                if ((gearEntry1 == null) || (gearEntry1.Content == null))
                {
                    return ((gearEntry2 == null) || (gearEntry2.Content == null) ?
                        0 : 1);
                }
                else if ((gearEntry2 == null) || (gearEntry2.Content == null))
                {
                    return -1;
                }

                // sort by name
                return gearEntry1.Content.Name.CompareTo(gearEntry2.Content.Name);
            });

            return dataList.AsReadOnly();
        }

        /// <summary>
        /// Constructs a new InventoryScreen object.
        /// </summary>
        public InventoryScreen(bool isItems) : base()
        {
            this._isItems = isItems;

            // configure the menu text
            _titleText = "Inventory";
            _selectButtonText = "Select";
            _backButtonText = "Back";
            _xButtonText = "Drop";
            _yButtonText = string.Empty;
            ResetTriggerText();
        }

        /// <summary>
        /// Delegate for item-selection events.
        /// </summary>
        public delegate void GearSelectedHandler(Gear gear);

        /// <summary>
        /// Responds when an item is selected by this menu.
        /// </summary>
        /// <remarks>
        /// Typically used by the calling menu, like the combat HUD menu, 
        /// to respond to selection.
        /// </remarks>
        public event GearSelectedHandler GearSelected;

        /// <summary>
        /// Respond to the triggering of the Select action (and related key).
        /// </summary>
        protected override void SelectTriggered(ContentEntry<Gear> entry)
        {
            // check the parameter
            if ((entry == null) || (entry.Content == null))
            {
                return;
            }

            // if the event is valid, fire it and exit this screen
            if (GearSelected != null)
            {
                GearSelected(entry.Content);
                ExitScreen();
                return;
            }

            // otherwise, open the selection screen over this screen
            ScreenManager.AddScreen(new PlayerSelectionScreen(entry.Content));
        }

        /// <summary>
        /// Respond to the triggering of the X button (and related key).
        /// </summary>
        protected override void ButtonXPressed(ContentEntry<Gear> entry)
        {
            // check the parameter
            if ((entry == null) || (entry.Content == null))
            {
                return;
            }

            // check whether the gear could be dropped
            if (!entry.Content.IsDroppable)
            {
                return;
            }

            // add a message box confirming the drop
            MessageBoxScreen dropEquipmentConfirmationScreen = new("Are you sure you want to drop the " + entry.Content.Name + "?");
            dropEquipmentConfirmationScreen.Accepted += new EventHandler<EventArgs>(delegate (object sender, EventArgs args)
            {
                Session.Party.RemoveFromInventory(entry.Content, 1);
            });
            ScreenManager.AddScreen(dropEquipmentConfirmationScreen);
        }

        /// <summary>
        /// Switch to the screen to the "left" of this one in the UI.
        /// </summary>
        protected override void PageScreenLeft()
        {
            if (CombatEngine.IsActive)
            {
                return;
            }

            if (_isItems)
            {
                ExitScreen();
                ScreenManager.AddScreen(new StatisticsScreen(Session.Party.Players[0]));
            }
            else
            {
                _isItems = !_isItems;
                ResetTriggerText();
            }
        }

        /// <summary>
        /// Switch to the screen to the "right" of this one in the UI.
        /// </summary>
        protected override void PageScreenRight()
        {
            if (CombatEngine.IsActive)
            {
                return;
            }

            if (_isItems)
            {
                _isItems = !_isItems;
                ResetTriggerText();
            }
            else
            {
                ExitScreen();
                ScreenManager.AddScreen(new QuestLogScreen(null));
            }
        }

        /// <summary>
        /// Reset the trigger button text to the names of the 
        /// previous and next UI screens.
        /// </summary>
        protected virtual void ResetTriggerText()
        {
            if (CombatEngine.IsActive)
            {
                _leftTriggerText = _rightTriggerText = string.Empty;
            }
            else
            {
                if (_isItems)
                {
                    _leftTriggerText = "Statistics";
                    _rightTriggerText = "Equipment";
                }
                else
                {
                    _leftTriggerText = "Items";
                    _rightTriggerText = "Quests";
                }
            }
        }

        /// <summary>
        /// Draw the gear's content entry at the given position in the list.
        /// </summary>
        /// <param name="contentEntry">The content entry to draw.</param>
        /// <param name="position">The position to draw the entry at.</param>
        /// <param name="isSelected">If true, this item is selected.</param>
        protected override void DrawEntry(ContentEntry<Gear> entry, Vector2 position, bool isSelected)
        {
            // check the parameter
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            Gear gear = entry.Content;
            if (gear == null)
            {
                return;
            }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 drawPosition = position;

            // draw the icon
            spriteBatch.Draw(gear.IconTexture, drawPosition + _iconOffset, Color.White);

            // draw the name
            Color color = isSelected ? Fonts.HighlightColor : Fonts.DisplayColor;
            drawPosition.Y += _listLineSpacing / 4;
            drawPosition.X += _nameColumnInterval;
            spriteBatch.DrawString(Fonts.GearInfoFont, gear.Name, drawPosition, color);

            // draw the power
            drawPosition.X += _powerColumnInterval;
            string powerText = gear.GetPowerText();
            Vector2 powerTextSize = Fonts.GearInfoFont.MeasureString(powerText);
            Vector2 powerPosition = drawPosition;
            powerPosition.Y -= (float)Math.Ceiling((powerTextSize.Y - 30f) / 2);
            spriteBatch.DrawString(Fonts.GearInfoFont, powerText, powerPosition, color);

            // draw the quantity
            drawPosition.X += _quantityColumnInterval;
            spriteBatch.DrawString(Fonts.GearInfoFont, entry.Count.ToString(), drawPosition, color);

            // turn on or off the select and drop buttons
            if (isSelected)
            {
                _selectButtonText = "Select";
                _xButtonText = entry.Content.IsDroppable ? "Drop" : string.Empty;
            }
        }

        /// <summary>
        /// Draw the description of the selected item.
        /// </summary>
        protected override void DrawSelectedDescription(ContentEntry<Gear> entry)
        {
            // check the parameter
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            Gear gear = entry.Content;
            if (gear == null)
            {
                return;
            }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 position = _descriptionTextPosition;

            // draw the description
            // -- it's up to the content owner to fit the description
            string text = gear.Description;
            if (!string.IsNullOrEmpty(text))
            {
                spriteBatch.DrawString(Fonts.DescriptionFont, text, position, Fonts.DescriptionColor);
                position.Y += Fonts.DescriptionFont.LineSpacing;
            }

            // draw additional information for equipment
            Equipment equipment = entry.Content as Equipment;
            if (equipment != null)
            {
                // draw the modifiers
                text = equipment.OwnerBuffStatistics.GetModifierString();
                if (!string.IsNullOrEmpty(text))
                {
                    spriteBatch.DrawString(Fonts.DescriptionFont, text, position, Fonts.DescriptionColor);
                    position.Y += Fonts.DescriptionFont.LineSpacing;
                }
            }

            // draw the restrictions
            text = entry.Content.GetRestrictionsText();
            if (!string.IsNullOrEmpty(text))
            {
                spriteBatch.DrawString(Fonts.DescriptionFont, text, position, Fonts.DescriptionColor);
                position.Y += Fonts.DescriptionFont.LineSpacing;
            }
        }

        /// <summary>
        /// Draw the column headers above the gear list.
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

            position.X += _quantityColumnInterval;
            if (!string.IsNullOrEmpty(_quantityColumnText))
            {
                spriteBatch.DrawString(Fonts.CaptionFont, _quantityColumnText, position, Fonts.CaptionColor);
            }
        }
    }
}
