﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Quests;
using RolePlayingGame.SessionObjects;
using RolePlayingGame.TextFonts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RolePlayingGame.ScreensManager.Screens.GameScreens
{
    public class QuestLogScreen : ListScreen<Quest>
    {
        /// <summary>
        /// The quest that is shown when the screen is created.
        /// </summary>
        /// <remarks>
        /// Stored because new screens can't be added until the first update.
        /// </remarks>
        private Quest initialDetailQuest;

        protected string nameColumnText = "Name";
        private const int nameColumnInterval = 20;

        protected string stageColumnText = "Stage";
        private const int stageColumnInterval = 450;

        /// <summary>
        /// Get the list that this screen displays.
        /// </summary>
        public override ReadOnlyCollection<Quest> GetDataList()
        {
            List<Quest> quests = new List<Quest>();
            for (int i = 0; i <= Session.CurrentQuestIndex; i++)
            {
                if (i < Session.QuestLine.QuestsList.Count)
                {
                    quests.Add(Session.QuestLine.QuestsList[i]);
                }
            }

            return quests.AsReadOnly();
        }

        /// <summary>
        /// Creates a new EquipmentScreen object for the given player.
        /// </summary>
        public QuestLogScreen(Quest initialDetailQuest)
            : base()
        {
            // assign the parameter - null is permitted
            this.initialDetailQuest = initialDetailQuest;

            // configure the menu text
            _titleText = Session.QuestLine.Name;
            _selectButtonText = "Select";
            _backButtonText = "Back";
            _xButtonText = String.Empty;
            _yButtonText = String.Empty;
            _leftTriggerText = "Equipment";
            _rightTriggerText = "Statistics";

            // select the current quest
            SelectedIndex = Session.CurrentQuestIndex;
        }

        /// <summary>
        /// Handle user input.
        /// </summary>
        public override void HandleInput()
        {
            // open the initial QuestDetailScreen, if any
            // -- this is the first opportunity to add another screen
            if (initialDetailQuest != null)
            {
                ScreenManager.AddScreen(new QuestDetailsScreen(initialDetailQuest));
                // if the selected quest is in the list, make sure it's visible
                SelectedIndex = Session.QuestLine.QuestsList.IndexOf(initialDetailQuest);
                // only open the screen once
                initialDetailQuest = null;
            }

            base.HandleInput();
        }


        /// <summary>
        /// Respond to the triggering of the X button (and related key).
        /// </summary>
        protected override void SelectTriggered(Quest entry)
        {
            ScreenManager.AddScreen(new QuestDetailsScreen(entry));
        }


        /// <summary>
        /// Switch to the screen to the "left" of this one in the UI.
        /// </summary>
        protected override void PageScreenLeft()
        {
            ExitScreen();
            ScreenManager.AddScreen(new InventoryScreen(false));
        }


        /// <summary>
        /// Switch to the screen to the "right" of this one in the UI.
        /// </summary>
        protected override void PageScreenRight()
        {
            ExitScreen();
            ScreenManager.AddScreen(new StatisticsScreen(Session.Party.Players[0]));
        }

        /// <summary>
        /// Draw the quest at the given position in the list.
        /// </summary>
        /// <param name="contentEntry">The quest to draw.</param>
        /// <param name="position">The position to draw the entry at.</param>
        /// <param name="isSelected">If true, this item is selected.</param>
        protected override void DrawEntry(Quest entry, Vector2 position,
            bool isSelected)
        {
            // check the parameter
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 drawPosition = position;

            // draw the name
            Color color = isSelected ? Fonts.HighlightColor : Fonts.DisplayColor;
            drawPosition.Y += _listLineSpacing / 4;
            drawPosition.X += nameColumnInterval;
            spriteBatch.DrawString(Fonts.GearInfoFont, entry.Name, drawPosition, color);

            // draw the stage
            drawPosition.X += stageColumnInterval;
            string stageText = String.Empty;
            switch (entry.Stage)
            {
                case Quest.QuestStage.Completed:
                    stageText = "Completed";
                    break;

                case Quest.QuestStage.InProgress:
                    stageText = "In Progress";
                    break;

                case Quest.QuestStage.NotStarted:
                    stageText = "Not Started";
                    break;

                case Quest.QuestStage.RequirementsMet:
                    stageText = "Requirements Met";
                    break;
            }
            spriteBatch.DrawString(Fonts.GearInfoFont, stageText, drawPosition, color);

            // turn on or off the select button
            if (isSelected)
            {
                _selectButtonText = "Select";
            }
        }


        /// <summary>
        /// Draw the description of the selected item.
        /// </summary>
        protected override void DrawSelectedDescription(Quest entry) { }


        /// <summary>
        /// Draw the column headers above the list.
        /// </summary>
        protected override void DrawColumnHeaders()
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 position = _listEntryStartPosition;

            position.X += nameColumnInterval;
            if (!String.IsNullOrEmpty(nameColumnText))
            {
                spriteBatch.DrawString(Fonts.CaptionFont, nameColumnText, position,
                    Fonts.CaptionColor);
            }

            position.X += stageColumnInterval;
            if (!String.IsNullOrEmpty(stageColumnText))
            {
                spriteBatch.DrawString(Fonts.CaptionFont, stageColumnText, position,
                    Fonts.CaptionColor);
            }
        }
    }
}
