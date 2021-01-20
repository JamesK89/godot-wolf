using Godot;
using System;
using System.Collections.Generic;

using CharacterType = Wolf.CharacterBase.CharacterType;

namespace Wolf
{
	public static class CharacterFactory
	{
		private static Dictionary<CharacterType, Type> _charTypeMap = new Dictionary<CharacterType, Type>()
		{
			{ CharacterType.Player_North, typeof(CharacterPlayer) },
			{ CharacterType.Player_East, typeof(CharacterPlayer) },
			{ CharacterType.Player_South, typeof(CharacterPlayer) },
			{ CharacterType.Player_West, typeof(CharacterPlayer) },
		};

        public static CharacterBase CreateCharacter(int x, int y, Level level)
        {
            CharacterBase ret = null;

            CharacterType type = (CharacterType)level.Map.Planes[(int)Level.Planes.Objects][y, x];

            if (_charTypeMap.ContainsKey(type))
            {
                ret = (CharacterBase)Activator.CreateInstance(
                    _charTypeMap[type], new object[] { x, y, level });
            }

            return ret;
        }
    }
}
