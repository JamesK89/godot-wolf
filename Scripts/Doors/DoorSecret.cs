using Godot;
using System;
using System.Collections.Generic;

using Direction = Wolf.Level.Direction;

namespace Wolf
{
    public class DoorSecret : Spatial
    {
        private static Dictionary<int, ArrayMesh> _meshes =
            new Dictionary<int, ArrayMesh>();

        public const int DoorSecretId = 98;

        public enum DoorState : int
        {
            Stopped = 0,
            Moving
        }

        private Tween _tween;

        private MeshInstance _mesh;

        private float _moveDuration = 3.0f;

        private Direction _moveDir;
        private int _moveDist;

        private CollisionShape _wallShape;
        private RigidBody _wallBody;

        private AudioStreamPlayer3D _audioPlayer;
        private AudioStream _activateSound;

        private DoorSecret()
        {
        }

        public DoorSecret(int x, int y, Level level)
        {
            if (level != null)
            {
                Location = new Point2(x, y);
                Level = level;

                Type = level.Map.Planes[(int)Level.Planes.Walls][y, x];

                // This will be the physical body for the door itself.

                BoxShape box = new BoxShape();
                box.Extents = new Vector3(Level.CellSize * 0.5f, Level.CellSize * 0.5f, Level.CellSize * 0.5f);

                _wallShape = new CollisionShape();
                _wallShape.Shape = box;

                _wallBody = new RigidBody();
                _wallBody.Mode = RigidBody.ModeEnum.Static;
                _wallBody.CollisionLayer = (uint)Level.CollisionLayers.Walls;
                _wallBody.CollisionMask = (uint)(Level.CollisionLayers.Characters | Level.CollisionLayers.Projectiles);
                _wallBody.AddChild(_wallShape);

                AddChild(_wallBody);

                // Create the actual mesh for the door

                _mesh = new MeshInstance();
                _mesh.Mesh = GetMeshForDoor(Type);

                //_wallBody.AddChild(_mesh);

                // Add an audio player to play the "pushing" sound when the door
                // is activated.

                _audioPlayer = new AudioStreamPlayer3D();

                _wallBody.AddChild(_audioPlayer);

                _activateSound = Assets.GetSoundClip(Assets.DigitalSoundList.PushWallActivation);

                // Add the tween that will be used to animate the door.

                _tween = new Tween();
                _tween.Connect("tween_all_completed", this, "OnTweenCompleted");

                AddChild(_tween);

                // Add myself to the world and set my position.

                level.AddChild(this);

                Transform tform = this.Transform;
                tform.origin = level.MapToWorld(x, y);
                this.Transform = tform;

                // Set my default state.

                State = DoorState.Stopped;
                Enabled = true;

                _moveDir = Direction.North;
                _moveDist = 0;

                SetProcess(true);
                SetPhysicsProcess(true);
            }
        }

        public DoorState State
        {
            get;
            protected set;
        }

        public bool Enabled
        {
            get;
            protected set;
        }

        public int Type
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

        public bool Use(Node user)
        {
            bool result = false;

            if (Enabled &&
                State == DoorState.Stopped &&
                user != null &&
                user is Spatial)
            {
                Vector3 movDir = Vector3.Zero;

                var userMapPos = Level.WorldToMap((user as Spatial).Translation);
                var myMapPos = Level.WorldToMap(Translation);

                (int x, int y, Direction dir)[] dirs = new (int, int, Direction)[]
                {
                    (myMapPos.x - 1, myMapPos.y, Direction.East),
                    (myMapPos.x + 1, myMapPos.y, Direction.West),
                    (myMapPos.x, myMapPos.y - 1, Direction.South),
                    (myMapPos.x, myMapPos.y + 1, Direction.North)
                };

                foreach (var dir in dirs)
                {
                    if (userMapPos.x == dir.x &&
                        userMapPos.y == dir.y)
                    {
                        _moveDir = dir.dir;
                        _moveDist = 2;
                        break;
                    }
                }
            }

            return result;
        }

        private void OnTweenCompleted()
        {
            State = DoorState.Stopped;

            var tform = Transform;
            tform.origin = _wallBody.GlobalTransform.origin;

            _wallBody.Translation = Vector3.Zero;

            Transform = tform;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_moveDist > 0)
            {
                Vector3 dir = Level.DirectionVectors[(int)_moveDir];

                var space = _wallBody.GetWorld().DirectSpaceState;

                using (var query = new PhysicsShapeQueryParameters())
                {
                    using (BoxShape box = new BoxShape())
                    {
                        while (_moveDist > 0)
                        {
                            float lg_extent = (Level.CellSize * (float)_moveDist * 0.5f);
                            float st_extent = (Level.CellSize * 0.5f);

                            box.Extents = new Vector3(
                                _moveDir == Direction.West || _moveDir == Direction.East ?
                                lg_extent : st_extent,
                                st_extent,
                                _moveDir == Direction.North || _moveDir == Direction.South ?
                                lg_extent : st_extent);

                            Transform tform = this.GlobalTransform;
                            tform.origin += (dir * ((Level.CellSize * 0.5f) + (Level.CellSize * (float)_moveDist * 0.5f)));

                            query.SetShape(box);
                            query.CollisionMask = (int)(Level.CollisionLayers.Characters | Level.CollisionLayers.Walls);
                            query.CollideWithBodies = true;
                            query.CollideWithAreas = false;
                            query.Exclude.Add(_wallBody);
                            query.Transform = tform;

                            using (var results = space.IntersectShape(query))
                            {
                                if (results == null || results.Count < 1)
                                {
                                    State = DoorState.Moving;
                                    Enabled = false;

                                    _audioPlayer.Stream = _activateSound;
                                    _audioPlayer.Seek(0.0f);
                                    _audioPlayer.Play();

                                    _tween.PlaybackProcessMode = Tween.TweenProcessMode.Physics;
                                    _tween.InterpolateProperty(_wallBody, "translation", Vector3.Zero, dir * Level.CellSize * (float)_moveDist, _moveDuration, Tween.TransitionType.Linear, Tween.EaseType.InOut);
                                    _tween.ResetAll();
                                    _tween.Start();

                                    _moveDist = 0;

                                    break;
                                }
                            }

                            _moveDist -= 1;
                        }
                    }
                }
            }

            base._PhysicsProcess(delta);
        }

        public static ArrayMesh GetMeshForDoor(int id)
        {
            ArrayMesh result = _meshes.ContainsKey(id) ?
                _meshes[id] : null;

            if (result == null)
            {
                result = new ArrayMesh();

                using (SurfaceTool st = new SurfaceTool())
                {
                    st.Begin(Godot.Mesh.PrimitiveType.Triangles);
                    st.SetMaterial(Assets.GetTexture((id - 1) << 1));
                    Level.CreateCube(st, Level.CellSize, Vector3.Zero, Level.Sides.North_South);
                    st.Commit(result);

                    st.Begin(Godot.Mesh.PrimitiveType.Triangles);
                    st.SetMaterial(Assets.GetTexture(((id - 1) << 1) + 1));
                    Level.CreateCube(st, Level.CellSize, Vector3.Zero, Level.Sides.East_West);
                    st.Commit(result);
                }

                _meshes.Add(id, result);
            }

            return result;
        }
    }
}
