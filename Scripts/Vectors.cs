using Godot;

namespace Wolf.Scripts
{
    public static class Vectors
    {
        public static Vector3 North = new Vector3(0, 0, -1);
        public static Vector3 NorthEast = new Vector3(1, 0, -1);
        public static Vector3 East = new Vector3(1, 0, 0);
        public static Vector3 SouthEast = new Vector3(1, 0, 1);
        public static Vector3 South = new Vector3(0, 0, 1);
        public static Vector3 SouthWest = new Vector3(-1, 0, 1);
        public static Vector3 West = new Vector3(-1, 0, 0);
        public static Vector3 NorthWest = new Vector3(-1, 0, -1);

        public static Vector3[] DirectionVectors = {
            Vector3.Zero,
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest
        };

        public static Vector3[] OppositeDirectionVectors = {
            Vector3.Zero,
            South,
            SouthWest,
            West,
            NorthWest,
            North,
            NorthEast,
            East,
            SouthEast
        };
    }
}
