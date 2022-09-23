using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wolf.Scripts.Doors
{
    public class DoorBase : Spatial
    {
        public virtual void Open()
        {
        }

        public virtual void Close()
        {
        }
    }
}
