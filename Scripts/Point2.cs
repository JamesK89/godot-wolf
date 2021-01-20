using Godot;
using System;

namespace Wolf
{
	public struct Point2
	{
		public int x;
		public int y;

		public Point2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public Point2(Vector2 vec)
		{
			this.x = (int)vec.x;
			this.y = (int)vec.y;
		}

		public static Point2 Zero
		{
			get
			{
				return new Point2(0, 0);
			}
		}

		public static implicit operator Vector2(Point2 point) => new Vector2((float)point.x, (float)point.y);
		public static implicit operator Point2(Vector2 vec) => new Point2((int)vec.x, (int)vec.y);

		public static implicit operator Point2((int, int) tuple) => new Point2(tuple.Item1, tuple.Item2);
		public static implicit operator (int, int)(Point2 point) => (point.x, point.y);
	}
}
