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
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Point mapDimensions;
        public Point MapDimensions
        {
            get { return mapDimensions; }
            set { mapDimensions = value; }
        }

        private Point tileSize;
        public Point TileSize
        {
            get { return tileSize; }
            set { tileSize = value; }
        }

        private int tilesPerRow;
        [ContentSerializerIgnore]
        public int TilesPerRow
        {
            get { return tilesPerRow; }
        }

        private Point spawnMapPosition;
        public Point SpawnMapPosition
        {
            get { return spawnMapPosition; }
            set { spawnMapPosition = value; }
        }

        private string textureName;
        public string TextureName
        {
            get { return textureName; }
            set { textureName = value; }
        }

        private Texture2D texture;
        [ContentSerializerIgnore]
        public Texture2D Texture
        {
            get { return texture; }
        }

        private string combatTextureName;
        public string CombatTextureName
        {
            get { return combatTextureName; }
            set { combatTextureName = value; }
        }

        private Texture2D combatTexture;
        [ContentSerializerIgnore]
        public Texture2D CombatTexture
        {
            get { return combatTexture; }
        }

        private string musicCueName;
        public string MusicCueName
        {
            get { return musicCueName; }
            set { musicCueName = value; }
        }

        private string combatMusicCueName;
        public string CombatMusicCueName
        {
            get { return combatMusicCueName; }
            set { combatMusicCueName = value; }
        }

        private int[] baseLayer;
        public int[] BaseLayer
        {
            get { return baseLayer; }
            set { baseLayer = value; }
        }

        public int GetBaseLayerValue(Point mapPosition)
        {
            // check the parameter
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                throw new ArgumentOutOfRangeException("mapPosition");
            }

            return baseLayer[mapPosition.Y * mapDimensions.X + mapPosition.X];
        }

        public Rectangle GetBaseLayerSourceRectangle(Point mapPosition)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                return Rectangle.Empty;
            }

            int baseLayerValue = GetBaseLayerValue(mapPosition);
            if (baseLayerValue < 0)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(
                (baseLayerValue % tilesPerRow) * tileSize.X,
                (baseLayerValue / tilesPerRow) * tileSize.Y,
                tileSize.X, tileSize.Y);
        }

        private int[] fringeLayer;
        public int[] FringeLayer
        {
            get { return fringeLayer; }
            set { fringeLayer = value; }
        }

        public int GetFringeLayerValue(Point mapPosition)
        {
            // check the parameter
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                throw new ArgumentOutOfRangeException("mapPosition");
            }

            return fringeLayer[mapPosition.Y * mapDimensions.X + mapPosition.X];
        }

        public Rectangle GetFringeLayerSourceRectangle(Point mapPosition)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                return Rectangle.Empty;
            }

            int fringeLayerValue = GetFringeLayerValue(mapPosition);
            if (fringeLayerValue < 0)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(
                (fringeLayerValue % tilesPerRow) * tileSize.X,
                (fringeLayerValue / tilesPerRow) * tileSize.Y,
                tileSize.X, tileSize.Y);
        }

        private int[] objectLayer;
        public int[] ObjectLayer
        {
            get { return objectLayer; }
            set { objectLayer = value; }
        }

        public int GetObjectLayerValue(Point mapPosition)
        {
            // check the parameter
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                throw new ArgumentOutOfRangeException("mapPosition");
            }

            return objectLayer[mapPosition.Y * mapDimensions.X + mapPosition.X];
        }

        public Rectangle GetObjectLayerSourceRectangle(Point mapPosition)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                return Rectangle.Empty;
            }

            int objectLayerValue = GetObjectLayerValue(mapPosition);
            if (objectLayerValue < 0)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(
                (objectLayerValue % tilesPerRow) * tileSize.X,
                (objectLayerValue / tilesPerRow) * tileSize.Y,
                tileSize.X, tileSize.Y);
        }

        private int[] collisionLayer;
        public int[] CollisionLayer
        {
            get { return collisionLayer; }
            set { collisionLayer = value; }
        }

        public int GetCollisionLayerValue(Point mapPosition)
        {
            // check the parameter
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                throw new ArgumentOutOfRangeException("mapPosition");
            }

            return collisionLayer[mapPosition.Y * mapDimensions.X + mapPosition.X];
        }

        public bool IsBlocked(Point mapPosition)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                return true;
            }

            return (GetCollisionLayerValue(mapPosition) != 0);
        }

        private List<Portal> portals = new List<Portal>();
        public List<Portal> Portals
        {
            get { return portals; }
            set { portals = value; }
        }

        private List<MapEntry<Portal>> portalEntries = new List<MapEntry<Portal>>();
        public List<MapEntry<Portal>> PortalEntries
        {
            get { return portalEntries; }
            set { portalEntries = value; }
        }

        public MapEntry<Portal> FindPortal(string name)
        {
            // check the parameter
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            return portalEntries.Find(delegate (MapEntry<Portal> portalEntry)
            {
                return (portalEntry.ContentName == name);
            });
        }

        private List<MapEntry<Chest>> chestEntries = new List<MapEntry<Chest>>();
        public List<MapEntry<Chest>> ChestEntries
        {
            get { return chestEntries; }
            set { chestEntries = value; }
        }

        private List<MapEntry<FixedCombat>> fixedCombatEntries = new List<MapEntry<FixedCombat>>();
        public List<MapEntry<FixedCombat>> FixedCombatEntries
        {
            get { return fixedCombatEntries; }
            set { fixedCombatEntries = value; }
        }

        private RandomCombat randomCombat;
        public RandomCombat RandomCombat
        {
            get { return randomCombat; }
            set { randomCombat = value; }
        }

        private List<MapEntry<QuestNpc>> questNpcEntries = new List<MapEntry<QuestNpc>>();
        public List<MapEntry<QuestNpc>> QuestNpcEntries
        {
            get { return questNpcEntries; }
            set { questNpcEntries = value; }
        }

        private List<MapEntry<Player>> playerNpcEntries = new List<MapEntry<Player>>();
        public List<MapEntry<Player>> PlayerNpcEntries
        {
            get { return playerNpcEntries; }
            set { playerNpcEntries = value; }
        }

        private List<MapEntry<Inn>> innEntries = new List<MapEntry<Inn>>();
        public List<MapEntry<Inn>> InnEntries
        {
            get { return innEntries; }
            set { innEntries = value; }
        }

        private List<MapEntry<Store>> storeEntries = new List<MapEntry<Store>>();
        public List<MapEntry<Store>> StoreEntries
        {
            get { return storeEntries; }
            set { storeEntries = value; }
        }



        public object Clone()
        {
            Map map = new Map();

            map.AssetName = AssetName;
            map.baseLayer = BaseLayer.Clone() as int[];
            foreach (MapEntry<Chest> chestEntry in chestEntries)
            {
                MapEntry<Chest> mapEntry = new MapEntry<Chest>();
                mapEntry.Content = chestEntry.Content.Clone() as Chest;
                mapEntry.ContentName = chestEntry.ContentName;
                mapEntry.Count = chestEntry.Count;
                mapEntry.Direction = chestEntry.Direction;
                mapEntry.MapPosition = chestEntry.MapPosition;
                map.chestEntries.Add(mapEntry);
            }
            map.chestEntries.AddRange(ChestEntries);
            map.collisionLayer = CollisionLayer.Clone() as int[];
            map.combatMusicCueName = CombatMusicCueName;
            map.combatTexture = CombatTexture;
            map.combatTextureName = CombatTextureName;
            map.fixedCombatEntries.AddRange(FixedCombatEntries);
            map.fringeLayer = FringeLayer.Clone() as int[];
            map.innEntries.AddRange(InnEntries);
            map.mapDimensions = MapDimensions;
            map.musicCueName = MusicCueName;
            map.name = Name;
            map.objectLayer = ObjectLayer.Clone() as int[];
            map.playerNpcEntries.AddRange(PlayerNpcEntries);
            map.portals.AddRange(Portals);
            map.portalEntries.AddRange(PortalEntries);
            map.questNpcEntries.AddRange(QuestNpcEntries);
            map.randomCombat = new RandomCombat();
            map.randomCombat.CombatProbability = RandomCombat.CombatProbability;
            map.randomCombat.Entries.AddRange(RandomCombat.Entries);
            map.randomCombat.FleeProbability = RandomCombat.FleeProbability;
            map.randomCombat.MonsterCountRange = RandomCombat.MonsterCountRange;
            map.spawnMapPosition = SpawnMapPosition;
            map.storeEntries.AddRange(StoreEntries);
            map.texture = Texture;
            map.textureName = TextureName;
            map.tileSize = TileSize;
            map.tilesPerRow = tilesPerRow;

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
                map.texture = input.ContentManager.Load<Texture2D>(
                    System.IO.Path.Combine(@"Textures\Maps\NonCombat",
                    map.TextureName));
                map.tilesPerRow = map.texture.Width / map.TileSize.X;

                map.CombatTextureName = input.ReadString();
                map.combatTexture = input.ContentManager.Load<Texture2D>(
                    System.IO.Path.Combine(@"Textures\Maps\Combat",
                    map.CombatTextureName));

                map.MusicCueName = input.ReadString();
                map.CombatMusicCueName = input.ReadString();

                map.BaseLayer = input.ReadObject<int[]>();
                map.FringeLayer = input.ReadObject<int[]>();
                map.ObjectLayer = input.ReadObject<int[]>();
                map.CollisionLayer = input.ReadObject<int[]>();
                map.Portals.AddRange(input.ReadObject<List<Portal>>());

                map.PortalEntries.AddRange(
                    input.ReadObject<List<MapEntry<Portal>>>());
                foreach (MapEntry<Portal> portalEntry in map.PortalEntries)
                {
                    portalEntry.Content = map.Portals.Find(delegate (Portal portal)
                    {
                        return (portal.Name == portalEntry.ContentName);
                    });
                }

                map.ChestEntries.AddRange(
                    input.ReadObject<List<MapEntry<Chest>>>());
                foreach (MapEntry<Chest> chestEntry in map.chestEntries)
                {
                    chestEntry.Content = input.ContentManager.Load<Chest>(
                        System.IO.Path.Combine(@"Maps\Chests",
                        chestEntry.ContentName)).Clone() as Chest;
                }

                // load the fixed combat entries
                Random random = new Random();
                map.FixedCombatEntries.AddRange(
                    input.ReadObject<List<MapEntry<FixedCombat>>>());
                foreach (MapEntry<FixedCombat> fixedCombatEntry in
                    map.fixedCombatEntries)
                {
                    fixedCombatEntry.Content =
                        input.ContentManager.Load<FixedCombat>(
                        System.IO.Path.Combine(@"Maps\FixedCombats",
                        fixedCombatEntry.ContentName));
                    // clone the map sprite in the entry, as there may be many entries
                    // per FixedCombat
                    fixedCombatEntry.MapSprite =
                        fixedCombatEntry.Content.Entries[0].Content.MapSprite.Clone()
                        as AnimatingSprite;
                    // play the idle animation
                    fixedCombatEntry.MapSprite.PlayAnimation("Idle",
                        fixedCombatEntry.Direction);
                    // advance in a random amount so the animations aren't synchronized
                    fixedCombatEntry.MapSprite.UpdateAnimation(
                        4f * (float)random.NextDouble());
                }

                map.RandomCombat = input.ReadObject<RandomCombat>();

                map.QuestNpcEntries.AddRange(
                    input.ReadObject<List<MapEntry<QuestNpc>>>());
                foreach (MapEntry<QuestNpc> questNpcEntry in
                    map.questNpcEntries)
                {
                    questNpcEntry.Content = input.ContentManager.Load<QuestNpc>(
                        System.IO.Path.Combine(@"Characters\QuestNpcs",
                        questNpcEntry.ContentName));
                    questNpcEntry.Content.MapPosition = questNpcEntry.MapPosition;
                    questNpcEntry.Content.Direction = questNpcEntry.Direction;
                }

                map.PlayerNpcEntries.AddRange(
                    input.ReadObject<List<MapEntry<Player>>>());
                foreach (MapEntry<Player> playerNpcEntry in
                    map.playerNpcEntries)
                {
                    playerNpcEntry.Content = input.ContentManager.Load<Player>(
                        System.IO.Path.Combine(@"Characters\Players",
                        playerNpcEntry.ContentName)).Clone() as Player;
                    playerNpcEntry.Content.MapPosition = playerNpcEntry.MapPosition;
                    playerNpcEntry.Content.Direction = playerNpcEntry.Direction;
                }

                map.InnEntries.AddRange(
                    input.ReadObject<List<MapEntry<Inn>>>());
                foreach (MapEntry<Inn> innEntry in
                    map.innEntries)
                {
                    innEntry.Content = input.ContentManager.Load<Inn>(
                        System.IO.Path.Combine(@"Maps\Inns",
                        innEntry.ContentName));
                }

                map.StoreEntries.AddRange(
                    input.ReadObject<List<MapEntry<Store>>>());
                foreach (MapEntry<Store> storeEntry in
                    map.storeEntries)
                {
                    storeEntry.Content = input.ContentManager.Load<Store>(
                        System.IO.Path.Combine(@"Maps\Stores",
                        storeEntry.ContentName));
                }

                return map;
            }
        }
    }
}
