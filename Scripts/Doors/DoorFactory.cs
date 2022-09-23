using Godot;
using System;

using DoorType = Wolf.Scripts.DoorSliding.DoorType;

namespace Wolf.Scripts
{
	public static class DoorFactory
	{
        public static DoorSliding CreateSlidingDoor(int x, int y, Level level)
        {
            DoorSliding ret = null;

            Level.Cell cell = level.Cells[y, x];

            if (Enum.IsDefined(typeof(DoorType), cell.Wall))
            {
                ret = new DoorSliding(x, y, level);
            }

            return ret;
        }

        public static DoorSecret CreateSecretDoor(int x, int y, Level level)
        {
            DoorSecret ret = null;

            Level.Cell cell = level.Cells[y, x];

            if (DoorSecret.DoorSecretId == cell.Object &&
                cell.Wall >= Level.MapWallBegin && cell.Wall <= Level.MapWallEnd)
            {
                ret = new DoorSecret(x, y, level);
            }

            return ret;
        }
    }
}
