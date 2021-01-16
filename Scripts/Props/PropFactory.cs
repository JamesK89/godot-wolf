using Godot;
using System;
using System.Reflection;
using System.Collections.Generic;

using PropType = Wolf.PropBase.PropType;

namespace Wolf
{
	public static class PropFactory
	{
		private static Dictionary<PropType, Type> _propTypeMap = new Dictionary<PropType, Type>()
		{
            /* Static Props */
            {PropType.Puddle, typeof(PropStatic)},
            {PropType.Chandelier, typeof(PropStatic)},
            {PropType.Skeleton, typeof(PropStatic)},
            {PropType.CeilingLight, typeof(PropStatic)},
            {PropType.PotsAndPans, typeof(PropStatic)},
            {PropType.PileOfBones, typeof(PropStatic)},
            {PropType.Pots, typeof(PropStatic)},
            {PropType.Vines, typeof(PropStatic)},
            {PropType.Basket, typeof(PropStatic)},
            {PropType.Junk1, typeof(PropStatic)},
            {PropType.Junk2, typeof(PropStatic)},
            {PropType.Junk3, typeof(PropStatic)},
            {PropType.Gibs, typeof(PropStatic)},
            {PropType.Blood, typeof(PropStatic)},

            /* Pickup Props */
            {PropType.DogFood, typeof(PropPickup)},
            {PropType.KeyGold, typeof(PropPickup)},
            {PropType.KeySilver, typeof(PropPickup)},
            {PropType.Food, typeof(PropPickup)},
            {PropType.FirstAid, typeof(PropPickup)},
            {PropType.Magazine, typeof(PropPickup)},
            {PropType.MachineGun, typeof(PropPickup)},
            {PropType.GatlingGun, typeof(PropPickup)},
            {PropType.Cross, typeof(PropPickup)},
            {PropType.Chalice, typeof(PropPickup)},
            {PropType.Chest, typeof(PropPickup)},
            {PropType.Crown, typeof(PropPickup)},
            {PropType.OneUp, typeof(PropPickup)},

            /* Blocking Props */
            {PropType.Lamp, typeof(PropBlock)},
            {PropType.MetalBarrel, typeof(PropBlock)},
            {PropType.TableWithChairs, typeof(PropBlock)},
            {PropType.HangedMan, typeof(PropBlock)},
            {PropType.Pillar, typeof(PropBlock)},
            {PropType.Tree, typeof(PropBlock)},
            {PropType.Sink, typeof(PropBlock)},
            {PropType.Plant, typeof(PropBlock)},
            {PropType.Urn, typeof(PropBlock)},
            {PropType.EmptyTable, typeof(PropBlock)},
            {PropType.Armor, typeof(PropBlock)},
            {PropType.Cage, typeof(PropBlock)},
            {PropType.SkeletonInCage, typeof(PropBlock)},
            {PropType.Bed, typeof(PropBlock)},
            {PropType.WoodBarrel, typeof(PropBlock)},
            {PropType.Well, typeof(PropBlock)},
            {PropType.EmptyWell, typeof(PropBlock)},
            {PropType.Flag, typeof(PropBlock)},
            {PropType.Stove, typeof(PropBlock)},
            {PropType.Spears, typeof(PropBlock)},
            {PropType.Apogee, typeof(PropBlock)}
        };

        public static PropBase CreateProp(int x, int y, Level level)
        {
            PropBase ret = null;

            PropType type = (PropType)level.Map.Planes[1].Data[x, y];

            if (_propTypeMap.ContainsKey(type))
            {
                ret = (PropBase)Activator.CreateInstance(
                    _propTypeMap[type], new object[] { x, y, level });
            }

            return ret;
        }
	}
}
