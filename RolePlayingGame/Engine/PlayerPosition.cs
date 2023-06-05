using Microsoft.Xna.Framework;
using RolePlayingGame.Directions;

namespace RolePlayingGame.Engine
{
    public class PlayerPosition
    {
        public Point TilePosition = Point.Zero;
        public Vector2 TileOffset = Vector2.Zero;

        public Vector2 ScreenPosition
        {
            get
            {
                return TileEngine.GetScreenPosition(TilePosition) + TileOffset;
            }
        }

        public Direction Direction = Direction.South;

        private bool isMoving = false;
        public bool IsMoving
        {
            get { return isMoving; }
        }

        public void Move(Vector2 movement)
        {
            isMoving = (movement != Vector2.Zero);

            CalculateMovement(movement, ref TilePosition, ref TileOffset);

            // if the position is moving, up the direction
            if (IsMoving)
            {
                Direction = CalculateDirection(movement);
            }
        }

        public static void CalculateMovement(Vector2 movement, ref Point TilePosition, ref Vector2 TileOffset)
        {
            // add the movement
            TileOffset += movement;

            while (TileOffset.X > TileEngine.Map.TileSize.X / 2f)
            {
                TilePosition.X++;
                TileOffset.X -= TileEngine.Map.TileSize.X;
            }
            while (TileOffset.X < -TileEngine.Map.TileSize.X / 2f)
            {
                TilePosition.X--;
                TileOffset.X += TileEngine.Map.TileSize.X;
            }
            while (TileOffset.Y > TileEngine.Map.TileSize.Y / 2f)
            {
                TilePosition.Y++;
                TileOffset.Y -= TileEngine.Map.TileSize.Y;
            }
            while (TileOffset.Y < -TileEngine.Map.TileSize.Y / 2f)
            {
                TilePosition.Y--;
                TileOffset.Y += TileEngine.Map.TileSize.Y;
            }
        }

        public static Direction CalculateDirection(Vector2 vector)
        {
            if (vector.X > 0)
            {
                if (vector.Y > 0)
                {
                    return Direction.SouthEast;
                }
                else if (vector.Y < 0)
                {
                    return Direction.NorthEast;
                }
                else // y == 0
                {
                    return Direction.East;
                }
            }
            else if (vector.X < 0)
            {
                if (vector.Y > 0)
                {
                    return Direction.SouthWest;
                }
                else if (vector.Y < 0)
                {
                    return Direction.NorthWest;
                }
                else // y == 0
                {
                    return Direction.West;
                }
            }
            else // x == 0
            {
                if (vector.Y > 0)
                {
                    return Direction.South;
                }
                else if (vector.Y < 0)
                {
                    return Direction.North;
                }
            }
            // x == 0 && y == 0, so... south?
            return Direction.South;
        }
    }
}
