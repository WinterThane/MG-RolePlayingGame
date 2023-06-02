using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RolePlayingGame.Directions;
using RolePlayingGame.InputsManager;
using RolePlayingGame.MapObjects;
using RolePlayingGame.ScreensManager.Screens.GameScreens;
using RolePlayingGame.SessionObjects;
using System;

namespace RolePlayingGame.Engine
{
    public static class TileEngine
    {
        private static Map map = null;

        public static Map Map
        {
            get { return map; }
        }

        private static Vector2 mapOriginPosition;

        public static Vector2 GetScreenPosition(Point mapPosition)
        {
            return new Vector2(
                mapOriginPosition.X + mapPosition.X * map.TileSize.X,
                mapOriginPosition.Y + mapPosition.Y * map.TileSize.Y);
        }

        public static void SetMap(Map newMap, MapEntry<Portal> portalEntry)
        {
            // check the parameter
            if (newMap == null)
            {
                throw new ArgumentNullException("newMap");
            }

            // assign the new map
            map = newMap;

            // reset the map origin, which will be recalculate on the first update
            mapOriginPosition = Vector2.Zero;

            // move the party to its initial position
            if (portalEntry == null)
            {
                // no portal - use the spawn position
                partyLeaderPosition.TilePosition = map.SpawnMapPosition;
                partyLeaderPosition.TileOffset = Vector2.Zero;
                partyLeaderPosition.Direction = Direction.South;
            }
            else
            {
                // use the portal provided, which may include automatic movement
                partyLeaderPosition.TilePosition = portalEntry.MapPosition;
                partyLeaderPosition.TileOffset = Vector2.Zero;
                partyLeaderPosition.Direction = portalEntry.Direction;
                autoPartyLeaderMovement = Vector2.Multiply(
                    new Vector2(map.TileSize.X, map.TileSize.Y), new Vector2(
                    portalEntry.Content.LandingMapPosition.X -
                        partyLeaderPosition.TilePosition.X,
                    portalEntry.Content.LandingMapPosition.Y -
                        partyLeaderPosition.TilePosition.Y));
            }
        }

        private static Viewport viewport;
        private static Vector2 viewportCenter;

        public static Viewport Viewport
        {
            get { return viewport; }
            set
            {
                viewport = value;
                viewportCenter = new Vector2(
                    viewport.X + viewport.Width / 2f,
                    viewport.Y + viewport.Height / 2f);
            }
        }

        private const float partyLeaderMovementSpeed = 3f;

        private static PlayerPosition partyLeaderPosition = new PlayerPosition();
        public static PlayerPosition PartyLeaderPosition
        {
            get { return partyLeaderPosition; }
            set { partyLeaderPosition = value; }
        }

        private static Vector2 autoPartyLeaderMovement = Vector2.Zero;

        private static Vector2 UpdatePartyLeaderAutoMovement(GameTime gameTime)
        {
            // check for any remaining auto-movement
            if (autoPartyLeaderMovement == Vector2.Zero)
            {
                return Vector2.Zero;
            }

            // get the remaining-movement direction
            Vector2 autoMovementDirection = Vector2.Normalize(autoPartyLeaderMovement);

            // calculate the potential movement vector
            Vector2 movement = Vector2.Multiply(autoMovementDirection,
                partyLeaderMovementSpeed);

            // limit the potential movement vector by the remaining auto-movement
            movement.X = Math.Sign(movement.X) * MathHelper.Min(Math.Abs(movement.X),
                Math.Abs(autoPartyLeaderMovement.X));
            movement.Y = Math.Sign(movement.Y) * MathHelper.Min(Math.Abs(movement.Y),
                Math.Abs(autoPartyLeaderMovement.Y));

            // remove the movement from the total remaining auto-movement
            autoPartyLeaderMovement -= movement;

            return movement;
        }

        private static Vector2 UpdateUserMovement(GameTime gameTime)
        {
            Vector2 desiredMovement = Vector2.Zero;

            // accumulate the desired direction from user input
            if (InputManager.IsActionPressed(InputManager.Action.MoveCharacterUp))
            {
                if (CanPartyLeaderMoveUp())
                {
                    desiredMovement.Y -= partyLeaderMovementSpeed;
                }
            }
            if (InputManager.IsActionPressed(InputManager.Action.MoveCharacterDown))
            {
                if (CanPartyLeaderMoveDown())
                {
                    desiredMovement.Y += partyLeaderMovementSpeed;
                }
            }
            if (InputManager.IsActionPressed(InputManager.Action.MoveCharacterLeft))
            {
                if (CanPartyLeaderMoveLeft())
                {
                    desiredMovement.X -= partyLeaderMovementSpeed;
                }
            }
            if (InputManager.IsActionPressed(InputManager.Action.MoveCharacterRight))
            {
                if (CanPartyLeaderMoveRight())
                {
                    desiredMovement.X += partyLeaderMovementSpeed;
                }
            }

            // if there is no desired movement, then we can't determine a direction
            if (desiredMovement == Vector2.Zero)
            {
                return Vector2.Zero;
            }

            return desiredMovement;
        }

        const int movementCollisionTolerance = 12;

        private static bool CanPartyLeaderMoveUp()
        {
            // if they're not within the tolerance of the next tile, then this is moot
            if (partyLeaderPosition.TileOffset.Y > -movementCollisionTolerance)
            {
                return true;
            }

            // if the player is at the outside left and right edges, 
            // then check the diagonal tiles
            if (partyLeaderPosition.TileOffset.X < -movementCollisionTolerance)
            {
                if (map.IsBlocked(new Point(
                    partyLeaderPosition.TilePosition.X - 1,
                    partyLeaderPosition.TilePosition.Y - 1)))
                {
                    return false;
                }
            }
            else if (partyLeaderPosition.TileOffset.X > movementCollisionTolerance)
            {
                if (map.IsBlocked(new Point(
                    partyLeaderPosition.TilePosition.X + 1,
                    partyLeaderPosition.TilePosition.Y - 1)))
                {
                    return false;
                }
            }

            // check the tile above the current one
            return !map.IsBlocked(new Point(
                    partyLeaderPosition.TilePosition.X,
                    partyLeaderPosition.TilePosition.Y - 1));
        }

        private static bool CanPartyLeaderMoveDown()
        {
            // if they're not within the tolerance of the next tile, then this is moot
            if (partyLeaderPosition.TileOffset.Y < movementCollisionTolerance)
            {
                return true;
            }

            // if the player is at the outside left and right edges, 
            // then check the diagonal tiles
            if (partyLeaderPosition.TileOffset.X < -movementCollisionTolerance)
            {
                if (map.IsBlocked(new Point(
                    partyLeaderPosition.TilePosition.X - 1,
                    partyLeaderPosition.TilePosition.Y + 1)))
                {
                    return false;
                }
            }
            else if (partyLeaderPosition.TileOffset.X > movementCollisionTolerance)
            {
                if (map.IsBlocked(new Point(
                    partyLeaderPosition.TilePosition.X + 1,
                    partyLeaderPosition.TilePosition.Y + 1)))
                {
                    return false;
                }
            }

            // check the tile below the current one
            return !map.IsBlocked(new Point(
                    partyLeaderPosition.TilePosition.X,
                    partyLeaderPosition.TilePosition.Y + 1));
        }

        private static bool CanPartyLeaderMoveLeft()
        {
            // if they're not within the tolerance of the next tile, then this is moot
            if (partyLeaderPosition.TileOffset.X > -movementCollisionTolerance)
            {
                return true;
            }

            // if the player is at the outside left and right edges, 
            // then check the diagonal tiles
            if (partyLeaderPosition.TileOffset.Y < -movementCollisionTolerance)
            {
                if (map.IsBlocked(new Point(
                    partyLeaderPosition.TilePosition.X - 1,
                    partyLeaderPosition.TilePosition.Y - 1)))
                {
                    return false;
                }
            }
            else if (partyLeaderPosition.TileOffset.Y > movementCollisionTolerance)
            {
                if (map.IsBlocked(new Point(
                    partyLeaderPosition.TilePosition.X - 1,
                    partyLeaderPosition.TilePosition.Y + 1)))
                {
                    return false;
                }
            }

            // check the tile to the left of the current one
            return !map.IsBlocked(new Point(
                    partyLeaderPosition.TilePosition.X - 1,
                    partyLeaderPosition.TilePosition.Y));
        }

        private static bool CanPartyLeaderMoveRight()
        {
            // if they're not within the tolerance of the next tile, then this is moot
            if (partyLeaderPosition.TileOffset.X < movementCollisionTolerance)
            {
                return true;
            }

            // if the player is at the outside left and right edges, 
            // then check the diagonal tiles
            if (partyLeaderPosition.TileOffset.Y < -movementCollisionTolerance)
            {
                if (map.IsBlocked(new Point(
                    partyLeaderPosition.TilePosition.X + 1,
                    partyLeaderPosition.TilePosition.Y - 1)))
                {
                    return false;
                }
            }
            else if (partyLeaderPosition.TileOffset.Y > movementCollisionTolerance)
            {
                if (map.IsBlocked(new Point(
                    partyLeaderPosition.TilePosition.X + 1,
                    partyLeaderPosition.TilePosition.Y + 1)))
                {
                    return false;
                }
            }

            // check the tile to the right of the current one
            return !map.IsBlocked(new Point(
                    partyLeaderPosition.TilePosition.X + 1,
                    partyLeaderPosition.TilePosition.Y));
        }

        public static void Update(GameTime gameTime)
        {
            // check for auto-movement
            Vector2 autoMovement = UpdatePartyLeaderAutoMovement(gameTime);

            // if there is no auto-movement, handle user controls
            Vector2 userMovement = Vector2.Zero;
            if (autoMovement == Vector2.Zero)
            {
                userMovement = UpdateUserMovement(gameTime);
                // calculate the desired position
                if (userMovement != Vector2.Zero)
                {
                    Point desiredTilePosition = partyLeaderPosition.TilePosition;
                    Vector2 desiredTileOffset = partyLeaderPosition.TileOffset;
                    PlayerPosition.CalculateMovement(
                        Vector2.Multiply(userMovement, 15f),
                        ref desiredTilePosition, ref desiredTileOffset);
                    // check for collisions or encounters in the new tile
                    if ((partyLeaderPosition.TilePosition != desiredTilePosition) &&
                        !MoveIntoTile(desiredTilePosition))
                    {
                        userMovement = Vector2.Zero;
                    }
                }
            }

            Point oldPartyLeaderTilePosition = partyLeaderPosition.TilePosition;
            partyLeaderPosition.Move(autoMovement + userMovement);

            // if the tile position has changed, check for random combat
            if ((autoMovement == Vector2.Zero) &&
                (partyLeaderPosition.TilePosition != oldPartyLeaderTilePosition))
            {
                Session.CheckForRandomCombat(Map.RandomCombat);
            }

            // adjust the map origin so that the party is at the center of the viewport
            mapOriginPosition += viewportCenter - (partyLeaderPosition.ScreenPosition +
                Session.Party.Players[0].MapSprite.SourceOffset);

            // make sure the boundaries of the map are never inside the viewport
            mapOriginPosition.X = MathHelper.Min(mapOriginPosition.X, viewport.X);
            mapOriginPosition.Y = MathHelper.Min(mapOriginPosition.Y, viewport.Y);
            mapOriginPosition.X += MathHelper.Max(
                (viewport.X + viewport.Width) -
                (mapOriginPosition.X + map.MapDimensions.X * map.TileSize.X), 0f);
            mapOriginPosition.Y += MathHelper.Max(
                (viewport.Y + viewport.Height - Hud.HudHeight) -
                (mapOriginPosition.Y + map.MapDimensions.Y * map.TileSize.Y), 0f);
        }

        private static bool MoveIntoTile(Point mapPosition)
        {
            // if the tile is blocked, then this is simple
            if (map.IsBlocked(mapPosition))
            {
                return false;
            }

            // check for anything that might be in the tile
            if (Session.EncounterTile(mapPosition))
            {
                return false;
            }

            // nothing stops the party from moving into the tile
            return true;
        }

        public static void DrawLayers(SpriteBatch spriteBatch, bool drawBase,
            bool drawFringe, bool drawObject)
        {
            // check the parameters
            if (spriteBatch == null)
            {
                throw new ArgumentNullException("spriteBatch");
            }
            if (!drawBase && !drawFringe && !drawObject)
            {
                return;
            }

            Rectangle destinationRectangle =
                new Rectangle(0, 0, map.TileSize.X, map.TileSize.Y);

            for (int y = 0; y < map.MapDimensions.Y; y++)
            {
                for (int x = 0; x < map.MapDimensions.X; x++)
                {
                    destinationRectangle.X =
                        (int)mapOriginPosition.X + x * map.TileSize.X;
                    destinationRectangle.Y =
                        (int)mapOriginPosition.Y + y * map.TileSize.Y;

                    // If the tile is inside the screen
                    if (CheckVisibility(destinationRectangle))
                    {
                        Point mapPosition = new Point(x, y);
                        if (drawBase)
                        {
                            Rectangle sourceRectangle =
                                map.GetBaseLayerSourceRectangle(mapPosition);
                            if (sourceRectangle != Rectangle.Empty)
                            {
                                spriteBatch.Draw(map.Texture, destinationRectangle,
                                    sourceRectangle, Color.White);
                            }
                        }
                        if (drawFringe)
                        {
                            Rectangle sourceRectangle =
                                map.GetFringeLayerSourceRectangle(mapPosition);
                            if (sourceRectangle != Rectangle.Empty)
                            {
                                spriteBatch.Draw(map.Texture, destinationRectangle,
                                    sourceRectangle, Color.White);
                            }
                        }
                        if (drawObject)
                        {
                            Rectangle sourceRectangle =
                                map.GetObjectLayerSourceRectangle(mapPosition);
                            if (sourceRectangle != Rectangle.Empty)
                            {
                                spriteBatch.Draw(map.Texture, destinationRectangle,
                                    sourceRectangle, Color.White);
                            }
                        }
                    }
                }
            }
        }

        public static bool CheckVisibility(Rectangle screenRectangle)
        {
            return ((screenRectangle.X > viewport.X - screenRectangle.Width) &&
                (screenRectangle.Y > viewport.Y - screenRectangle.Height) &&
                (screenRectangle.X < viewport.X + viewport.Width) &&
                (screenRectangle.Y < viewport.Y + viewport.Height));
        }
    }
}
