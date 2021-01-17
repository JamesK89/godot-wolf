using Godot;
using System;

namespace Wolf
{
    public class CharacterPlayer : CharacterBase
    {
        [Flags]
        protected enum InputMoveFlags : uint
        {
            None = 0,
            Turn_Left = 1,
            Turn_Right = 2,
            Move_Forward = 4,
            Move_Backward = 8
        }

        protected InputMoveFlags InputFlags
        {
            get;
            set;
        }

        public CharacterPlayer(int x, int y, Level level)
            : base(x, y, level)
        {
            Camera = new Camera();
            Camera.Current = true;

            switch (Type)
            {
                case CharacterType.Player_East:
                    Camera.Transform = Transform.Identity.Rotated(Vector3.Up, Mathf.Pi);
                break;
                case CharacterType.Player_North:
                    Camera.Transform = Transform.Identity.Rotated(Vector3.Up, Mathf.Pi * -0.5f);
                break;
                case CharacterType.Player_South:
                    Camera.Transform = Transform.Identity.Rotated(Vector3.Up, Mathf.Pi * 0.5f);
                break;
            }

            MoveSpeed = Level.CellSize * 200.0f;
            TurnSpeed = 3f;

            UseRay = new RayCast();
            UseRay.Enabled = true;
            UseRay.ExcludeParent = true;
            UseRay.CastTo = Vector3.Forward * (Level.CellSize * 0.5f);
            UseRay.CollisionMask = (uint)(
                Level.CollisionLayers.Characters | 
                Level.CollisionLayers.Static | 
                Level.CollisionLayers.Walls);
            UseRay.AddException(this);

            Camera.AddChild(UseRay);
            AddChild(Camera);
            
            SetProcess(true);
            SetPhysicsProcess(true);
        }

        public RayCast UseRay
        {
            get;
            private set;
        }

        public Camera Camera
        {
            get;
            private set;
        }

        public float MoveSpeed
        {
            get;
            set;
        }

        public float TurnSpeed
        {
            get;
            set;
        }

        protected virtual void ProcessMovement(float delta)
        {
            if (Input.IsActionPressed("move_forward"))
            {
                InputFlags |= InputMoveFlags.Move_Forward;
            }
            else if (Input.IsActionPressed("move_backward"))
            {
                InputFlags |= InputMoveFlags.Move_Backward;
            }

            if (Input.IsActionPressed("turn_left"))
            {
                InputFlags |= InputMoveFlags.Turn_Left;
            }
            else if (Input.IsActionPressed("turn_right"))
            {
                InputFlags |= InputMoveFlags.Turn_Right;
            }

            float yaw = 0.0f;

            if (InputFlags.HasFlag(InputMoveFlags.Turn_Left))
            {
                yaw = 1.0f;
            }
            else if (InputFlags.HasFlag(InputMoveFlags.Turn_Right))
            {
                yaw = -1.0f;
            }

            Camera.Transform = Camera.Transform.Rotated(Vector3.Up, yaw * TurnSpeed * delta);
        }

        protected virtual void ProcessUseRay(float delta)
        {
            if (Input.IsActionJustPressed("use"))
            {
                if (UseRay.IsColliding())
                {
                    Node collider = (Node)UseRay.GetCollider();

                    if (collider.HasMethod("Use"))
                    {
                        collider.Call("Use", this);
                    }
                    else if (collider.GetParent()?.HasMethod("Use") ?? false)
                    {
                        collider.GetParent().Call("Use", this);
                    }
                }
            }
        }

        public override void _Process(float delta)
		{
            InputFlags = InputMoveFlags.None;

            ProcessMovement(delta);
            ProcessUseRay(delta);

            base._Process(delta);
        }

        public override void _PhysicsProcess(float delta)
		{
            DesiredVelocity = Vector3.Zero;

            if (InputFlags.HasFlag(InputMoveFlags.Move_Forward))
            {
                DesiredVelocity = Camera.Transform.basis.z * MoveSpeed * -1f;
            }
            else if (InputFlags.HasFlag(InputMoveFlags.Move_Backward))
            {
                DesiredVelocity = Camera.Transform.basis.z * MoveSpeed;
            }

            DesiredVelocity *= delta;

			base._PhysicsProcess(delta);
		}
	}
}
