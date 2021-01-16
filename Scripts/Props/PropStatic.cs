using Godot;
using System;

namespace Wolf
{
	public class PropStatic : PropBase
	{
		private PropStatic()
			: base(0, 0, null)
		{
		}

		public PropStatic(int x, int y, Level level)
			: base(x, y, level)
		{
		}
	}
}
