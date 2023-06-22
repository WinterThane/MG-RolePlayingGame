using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using RolePlayingGame.Animations;
using RolePlayingGame.Data;

namespace RolePlayingGame.GearObjects
{
    public class Item : Gear
    {
        /// <summary>
        /// Flags that specify when an item may be used.
        /// </summary>
        public enum ItemUsage
        {
            Combat = 1,
            NonCombat = 2,
        };

        /// <summary>
        /// Description of when the item may be used.
        /// </summary>
        /// <remarks>Defaults to "either", with both values.</remarks>
        private ItemUsage _usage = ItemUsage.Combat | ItemUsage.NonCombat;

        /// <summary>
        /// Description of when the item may be used.
        /// </summary>
        /// <remarks>Defaults to "either", with both values.</remarks>
        [ContentSerializer(Optional = true)]
        public ItemUsage Usage
        {
            get => _usage;
            set => _usage = value;
        }

        /// <summary>
        /// Builds and returns a string describing the power of this item.
        /// </summary>
        public override string GetPowerText()
        {
            return TargetEffectRange.GetModifierString();
        }

        /// <summary>
        /// If true, the statistics change are used as a debuff (subtracted).
        /// Otherwise, the statistics change is used as a buff (added).
        /// </summary>
        private bool _isOffensive;

        /// <summary>
        /// If true, the statistics change are used as a debuff (subtracted).
        /// Otherwise, the statistics change is used as a buff (added).
        /// </summary>
        public bool IsOffensive
        {
            get => _isOffensive;
            set => _isOffensive = value;
        }

        /// <summary>
        /// The duration of the effect of this item on its target, in rounds.
        /// </summary>
        /// <remarks>
        /// If the duration is zero, then the effects last for the rest of the battle.
        /// </remarks>
        private int _targetDuration;

        /// <summary>
        /// The duration of the effect of this item on its target, in rounds.
        /// </summary>
        /// <remarks>
        /// If the duration is zero, then the effects last for the rest of the battle.
        /// </remarks>
        public int TargetDuration
        {
            get => _targetDuration;
            set => _targetDuration = value;
        }

        /// <summary>
        /// The range of statistics effects of this item on its target.
        /// </summary>
        /// <remarks>
        /// This is a debuff if IsOffensive is true, otherwise it's a buff.
        /// </remarks>
        private StatisticsRange _targetEffectRange = new StatisticsRange();

        /// <summary>
        /// The range of statistics effects of this item on its target.
        /// </summary>
        /// <remarks>
        /// This is a debuff if IsOffensive is true, otherwise it's a buff.
        /// </remarks>
        public StatisticsRange TargetEffectRange
        {
            get => _targetEffectRange;
            set => _targetEffectRange = value;
        }

        /// <summary>
        /// The number of simultaneous, adjacent targets affected by this item.
        /// </summary>
        private int _adjacentTargets;

        /// <summary>
        /// The number of simultaneous, adjacent targets affected by this item.
        /// </summary>
        public int AdjacentTargets
        {
            get => _adjacentTargets;
            set => _adjacentTargets = value;
        }

        /// <summary>
        /// The name of the sound effect cue played when the item is used.
        /// </summary>
        private string _usingCueName;

        /// <summary>
        /// The name of the sound effect cue played when the item is used.
        /// </summary>
        public string UsingCueName
        {
            get => _usingCueName;
            set => _usingCueName = value;
        }

        /// <summary>
        /// The name of the sound effect cue played when the item effect is traveling.
        /// </summary>
        private string _travelingCueName;

        /// <summary>
        /// The name of the sound effect cue played when the item effect is traveling.
        /// </summary>
        public string TravelingCueName
        {
            get => _travelingCueName;
            set => _travelingCueName = value;
        }

        /// <summary>
        /// The name of the sound effect cue played when the item affects its target.
        /// </summary>
        private string _impactCueName;

        /// <summary>
        /// The name of the sound effect cue played when the item affects its target.
        /// </summary>
        public string ImpactCueName
        {
            get => _impactCueName;
            set => _impactCueName = value;
        }

        /// <summary>
        /// The name of the sound effect cue played when the item effect is blocked.
        /// </summary>
        private string _blockCueName;

        /// <summary>
        /// The name of the sound effect cue played when the item effect is blocked.
        /// </summary>
        public string BlockCueName
        {
            get => _blockCueName;
            set => _blockCueName = value;
        }

        /// <summary>
        /// An animating sprite used when this item is used.
        /// </summary>
        /// <remarks>
        /// This is optional.  If it is null, then a Using or Creating animation
        /// in SpellSprite is used.
        /// </remarks>
        private AnimatingSprite _creationSprite;

        /// <summary>
        /// An animating sprite used when this item is used.
        /// </summary>
        /// <remarks>
        /// This is optional.  If it is null, then a Using or Creating animation
        /// in SpellSprite is used.
        /// </remarks>
        [ContentSerializer(Optional = true)]
        public AnimatingSprite CreationSprite
        {
            get => _creationSprite;
            set => _creationSprite = value;
        }

        /// <summary>
        /// The animating sprite used when this item is in action.
        /// </summary>
        private AnimatingSprite _spellSprite;

        /// <summary>
        /// The animating sprite used when this item is in action.
        /// </summary>
        public AnimatingSprite SpellSprite
        {
            get => _spellSprite;
            set => _spellSprite = value;
        }

        /// <summary>
        /// The overlay sprite for this item.
        /// </summary>
        private AnimatingSprite _overlay;

        /// <summary>
        /// The overlay sprite for this item.
        /// </summary>
        public AnimatingSprite Overlay
        {
            get => _overlay;
            set => _overlay = value;
        }

        /// <summary>
        /// Read an Item object from the content pipeline
        /// </summary>
        public class ItemReader : ContentTypeReader<Item>
        {
            protected override Item Read(ContentReader input, Item existingInstance)
            {
                Item item = existingInstance;
                if (item == null)
                {
                    item = new Item();
                }

                // read gear settings
                input.ReadRawObject<Gear>(item as Gear);

                // read item settings
                item.Usage = (ItemUsage)input.ReadInt32();
                item.IsOffensive = input.ReadBoolean();
                item.TargetDuration = input.ReadInt32();
                item.TargetEffectRange = input.ReadObject<StatisticsRange>();
                item.AdjacentTargets = input.ReadInt32();
                item.UsingCueName = input.ReadString();
                item.TravelingCueName = input.ReadString();
                item.ImpactCueName = input.ReadString();
                item.BlockCueName = input.ReadString();
                item.CreationSprite = input.ReadObject<AnimatingSprite>();
                item.CreationSprite.SourceOffset = new Vector2(item.CreationSprite.FrameDimensions.X / 2, item.CreationSprite.FrameDimensions.Y);
                item.SpellSprite = input.ReadObject<AnimatingSprite>();
                item.SpellSprite.SourceOffset = new Vector2(item.SpellSprite.FrameDimensions.X / 2, item.SpellSprite.FrameDimensions.Y);
                item.Overlay = input.ReadObject<AnimatingSprite>();
                item.Overlay.SourceOffset = new Vector2(item.Overlay.FrameDimensions.X / 2, item.Overlay.FrameDimensions.Y);

                return item;
            }
        }
    }
}
