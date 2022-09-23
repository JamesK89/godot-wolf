using Godot;
using static Godot.HTTPRequest;

namespace Wolf.Scripts
{
    public static class EnumExtensions
    {
        public static Direction Opposite(this Direction dir)
        {
            Direction result = Direction.None;

            switch (dir)
            {
                case Direction.North:
                    result = Direction.South;
                    break;
                case Direction.NorthEast:
                    result = Direction.SouthWest;
                    break;
                case Direction.East:
                    result = Direction.West;
                    break;
                case Direction.SouthEast:
                    result = Direction.NorthWest;
                    break;
                case Direction.South:
                    result = Direction.North;
                    break;
                case Direction.SouthWest:
                    result = Direction.NorthEast;
                    break;
                case Direction.West:
                    result = Direction.East;
                    break;
                case Direction.NorthWest:
                    result = Direction.SouthEast;
                    break;
            }

            return result;
        }

        public static Vector3 AsVector(this Direction dir)
        {
            return Vectors.DirectionVectors[(int)dir];
        }

        public static Direction GetDiagonal(this Direction dir, Direction otherDir)
        {
            Direction result = Direction.None;

            switch (dir)
            {
                case Direction.North:
                    if (otherDir == Direction.West)
                        result = Direction.NorthWest;
                    else if (otherDir == Direction.East)
                        result = Direction.NorthEast;
                    break;
                case Direction.East:
                    if (otherDir == Direction.North)
                        result = Direction.NorthEast;
                    else if (otherDir == Direction.South)
                        result = Direction.SouthEast;
                    break;
                case Direction.South:
                    if (otherDir == Direction.West)
                        result = Direction.SouthWest;
                    else if (otherDir == Direction.East)
                        result = Direction.SouthEast;
                    break;
                case Direction.West:
                    if (otherDir == Direction.North)
                        result = Direction.NorthWest;
                    else if (otherDir == Direction.South)
                        result = Direction.SouthWest;
                    break;
            }

            return result;
        }

        public static Point2 GetDelta(this Direction dir)
        {
            Point2 result = Point2.Zero;

            switch (dir)
            {
                case Direction.North:
                    result.y = -1;
                    break;
                case Direction.NorthEast:
                    result.y = -1;
                    result.x = 1;
                    break;
                case Direction.East:
                    result.x = 1;
                    break;
                case Direction.SouthEast:
                    result.y = 1;
                    result.x = 1;
                    break;
                case Direction.South:
                    result.y = 1;
                    break;
                case Direction.SouthWest:
                    result.x = -1;
                    result.y = 1;
                    break;
                case Direction.West:
                    result.x = -1;
                    break;
                case Direction.NorthWest:
                    result.x = -1;
                    result.y = -1;
                    break;
            }

            return result;
        }
    }
}
