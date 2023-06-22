using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Animations;
using RolePlayingGame.Characters;
using RolePlayingGame.Engine;
using System;
using System.Collections.Generic;

namespace RolePlayingGame.MapObjects
{
    public class Map : ContentObject, ICloneable
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        private Point _mapDimensions;
        public Point MapDimensions
        {
            get => _mapDimensions;
            set => _mapDimensions = value;
        }

        private Point _tileSize;
        public Point TileSize
        {
            get => _tileSize;
            set => _tileSize = value;
        }

        private int _tilesPerRow;
        [ContentSerializerIgnore]
        public int TilesPerRow => _tilesPerRow;

        private Point _spawnMapPosition;
        public Point SpawnMapPosition
        {
            get => _spawnMapPosition;
            set => _spawnMapPosition = value;
        }

        private string _textureName;
        public string TextureName
        {
            get => _textureName;
            set => _textureName = value;
        }

        private Texture2D _texture;
        [ContentSerializerIgnore]
        public Texture2D Texture => _texture;

        private string _combatTextureName;
        public string CombatTextureName
        {
            get => _combatTextureName;
            set => _combatTextureName = value;
        }

        private Texture2D _combatTexture;
        [ContentSerializerIgnore]
        public Texture2D CombatTexture => _combatTexture;

        private string _musicCueName;
        public string MusicCueName
        {
            get => _musicCueName;
            set => _musicCueName = value;
        }

        private string _combatMusicCueName;
        public string CombatMusicCueName
        {
            get => _combatMusicCueName;
            set => _combatMusicCueName = value;
        }

        private int[] _baseLayer;
        public int[] BaseLayer
        {
            get => _baseLayer;
            set => _baseLayer = value;
        }

        public int GetBaseLayerValue(Point mapPosition)
        {
            // check the parameter
            if ((mapPosition.X < 0) || (mapPosition.X >= _mapDimensions.X) || (mapPosition.Y < 0) || (mapPosition.Y >= _mapDimensions.Y))
            {
                throw new ArgumentOutOfRangeException("mapPosition");
            }

            return _baseLayer[mapPosition.Y * _mapDimensions.X + mapPosition.X];
        }

        public Rectangle GetBaseLayerSourceRectangle(Point mapPosition)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((mapPosition.X < 0) || (mapPosition.X >= _mapDimensions.X) || (mapPosition.Y < 0) || (mapPosition.Y >= _mapDimensions.Y))
            {
                return Rectangle.Empty;
            }

            int baseLayerValue = GetBaseLayerValue(mapPosition);
            if (baseLayerValue < 0)
            {
                return Rectangle.Empty;
            }

            return new Rectangle((baseLayerValue % _tilesPerRow) * _tileSize.X, (baseLayerValue / _tilesPerRow) * _tileSize.Y, _tileSize.X, _tileSize.Y);
        }

        private int[] _fringeLayer;
        public int[] FringeLayer
        {
            get => _fringeLayer;
            set => _fringeLayer = value;
        }

        public int GetFringeLayerValue(Point mapPosition)
        {
            // check the parameter
            if ((mapPosition.X < 0) || (mapPosition.X >= _mapDimensions.X) || (mapPosition.Y < 0) || (mapPosition.Y >= _mapDimensions.Y))
            {
                throw new ArgumentOutOfRangeException("mapPosition");
            }

            return _fringeLayer[mapPosition.Y * _mapDimensions.X + mapPosition.X];
        }

        public Rectangle GetFringeLayerSourceRectangle(Point mapPosition)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((mapPosition.X < 0) || (mapPosition.X >= _mapDimensions.X) || (mapPosition.Y < 0) || (mapPosition.Y >= _mapDimensions.Y))
            {
                return Rectangle.Empty;
            }

            int fringeLayerValue = GetFringeLayerValue(mapPosition);
            if (fringeLayerValue < 0)
            {
                return Rectangle.Empty;
            }

            return new Rectangle((fringeLayerValue % _tilesPerRow) * _tileSize.X, (fringeLayerValue / _tilesPerRow) * _tileSize.Y, _tileSize.X, _tileSize.Y);
        }

        private int[] _objectLayer;
        public int[] ObjectLayer
        {
            get => _objectLayer;
            set => _objectLayer = value;
        }

        public int GetObjectLayerValue(Point mapPosition)
        {
            // check the parameter
            if ((mapPosition.X < 0) || (mapPosition.X >= _mapDimensions.X) || (mapPosition.Y < 0) || (mapPosition.Y >= _mapDimensions.Y))
            {
                throw new ArgumentOutOfRangeException("mapPosition");
            }

            return _objectLayer[mapPosition.Y * _mapDimensions.X + mapPosition.X];
        }

        public Rectangle GetObjectLayerSourceRectangle(Point mapPosition)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((mapPosition.X < 0) || (mapPosition.X >= _mapDimensions.X) || (mapPosition.Y < 0) || (mapPosition.Y >= _mapDimensions.Y))
            {
                return Rectangle.Empty;
            }

            int objectLayerValue = GetObjectLayerValue(mapPosition);
            if (objectLayerValue < 0)
            {
                return Rectangle.Empty;
            }

            return new Rectangle((objectLayerValue % _tilesPerRow) * _tileSize.X, (objectLayerValue / _tilesPerRow) * _tileSize.Y, _tileSize.X, _tileSize.Y);
        }

        private int[] _collisionLayer;
        public int[] CollisionLayer
        {
            get => _collisionLayer;
            set => _collisionLayer = value;
        }

        public int GetCollisionLayerValue(Point mapPosition)
        {
            // check the parameter
            if ((mapPosition.X < 0) || (mapPosition.X >= _mapDimensions.X) || (mapPosition.Y < 0) || (mapPosition.Y >= _mapDimensions.Y))
            {
                throw new ArgumentOutOfRangeException("mapPosition");
            }

            return _collisionLayer[mapPosition.Y * _mapDimensions.X + mapPosition.X];
        }

        public bool IsBlocked(Point mapPosition)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((mapPosition.X < 0) || (mapPosition.X >= _mapDimensions.X) || (mapPosition.Y < 0) || (mapPosition.Y >= _mapDimensions.Y))
            {
                return true;
            }

            return (GetCollisionLayerValue(mapPosition) != 0);
        }

        private List<Portal> _portals = new();
        public List<Portal> Portals
        {
            get => _portals;
            set => _portals = value;
        }

        private List<MapEntry<Portal>> _portalEntries = new();
        public List<MapEntry<Portal>> PortalEntries
        {
            get => _portalEntries;
            set => _portalEntries = value;
        }

        public MapEntry<Portal> FindPortal(string name)
        {
            // check the parameter
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            return _portalEntries.Find(delegate (MapEntry<Portal> portalEntry)
            {
                return (portalEntry.ContentName == name);
            });
        }

        private List<MapEntry<Chest>> _chestEntries = new();
        public List<MapEntry<Chest>> ChestEntries
        {
            get => _chestEntries;
            set => _chestEntries = value;
        }

        private List<MapEntry<FixedCombat>> _fixedCombatEntries = new();
        public List<MapEntry<FixedCombat>> FixedCombatEntries
        {
            get => _fixedCombatEntries;
            set => _fixedCombatEntries = value;
        }

        private RandomCombat _randomCombat;
        public RandomCombat RandomCombat
        {
            get => _randomCombat;
            set => _randomCombat = value;
        }

        private List<MapEntry<QuestNpc>> _questNpcEntries = new();
        public List<MapEntry<QuestNpc>> QuestNpcEntries
        {
            get => _questNpcEntries;
            set => _questNpcEntries = value;
        }

        private List<MapEntry<Player>> _playerNpcEntries = new();
        public List<MapEntry<Player>> PlayerNpcEntries
        {
            get => _playerNpcEntries;
            set => _playerNpcEntries = value;
        }

        private List<MapEntry<Inn>> _innEntries = new();
        public List<MapEntry<Inn>> InnEntries
        {
            get => _innEntries;
            set => _innEntries = value;
        }

        private List<MapEntry<Store>> _storeEntries = new();
        public List<MapEntry<Store>> StoreEntries
        {
            get => _storeEntries;
            set => _storeEntries = value;
        }

        public object Clone()
        {
            Map map = new()
            {
                AssetName = AssetName,
                _baseLayer = BaseLayer.Clone() as int[]
            };

            foreach (MapEntry<Chest> chestEntry in _chestEntries)
            {
                MapEntry<Chest> mapEntry = new()
                {
                    Content = chestEntry.Content.Clone() as Chest,
                    ContentName = chestEntry.ContentName,
                    Count = chestEntry.Count,
                    Direction = chestEntry.Direction,
                    MapPosition = chestEntry.MapPosition
                };
                map._chestEntries.Add(mapEntry);
            }
            map._chestEntries.AddRange(ChestEntries);
            map._collisionLayer = CollisionLayer.Clone() as int[];
            map._combatMusicCueName = CombatMusicCueName;
            map._combatTexture = CombatTexture;
            map._combatTextureName = CombatTextureName;
            map._fixedCombatEntries.AddRange(FixedCombatEntries);
            map._fringeLayer = FringeLayer.Clone() as int[];
            map._innEntries.AddRange(InnEntries);
            map._mapDimensions = MapDimensions;
            map._musicCueName = MusicCueName;
            map._name = Name;
            map._objectLayer = ObjectLayer.Clone() as int[];
            map._playerNpcEntries.AddRange(PlayerNpcEntries);
            map._portals.AddRange(Portals);
            map._portalEntries.AddRange(PortalEntries);
            map._questNpcEntries.AddRange(QuestNpcEntries);
            map._randomCombat = new RandomCombat
            {
                CombatProbability = RandomCombat.CombatProbability
            };
            map._randomCombat.EntriesList.AddRange(RandomCombat.EntriesList);
            map._randomCombat.FleeProbability = RandomCombat.FleeProbability;
            map._randomCombat.MonsterCountRange = RandomCombat.MonsterCountRange;
            map._spawnMapPosition = SpawnMapPosition;
            map._storeEntries.AddRange(StoreEntries);
            map._texture = Texture;
            map._textureName = TextureName;
            map._tileSize = TileSize;
            map._tilesPerRow = _tilesPerRow;

            return map;
        }

        public class MapReader : ContentTypeReader<Map>
        {
            protected override Map Read(ContentReader input, Map existingInstance)
            {
                Map map = existingInstance;
                if (map == null)
                {
                    map = new Map();
                }

                map.AssetName = input.AssetName;

                map.Name = input.ReadString();
                map.MapDimensions = input.ReadObject<Point>();
                map.TileSize = input.ReadObject<Point>();
                map.SpawnMapPosition = input.ReadObject<Point>();

                map.TextureName = input.ReadString();
                map._texture = input.ContentManager.Load<Texture2D>(System.IO.Path.Combine("Textures/Maps/NonCombat", map.TextureName));
                map._tilesPerRow = map._texture.Width / map.TileSize.X;

                map.CombatTextureName = input.ReadString();
                map._combatTexture = input.ContentManager.Load<Texture2D>(System.IO.Path.Combine("Textures/Maps/Combat", map.CombatTextureName));

                map.MusicCueName = input.ReadString();
                map.CombatMusicCueName = input.ReadString();

                map.BaseLayer = input.ReadObject<int[]>();
                map.FringeLayer = input.ReadObject<int[]>();
                map.ObjectLayer = input.ReadObject<int[]>();
                map.CollisionLayer = input.ReadObject<int[]>();
                map.Portals.AddRange(input.ReadObject<List<Portal>>());

                map.PortalEntries.AddRange(input.ReadObject<List<MapEntry<Portal>>>());
                foreach (MapEntry<Portal> portalEntry in map.PortalEntries)
                {
                    portalEntry.Content = map.Portals.Find(delegate (Portal portal)
                    {
                        return (portal.Name == portalEntry.ContentName);
                    });
                }

                map.ChestEntries.AddRange(input.ReadObject<List<MapEntry<Chest>>>());
                foreach (MapEntry<Chest> chestEntry in map._chestEntries)
                {
                    chestEntry.Content = input.ContentManager.Load<Chest>(System.IO.Path.Combine("Maps/Chests", chestEntry.ContentName)).Clone() as Chest;
                }

                // load the fixed combat entries
                Random random = new();
                map.FixedCombatEntries.AddRange(input.ReadObject<List<MapEntry<FixedCombat>>>());
                foreach (MapEntry<FixedCombat> fixedCombatEntry in map._fixedCombatEntries)
                {
                    fixedCombatEntry.Content = input.ContentManager.Load<FixedCombat>(System.IO.Path.Combine("Maps/FixedCombats", fixedCombatEntry.ContentName));
                    // clone the map sprite in the entry, as there may be many entries
                    // per FixedCombat
                    fixedCombatEntry.MapSprite = fixedCombatEntry.Content.Entries[0].Content.MapSprite.Clone() as AnimatingSprite;
                    // play the idle animation
                    fixedCombatEntry.MapSprite.PlayAnimation("Idle", fixedCombatEntry.Direction);
                    // advance in a random amount so the animations aren't synchronized
                    fixedCombatEntry.MapSprite.UpdateAnimation(4f * (float)random.NextDouble());
                }

                map.RandomCombat = input.ReadObject<RandomCombat>();

                map.QuestNpcEntries.AddRange(input.ReadObject<List<MapEntry<QuestNpc>>>());
                foreach (MapEntry<QuestNpc> questNpcEntry in map._questNpcEntries)
                {
                    questNpcEntry.Content = input.ContentManager.Load<QuestNpc>(System.IO.Path.Combine("Characters/QuestNPCs", questNpcEntry.ContentName));
                    questNpcEntry.Content.MapPosition = questNpcEntry.MapPosition;
                    questNpcEntry.Content.Direction = questNpcEntry.Direction;
                }

                map.PlayerNpcEntries.AddRange(input.ReadObject<List<MapEntry<Player>>>());
                foreach (MapEntry<Player> playerNpcEntry in map._playerNpcEntries)
                {
                    playerNpcEntry.Content = input.ContentManager.Load<Player>(System.IO.Path.Combine("Characters/Players", playerNpcEntry.ContentName)).Clone() as Player;
                    playerNpcEntry.Content.MapPosition = playerNpcEntry.MapPosition;
                    playerNpcEntry.Content.Direction = playerNpcEntry.Direction;
                }

                map.InnEntries.AddRange(input.ReadObject<List<MapEntry<Inn>>>());
                foreach (MapEntry<Inn> innEntry in map._innEntries)
                {
                    innEntry.Content = input.ContentManager.Load<Inn>(System.IO.Path.Combine("Maps/Inns", innEntry.ContentName));
                }

                map.StoreEntries.AddRange(input.ReadObject<List<MapEntry<Store>>>());
                foreach (MapEntry<Store> storeEntry in map._storeEntries)
                {
                    storeEntry.Content = input.ContentManager.Load<Store>(System.IO.Path.Combine("Maps/Stores", storeEntry.ContentName));
                }

                return map;
            }
        }
    }
}
