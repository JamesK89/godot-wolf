using Godot;
using System;
using Fig;
using System.Linq;
using System.Collections.Generic;

namespace Wolf.Scripts
{
	public static class Configuration
	{
		public const string FileName = @"game.cfg";

		private static Config _config;

		static Configuration()
		{
			if (_config == null)
			{
				_config = new Config();
				_config.ResolveLiteral += ResolveLiteral;

				Files = new FilesConfig(_config.Root);
				Assets = new AssetsConfig(_config.Root);

				if (System.IO.File.Exists(FileName))
				{
					_config.Load(FileName);
				}
			}
		}

		public static FilesConfig Files
		{
			get;
			private set;
		}

		public static AssetsConfig Assets
		{
			get;
			private set;
		}

		private static void ResolveLiteral(
			object sender,
			ResolveLiteralEventArgs e)
		{
			if (GetValueForLiteral(e.Value, out string value))
			{
				e.Variable.Set(value);
				e.Success = true;
			}
		}

		private static bool GetValueForLiteral(
			string name,
			out string value)
		{
			bool result = false;

			value = string.Empty;

			if (string.Equals(
					name, Boolean.TrueString,
					StringComparison.OrdinalIgnoreCase))
			{
				value = Boolean.TrueString;
				result = true;
			}
			else if (string.Equals(
					name,
					Boolean.FalseString,
					StringComparison.OrdinalIgnoreCase))
			{
				value = Boolean.FalseString;
				result = true;
			}
			else if (string.Equals(
					name,
					"Yes",
					StringComparison.OrdinalIgnoreCase))
			{
				value = Boolean.TrueString;
				result = true;
			}
			else if (string.Equals(
					name,
					"No",
					StringComparison.OrdinalIgnoreCase))
			{
				value = Boolean.FalseString;
				result = true;
			}
			else if (string.Equals(
					name,
					"Y",
					StringComparison.OrdinalIgnoreCase))
			{
				value = Boolean.TrueString;
				result = true;
			}
			else if (string.Equals(
					name,
					"N",
					StringComparison.OrdinalIgnoreCase))
			{
				value = Boolean.FalseString;
				result = true;
			}

			return result;
		}
	}
}