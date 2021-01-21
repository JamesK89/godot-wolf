using Godot;
using System;
using wolfread;
using Fig;
using System.Linq;
using System.Collections.Generic;

namespace Wolf
{
    public class Level : MeshInstance
    {
        public const int MapWallBegin = 1;
        public const int MapWallEnd = 63;
        
        public enum Planes : int
        {
            Walls = 0,
            Objects
        }

        public static Vector3 North = new Vector3(0, 0, -1);
        public static Vector3 South = new Vector3(0, 0, 1);
        public static Vector3 West = new Vector3(-1, 0, 0);
        public static Vector3 East = new Vector3(1, 0, 0);

        public enum Direction : int
        {
            North = 0,
            East,
            South,
            West
        }

        public static Vector3[] DirectionVectors = {
            North,
            East,
            South,
            West
        };

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

        public class Cell
        {
            public const int NoWall = int.MaxValue;

            public int North;
            public int East;
            public int South;
            public int West;

            public static Cell Default()
            {
                Cell ret = new Cell();
                ret.North =
                    ret.East =
                    ret.South =
                    ret.West =
                    NoWall;

                return ret;
            }

            public bool HasWall
            {
                get
                {
                    return (North != NoWall ||
                        South != NoWall ||
                        West != NoWall ||
                        East != NoWall);
                }
            }
        }

        public const float CellSize = 1f;

        public Level()
        {
        }

        public int Index
        {
            get;
            private set;
        }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public wolfread.Map Map
        {
            get;
            private set;
        }

        public StaticBody WorldBody
        {
            get;
            private set;
        }

        public StaticBody FloorBody
        {
            get;
            private set;
        }

        public StaticBody CeilingBody
        {
            get;
            private set;
        }

        public Cell[,] Cells
        {
            get;
            private set;
        }

        public bool IsWall(int x, int y)
        {
            bool result = false;

            if (x > -1 && y > -1 &&
                x < Map.Width &&
                y < Map.Height)
            {
                int pl0 = Map.Planes[(int)Planes.Walls][y, x];
                int pl1 = Map.Planes[(int)Planes.Objects][y, x];

                result = (pl0 >= MapWallBegin &&
                    pl0 <= MapWallEnd &&
                    pl1 != DoorSecret.DoorSecretId);
            }

            return result;
        }

        public Vector3 MapToWorld(int x, int y)
        {
            return new Vector3(
                    ((float)x * Level.CellSize) + (Level.CellSize * 0.5f),
                    0,
                    ((float)y * Level.CellSize) + (Level.CellSize * 0.5f));
        }

        public Point2 WorldToMap(Vector3 pos)
        {
            return new Point2((int)(pos.x / Level.CellSize), (int)(pos.z / Level.CellSize));
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            Load(0);
            SetProcess(true);
        }
        
        public override void _Process(float delta)
        {
            int indexChange = 0;

            if (Input.IsActionJustPressed("prev_map"))
            {
                indexChange = -1;
            }
            else if (Input.IsActionJustPressed("next_map"))
            {
                indexChange = 1;
            }

            if (indexChange != 0)
            {
                indexChange += Index;

                if (indexChange != Index && 
                    indexChange >= 0 && 
                    indexChange < Configuration.Assets.NUMMAPS)
                {
                    Load(indexChange);
                }
            }
        }

        private Cell[,] BuildCellsFromMap(wolfread.Map map)
        {
            Cell[,] ret = new Cell[map.Height, map.Width];

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    Cell cube = Cell.Default();

                    int mapValue = map.Planes[0].Data[y, x];
                    int mapValue_pl1 = map.Planes[1].Data[y, x];

                    if (mapValue >= MapWallBegin && mapValue <= MapWallEnd 
                        && mapValue_pl1 != DoorSecret.DoorSecretId)
                    {
                        if ((y - 1) > -1)
                        {
                            int neighbor = map.Planes[0].Data[y - 1, x];
                            int neighbor_pl1 = map.Planes[1].Data[y - 1, x];

                            if (neighbor < MapWallBegin || neighbor > MapWallEnd ||
                                neighbor_pl1 == DoorSecret.DoorSecretId)
                            {
                                cube.North = (mapValue - 1) << 1;
                            }
                        }

                        if ((x + 1) < map.Width)
                        {
                            int neighbor = map.Planes[0].Data[y, x + 1];
                            int neighbor_pl1 = map.Planes[1].Data[y, x + 1];

                            if (neighbor < MapWallBegin || neighbor > MapWallEnd || 
                                neighbor_pl1 == DoorSecret.DoorSecretId)
                            {
                                cube.East = ((mapValue - 1) << 1) + 1;
                            }
                        }

                        if ((y + 1) < map.Height)
                        {
                            int neighbor = map.Planes[0].Data[y + 1, x];
                            int neighbor_pl1 = map.Planes[1].Data[y + 1, x];

                            if (neighbor < MapWallBegin || neighbor > MapWallEnd ||
                                neighbor_pl1 == DoorSecret.DoorSecretId)
                            {
                                cube.South = (mapValue - 1) << 1;
                            }
                        }

                        if ((x - 1) > -1)
                        {
                            int neighbor = map.Planes[0].Data[y, x - 1];
                            int neighbor_pl1 = map.Planes[1].Data[y, x - 1];

                            if (neighbor < MapWallBegin || neighbor > MapWallEnd || 
                                neighbor_pl1 == DoorSecret.DoorSecretId)
                            {
                                cube.West = ((mapValue - 1) << 1) + 1;
                            }
                        }
                    }

                    ret[y, x] = cube;
                }
            }

            return ret;
        }

        private void LoadMap(int index)
        {
            var vga = new VGAGRAPH(Configuration.Assets.GetDefinitions());
            Map = Assets.Maps[index];
            Cells = BuildCellsFromMap(Map);

            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    if (DoorFactory.CreateSlidingDoor(x, y, this) != null)
                        continue;

                    if (DoorFactory.CreateSecretDoor(x, y, this) != null)
                        continue;

                    if (PropFactory.CreateProp(x, y, this) != null)
                        continue;

                    if (CharacterFactory.CreateCharacter(x, y, this) != null)
                        continue;
                }
            }
        }

        public void Load(int index)
        {
            Index = index;

            foreach (Node child in GetChildren())
            {
                if (child is DoorSliding || 
                    child is DoorSecret ||
                    child is PropBase ||
                    child is CharacterBase)
                {
                    RemoveChild(child);
                    child.QueueFree();
                }
            }

            if (WorldBody != null)
            {
                RemoveChild(WorldBody);
                WorldBody.QueueFree();
            }

            LoadMap(index);
            BuildMeshFromCells();
            
            Vector3 plPos = new Vector3(
                CellSize * (float)Map.Width * -0.5f,
                CellSize * -0.5f,
                CellSize * (float)Map.Height * -0.5f);

            if (FloorBody == null)
            {
                FloorBody = CreateCollisionPlane(plPos, Vector3.Up, 0f);
                AddChild(FloorBody);
            }

            plPos.y *= -1f;

            if (CeilingBody == null)
            {
                CeilingBody = CreateCollisionPlane(plPos, Vector3.Down, 0f);
                AddChild(CeilingBody);
            }
        }

        private StaticBody CreateCollisionPlane(Vector3 position, Vector3 normal, float d)
        {
            StaticBody body = new StaticBody();
            CollisionShape colShape = new CollisionShape();
            PlaneShape plaShape = new PlaneShape();
            
            plaShape.Plane = new Plane(normal, d);
            colShape.Shape = plaShape;

            Transform tform = Transform.Identity;
            tform.origin = position;
            
            body.CollisionLayer = (int)CollisionLayers.Floor;
            body.Transform = tform;

            body.AddChild(colShape);

            return body;
        }

        public void BuildMeshFromCells()
        {
            if (Mesh != null)
            {
                var msh = Mesh;
                Mesh = null;
                msh.Dispose();
            }

            Dictionary<int, SurfaceTool> surfaces = new Dictionary<int, SurfaceTool>();

            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    Cell cell = Cells[y, x];

                    var indices = (
                        new int[] {
                        cell.North,
                        cell.East,
                        cell.South,
                        cell.West })
                            .Select(i => i)
                            .Distinct();

                    foreach (var index in indices)
                    {
                        if (index == Cell.NoWall)
                            continue;

                        if (!surfaces.ContainsKey(index))
                        {
                            surfaces.Add(index, new SurfaceTool());
                        }
                    }
                }
            }

            ArrayMesh mesh = new ArrayMesh();
            SurfaceTool st = new SurfaceTool();

            foreach (var kvp in surfaces)
            {
                kvp.Value.Begin(Mesh.PrimitiveType.Triangles);
                kvp.Value.SetMaterial(Assets.GetTexture(kvp.Key));
            }

            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    Cell cell = Cells[y, x];

                    Vector3 pos = MapToWorld(x, y);

                    if (cell.North != Cell.NoWall)
                    {
                        CreateCube(surfaces[cell.North], CellSize, pos, Sides.North);
                    }

                    if (cell.East != Cell.NoWall)
                    {
                        CreateCube(surfaces[cell.East], CellSize, pos, Sides.East);
                    }

                    if (cell.South != Cell.NoWall)
                    {
                        CreateCube(surfaces[cell.South], CellSize, pos, Sides.South);
                    }

                    if (cell.West != Cell.NoWall)
                    {
                        CreateCube(surfaces[cell.West], CellSize, pos, Sides.West);
                    }
                }
            }

            foreach (var kvp in surfaces)
            {
                kvp.Value.Commit(mesh);
                kvp.Value.Dispose();
            }

            Mesh = mesh;

            CreateTrimeshCollision();

            WorldBody = GetNode<StaticBody>($"{nameof(Level)}_col");
            WorldBody.CollisionLayer = (int)CollisionLayers.Walls;
        }

        public static Vector3[] GetVerticesForCell()
        {
            return GetVerticesForCell(CellSize, Vector3.Zero);
        }

        public static Vector3[] GetVerticesForCell(float size)
        {
            return GetVerticesForCell(size, Vector3.Zero);
        }

        public static Vector3[] GetVerticesForCell(Vector3 position)
        {
            return GetVerticesForCell(CellSize, position);
        }

        public static Vector3[] GetVerticesForCell(float size, Vector3 position)
        {
            Vector3 vs = Vector3.One * (size * 0.5f);

            Vector3 vt = Vector3.Up * vs;
            Vector3 vd = Vector3.Down * vs;
            Vector3 vf = North * vs;
            Vector3 vr = South * vs;

            // Cube's top four vertices,
            // clock-wise (with North being 12 o' clock)
            Vector3 cvta = position + (West * vs) + vt + vf;
            Vector3 cvtb = position + (East * vs) + vt + vf;
            Vector3 cvtc = position + (East * vs) + vt + vr;
            Vector3 cvtd = position + (West * vs) + vt + vr;

            // Cube's bottom four vertices,
            // clock-wise (with North being 12 o' clock)
            Vector3 cvte = position + (West * vs) + vd + vf;
            Vector3 cvtf = position + (East * vs) + vd + vf;
            Vector3 cvtg = position + (East * vs) + vd + vr;
            Vector3 cvth = position + (West * vs) + vd + vr;

            return new Vector3[] {
                cvta, cvtb, cvtc, cvtd,
                cvte, cvtf, cvtg, cvth
            };
        }

        public static void CreateCube(SurfaceTool st, float size, Vector3 position, Sides sides)
        {
            if (sides.HasFlag(Sides.North) ||
                sides.HasFlag(Sides.East) ||
                sides.HasFlag(Sides.South) ||
                sides.HasFlag(Sides.West))
            {
                Color white = new Color(1f, 1f, 1f, 1f);

                Vector3[] vertices = GetVerticesForCell(size, position);

                if (sides.HasFlag(Sides.North))
                {
                    st.AddUv(new Vector2(1, 1));
                    st.AddNormal(North);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Bot_NW]);
                    st.AddUv(new Vector2(0, 0));
                    st.AddNormal(North);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Top_NE]);
                    st.AddUv(new Vector2(1, 0));
                    st.AddNormal(North);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Top_NW]);

                    st.AddUv(new Vector2(1, 1));
                    st.AddNormal(North);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Bot_NW]);
                    st.AddUv(new Vector2(0, 1));
                    st.AddNormal(North);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Bot_NE]);
                    st.AddUv(new Vector2(0, 0));
                    st.AddNormal(North);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Top_NE]);
                }

                if (sides.HasFlag(Sides.East))
                {
                    st.AddUv(new Vector2(0, 0));
                    st.AddNormal(East);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Top_SE]);
                    st.AddUv(new Vector2(1, 0));
                    st.AddNormal(East);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Top_NE]);
                    st.AddUv(new Vector2(0, 1));
                    st.AddNormal(East);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Bot_SE]);

                    st.AddUv(new Vector2(1, 0));
                    st.AddNormal(East);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Top_NE]);
                    st.AddUv(new Vector2(1, 1));
                    st.AddNormal(East);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Bot_NE]);
                    st.AddUv(new Vector2(0, 1));
                    st.AddNormal(East);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Bot_SE]);
                }

                if (sides.HasFlag(Sides.South))
                {
                    st.AddUv(new Vector2(0, 0));
                    st.AddNormal(South);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Top_SW]);
                    st.AddUv(new Vector2(1, 0));
                    st.AddNormal(South);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Top_SE]);
                    st.AddUv(new Vector2(0, 1));
                    st.AddNormal(South);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Bot_SW]);

                    st.AddUv(new Vector2(1, 0));
                    st.AddNormal(South);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Top_SE]);
                    st.AddUv(new Vector2(1, 1));
                    st.AddNormal(South);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Bot_SE]);
                    st.AddUv(new Vector2(0, 1));
                    st.AddNormal(South);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Bot_SW]);
                }

                if (sides.HasFlag(Sides.West))
                {
                    st.AddUv(new Vector2(1, 1));
                    st.AddNormal(West);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Bot_SW]);
                    st.AddUv(new Vector2(0, 0));
                    st.AddNormal(West);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Top_NW]);
                    st.AddUv(new Vector2(1, 0));
                    st.AddNormal(West);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Top_SW]);

                    st.AddUv(new Vector2(1, 1));
                    st.AddNormal(West);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Bot_SW]);
                    st.AddUv(new Vector2(0, 1));
                    st.AddNormal(West);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Bot_NW]);
                    st.AddUv(new Vector2(0, 0));
                    st.AddNormal(West);
                    st.AddColor(white);
                    st.AddVertex(vertices[(int)CellVertexIndex.Top_NW]);
                }
            }
        }
    }
}
