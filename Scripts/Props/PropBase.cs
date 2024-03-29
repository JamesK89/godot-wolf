﻿using Godot;
using System;
using System.Collections.Generic;

namespace Wolf.Scripts
{
	public abstract class PropBase : Sprite3D
	{
		private Level _level;

		public enum PropType : int
		{
            Puddle = 23,
            MetalBarrel = 24,
            TableWithChairs = 25,
            Lamp = 26,
            Chandelier = 27,
            HangedMan = 28,
            DogFood = 29,
            Pillar = 30,
            Tree = 31,
            Skeleton = 32,
            Sink = 33,
            Plant = 34,
            Urn = 35,
            EmptyTable = 36,
            CeilingLight = 37,
            PotsAndPans = 38,
            Armor = 39,
            Cage = 40,
            SkeletonInCage = 41,
            PileOfBones = 42,
            KeyGold = 43,
            KeySilver = 44,
            Bed = 45,
            Basket = 46,
            Food = 47,
            FirstAid = 48,
            Magazine = 49,
            MachineGun = 50,
            GatlingGun = 51,
            Cross = 52,
            Chalice = 53,
            Chest = 54,
            Crown = 55,
            OneUp = 56,
            Gibs = 57,
            WoodBarrel = 58,
            Well = 59,
            EmptyWell = 60,
            Blood = 61,
            Flag = 62,
            Apogee = 63,
            Junk1 = 64,
            Junk2 = 65,
            Junk3 = 66,
            Pots = 67,
            Stove = 68,
            Spears = 69,
            Vines = 70,
            DeadGuard = 124
        }

        protected static Dictionary<PropType, int> PropSpriteIndices = new Dictionary<PropType, int>()
        {
            {PropType.Puddle, 2},
            {PropType.MetalBarrel, 3},
            {PropType.TableWithChairs, 4},
            {PropType.Lamp, 5},
            {PropType.Chandelier, 6},
            {PropType.HangedMan, 7},
            {PropType.DogFood, 8},
            {PropType.Pillar, 9},
            {PropType.Tree, 10},
            {PropType.Skeleton, 11},
            {PropType.Sink, 12},
            {PropType.Plant, 13},
            {PropType.Urn, 14},
            {PropType.EmptyTable, 15},
            {PropType.CeilingLight, 16},
            {PropType.PotsAndPans, 17},
            {PropType.Armor, 18},
            {PropType.Cage, 19},
            {PropType.SkeletonInCage, 20},
            {PropType.PileOfBones, 21},
            {PropType.KeyGold, 22},
            {PropType.KeySilver, 23},
            {PropType.Bed, 24},
            {PropType.Basket, 25},
            {PropType.Food, 26},
            {PropType.FirstAid, 27},
            {PropType.Magazine, 28},
            {PropType.MachineGun, 29},
            {PropType.GatlingGun, 30},
            {PropType.Cross, 31},
            {PropType.Chalice, 32},
            {PropType.Chest, 33},
            {PropType.Crown, 34},
            {PropType.OneUp, 35},
            {PropType.Gibs, 36},
            {PropType.WoodBarrel, 37},
            {PropType.Well, 38},
            {PropType.EmptyWell, 39},
            {PropType.Blood, 40},
            {PropType.Flag, 41},
            {PropType.Apogee, 42},
            {PropType.Junk1, 43},
            {PropType.Junk2, 44},
            {PropType.Junk3, 45},
            {PropType.Pots, 46},
            {PropType.Stove, 47},
            {PropType.Spears, 48},
            {PropType.Vines, 49},
            {PropType.DeadGuard, 95}
        };

        private PropBase()
        {
        }

		protected PropBase(int x, int y, Level level)
            : this()
		{
            if (level != null)
            {
                Level = level;
                Type = (PropType)Level.Cells[y, x].Object;
                Location = new Point2(x, y);

                Billboard = SpatialMaterial.BillboardMode.FixedY;
                Texture = (Assets.GetSprite(PropSpriteIndices[Type]) as SpatialMaterial).AlbedoTexture;
                PixelSize = Level.CellSize / (float)Assets.VSWAP.SpriteSize.Height;
                
                Level.AddChild(this);
                Level.Cells[y, x].Nodes.Add(this);

                Transform tform = this.Transform;

                tform.origin = level.MapToWorld(x, y);

                this.Transform = tform;

                Walkable = true;
            }
		}

        public Level Level
        {
            get;
            protected set;
        }

        public PropType Type
        {
            get;
            protected set;
        }

        public Point2 Location
        {
            get;
            protected set;
        }

        public bool Walkable
        {
            get;
            protected set;
        }
	}
}
