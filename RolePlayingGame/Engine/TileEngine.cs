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
        private static Map _map = null;

        public static Map Map => _map;

        private static Vector2 _mapOriginPosition;

        public static Vector2 GetScreenPosition(Point mapPosition)
        {
            return new Vector2(_mapOriginPosition.X + mapPosition.X * _map.TileSize.X, _mapOriginPosition.Y + mapPosition.Y * _map.TileSize.Y);
        }

        public static void SetMap(Map newMap, MapEntry<Portal> portalEntry)
        {
            // check the parameter
            if (newMap == null)
            {
                throw new ArgumentNullException("newMap");
            }

            // assign the new map
            _map = newMap;

            // reset the map origin, which will be recalculate on the first update
            _mapOriginPosition = Vector2.Zero;

            // move the party to its initial position
            if (portalEntry == null)
            {
                // no portal - use the spawn position
                _partyLeaderPosition.TilePosition = _map.SpawnMapPosition;
                _partyLeaderPosition.TileOffset = Vector2.Zero;
                _partyLeaderPosition.Direction = Direction.South;
            }
            else
            {
                // use the portal provided, which may include automatic movement
                _partyLeaderPosition.TilePosition = portalEntry.MapPosition;
                _partyLeaderPosition.TileOffset = Vector2.Zero;
                _partyLeaderPosition.Direction = portalEntry.Direction;
                _autoPartyLeaderMovement = Vector2.Multiply(new Vector2(_map.TileSize.X, _map.TileSize.Y), new Vector2(portalEntry.Content.LandingMapPosition.X - _partyLeaderPosition.TilePosition.X, portalEntry.Content.LandingMapPosition.Y - _partyLeaderPosition.TilePosition.Y));
            }
        }

        private static Viewport _viewport;
        private static Vector2 _viewportCenter;

        public static Viewport Viewport
        {
            get => _viewport;
            set
            {
                _viewport = value;
                _viewportCenter = new Vector2(_viewport.X + _viewport.Width / 2f, _viewport.Y + _viewport.Height / 2f);
            }
        }

        private const float _partyLeaderMovementSpeed = 3f;

        private static PlayerPosition _partyLeaderPosition = new();
        public static PlayerPosition PartyLeaderPosition
        {
            get => _partyLeaderPosition;
            set => _partyLeaderPosition = value;
        }

        private static Vector2 _autoPartyLeaderMovement = Vector2.Zero;

        private static Vector2 UpdatePartyLeaderAutoMovement(GameTime gameTime)
        {
            // check for any remaining auto-movement
            if (_autoPartyLeaderMovement == Vector2.Zero)
            {
                return Vector2.Zero;
            }

            // get the remaining-movement direction
            Vector2 autoMovementDirection = Vector2.Normalize(_autoPartyLeaderMovement);

            // calculate the potential movement vector
            Vector2 movement = Vector2.Multiply(autoMovementDirection, _partyLeaderMovementSpeed);

            // limit the potential movement vector by the remaining auto-movement
            movement.X = Math.Sign(movement.X) * MathHelper.Min(Math.Abs(movement.X), Math.Abs(_autoPartyLeaderMovement.X));
            movement.Y = Math.Sign(movement.Y) * MathHelper.Min(Math.Abs(movement.Y), Math.Abs(_autoPartyLeaderMovement.Y));

            // remove the movement from the total remaining auto-movement
            _autoPartyLeaderMovement -= movement;

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
                    desiredMovement.Y -= _partyLeaderMovementSpeed;
                }
            }
            if (InputManager.IsActionPressed(InputManager.Action.MoveCharacterDown))
            {
                if (CanPartyLeaderMoveDown())
                {
                    desiredMovement.Y += _partyLeaderMovementSpeed;
                }
            }
            if (InputManager.IsActionPressed(InputManager.Action.MoveCharacterLeft))
            {
                if (CanPartyLeaderMoveLeft())
                {
                    desiredMovement.X -= _partyLeaderMovementSpeed;
                }
            }
            if (InputManager.IsActionPressed(InputManager.Action.MoveCharacterRight))
            {
                if (CanPartyLeaderMoveRight())
                {
                    desiredMovement.X += _partyLeaderMovementSpeed;
                }
            }

            // if there is no desired movement, then we can't determine a direction
            if (desiredMovement == Vector2.Zero)
            {
                return Vector2.Zero;
            }

            return desiredMovement;
        }

        const int _movementCollisionTolerance = 12;

        private static bool CanPartyLeaderMoveUp()
        {
            // if they're not within the tolerance of the next tile, then this is moot
            if (_partyLeaderPosition.TileOffset.Y > -_movementCollisionTolerance)
            {
                return true;
            }

            // if the player is at the outside left and right edges, 
            // then check the diagonal tiles
            if (_partyLeaderPosition.TileOffset.X < -_movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(_partyLeaderPosition.TilePosition.X - 1, _partyLeaderPosition.TilePosition.Y - 1))) 
                {
                    return false;
                }
            }
            else if (_partyLeaderPosition.TileOffset.X > _movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(_partyLeaderPosition.TilePosition.X + 1, _partyLeaderPosition.TilePosition.Y - 1)))
                {
                    return false;
                }
            }

            // check the tile above the current one
            return !_map.IsBlocked(new Point(_partyLeaderPosition.TilePosition.X, _partyLeaderPosition.TilePosition.Y - 1));
        }

        private static bool CanPartyLeaderMoveDown()
        {
            // if they're not within the tolerance of the next tile, then this is moot
            if (_partyLeaderPosition.TileOffset.Y < _movementCollisionTolerance)
            {
                return true;
            }

            // if the player is at the outside left and right edges, 
            // then check the diagonal tiles
            if (_partyLeaderPosition.TileOffset.X < -_movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(_partyLeaderPosition.TilePosition.X - 1, _partyLeaderPosition.TilePosition.Y + 1)))
                {
                    return false;
                }
            }
            else if (_partyLeaderPosition.TileOffset.X > _movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(_partyLeaderPosition.TilePosition.X + 1, _partyLeaderPosition.TilePosition.Y + 1)))
                {
                    return false;
                }
            }

            // check the tile below the current one
            return !_map.IsBlocked(new Point(_partyLeaderPosition.TilePosition.X, _partyLeaderPosition.TilePosition.Y + 1));
        }

        private static bool CanPartyLeaderMoveLeft()
        {
            // if they're not within the tolerance of the next tile, then this is moot
            if (_partyLeaderPosition.TileOffset.X > -_movementCollisionTolerance)
            {
                return true;
            }

            // if the player is at the outside left and right edges, 
            // then check the diagonal tiles
            if (_partyLeaderPosition.TileOffset.Y < -_movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(_partyLeaderPosition.TilePosition.X - 1, _partyLeaderPosition.TilePosition.Y - 1)))
                {
                    return false;
                }
            }
            else if (_partyLeaderPosition.TileOffset.Y > _movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(_partyLeaderPosition.TilePosition.X - 1, _partyLeaderPosition.TilePosition.Y + 1)))
                {
                    return false;
                }
            }

            // check the tile to the left of the current one
            return !_map.IsBlocked(new Point(_partyLeaderPosition.TilePosition.X - 1, _partyLeaderPosition.TilePosition.Y));
        }

        private static bool CanPartyLeaderMoveRight()
        {
            // if they're not within the tolerance of the next tile, then this is moot
            if (_partyLeaderPosition.TileOffset.X < _movementCollisionTolerance)
            {
                return true;
            }

            // if the player is at the outside left and right edges, 
            // then check the diagonal tiles
            if (_partyLeaderPosition.TileOffset.Y < -_movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(_partyLeaderPosition.TilePosition.X + 1, _partyLeaderPosition.TilePosition.Y - 1)))
                {
                    return false;
                }
            }
            else if (_partyLeaderPosition.TileOffset.Y > _movementCollisionTolerance)
            {
                if (_map.IsBlocked(new Point(_partyLeaderPosition.TilePosition.X + 1, _partyLeaderPosition.TilePosition.Y + 1)))
                {
                    return false;
                }
            }

            // check the tile to the right of the current one
            return !_map.IsBlocked(new Point(_partyLeaderPosition.TilePosition.X + 1, _partyLeaderPosition.TilePosition.Y));
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
                    Point desiredTilePosition = _partyLeaderPosition.TilePosition;
                    Vector2 desiredTileOffset = _partyLeaderPosition.TileOffset;
                    PlayerPosition.CalculateMovement(Vector2.Multiply(userMovement, 15f), ref desiredTilePosition, ref desiredTileOffset);
                    // check for collisions or encounters in the new tile
                    if ((_partyLeaderPosition.TilePosition != desiredTilePosition) && !MoveIntoTile(desiredTilePosition))
                    {
                        userMovement = Vector2.Zero;
                    }
                }
            }

            Point oldPartyLeaderTilePosition = _partyLeaderPosition.TilePosition;
            _partyLeaderPosition.Move(autoMovement + userMovement);

            // if the tile position has changed, check for random combat
            if ((autoMovement == Vector2.Zero) && (_partyLeaderPosition.TilePosition != oldPartyLeaderTilePosition))
            {
                Session.CheckForRandomCombat(Map.RandomCombat);
            }

            // adjust the map origin so that the party is at the center of the viewport
            _mapOriginPosition += _viewportCenter - (_partyLeaderPosition.ScreenPosition + Session.Party.Players[0].MapSprite.SourceOffset);

            // make sure the boundaries of the map are never inside the viewport
            _mapOriginPosition.X = MathHelper.Min(_mapOriginPosition.X, _viewport.X);
            _mapOriginPosition.Y = MathHelper.Min(_mapOriginPosition.Y, _viewport.Y);
            _mapOriginPosition.X += MathHelper.Max((_viewport.X + _viewport.Width) - (_mapOriginPosition.X + _map.MapDimensions.X * _map.TileSize.X), 0f);
            _mapOriginPosition.Y += MathHelper.Max((_viewport.Y + _viewport.Height - Hud.HudHeight) - (_mapOriginPosition.Y + _map.MapDimensions.Y * _map.TileSize.Y), 0f);
        }

        private static bool MoveIntoTile(Point mapPosition)
        {
            // if the tile is blocked, then this is simple
            if (_map.IsBlocked(mapPosition))
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

        public static void DrawLayers(SpriteBatch spriteBatch, bool drawBase, bool drawFringe, bool drawObject)
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

            Rectangle destinationRectangle = new(0, 0, _map.TileSize.X, _map.TileSize.Y);

            for (int y = 0; y < _map.MapDimensions.Y; y++)
            {
                for (int x = 0; x < _map.MapDimensions.X; x++)
                {
                    destinationRectangle.X = (int)_mapOriginPosition.X + x * _map.TileSize.X;
                    destinationRectangle.Y = (int)_mapOriginPosition.Y + y * _map.TileSize.Y;

                    // If the tile is inside the screen
                    if (CheckVisibility(destinationRectangle))
                    {
                        Point mapPosition = new(x, y);
                        if (drawBase)
                        {
                            Rectangle sourceRectangle = _map.GetBaseLayerSourceRectangle(mapPosition);
                            if (sourceRectangle != Rectangle.Empty)
                            {
                                spriteBatch.Draw(_map.Texture, destinationRectangle, sourceRectangle, Color.White);
                            }
                        }
                        if (drawFringe)
                        {
                            Rectangle sourceRectangle = _map.GetFringeLayerSourceRectangle(mapPosition);
                            if (sourceRectangle != Rectangle.Empty)
                            {
                                spriteBatch.Draw(_map.Texture, destinationRectangle, sourceRectangle, Color.White);
                            }
                        }
                        if (drawObject)
                        {
                            Rectangle sourceRectangle = _map.GetObjectLayerSourceRectangle(mapPosition);
                            if (sourceRectangle != Rectangle.Empty)
                            {
                                spriteBatch.Draw(_map.Texture, destinationRectangle, sourceRectangle, Color.White);
                            }
                        }
                    }
                }
            }
        }

        public static bool CheckVisibility(Rectangle screenRectangle)
        {
            return (screenRectangle.X > _viewport.X - screenRectangle.Width) && 
                   (screenRectangle.Y > _viewport.Y - screenRectangle.Height) && 
                   (screenRectangle.X < _viewport.X + _viewport.Width) && 
                   (screenRectangle.Y < _viewport.Y + _viewport.Height);
        }
    }
}
