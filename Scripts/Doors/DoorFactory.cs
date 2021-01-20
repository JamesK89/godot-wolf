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

            int wallId = (int)level.Map.Planes[(int)Level.Planes.Walls][y, x];

            if (Enum.IsDefined(typeof(DoorType), wallId))
            {
                ret = new DoorSliding(x, y, level);
            }

            return ret;
        }

        public static DoorSecret CreateSecretDoor(int x, int y, Level level)
        {
            DoorSecret ret = null;

            ushort objId = level.Map.Planes[(int)Level.Planes.Objects][y, x];
            ushort wallId = level.Map.Planes[(int)Level.Planes.Walls][y, x];

            if (DoorSecret.DoorSecretId == (int)objId &&
                wallId >= Level.MapWallBegin && wallId <= Level.MapWallEnd)
            {
                ret = new DoorSecret(x, y, level);
            }

            return ret;
        }
    }
}
