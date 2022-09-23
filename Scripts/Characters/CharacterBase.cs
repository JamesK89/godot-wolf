using Godot;
using System;

namespace Wolf.Scripts
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

        private CharacterBase()
        {
        }

        public CharacterBase(int x, int y, Level level)
            : this()
        {
            if (level != null)
            {
                Level = level;

                Location = new Point2(x, y);

                Type = (CharacterType)level.Cells[y, x].Object;

                level.Cells[y, x].Nodes.Add(this);

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

                CollisionLayer = (uint)CollisionLayers.Characters;
                CollisionMask = (uint)(
                    CollisionLayers.Characters |
                    CollisionLayers.Static |
                    CollisionLayers.Doors |
                    CollisionLayers.Walls |
                    CollisionLayers.Floor |
                    CollisionLayers.Pickups |
                    CollisionLayers.Projectiles);

                Level.AddChild(this);

                Transform tform = this.Transform;

                tform.origin = level.MapToWorld(x, y);

                this.Transform = tform;

                SetPhysicsProcess(true);
            }
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

        public Point2 Location
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
            base._PhysicsProcess(delta);

            Vector3 vel = (Velocity + DesiredVelocity) * 0.5f;

            Velocity = MoveAndSlide(vel, Vector3.Up);

            Point2 newLoc = Level.WorldToMap(this.GlobalTransform.origin);

            if (!newLoc.Equals(Location))
            {
                if (Location.x >= 0 && Location.x < Level.Width &&
                    Location.y >= 0 && Location.y < Level.Height)
                {
                    Level.Cells[Location.y, Location.x].Nodes.Remove(this);
                    Level.Cells[newLoc.y, newLoc.x].Nodes.Add(this);
                }

                Location = newLoc;
            }
		}
	}
}
