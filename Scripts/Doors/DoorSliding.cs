using Godot;
using System;
using System.Collections.Generic;

namespace Wolf
{
    public class DoorSliding : Spatial
    {
        public const int DoorWestEastWall = 100;
        public const int DoorNorthSouthWall = 101;

        public const float StayOpenTime = 5.0f;

        public enum DoorType : int
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

        private static Dictionary<DoorType, int> _doorTexture = new Dictionary<DoorType, int>()
        {
            { DoorType.Normal_Vertical, 99 },
            { DoorType.Normal_Horizontal,  98 },

            { DoorType.GoldKey_Vertical, 105 },
            { DoorType.GoldKey_Horizontal, 104 },

            { DoorType.SilverKey_Vertical, 105 },
            { DoorType.SilverKey_Horizontal, 104 },

            { DoorType.Elevator_Vertical, 103 },
            { DoorType.Elevator_Horizontal, 102 },
        };

        public enum DoorState : int
        {
            Closed = 0,
            Closing,
            Opening,
            Opened
        }

        private static ArrayMesh _doorMesh = null;

        private Tween _tween;

        private float _openTimer;
        private bool _canClose;

        private DoorSliding()
        {
        }

        public DoorSliding(int x, int y, Level level)
        {
            Location = (x, y);
            Level = level;

            Type = (DoorType)level.Map.Planes[0].Data[x, y];

            level.Cells[x, y] = Level.Cell.Default();

            if (IsVertical)
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
            shape.Name = "CollisionShape";

            CellBody = new StaticBody();
            CellBody.CollisionLayer = (uint)Level.CollisionLayers.Static;
            CellBody.CollisionMask = (uint)(Level.CollisionLayers.Characters);
            CellBody.AddChild(shape);

            AddChild(CellBody);

            BuildDoorMesh();

            Mesh = new MeshInstance();
            Mesh.Mesh = _doorMesh;
            Mesh.MaterialOverride = Assets.GetTexture(_doorTexture[Type]);

            if (!IsVertical)
            {
                Mesh.Transform = Transform.Identity.Rotated(Vector3.Up, Mathf.Pi * 0.5f);
            }

            shape = new CollisionShape();
            box = new BoxShape();
            box.Extents = new Vector3(Level.CellSize * 0.5f, Level.CellSize * 0.5f, Mathf.Epsilon);

            shape.Shape = box;
            shape.Name = "CollisionShape";

            DoorBody = new RigidBody();
            DoorBody.CollisionLayer = (uint)Level.CollisionLayers.Doors;
            DoorBody.CollisionMask = (uint)Level.CollisionLayers.Projectiles;
            DoorBody.AddChild(shape);
            DoorBody.Mode = RigidBody.ModeEnum.Static;

            Mesh.AddChild(DoorBody);

            AddChild(Mesh);

            level.AddChild(this);

            Vector3 origin = new Vector3(
                (((float)level.Map.Width * Level.CellSize) - ((float)x * Level.CellSize)) + (Level.CellSize * 0.5f),
                0,
                ((float)y * Level.CellSize) + (Level.CellSize * 0.5f));

            Transform tform = this.Transform;

            tform.origin = origin;

            this.Transform = tform;

            State = DoorState.Closed;
            Speed = 1f;

            _tween = new Tween();
            _tween.Connect("tween_all_completed", this, "OnTweenCompleted");

            Mesh.AddChild(_tween);

            _canClose = true;

            SetProcess(true);
            SetPhysicsProcess(true);
        }

        public override void _Ready()
        {
        }

		public bool Use(Node user)
        {
            return Toggle();
        }
        
        public bool IsVertical
        {
            get
            {
                return (Type == DoorType.Elevator_Vertical ||
                    Type == DoorType.GoldKey_Vertical ||
                    Type == DoorType.Normal_Vertical ||
                    Type == DoorType.SilverKey_Vertical);
            }
        }

        public float Speed
        {
            get;
            set;
        }

        private void OnTweenCompleted()
        {
            CollisionShape shape = CellBody.GetNode<CollisionShape>("CollisionShape");

            switch (State)
            {
                case DoorState.Opening:
                    State = DoorState.Opened;
                    shape.Disabled = true;
                    _openTimer = StayOpenTime;
                    break;
                case DoorState.Closing:
                    State = DoorState.Closed;
                    shape.Disabled = false;
                    break;
            }
        }

        public bool Toggle()
        {
            return (Open() || Close());
        }

        public bool Open()
        {
            bool success = false;

            if (State == DoorState.Closed)
            {
                State = DoorState.Opening;

                Vector3 openPosition = IsVertical ? new Vector3(Level.CellSize * -1f, 0, 0) : new Vector3(0, 0, Level.CellSize);

                _tween.PlaybackProcessMode = Tween.TweenProcessMode.Physics;
                _tween.InterpolateProperty(Mesh, "translation", Mesh.Translation, openPosition, Speed, Tween.TransitionType.Linear, Tween.EaseType.InOut);
                _tween.ResetAll();
                _tween.Start();

                success = true;
            }

            return success;
        }

        public bool Close()
        {
            bool success = false;

            if (State == DoorState.Opened && _canClose)
            {
                State = DoorState.Closing;

                _tween.PlaybackProcessMode = Tween.TweenProcessMode.Physics;
                _tween.InterpolateProperty(Mesh, "translation", Mesh.Translation, Vector3.Zero, Speed, Tween.TransitionType.Linear, Tween.EaseType.InOut);
                _tween.ResetAll();
                _tween.Start();
                
                success = true;
            }

            return success;
        }

        public bool CanClose
        {
            get
            {
                return _canClose;
            }
        }

        public bool IsOpened
        {
            get
            {
                return (State == DoorState.Opened);
            }
        }

        public bool IsOpening
        {
            get
            {
                return (State == DoorState.Opening);
            }
        }

        public bool IsClosed
        {
            get
            {
                return (State == DoorState.Closed);
            }
        }

        public bool IsClosing
        {
            get
            {
                return (State == DoorState.Closing);
            }
        }

        public DoorState State
        {
            get;
            protected set;
        }

        public MeshInstance Mesh
        {
            get;
            protected set;
        }

        public StaticBody CellBody
        {
            get;
            protected set;
        }

        public RigidBody DoorBody
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

        public DoorType Type
        {
            get;
            protected set;
        }

		public override void _Process(float delta)
		{
            if (State == DoorState.Opened)
            {
                _openTimer -= delta;

                if (_openTimer < 0.0f && _canClose)
                {
                    Close();
                }
            }

			base._Process(delta);
		}

		public override void _PhysicsProcess(float delta)
        {
            if (State == DoorState.Opened)
            {
                var space =  CellBody.GetWorld().DirectSpaceState;
                var query = new PhysicsShapeQueryParameters();

                CollisionShape colNode = CellBody.GetNode<CollisionShape>("CollisionShape");

                query.SetShape(colNode.Shape);
                query.CollisionMask = (int)Level.CollisionLayers.Characters;
                query.CollideWithBodies = true;
                query.CollideWithAreas = false;
                query.Transform = colNode.GlobalTransform;

                var results = space.IntersectShape(query);
                
                _canClose = (results == null || results.Count < 1);
            }

            base._PhysicsProcess(delta);
		}

		private static void BuildDoorMesh()
        {
            if (_doorMesh == null)
            {
                Color white = new Color(1f, 1f, 1f, 1f);

                SurfaceTool st = new SurfaceTool();

                Vector3[] cellVerts = Level.GetVerticesForCell();

                Vector3[] verts = new Vector3[] {
                    Lerp(cellVerts[(int)Level.CellVertexIndex.Top_SW], cellVerts[(int)Level.CellVertexIndex.Top_NW], 0.5f),
                    Lerp(cellVerts[(int)Level.CellVertexIndex.Top_SE], cellVerts[(int)Level.CellVertexIndex.Top_NE], 0.5f),
                    Lerp(cellVerts[(int)Level.CellVertexIndex.Bot_SW], cellVerts[(int)Level.CellVertexIndex.Bot_NW], 0.5f),
                    Lerp(cellVerts[(int)Level.CellVertexIndex.Bot_SE], cellVerts[(int)Level.CellVertexIndex.Bot_NE], 0.5f)
                };

                st.Begin(Godot.Mesh.PrimitiveType.Triangles);

                st.AddUv(new Vector2(1f, 1f));
                st.AddColor(white);
                st.AddNormal(Level.South);
                st.AddVertex(verts[3]);
                st.AddUv(new Vector2(1f, 0f));
                st.AddColor(white);
                st.AddNormal(Level.South);
                st.AddVertex(verts[1]);
                st.AddUv(new Vector2(0f, 0f));
                st.AddColor(white);
                st.AddNormal(Level.South);
                st.AddVertex(verts[0]);

                st.AddUv(new Vector2(0f, 1f));
                st.AddColor(white);
                st.AddNormal(Level.South);
                st.AddVertex(verts[2]);
                st.AddUv(new Vector2(1f, 1f));
                st.AddColor(white);
                st.AddNormal(Level.South);
                st.AddVertex(verts[3]);
                st.AddUv(new Vector2(0f, 0f));
                st.AddColor(white);
                st.AddNormal(Level.South);
                st.AddVertex(verts[0]);


                st.AddUv(new Vector2(0f, 0f));
                st.AddColor(white);
                st.AddNormal(Level.North);
                st.AddVertex(verts[0]);
                st.AddUv(new Vector2(1f, 0f));
                st.AddColor(white);
                st.AddNormal(Level.North);
                st.AddVertex(verts[1]);
                st.AddUv(new Vector2(1f, 1f));
                st.AddColor(white);
                st.AddNormal(Level.North);
                st.AddVertex(verts[3]);

                st.AddUv(new Vector2(0f, 0f));
                st.AddColor(white);
                st.AddNormal(Level.North);
                st.AddVertex(verts[0]);
                st.AddUv(new Vector2(1f, 1f));
                st.AddColor(white);
                st.AddNormal(Level.North);
                st.AddVertex(verts[3]);
                st.AddUv(new Vector2(0f, 1f));
                st.AddColor(white);
                st.AddNormal(Level.North);
                st.AddVertex(verts[2]);

                _doorMesh = st.Commit();
            }
        }

        private static Vector3 Lerp(Vector3 a, Vector3 b, float n)
        {
            return a + ((b - a).Normalized() * (a.DistanceTo(b) * n));
        }
    }
}
