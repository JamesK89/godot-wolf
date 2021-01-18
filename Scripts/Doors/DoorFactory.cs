using Godot;
using System;

using DoorType = Wolf.DoorSliding.DoorType;

namespace Wolf
{
	public static class DoorFactory
	{
        public static DoorSliding CreateSlidingDoor(int x, int y, Level level)
        {
            DoorSliding ret = null;

            int rawId = (int)level.Map.Planes[(int)Level.Planes.Walls].Data[x, y];

            if (Enum.IsDefined(typeof(DoorType), rawId))
            {
                ret = new DoorSliding(x, y, level);
            }

            return ret;
        }

        public static DoorSecret CreateSecretDoor(int x, int y, Level level)
        {
            DoorSecret ret = null;

            ushort rawId = level.Map.Planes[(int)Level.Planes.Objects].Data[x, y];

            if (DoorSecret.DoorSecretId == (int)rawId)
            {
                ret = new DoorSecret(x, y, level);
            }

            return ret;
        }
    }
}
