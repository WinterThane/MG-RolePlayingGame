using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Animations;
using RolePlayingGame.Data;
using System;

namespace RolePlayingGame.Characters
{
    public class Player : FightingCharacter, ICloneable
    {
        /// <summary>
        /// The current set of persistent statistics modifiers - damage, etc.
        /// </summary>
        [ContentSerializerIgnore]
        public StatisticsValue StatisticsModifiers = new();

        /// <summary>
        /// The current set of statistics, including damage, etc.
        /// </summary>
        [ContentSerializerIgnore]
        public StatisticsValue CurrentStatistics => CharacterStatistics + StatisticsModifiers;

        /// <summary>
        /// The amount of gold that the player has when it joins the party.
        /// </summary>
        private int _gold;

        /// <summary>
        /// The amount of gold that the player has when it joins the party.
        /// </summary>
        public int Gold
        {
            get => _gold;
            set => _gold = value;
        }

        /// <summary>
        /// The dialogue that the player says when it is greeted as an Npc in the world.
        /// </summary>
        private string _introductionDialogue;

        /// <summary>
        /// The dialogue that the player says when it is greeted as an Npc in the world.
        /// </summary>
        public string IntroductionDialogue
        {
            get => _introductionDialogue;
            set => _introductionDialogue = value;
        }

        /// <summary>
        /// The dialogue that the player says when its offer to join is accepted.
        /// </summary>
        private string _joinAcceptedDialogue;

        /// <summary>
        /// The dialogue that the player says when its offer to join is accepted.
        /// </summary>
        public string JoinAcceptedDialogue
        {
            get => _joinAcceptedDialogue;
            set => _joinAcceptedDialogue = value;
        }

        /// <summary>
        /// The dialogue that the player says when its offer to join is rejected.
        /// </summary>
        private string _joinRejectedDialogue;

        /// <summary>
        /// The dialogue that the player says when its offer to join is rejected.
        /// </summary>
        public string JoinRejectedDialogue
        {
            get => _joinRejectedDialogue;
            set => _joinRejectedDialogue = value;
        }

        /// <summary>
        /// The name of the active portrait texture.
        /// </summary>
        private string _activePortraitTextureName;

        /// <summary>
        /// The name of the active portrait texture.
        /// </summary>
        public string ActivePortraitTextureName
        {
            get => _activePortraitTextureName;
            set => _activePortraitTextureName = value;
        }

        /// <summary>
        /// The active portrait texture.
        /// </summary>
        private Texture2D _activePortraitTexture;

        /// <summary>
        /// The active portrait texture.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D ActivePortraitTexture => _activePortraitTexture;

        /// <summary>
        /// The name of the inactive portrait texture.
        /// </summary>
        private string _inactivePortraitTextureName;

        /// <summary>
        /// The name of the inactive portrait texture.
        /// </summary>
        public string InactivePortraitTextureName
        {
            get => _inactivePortraitTextureName;
            set => _inactivePortraitTextureName = value;
        }

        /// <summary>
        /// The inactive portrait texture.
        /// </summary>
        private Texture2D _inactivePortraitTexture;

        /// <summary>
        /// The inactive portrait texture.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D InactivePortraitTexture => _inactivePortraitTexture;

        /// <summary>
        /// The name of the unselectable portrait texture.
        /// </summary>
        private string _unselectablePortraitTextureName;

        /// <summary>
        /// The name of the unselectable portrait texture.
        /// </summary>
        public string UnselectablePortraitTextureName
        {
            get => _unselectablePortraitTextureName;
            set { _unselectablePortraitTextureName = value; }
        }

        /// <summary>
        /// The unselectable portrait texture.
        /// </summary>
        private Texture2D _unselectablePortraitTexture;

        /// <summary>
        /// The unselectable portrait texture.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D UnselectablePortraitTexture => _unselectablePortraitTexture;

        /// <summary>
        /// Read a Player object from the content pipeline.
        /// </summary>
        public class PlayerReader : ContentTypeReader<Player>
        {
            protected override Player Read(ContentReader input, Player existingInstance)
            {
                Player player = existingInstance;
                if (player == null)
                {
                    player = new Player();
                }

                input.ReadRawObject<FightingCharacter>(player as FightingCharacter);

                player.Gold = input.ReadInt32();
                player.IntroductionDialogue = input.ReadString();
                player.JoinAcceptedDialogue = input.ReadString();
                player.JoinRejectedDialogue = input.ReadString();
                player.ActivePortraitTextureName = input.ReadString();
                player._activePortraitTexture = input.ContentManager.Load<Texture2D>(System.IO.Path.Combine("Textures/Characters/Portraits", player.ActivePortraitTextureName));
                player.InactivePortraitTextureName = input.ReadString();
                player._inactivePortraitTexture = input.ContentManager.Load<Texture2D>(System.IO.Path.Combine("Textures/Characters/Portraits", player.InactivePortraitTextureName));
                player.UnselectablePortraitTextureName = input.ReadString();
                player._unselectablePortraitTexture = input.ContentManager.Load<Texture2D>(System.IO.Path.Combine("Textures/Characters/Portraits", player.UnselectablePortraitTextureName));

                return player;
            }
        }

        public object Clone()
        {
            Player player = new()
            {
                _activePortraitTexture = _activePortraitTexture,
                _activePortraitTextureName = _activePortraitTextureName,
                AssetName = AssetName,
                CharacterClass = CharacterClass,
                CharacterClassContentName = CharacterClassContentName,
                CharacterLevel = CharacterLevel,
                CombatAnimationInterval = CombatAnimationInterval,
                CombatSprite = CombatSprite.Clone() as AnimatingSprite,
                Direction = Direction
            };
            player.EquippedEquipment.AddRange(EquippedEquipment);
            player.Experience = Experience;
            player._gold = _gold;
            player._inactivePortraitTexture = _inactivePortraitTexture;
            player._inactivePortraitTextureName = _inactivePortraitTextureName;
            player.InitialEquipmentContentNames.AddRange(InitialEquipmentContentNames);
            player._introductionDialogue = _introductionDialogue;
            player.Inventory.AddRange(Inventory);
            player._joinAcceptedDialogue = _joinAcceptedDialogue;
            player._joinRejectedDialogue = _joinRejectedDialogue;
            player.MapIdleAnimationInterval = MapIdleAnimationInterval;
            player.MapPosition = MapPosition;
            player.MapSprite = MapSprite.Clone() as AnimatingSprite;
            player.MapWalkingAnimationInterval = MapWalkingAnimationInterval;
            player.Name = Name;
            player.ShadowTexture = ShadowTexture;
            player.State = State;
            player._unselectablePortraitTexture = _unselectablePortraitTexture;
            player._unselectablePortraitTextureName = _unselectablePortraitTextureName;
            player.WalkingSprite = WalkingSprite.Clone() as AnimatingSprite;

            player.RecalculateEquipmentStatistics();
            player.RecalculateTotalDefenseRanges();
            player.RecalculateTotalTargetDamageRange();
            player.ResetAnimation(false);
            player.ResetBaseStatistics();

            return player;
        }
    }
}
