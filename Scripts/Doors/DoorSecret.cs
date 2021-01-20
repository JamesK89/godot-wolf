using Godot;
using System;
using System.Collections.Generic;

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

        private DoorSecret()
        {
        }

        public DoorSecret(int x, int y, Level level)
        {
            Location = new Point2(x, y);
            Level = level;
            
            Type = level.Map.Planes[(int)Level.Planes.Walls][y, x];

            Mesh = new MeshInstance();
            Mesh.Mesh = GetMeshForDoor(Type);

            AddChild(Mesh);
            
            CollisionShape shape = new CollisionShape();
            BoxShape box = new BoxShape();
            box.Extents = new Vector3(Level.CellSize * 0.5f, Level.CellSize * 0.5f, Level.CellSize * 0.5f);

            shape.Shape = box;
            shape.Name = "CollisionShape";

            Body = new RigidBody();
            Body.Mode = RigidBody.ModeEnum.Static;
            Body.CollisionLayer = (uint)Level.CollisionLayers.Walls;
            Body.CollisionMask = (uint)(Level.CollisionLayers.Characters);
            Body.AddChild(shape);

            AddChild(Body);
            
            AudioStreamPlayer3D audioPlayer = new AudioStreamPlayer3D();
            audioPlayer.Name = "AudioPlayer";

            AddChild(audioPlayer);

            ActivateSound = Assets.GetSoundClip(Assets.DigitalSoundList.PushWallActivation);
            
            _tween = new Tween();
            _tween.Connect("tween_all_completed", this, "OnTweenCompleted");

            AddChild(_tween);

            level.AddChild(this);

            Transform tform = this.Transform;

            tform.origin = level.MapToWorld(x, y);

            this.Transform = tform;

            State = DoorState.Stopped;
            
            SetProcess(true);
            SetPhysicsProcess(true);
        }

        public DoorState State
        {
            get;
            protected set;
        }

        public int Type
        {
            get;
            protected set;
        }

        public RigidBody Body
        {
            get;
            protected set;
        }

        public MeshInstance Mesh
        {
            get;
            protected set;
        }

        public AudioStream ActivateSound
        {
            get;
            set;
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
            
            if (State == DoorState.Stopped &&
                user != null &&
                user is Spatial)
            {
                Vector3 movDir = Vector3.Zero;

                var userMapPos = Level.WorldToMap((user as Spatial).Translation);
                var myMapPos = Level.WorldToMap(Translation);

                (int x, int y, Vector3 dir)[] dirs = new (int, int, Vector3)[]
                {
                    (myMapPos.x - 1, myMapPos.y, Level.South),
                    (myMapPos.x + 1, myMapPos.y, Level.North),
                    (myMapPos.x, myMapPos.y - 1, Level.East),
                    (myMapPos.x, myMapPos.y + 1, Level.West)
                };

                foreach (var dir in dirs)
                {
                    if (userMapPos.x == dir.x &&
                        userMapPos.y == dir.y)
                    {
                        AudioStreamPlayer3D audioPlayer = GetNode<AudioStreamPlayer3D>("AudioPlayer");
                        audioPlayer.Stream = ActivateSound;
                        audioPlayer.Seek(0.0f);
                        audioPlayer.Play();

                        _tween.PlaybackProcessMode = Tween.TweenProcessMode.Physics;
                        _tween.InterpolateProperty(Mesh, "translation", Vector3.Zero, dir.dir * Level.CellSize * 2f, 1.0f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
                        _tween.ResetAll();
                        _tween.Start();

                        break;
                    }
                }
            }

            return result;
        }

        private void OnTweenCompleted()
        {
            State = DoorState.Stopped;
        }
        
        public override void _Process(float delta)
		{
			base._Process(delta);
		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);
		}

		public static ArrayMesh GetMeshForDoor(int id)
        {
            ArrayMesh result = _meshes.ContainsKey(id) ?
                _meshes[id] : null;

            if (result == null)
            {
                result = new ArrayMesh();

                SurfaceTool st = new SurfaceTool();

                st.Begin(Godot.Mesh.PrimitiveType.Triangles);
                //st.SetMaterial(Assets.GetTexture(((id - 1) << 1) + 1));
                Level.CreateCube(st, Level.CellSize, Vector3.Zero, Level.Sides.North_South);
                st.Commit(result);

                st.Begin(Godot.Mesh.PrimitiveType.Triangles);
                //st.SetMaterial(Assets.GetTexture((id - 1) << 1));
                Level.CreateCube(st, Level.CellSize, Vector3.Zero, Level.Sides.East_West);
                st.Commit(result);

                _meshes.Add(id, result);
            }

            return result;
        }
    }
}
