﻿using RolePlayingGame.Characters;
using RolePlayingGame.Data;
using RolePlayingGame.GearObjects;
using System;
using System.Collections.Generic;

namespace RolePlayingGame.SessionObjects
{
    public class PlayerSaveData
    {
        /// <summary>
        /// The asset name of the player itself.
        /// </summary>
        public string assetName;

        /// <summary>
        /// The character level of the player.
        /// </summary>
        public int characterLevel;

        /// <summary>
        /// The experience gained by the player since their last level.
        /// </summary>
        public int experience;

        /// <summary>
        /// The asset names of the equipped gear.
        /// </summary>
        public List<string> equipmentAssetNames = new List<string>();

        /// <summary>
        /// All permanent statistics modifiers to the character (i.e. damage).
        /// </summary>
        public StatisticsValue statisticsModifiers;

        /// <summary>
        /// Creates a new PlayerData object.
        /// </summary>
        public PlayerSaveData() { }


        /// <summary>
        /// Creates a new PlayerData object from the given Player object.
        /// </summary>
        public PlayerSaveData(Player player)
            : this()
        {
            // check the parameter
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            assetName = player.AssetName;
            characterLevel = player.CharacterLevel;
            experience = player.Experience;
            foreach (Equipment equipment in player.EquippedEquipment)
            {
                equipmentAssetNames.Add(equipment.AssetName);
            }
            statisticsModifiers = player.StatisticsModifiers;
        }
    }
}
