using Godot;
using System;
using Fig;
using System.Linq;
using System.Collections.Generic;

namespace Wolf
{
	public class FilesConfig
	{
		public FilesConfig(Section parent)
		{
			Section = parent.AddChild("Files");

			AUDIOHEAD =
				Section.AddVariable("AUDIOHEAD").Set(@"DATA\AUDIOHEAD.WL1");

			AUDIOT =
				Section.AddVariable("AUDIOT").Set(@"DATA\AUDIOT.WL1");

			CONFIG =
				Section.AddVariable("CONFIG").Set(@"DATA\CONFIG.WL1");

			VGAHEAD =
				Section.AddVariable("VGAHEAD").Set(@"DATA\VGAHEAD.WL1");

			VGADICT =
				Section.AddVariable("VGADICT").Set(@"DATA\VGADICT.WL1");

			VGAGRAPH =
				Section.AddVariable("VGAGRAPH").Set(@"DATA\VGAGRAPH.WL1");

			VSWAP =
				Section.AddVariable("VSWAP").Set(@"DATA\VSWAP.WL1");

			MAPHEAD =
				Section.AddVariable("MAPHEAD").Set(@"DATA\MAPHEAD.WL1");

			GAMEMAPS =
				Section.AddVariable("GAMEMAPS").Set(@"DATA\GAMEMAPS.WL1");

			WOLFPAL =
				Section.AddVariable("WOLFPAL").Set(@"DATA\WOLFPAL.INC");
		}

		public Section Section
		{
			get;
			private set;
		}

		public Variable AUDIOHEAD
		{
			get;
			private set;
		}

		public Variable AUDIOT
		{
			get;
			private set;
		}

		public Variable CONFIG
		{
			get;
			private set;
		}

		public Variable VGAHEAD
		{
			get;
			private set;
		}

		public Variable VGADICT
		{
			get;
			private set;
		}

		public Variable VGAGRAPH
		{
			get;
			private set;
		}

		public Variable VSWAP
		{
			get;
			private set;
		}

		public Variable MAPHEAD
		{
			get;
			private set;
		}

		public Variable GAMEMAPS
		{
			get;
			private set;
		}

		public Variable WOLFPAL
		{
			get;
			private set;
		}
	}
}
