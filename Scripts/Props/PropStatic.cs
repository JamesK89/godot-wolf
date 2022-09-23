using Godot;
using System;

namespace Wolf.Scripts
{
	public class PropStatic : PropBase
	{
		private PropStatic()
			: base(0, 0, null)
        {
            Walkable = false;
        }

		public PropStatic(int x, int y, Level level)
			: base(x, y, level)
		{
			Walkable = false;
		}
	}
}
