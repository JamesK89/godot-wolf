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

        private RayCast _useRay;
        private Camera _camera;

        private float _moveSpeed;
        private float _turnSpeed;

        public CharacterPlayer(int x, int y, Level level)
            : base(x, y, level)
        {
            _camera = new Camera();
            _camera.Current = true;

            switch (Type)
            {
                case CharacterType.Player_East:
                    _camera.Transform = Transform.Identity.Rotated(Vector3.Up, Mathf.Pi * -0.5f);
                break;
                case CharacterType.Player_West:
                    _camera.Transform = Transform.Identity.Rotated(Vector3.Up, Mathf.Pi * 0.5f);
                break;
                case CharacterType.Player_South:
                    _camera.Transform = Transform.Identity.Rotated(Vector3.Up, Mathf.Pi * -1.0f);
                break;
            }

            _moveSpeed = Level.CellSize * 200.0f;
            _turnSpeed = 3f;

            _useRay = new RayCast();
            _useRay.Enabled = true;
            _useRay.ExcludeParent = true;
            _useRay.CastTo = Vector3.Forward * (Level.CellSize * 0.5f);
            _useRay.CollisionMask = (uint)(
                Level.CollisionLayers.Characters | 
                Level.CollisionLayers.Static | 
                Level.CollisionLayers.Walls);
            _useRay.AddException(this);

            _camera.AddChild(_useRay);
            AddChild(_camera);
            
            SetProcess(true);
            SetPhysicsProcess(true);
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

            _camera.Transform = _camera.Transform.Rotated(Vector3.Up, yaw * _turnSpeed * delta);
        }

        protected virtual void ProcessUseRay(float delta)
        {
            if (Input.IsActionJustPressed("use"))
            {
                if (_useRay.IsColliding())
                {
                    Node node = _useRay.GetCollider() as Node;

                    while (node != null)
                    {
                        if (node.HasMethod("Use"))
                        {
                            node.Call("Use", this);
                            break;
                        }

                        node = node.GetParent();
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
                DesiredVelocity = _camera.Transform.basis.z * _moveSpeed * -1f;
            }
            else if (InputFlags.HasFlag(InputMoveFlags.Move_Backward))
            {
                DesiredVelocity = _camera.Transform.basis.z * _moveSpeed;
            }

            DesiredVelocity *= delta;

			base._PhysicsProcess(delta);
		}
	}
}
