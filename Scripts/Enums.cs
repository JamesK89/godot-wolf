using System;

namespace Wolf.Scripts
{
    public enum Direction : int
    {
        None = 0,
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest,
    }

    public enum Planes : int
    {
        Wall = 0,
        Object,
        Extra
    }
    
    [Flags]
    public enum CollisionLayers : uint
    {
        None = 0,
        Floor = 1,
        Walls = 2,
        Doors = 4,
        Static = 8,
        Characters = 16,
        Pickups = 32,
        Projectiles = 64
    }

    public enum CellVertexIndex : int
    {
        Top_NW = 0,
        Top_NE,
        Top_SE,
        Top_SW,
        Bot_NW,
        Bot_NE,
        Bot_SE,
        Bot_SW
    }

    [Flags]
    public enum Sides
    {
        None = 0,
        North = 1,
        East = 2,
        South = 4,
        West = 8,
        North_South = North | South,
        East_West = East | West,
        All = North | East | South | West
    }
}
