using Godot;
using System;
using System.Collections.Generic;

namespace Wolf
{
    public class DoorSliding : Spatial
    {
        public const int DoorWestEastWall = 101;
        public const int DoorNorthSouthWall = 100;

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

        private float _openCloseDuration;

        private AudioStreamSample _openSound;
        private AudioStreamSample _closeSound;

        private AudioStreamPlayer3D _audioPlayer;

        private CollisionShape _cellShape;
        private StaticBody _cellBody;

        private CollisionShape _doorShape;
        private RigidBody _doorBody;

        private MeshInstance _mesh;

        private DoorSliding()
        {
        }

        public DoorSliding(int x, int y, Level level)
        {
            if (level != null)
            {
                Location = new Point2(x, y);
                Level = level;

                Type = (DoorType)level.Map.Planes[(int)Level.Planes.Walls][y, x];

                level.Cells[y, x] = Level.Cell.Default();

                // Adjust neighboring walls to show the door excavation.

                if (!IsVertical)
                {
                    if ((x - 1) > -1 &&
                        level.Cells[y, x - 1].East != Level.Cell.NoWall)
                    {
                        level.Cells[y, x - 1].East = DoorWestEastWall;
                    }

                    if ((x + 1) < level.Map.Width &&
                        level.Cells[y, x + 1].West != Level.Cell.NoWall)
                    {
                        level.Cells[y, x + 1].West = DoorWestEastWall;
                    }
                }
                else
                {
                    if ((y + 1) < level.Map.Height &&
                        level.Cells[y + 1, x].North != Level.Cell.NoWall)
                    {
                        level.Cells[y + 1, x].North = DoorNorthSouthWall;
                    }

                    if ((y - 1) > -1 &&
                        level.Cells[y - 1, x].South != Level.Cell.NoWall)
                    {
                        level.Cells[y - 1, x].South = DoorNorthSouthWall;
                    }
                }

                // Create a static body for the cell so that
                // we can block the player from entering or exiting the cell
                // when the door is closed or moving.

                _cellShape = new CollisionShape();
                BoxShape box = new BoxShape();
                box.Extents = new Vector3(Level.CellSize * 0.5f, Level.CellSize * 0.5f, Level.CellSize * 0.5f);

                _cellShape.Shape = box;
                _cellShape.Name = "CollisionShape";

                _cellBody = new StaticBody();
                _cellBody.CollisionLayer = (uint)Level.CollisionLayers.Static;
                _cellBody.CollisionMask = (uint)(Level.CollisionLayers.Characters);
                _cellBody.AddChild(_cellShape);

                AddChild(_cellBody);

                // Create the actual mesh for the door and set it up for
                // this particular instance.

                BuildDoorMesh();

                _mesh = new MeshInstance();
                _mesh.Mesh = _doorMesh;
                _mesh.MaterialOverride = Assets.GetTexture(_doorTexture[Type]);

                if (IsVertical)
                {
                    _mesh.Transform = Transform.Identity.Rotated(Vector3.Up, Mathf.Pi * 0.5f);
                }
                else
                {
                    _mesh.Transform = Transform.Identity.Rotated(Vector3.Up, Mathf.Pi);
                }

                // Setup a physics body for the door itself for the purposes
                // of detecting collisions with projectiles since projectiles shouldn't
                // be blocked by the cell's static body that is normally only used for
                // ensuring characters can't enter or exit the cell if the door is closed
                // or moving.

                _doorShape = new CollisionShape();
                box = new BoxShape();
                box.Extents = new Vector3(Level.CellSize * 0.5f, Level.CellSize * 0.5f, Mathf.Epsilon);

                _doorShape.Shape = box;
                _doorShape.Name = "CollisionShape";

                _doorBody = new RigidBody();
                _doorBody.CollisionLayer = (uint)Level.CollisionLayers.Doors;
                _doorBody.CollisionMask = (uint)Level.CollisionLayers.Projectiles;
                _doorBody.AddChild(_doorShape);
                _doorBody.Mode = RigidBody.ModeEnum.Static;

                _mesh.AddChild(_doorBody);

                AddChild(_mesh);

                // Add myself to the world and set my position
                // along with some default state variables.

                level.AddChild(this);

                Transform tform = this.Transform;
                tform.origin = level.MapToWorld(x, y);
                this.Transform = tform;

                State = DoorState.Closed;

                _openCloseDuration = 0.75f;
                _canClose = true;

                _openSound = Assets.GetSoundClip(Assets.DigitalSoundList.DoorOpening);
                _closeSound = Assets.GetSoundClip(Assets.DigitalSoundList.DoorClosing);

                // Add a tween node for controlling the animation of the
                // door opening and closing.

                _tween = new Tween();
                _tween.Connect("tween_all_completed", this, "OnTweenCompleted");

                _mesh.AddChild(_tween);

                // Add an audio player so we can emit a sound
                // when the door opens and closes.

                _audioPlayer = new AudioStreamPlayer3D();
                _audioPlayer.Name = "AudioPlayer";

                AddChild(_audioPlayer);

                SetProcess(true);
                SetPhysicsProcess(true);
            }
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

        private void OnTweenCompleted()
        {
            switch (State)
            {
                case DoorState.Opening:
                    State = DoorState.Opened;
                    _cellShape.Disabled = true;
                    _openTimer = StayOpenTime;
                    break;
                case DoorState.Closing:
                    State = DoorState.Closed;
                    _cellShape.Disabled = false;
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

                Vector3 openPosition = IsVertical ? new Vector3(0, 0, Level.CellSize) : new Vector3(Level.CellSize, 0, 0);

                _tween.PlaybackProcessMode = Tween.TweenProcessMode.Physics;
                _tween.InterpolateProperty(_mesh, "translation", _mesh.Translation, openPosition, _openCloseDuration, Tween.TransitionType.Linear, Tween.EaseType.InOut);
                _tween.ResetAll();
                _tween.Start();
                
                _audioPlayer.Stream = _openSound;
                _audioPlayer.Seek(0.0f);
                _audioPlayer.Play();

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
                _tween.InterpolateProperty(_mesh, "translation", _mesh.Translation, Vector3.Zero, _openCloseDuration, Tween.TransitionType.Linear, Tween.EaseType.InOut);
                _tween.ResetAll();
                _tween.Start();
                
                _cellShape.Disabled = false;
                
                _audioPlayer.Stream = _closeSound;
                _audioPlayer.Seek(0.0f);
                _audioPlayer.Play();

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
        
        public Level Level
        {
            get;
            protected set;
        }

        public Point2 Location
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
                var space =  _cellBody.GetWorld().DirectSpaceState;
                var query = new PhysicsShapeQueryParameters();
                
                query.SetShape(_cellShape.Shape);
                query.CollisionMask = (int)Level.CollisionLayers.Characters;
                query.CollideWithBodies = true;
                query.CollideWithAreas = false;
                query.Transform = _cellShape.GlobalTransform;

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

                using (SurfaceTool st = new SurfaceTool())
                {

                    Vector3[] cellVerts = Level.GetVerticesForCell();

                    Vector3[] verts = new Vector3[] {
                        Lerp(cellVerts[(int)Level.CellVertexIndex.Top_SW], cellVerts[(int)Level.CellVertexIndex.Top_NW], 0.5f),
                        Lerp(cellVerts[(int)Level.CellVertexIndex.Top_SE], cellVerts[(int)Level.CellVertexIndex.Top_NE], 0.5f),
                        Lerp(cellVerts[(int)Level.CellVertexIndex.Bot_SW], cellVerts[(int)Level.CellVertexIndex.Bot_NW], 0.5f),
                        Lerp(cellVerts[(int)Level.CellVertexIndex.Bot_SE], cellVerts[(int)Level.CellVertexIndex.Bot_NE], 0.5f)
                    };

                    st.Begin(Godot.Mesh.PrimitiveType.Triangles);

                    st.AddUv(new Vector2(0f, 1f));
                    st.AddColor(white);
                    st.AddNormal(Level.South);
                    st.AddVertex(verts[3]);
                    st.AddUv(new Vector2(0f, 0f));
                    st.AddColor(white);
                    st.AddNormal(Level.South);
                    st.AddVertex(verts[1]);
                    st.AddUv(new Vector2(1f, 0f));
                    st.AddColor(white);
                    st.AddNormal(Level.South);
                    st.AddVertex(verts[0]);

                    st.AddUv(new Vector2(1f, 1f));
                    st.AddColor(white);
                    st.AddNormal(Level.South);
                    st.AddVertex(verts[2]);
                    st.AddUv(new Vector2(0f, 1f));
                    st.AddColor(white);
                    st.AddNormal(Level.South);
                    st.AddVertex(verts[3]);
                    st.AddUv(new Vector2(1f, 0f));
                    st.AddColor(white);
                    st.AddNormal(Level.South);
                    st.AddVertex(verts[0]);


                    st.AddUv(new Vector2(1f, 0f));
                    st.AddColor(white);
                    st.AddNormal(Level.North);
                    st.AddVertex(verts[0]);
                    st.AddUv(new Vector2(0f, 0f));
                    st.AddColor(white);
                    st.AddNormal(Level.North);
                    st.AddVertex(verts[1]);
                    st.AddUv(new Vector2(0f, 1f));
                    st.AddColor(white);
                    st.AddNormal(Level.North);
                    st.AddVertex(verts[3]);

                    st.AddUv(new Vector2(1f, 0f));
                    st.AddColor(white);
                    st.AddNormal(Level.North);
                    st.AddVertex(verts[0]);
                    st.AddUv(new Vector2(0f, 1f));
                    st.AddColor(white);
                    st.AddNormal(Level.North);
                    st.AddVertex(verts[3]);
                    st.AddUv(new Vector2(1f, 1f));
                    st.AddColor(white);
                    st.AddNormal(Level.North);
                    st.AddVertex(verts[2]);

                    _doorMesh = st.Commit();
                }
            }
        }

        private static Vector3 Lerp(Vector3 a, Vector3 b, float n)
        {
            return a + ((b - a).Normalized() * (a.DistanceTo(b) * n));
        }
    }
}
