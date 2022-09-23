using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wolf.Scripts.Doors;

namespace Wolf.Scripts
{
    public partial class CharacterActor
        : CharacterBase
    {
        public const float SightDotThreshold = 0.70f;

        public class SpriteAnimationInfo
        {
            public Texture[] Frames;
            public int FramesPerSecond;
        }

        protected AudioStreamPlayer3D _audioPlayer;

        protected Sprite3D _sprite;

        protected State _state;
        protected float _stateDuration;

        protected bool _actionFired;

        protected DoorBase _door;

        protected CharacterBase _attackTarget;

        protected ImmediateGeometry _debug;
        protected static Material _debugMaterial;

        private CharacterActor() :
            base(0, 0, null)
        {
        }

        public CharacterActor(int x, int y, Level level)
            : base(x, y, level)
        {
            if (level != null)
            {
                Level = level;
                Location = new Point2(x, y);
                Tile = Location;

                MoveSpeed = 60f * Level.CellSize;
                MoveDirection = Direction.None;

                FacingDirection = Direction.West.AsVector();

                _debug = new ImmediateGeometry();

                Level.AddChild(_debug);

                _sprite = new Sprite3D();

                _sprite.Billboard = SpatialMaterial.BillboardMode.FixedY;
                _sprite.Texture = (Assets.GetSprite(50) as SpatialMaterial).AlbedoTexture;
                _sprite.PixelSize = Level.CellSize / (float)Assets.VSWAP.SpriteSize.Height;

                AddChild(_sprite);

                _audioPlayer = new AudioStreamPlayer3D();

                AddChild(_audioPlayer);

                Transform tform = this.Transform;

                tform.origin = level.MapToWorld(x, y);

                this.Transform = tform;

                ChangeState(STATE_GUARD_STAND);

                SetProcess(true);
                SetPhysicsProcess(true);
            }
        }

        public StateFlags Flags
        {
            get;
            set;
        }
        
        public Vector3 FacingDirection
        {
            get;
            set;
        }

        public Direction MoveDirection
        {
            get;
            set;
        }
        
        public Point2 Tile
        {
            get;
            set;
        }

        public float MoveSpeed
        {
            get;
            set;
        }
        
        protected virtual Vector3 GetTargetTile3DPosition()
        {
            return Level.MapToWorld(Tile);
        }

        // See the excellent post at:
        // https://godotforums.org/discussion/23169/retro-fps-style-object-angling-tutorial
        // (Archived link: https://web.archive.org/web/20200922065233/https://godotforums.org/discussion/23169/retro-fps-style-object-angling-tutorial)

        public override void _Process(float delta)
        {
            Vector3 facingDirection = FacingDirection.Normalized();

            Think(delta);

            if (!_actionFired)
            {
                _state?.Action?.Invoke(this, delta);
                _actionFired = true;
            }

            if (_state?.SpriteTextures != null)
            {
                Camera camera = GetViewport().GetCamera();

                if (camera != null)
                {
                    if (_state.Rotated)
                    {
                        float ang_inc = Mathf.Tau / (float)_state.SpriteTextures.Length;

                        Vector3 diff = ((camera.GlobalTransform.origin - GlobalTransform.origin) * (new Vector3(1f, 0f, 1f))).Normalized();
                        float theta = wrap(Mathf.Atan2(diff.x, diff.z) - Mathf.Atan2(facingDirection.x, facingDirection.z));

                        int index = (int)((theta + (ang_inc * 0.5f)) / ang_inc) % _state.SpriteTextures.Length;

                        _sprite.Texture = _state.SpriteTextures[index];
                    }
                }
            }

			base._Process(delta);
		}

        private void DrawDebugShapes()
        {
            if (_state == STATE_GUARD_STAND)
                return;

            if (_debugMaterial == null)
            {
                _debugMaterial = new SpatialMaterial();
                (_debugMaterial as SpatialMaterial).VertexColorUseAsAlbedo = true;
            }

            Vector3 tilePos = GetTargetTile3DPosition();
            Vector3 floor = new Vector3(0, Level.CellSize * -0.5f, 0);

            _debug.Clear();

            _debug.MaterialOverride = _debugMaterial;

            _debug.Begin(Mesh.PrimitiveType.Lines);
            {
                _debug.SetColor(Color.Color8(0x00, 0xFF, 0x00));

                _debug.AddVertex(GlobalTransform.origin + (floor * 0.5f));
                _debug.AddVertex(tilePos + (floor * 0.5f));
            }
            _debug.End();

            _debug.Begin(Mesh.PrimitiveType.Lines);
            {
                _debug.SetColor(Color.Color8(0xFF, 0x00, 0x00));

                _debug.AddVertex(tilePos + (Direction.NorthWest.AsVector() * Level.CellSize * 0.5f) + floor);
                _debug.AddVertex(tilePos + (Direction.NorthEast.AsVector() * Level.CellSize * 0.5f) + floor);

                _debug.AddVertex(tilePos + (Direction.NorthEast.AsVector() * Level.CellSize * 0.5f) + floor);
                _debug.AddVertex(tilePos + (Direction.SouthEast.AsVector() * Level.CellSize * 0.5f) + floor);

                _debug.AddVertex(tilePos + (Direction.SouthEast.AsVector() * Level.CellSize * 0.5f) + floor);
                _debug.AddVertex(tilePos + (Direction.SouthWest.AsVector() * Level.CellSize * 0.5f) + floor);

                _debug.AddVertex(tilePos + (Direction.SouthWest.AsVector() * Level.CellSize * 0.5f) + floor);
                _debug.AddVertex(tilePos + (Direction.NorthWest.AsVector() * Level.CellSize * 0.5f) + floor);
            }
            _debug.End();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
        }

        public int _stateCount = 0;

        protected bool ChangeState(string stateName)
        {
            State state = GetStateByName(stateName);
            return ChangeState(state);
        }

        protected bool ChangeState(State state)
        {
            bool success = false;

            if (state != null)
            {
                _state = state;
                _stateDuration = state.Duration;

                if (_state.NextInstance == null &&
                    !string.IsNullOrEmpty(_state.Next))
                {
                    _state.NextInstance = GetStateByName(_state.Next);
                }

                //GD.Print($"[{_stateCount++}] Changing state from '{_state?.Name}' to '{state?.Name}' for {_stateDuration}");

                if (_state.SpriteTextures == null &&
                    _state.SpriteIndices?.Length > 0)
                {
                    _state.SpriteTextures = new Texture[_state.SpriteIndices.Length];

                    for (int i = 0; i < _state.SpriteIndices.Length; i++)
                    {
                        _state.SpriteTextures[i] = (
                            Assets.GetSprite(_state.SpriteIndices[i]) as SpatialMaterial)
                            .AlbedoTexture;
                    }
                }

                if (!_state.Rotated)
                {
                    _sprite.Texture = _state.SpriteTextures[0];
                }

                _actionFired = false;
                success = true;
            }

            return success;
        }
        
        private State GetStateByName(string name)
        {
            State result = null;

            var members = this.GetType().GetMembers(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy);

            foreach (var member in members)
            {
                if (string.Compare(name, member.Name, true) == 0)
                {
                    Type fType = (member as FieldInfo)?.FieldType;

                    if (fType == typeof(State) || fType.IsSubclassOf(typeof(State)))
                    {
                        result = ((FieldInfo)member).GetValue(this) as State;
                    }
                }
            }

            return result;
        }

        protected void Think(float delta)
        {
            _stateDuration -= delta;

            _state?.Think?.Invoke(this, delta);

            if (_stateDuration <= 0.0f)
            {
                _stateDuration = 0.0f;
                ChangeState(_state?.NextInstance);
            }
        }

        protected void Action(float delta)
        {
            _state?.Action?.Invoke(this, delta);
        }

        protected void Think_Stand(float delta)
        {
            CharacterPlayer[] players = GetPlayersInFOV();

            if (players != null && 
                players.Length > 0)
            {
                _attackTarget = players[Random.Next() % players.Length];

                FacingDirection = (_attackTarget.GlobalTransform.origin - GlobalTransform.origin).Normalized();
                MoveDirection = Direction.None;

                _audioPlayer.Stream = Assets.GetSoundClip(0);
                _audioPlayer.Play();
                
                ChangeState(STATE_GUARD_CHASE_1);
            }
        }

        protected void Think_Path(float delta)
        {
            Think_Stand(delta);
        }

        protected bool Think_Chase_Shoot(float delta)
        {
            bool result = false;
 
            int chance = 128;

            int deltaX = Math.Abs(Tile.x - _attackTarget.Location.x);
            int deltaY = Math.Abs(Tile.y - _attackTarget.Location.y);

            int dist = deltaX > deltaY ? deltaX : deltaY;

            if (dist == 0 || (dist == 1 && MoveDirection == Direction.None))
            {
                chance = 300;
            }
            else
            {
                chance = (int)(Tics.Count << 4) / dist;
            }

            if (Random.Next() < chance)
            {
                ChangeState(STATE_GUARD_SHOOT_1);
                result = true;
            }
            else
            {
                SelectDodgeDirection(delta);
            }

            return result;
        }

        protected void Think_Chase(float delta)
        {
            DesiredVelocity = Vector3.Zero;

            //if (Think_Chase_Shoot(delta))
            //    return;

            if (MoveDirection == Direction.None)
            {
                SelectChaseDirection(delta);
            }

            if (MoveDirection != Direction.None)
            {
                Vector3 moveTo = GetTargetTile3DPosition();

                float dist = GlobalTransform.origin.DistanceTo(moveTo);

                if (Math.Abs(dist) > 0.01f)
                {
                    DesiredVelocity = ((moveTo - GlobalTransform.origin).Normalized() * MoveSpeed) * delta;
                }
                else
                {
                    Transform transform = GlobalTransform;
                    Point2 location = Tile;

                    transform.origin = new Vector3(
                        (float)location.x * Level.CellSize * 1.5f,
                        transform.origin.y,
                        (float)location.y * Level.CellSize * 1.5f
                    );
                    
                    MoveDirection = Direction.None;
                    Location = location;
                }
            }
        }

        protected void Act_Shoot(float delta)
        {
            _audioPlayer.Stream = Assets.GetSoundClip(21);
            _audioPlayer.Play();
        }

        protected void Act_Scream(float delta)
        {
            _audioPlayer.Stream = Assets.GetSoundClip(20);
            _audioPlayer.Play();
        }

        protected bool SelectChaseDirection(float delta)
        {
            if (MoveDirection != Direction.None)
                return false;

            Point2 loc = Tile;

            Direction oldDirection = MoveDirection;

            int deltaX = _attackTarget.Location.x - Location.x;
            int deltaY = _attackTarget.Location.y - Location.y;

            Direction xDir = 
                deltaX > 0 ? 
                    Direction.East : 
                    Direction.West;

            Direction yDir =
                deltaY > 0 ?
                    Direction.South :
                    Direction.North;
            
            Direction oppositeDir = oldDirection.Opposite();

            if (Math.Abs(deltaY) > Math.Abs(deltaX))
            {
                SwapDirections(ref xDir, ref yDir);
            }

            if (xDir == oppositeDir)
                xDir = Direction.None;

            if (yDir == oppositeDir)
                yDir = Direction.None;

            if (xDir != Direction.None)
            {
                if (IsWalkableDir(loc, xDir))
                {
                    MoveToDirection(xDir);
                    return true;
                }
            }

            if (yDir != Direction.None)
            {
                if (IsWalkableDir(loc, yDir))
                {
                    MoveToDirection(yDir);
                    return true;
                }
            }

            if (oldDirection != Direction.None)
            {
                if (IsWalkableDir(loc, oldDirection))
                {
                    MoveToDirection(oldDirection);
                    return true;
                }
            }

            if (Random.Next() > 128)
            {
                for (Direction tempDir = Direction.North; tempDir <= Direction.NorthWest; tempDir++)
                {
                    if (IsWalkableDir(loc, tempDir))
                    {
                        MoveToDirection(tempDir);
                        return true;
                    }
                }
            }
            else
            {
                for (Direction tempDir = Direction.NorthWest; tempDir >= Direction.North; tempDir--)
                {
                    if (IsWalkableDir(loc, tempDir))
                    {
                        MoveToDirection(tempDir);
                        return true;
                    }
                }
            }

            if (oppositeDir != Direction.None)
            {
                if (IsWalkableDir(loc, oppositeDir))
                {
                    MoveToDirection(oppositeDir);
                    return true;
                }
            }

            MoveToDirection(Direction.None);
            return false;
        }

        protected bool SelectDodgeDirection(float delta)
        {
            if (MoveDirection != Direction.None)
                return false;

            Point2 loc = Tile;

            int deltaX, deltaY;
            int absDeltaX, absDeltaY;

            Direction opposite = MoveDirection.Opposite();

            deltaX = _attackTarget.Location.x - Location.x;
            deltaY = _attackTarget.Location.y - Location.y;

            absDeltaX = Math.Abs(deltaX);
            absDeltaY = Math.Abs(deltaY);

            Direction[] dirsToTry = new Direction[]
            {
                deltaX > 0 ?
                    Direction.East :
                    Direction.West,

                deltaY > 0 ?
                    Direction.South :
                    Direction.North,

                deltaX > 0 ?
                    Direction.West :
                    Direction.East,

                deltaY > 0 ?
                    Direction.South :
                    Direction.North
            };

            if (absDeltaX > absDeltaY)
            {
                SwapDirections(ref dirsToTry[0], ref dirsToTry[1]);
                SwapDirections(ref dirsToTry[2], ref dirsToTry[3]);
            }

            if (Random.Next() < 128)
            {
                SwapDirections(ref dirsToTry[0], ref dirsToTry[1]);
                SwapDirections(ref dirsToTry[2], ref dirsToTry[3]);
            }

            dirsToTry[0] = dirsToTry[0].GetDiagonal(dirsToTry[1]);

            for (int i = 0; i < dirsToTry.Length; i++)
            {
                Direction tempDir = dirsToTry[i];

                if (tempDir != Direction.None && tempDir != opposite)
                {
                    if (IsWalkableDir(loc, tempDir))
                    {
                        MoveToDirection(tempDir);
                        return true;
                    }
                }
            }

            if (opposite != Direction.None)
            {
                if (IsWalkableDir(loc, opposite))
                {
                    MoveToDirection(opposite);
                    return true;
                }
            }

            return false;
        }

        protected void SwapDirections(ref Direction a, ref Direction b)
        {
            Direction temp;

            temp = a;
            a = b;
            b = temp;
        }

        protected void MoveToDirection(Direction dir)
        {
            MoveDirection = dir;
            FacingDirection = dir.AsVector();

            Point2 newTileTarget = Tile + dir.GetDelta();

            Level.Cell oldCell = Level.Cells[Tile.y, Tile.x];
            Level.Cell newCell = Level.Cells[newTileTarget.y, newTileTarget.x];

            if (oldCell.Claim == this)
            {
                oldCell.Claim = null;
            }

            newCell.Claim = this;

            Tile = newTileTarget;

            DrawDebugShapes();
        }

        protected bool Walk(float delta)
        {
            /*
            bool result = false;

            float walkDist = GlobalTransform.origin.DistanceTo(_moveTo);

            if (walkDist >= float.Epsilon)
            {
                result = true;
                walkDist -= (Level.CellSize * MoveSpeed) * delta;

                if (walkDist <= float.Epsilon)
                {
                    DesiredVelocity = Vector3.Zero;
                    result = false;
                }
                else
                {
                    DesiredVelocity = ((_moveTo - GlobalTransform.origin).Normalized() * MoveSpeed) * delta;
                }
            }

            return result;
            */
            return false;
        }

        protected bool IsWalkableDir(Point2 location, Direction dir)
        {
            return IsWalkableDir(location.x, location.y, dir);
        }

        protected bool IsWalkableDir(int x, int y, Direction dir)
        {
            bool result = IsTileWalkable(x, y);

            if (result)
            {
                switch (dir)
                {
                    case Direction.None:
                        result = false;
                        break;
                    case Direction.North:
                        result = IsTileWalkable(x, y - 1);
                        break;
                    case Direction.NorthEast:
                        result = IsTileWalkable(x, y - 1) &&
                                 IsTileWalkable(x + 1, y - 1) &&
                                 IsTileWalkable(x + 1, y);
                        break;
                    case Direction.East:
                        result = IsTileWalkable(x + 1, y);
                        break;
                    case Direction.SouthEast:
                        result = IsTileWalkable(x, y + 1) &&
                                 IsTileWalkable(x + 1, y + 1) &&
                                 IsTileWalkable(x + 1, y);
                        break;
                    case Direction.South:
                        result = IsTileWalkable(x, y + 1);
                        break;
                    case Direction.SouthWest:
                        result = IsTileWalkable(x, y + 1) &&
                                 IsTileWalkable(x - 1, y + 1) &&
                                 IsTileWalkable(x - 1, y);
                        break;
                    case Direction.West:
                        result = IsTileWalkable(x - 1, y);
                        break;
                    case Direction.NorthWest:
                        result = IsTileWalkable(x, y - 1) &&
                                 IsTileWalkable(x - 1, y - 1) &&
                                 IsTileWalkable(x - 1, y);
                        break;
                    default:
                        throw new ArgumentException("Invalid direction!");
                }
            }

            return result;
        }

        protected bool IsTileWalkable(int x, int y)
        {
            bool result = false;

            bool withinBounds = 
                (x >= 0 && y >= 0) &&
                (x < Level.Width && y < Level.Height);
            
            if (withinBounds)
            {
                Level.Cell cell = Level.Cells[y, x];
                
                if (!cell.IsWall && (cell.Claim == null || cell.Claim == this))
                {
                    result = 
                        cell.Nodes.Where((node) => 
                            node != this &&
                            (((node as PropBase)?.Walkable ?? true) == false ||
                            (node as CharacterPlayer) != null)
                        ).Count() < 1;
                }
            }

            return result;
        }

        private float wrap(float ang)
        {
            while (ang < 0)
            {
                ang += Mathf.Tau;
            }

            while (ang > Mathf.Tau)
            {
                ang -= Mathf.Tau;
            }

            return ang;
        }

        protected CharacterPlayer[] GetPlayersInFOV()
        {
            List<CharacterPlayer> result = 
                new List<CharacterPlayer>();

            var space = PhysicsServer.SpaceGetDirectState(this.GetWorld().Space);

            foreach (CharacterPlayer player in Level.Players)
            {
                Vector3 dir = (player.Translation - Translation).Normalized();

                if (FacingDirection.Dot(dir) >= SightDotThreshold &&
                    player.Translation.DistanceSquaredTo(Translation) <= Mathf.Pow(Level.CellSize * 5f, 2f))
                {
                    var intersections = space.IntersectRay(
                        this.GlobalTransform.origin,
                        player.GlobalTransform.origin,
                        null,
                        (uint)CollisionLayers.Walls, true, false);

                    if (intersections?.Count < 1)
                        result.Add(player);
                }
            }

            return result.ToArray();
        }
	}
}
