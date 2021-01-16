using Godot;
using System;
using System.Collections.Generic;

namespace Wolf
{
    public class DoorSecret : MeshInstance
    {
        public const int DoorSecretId = 98;

        private DoorSecret()
        {
        }

        public DoorSecret(int x, int y, Level level)
        {
            Location = (x, y);
            Level = level;
        }

        public RigidBody Body
        {
            get;
            protected set;
        }

        public Level Level
        {
            get;
            protected set;
        }

        public (int X, int Y) Location
        {
            get;
            protected set;
        }
    }
}