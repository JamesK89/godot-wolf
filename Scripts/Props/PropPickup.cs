using Godot;
using System;

namespace Wolf
{
	public class PropPickup : PropBase
	{
		private PropPickup()
			: base(0, 0, null)
		{
		}

		public PropPickup(int x, int y, Level level)
			: base(x, y, level)
		{
			CollisionShape shape = new CollisionShape();
			BoxShape box = new BoxShape();
			box.Extents = new Vector3(Level.CellSize * 0.25f, Level.CellSize * 0.5f, Level.CellSize * 0.25f);
			shape.Shape = box;

			Area = new Area();
			Area.AddChild(shape);

			AddChild(Area);
		}

		public Area Area
		{
			get;
			protected set;
		}
	}
}
