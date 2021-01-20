﻿using Godot;
using System;

namespace Wolf
{
	public class PropBlock : PropBase
	{
		private PropBlock()
			: base(0, 0, null)
		{
		}

		public PropBlock(int x, int y, Level level)
			: base(x, y, level)
		{
			if (level != null)
			{
				CollisionShape shape = new CollisionShape();
				BoxShape box = new BoxShape();
				box.Extents = new Vector3(Level.CellSize * 0.5f, Level.CellSize * 0.5f, Level.CellSize * 0.5f);
				shape.Shape = box;

				Body = new StaticBody();
				Body.CollisionLayer = (uint)Level.CollisionLayers.Static;
				Body.CollisionMask = (uint)Level.CollisionLayers.Characters;
				Body.AddChild(shape);

				AddChild(Body);
			}
		}

		public StaticBody Body
		{
			get;
			protected set;
		}
	}
}
