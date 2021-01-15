using Godot;
using System;

namespace Wolf
{
	public class PropBlock : PropBase
	{
		public PropBlock(int x, int y, Level level)
			: base(x, y, level)
		{
			CollisionShape shape = new CollisionShape();
			BoxShape box = new BoxShape();
			box.Extents = new Vector3(Level.CellSize * 0.5f, Level.CellSize * 0.5f, Level.CellSize * 0.5f);
			shape.Shape = box;

			Body = new StaticBody();
			Body.AddChild(shape);

			AddChild(Body);
		}

		public StaticBody Body
		{
			get;
			protected set;
		}
	}
}
