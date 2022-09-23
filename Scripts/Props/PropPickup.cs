using Godot;
using System;

namespace Wolf.Scripts
{
	public class PropPickup : PropBase
	{
		[Signal]
		public delegate void PickedUp(Node who);

		private CollisionShape _areaShape;
		private Area _area;

		private PropPickup()
			: base(0, 0, null)
		{
		}

		public PropPickup(int x, int y, Level level)
			: base(x, y, level)
		{
			if (level != null)
			{
				BoxShape box = new BoxShape();
				box.Extents = new Vector3(Level.CellSize * 0.25f, Level.CellSize * 0.5f, Level.CellSize * 0.25f);

				_areaShape = new CollisionShape();
				_areaShape.Shape = box;

				_area = new Area();
				_area.CollisionLayer = (int)(CollisionLayers.Pickups);
				_area.CollisionMask = (int)(CollisionLayers.Characters);
				_area.AddChild(_areaShape);

				AddChild(_area);

				_area.Connect("body_entered", this, "OnBodyEntered");
			}
		}

		private void OnBodyEntered(Node node)
		{
			while (node != null)
			{
				if (node.HasMethod("Pickup"))
				{
					object result = node.Call("Pickup", new object[] { this });

					if (result != null && 
						Convert.ToBoolean(result))
					{
						_areaShape.SetDeferred("disabled", true);
						Visible = false;

						EmitSignal(nameof(PickedUp), new object[] { node });
					}

					break;
				}

				node = node.GetParent();
			}
		}
	}
}
