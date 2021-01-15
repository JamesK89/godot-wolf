using Godot;
using System;
using System.Collections.Generic;

namespace Wolf
{
    public class Door : Spatial
    {
        public const int DoorWestEastWall = 100;
        public const int DoorNorthSouthWall = 101;

        public enum DoorTypes : int
        {
            Normal_Vertical = 90,
            Normal_Horizontal = 91,

            GoldKey_Vertical = 92,
            GoldKey_Horizontal = 93,

            SilverKey_Vertical = 94,
            SilverKey_Horizontal = 95,

            Elevator_Vertical = 100,
            Elevator_Horizontal = 101
        }

        private static Dictionary<int, int> _doorTexture = new Dictionary<int, int>()
        {
            { (int)DoorTypes.Normal_Vertical, 100 },
            { (int)DoorTypes.Normal_Horizontal,  99 },

            { (int)DoorTypes.GoldKey_Vertical, 106 },
            { (int)DoorTypes.GoldKey_Horizontal, 105 },

            { (int)DoorTypes.SilverKey_Vertical, 106 },
            { (int)DoorTypes.SilverKey_Horizontal, 105 },

            { (int)DoorTypes.Elevator_Vertical, 104 },
            { (int)DoorTypes.Elevator_Horizontal, 103 },
        };

        public Door(int x, int y, Level level)
        {
            Location = (x, y);
            Level = level;

            Type = (DoorTypes)level.Map.Planes[0].Data[x, y];

            level.Cells[x, y] = Level.Cell.Default();

            if (IsVerticalDoorCell((int)Type))
            {
                if ((x - 1) > -1 &&
                    level.Cells[x - 1, y].East != Level.Cell.NoWall)
                {
                    level.Cells[x - 1, y].East = DoorWestEastWall;
                }

                if ((x + 1) < level.Map.Width &&
                    level.Cells[x + 1, y].West != Level.Cell.NoWall)
                {
                    level.Cells[x + 1, y].West = DoorWestEastWall;
                }
            }
            else
            {
                if ((y + 1) < level.Map.Height &&
                    level.Cells[x, y + 1].North != Level.Cell.NoWall)
                {
                    level.Cells[x, y + 1].North = DoorNorthSouthWall;
                }

                if ((y - 1) > -1 &&
                    level.Cells[x, y - 1].South != Level.Cell.NoWall)
                {
                    level.Cells[x, y - 1].South = DoorNorthSouthWall;
                }
            }

            CollisionShape shape = new CollisionShape();
            BoxShape box = new BoxShape();
            box.Extents = new Vector3(Level.CellSize * 0.5f, Level.CellSize * 0.5f, Level.CellSize * 0.5f);
            shape.Shape = box;

            Body = new StaticBody();
            Body.AddChild(shape);

            AddChild(Body);

            level.AddChild(this);

            Vector3 origin = new Vector3(
                (((float)level.Map.Width * Level.CellSize) - ((float)x * Level.CellSize)) + (Level.CellSize * 0.5f),
                0,
                ((float)y * Level.CellSize) + (Level.CellSize * 0.5f));

            Transform tform = this.Transform;

            tform.origin = origin;

            this.Transform = tform;
        }
        
        public override void _Ready()
        {

        }

        public static bool IsVerticalDoorCell(int id)
        {
            return (id == (int)DoorTypes.Normal_Vertical ||
                id == (int)DoorTypes.GoldKey_Vertical ||
                id == (int)DoorTypes.SilverKey_Vertical ||
                id == (int)DoorTypes.Elevator_Vertical);
        }

        public StaticBody Body
        {
            get;
            protected set;
        }

        public Level Level
        {
            get;
            protected set;
        }

        public (int X, int Y) Location
        {
            get;
            protected set;
        }

        public DoorTypes Type
        {
            get;
            protected set;
        }

        public static bool IsDoorCell(int id)
        {
            return (id >= 90 && id <= 101);
        }
    }
}
