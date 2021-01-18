using Godot;
using System;

namespace Wolf
{
    public abstract class CharacterBase : KinematicBody
    {
        public enum CharacterType : int
        {
            Player_North = 19,
            Player_East = 20,
            Player_South = 21,
            Player_West = 22
        }

        public CharacterBase(int x, int y, Level level)
        {
            Level = level;

            Location = (x, y);

            Type = (CharacterType)level.Map.Planes[(int)Level.Planes.Objects].Data[x, y];

            SetAxisLock(PhysicsServer.BodyAxis.AngularX, true);
            SetAxisLock(PhysicsServer.BodyAxis.AngularY, true);
            SetAxisLock(PhysicsServer.BodyAxis.AngularZ, true);

            MoveLockY = true;

            CollisionShape colShape = new CollisionShape();
            CylinderShape cylShape = new CylinderShape();

            cylShape.Height = Level.CellSize;
            cylShape.Radius = Level.CellSize * 0.25f;

            colShape.Shape = cylShape;

            AddChild(colShape);

            CollisionLayer = (uint)Level.CollisionLayers.Characters;
            CollisionMask = (uint)(
                Level.CollisionLayers.Characters |
                Level.CollisionLayers.Static |
                Level.CollisionLayers.Doors |
                Level.CollisionLayers.Walls |
                Level.CollisionLayers.Projectiles);

            Level.AddChild(this);

            Transform tform = this.Transform;

            tform.origin = level.MapToWorld(x, y);

            this.Transform = tform;

            SetPhysicsProcess(true);
        }

        public Vector3 Velocity
        {
            get;
            set;
        }

        public Vector3 DesiredVelocity
        {
            get;
            set;
        }

        public CharacterType Type
        {
            get;
            protected set;
        }

        public (int X, int Y) Location
        {
            get;
            protected set;
        }

        public Level Level
        {
            get;
            protected set;
        }
        
		public override void _PhysicsProcess(float delta)
		{
            Vector3 vel = (Velocity + DesiredVelocity) * 0.5f;

            Velocity = MoveAndSlide(vel, Vector3.Up);

			base._PhysicsProcess(delta);
		}
	}
}
