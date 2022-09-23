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

		public override int GetHashCode()
		{
			int result = 3583;

			result = result * 7907 + x;
			result = result * 7907 + y;

			return result;
		}

		public override bool Equals(object obj)
		{
			return (obj != null &&
				obj.GetType() == this.GetType() &&
				x == ((Point2)obj).x &&
				y == ((Point2)obj).y);
		}

		public override string ToString()
		{
			return $"({x}, {y}))";
		}

		public static Point2 operator + (Point2 a, Point2 b) => new Point2(a.x + b.x, a.y + b.y);
        public static Point2 operator - (Point2 a, Point2 b) => new Point2(a.x - b.x, a.y - b.y);
        public static Point2 operator * (Point2 a, Point2 b) => new Point2(a.x * b.x, a.y * b.y);
        public static Point2 operator / (Point2 a, Point2 b) => new Point2(a.x / b.x, a.y / b.y);

		public static bool operator == (Point2 a, Point2 b) => (a.x == b.x) && (a.y == b.y);
		public static bool operator != (Point2 a, Point2 b) => (a.x != b.x) && (a.y != b.y);

        public static implicit operator Vector2(Point2 point) => new Vector2((float)point.x, (float)point.y);
		public static implicit operator Point2(Vector2 vec) => new Point2((int)vec.x, (int)vec.y);

		public static implicit operator Point2((int, int) tuple) => new Point2(tuple.Item1, tuple.Item2);
		public static implicit operator (int, int)(Point2 point) => (point.x, point.y);
	}
}
