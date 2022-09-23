using Godot;
using System;
using Fig;
using System.Linq;
using System.Collections.Generic;

namespace Wolf.Scripts
{
	public class AssetsConfig
	{
		public AssetsConfig(Section parent)
		{
			Section = parent.AddChild("Assets");

			wolfread.VGAGRAPH.Definitions defs =
				wolfread.VGAGRAPH.Definitions.ForWL1();

			TEXTUREWIDTH =
				Section.AddVariable(nameof(TEXTUREWIDTH)).Set(64);
			TEXTUREHEIGHT =
				Section.AddVariable(nameof(TEXTUREHEIGHT)).Set(64);
			SPRITEWIDTH =
				Section.AddVariable(nameof(SPRITEWIDTH)).Set(64);
			SPRITEHEIGHT =
				Section.AddVariable(nameof(SPRITEHEIGHT)).Set(64);
			NUMMAPS =
				Section.AddVariable(nameof(NUMMAPS)).Set(100);
			BLOCK =
				Section.AddVariable(nameof(BLOCK)).Set(defs.BLOCK);
			MASKBLOCK =
				Section.AddVariable(nameof(MASKBLOCK)).Set(defs.MASKBLOCK);
			NUMCHUNKS =
				Section.AddVariable(nameof(NUMCHUNKS)).Set(defs.NUMCHUNKS);
			NUMFONT =
				Section.AddVariable(nameof(NUMFONT)).Set(defs.NUMFONT);
			NUMFONTM =
				Section.AddVariable(nameof(NUMFONTM)).Set(defs.NUMFONTM);
			NUMPICS =
				Section.AddVariable(nameof(NUMPICS)).Set(defs.NUMPICS);
			NUMPICM =
				Section.AddVariable(nameof(NUMPICM)).Set(defs.NUMPICM);
			NUMSPRITES =
				Section.AddVariable(nameof(NUMSPRITES)).Set(defs.NUMSPRITES);
			NUMTILE8 =
				Section.AddVariable(nameof(NUMTILE8)).Set(defs.NUMTILE8);
			NUMTILE8M =
				Section.AddVariable(nameof(NUMTILE8M)).Set(defs.NUMTILE8M);
			NUMTILE16 =
				Section.AddVariable(nameof(NUMTILE16)).Set(defs.NUMTILE16);
			NUMTILE16M =
				Section.AddVariable(nameof(NUMTILE16M)).Set(defs.NUMTILE16M);
			NUMTILE32 =
				Section.AddVariable(nameof(NUMTILE32)).Set(defs.NUMTILE32);
			NUMTILE32M =
				Section.AddVariable(nameof(NUMTILE32M)).Set(defs.NUMTILE32M);
			NUMEXTERNS =
				Section.AddVariable(nameof(NUMEXTERNS)).Set(defs.NUMEXTERNS);
			STRUCTPIC =
				Section.AddVariable(nameof(STRUCTPIC)).Set(defs.STRUCTPIC);
			STARTFONT =
				Section.AddVariable(nameof(STARTFONT)).Set(defs.STARTFONT);
			STARTFONTM =
				Section.AddVariable(nameof(STARTFONTM)).Set(defs.STARTFONTM);
			STARTPICS =
				Section.AddVariable(nameof(STARTPICS)).Set(defs.STARTPICS);
			STARTPICM =
				Section.AddVariable(nameof(STARTPICM)).Set(defs.STARTPICM);
			STARTSPRITES =
				Section.AddVariable(nameof(STARTSPRITES)).Set(defs.STARTSPRITES);
			STARTTILE8 =
				Section.AddVariable(nameof(STARTTILE8)).Set(defs.STARTTILE8);
			STARTTILE8M =
				Section.AddVariable(nameof(STARTTILE8M)).Set(defs.STARTTILE8M);
			STARTTILE16 =
				Section.AddVariable(nameof(STARTTILE16)).Set(defs.STARTTILE16);
			STARTTILE16M =
				Section.AddVariable(nameof(STARTTILE16M)).Set(defs.STARTTILE16M);
			STARTTILE32 =
				Section.AddVariable(nameof(STARTTILE32)).Set(defs.STARTTILE32);
			STARTTILE32M =
				Section.AddVariable(nameof(STARTTILE32M)).Set(defs.STARTTILE32M);
			STARTEXTERNS =
				Section.AddVariable(nameof(STARTEXTERNS)).Set(defs.STARTEXTERNS);
		}

		public Section Section
		{
			get;
			private set;
		}

		public wolfread.VGAGRAPH.Definitions GetDefinitions()
		{
			wolfread.VGAGRAPH.Definitions defs = new wolfread.VGAGRAPH.Definitions();

			defs.VGAHEAD = Configuration.Files.VGAHEAD;
			defs.VGAGRAPH = Configuration.Files.VGAGRAPH;
			defs.VGADICT = Configuration.Files.VGADICT;

			defs.BLOCK = BLOCK;
			defs.MASKBLOCK = MASKBLOCK;
			defs.NUMCHUNKS = NUMCHUNKS;
			defs.NUMFONT = NUMFONT;
			defs.NUMFONTM = NUMFONTM;
			defs.NUMPICS = NUMPICS;
			defs.NUMPICM = NUMPICM;
			defs.NUMSPRITES = NUMSPRITES;
			defs.NUMTILE8 = NUMTILE8;
			defs.NUMTILE8M = NUMTILE8M;
			defs.NUMTILE16 = NUMTILE16;
			defs.NUMTILE16M = NUMTILE16M;
			defs.NUMTILE32 = NUMTILE32;
			defs.NUMTILE32M = NUMTILE32M;
			defs.NUMEXTERNS = NUMEXTERNS;
			defs.STRUCTPIC = STRUCTPIC;
			defs.STARTFONT = STARTFONT;
			defs.STARTFONTM = STARTFONTM;
			defs.STARTPICS = STARTPICS;
			defs.STARTPICM = STARTPICM;
			defs.STARTSPRITES = STARTSPRITES;
			defs.STARTTILE8 = STARTTILE8;
			defs.STARTTILE8M = STARTTILE8M;
			defs.STARTTILE16 = STARTTILE16;
			defs.STARTTILE16M = STARTTILE16M;
			defs.STARTTILE32 = STARTTILE32;
			defs.STARTTILE32M = STARTTILE32M;
			defs.STARTEXTERNS = STARTEXTERNS;

			return defs;
		}

		public Variable TEXTUREWIDTH
		{
			get;
			private set;
		}
		public Variable TEXTUREHEIGHT
		{
			get;
			private set;
		}

		public Variable SPRITEWIDTH
		{
			get;
			private set;
		}

		public Variable SPRITEHEIGHT
		{
			get;
			private set;
		}

		public Variable NUMMAPS
		{
			get;
			private set;
		}

		public Variable BLOCK
		{
			get;
			private set;
		}

		public Variable MASKBLOCK
		{
			get;
			private set;
		}

		public Variable NUMCHUNKS
		{
			get;
			private set;
		}

		public Variable NUMFONT
		{
			get;
			private set;
		}

		public Variable NUMFONTM
		{
			get;
			private set;
		}

		public Variable NUMPICS
		{
			get;
			private set;
		}

		public Variable NUMPICM
		{
			get;
			private set;
		}

		public Variable NUMSPRITES
		{
			get;
			private set;
		}

		public Variable NUMTILE8
		{
			get;
			private set;
		}

		public Variable NUMTILE8M
		{
			get;
			private set;
		}

		public Variable NUMTILE16
		{
			get;
			private set;
		}

		public Variable NUMTILE16M
		{
			get;
			private set;
		}

		public Variable NUMTILE32
		{
			get;
			private set;
		}

		public Variable NUMTILE32M
		{
			get;
			private set;
		}

		public Variable NUMEXTERNS
		{
			get;
			private set;
		}

		public Variable STRUCTPIC
		{
			get;
			private set;
		}

		public Variable STARTFONT
		{
			get;
			private set;
		}

		public Variable STARTFONTM
		{
			get;
			private set;
		}

		public Variable STARTPICS
		{
			get;
			private set;
		}

		public Variable STARTPICM
		{
			get;
			private set;
		}

		public Variable STARTSPRITES
		{
			get;
			private set;
		}

		public Variable STARTTILE8
		{
			get;
			private set;
		}

		public Variable STARTTILE8M
		{
			get;
			private set;
		}

		public Variable STARTTILE16
		{
			get;
			private set;
		}

		public Variable STARTTILE16M
		{
			get;
			private set;
		}

		public Variable STARTTILE32
		{
			get;
			private set;
		}

		public Variable STARTTILE32M
		{
			get;
			private set;
		}

		public Variable STARTEXTERNS
		{
			get;
			private set;
		}
	}
}
