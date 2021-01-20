using Godot;
using System;
using wolfread;
using Fig;
using System.Linq;
using System.Collections.Generic;

using WolfPalette = wolfread.Palette;

namespace Wolf
{
	public static class Assets
	{
		public const int SoundSampleRate = 7042;

		public static Color Transparent =
			Color.Color8(152, 0, 136);
		
		private static Dictionary<int, Material> _vswapTexture =
			new Dictionary<int, Material>();

		private static Dictionary<int, Material> _vswapSprite =
			new Dictionary<int, Material>();

		private static Dictionary<int, AudioStreamSample> _vswapSound =
			new Dictionary<int, AudioStreamSample>();

		public enum DigitalSoundList : int
		{
			DoorOpening = 3,
			DoorClosing = 2,
			PushWallActivation = 15
		}

		public static Material GetTexture(int idx)
		{
			Material ret = _vswapTexture.ContainsKey(idx) ?
				_vswapTexture[idx] : null;

			if (ret == null)
			{
				byte[] data = new byte[VSWAP.TextureSize.Width * VSWAP.TextureSize.Height * 4];

				SpatialMaterial mat = new SpatialMaterial();

				ImageTexture tex = new ImageTexture();
				Image img = new Image();

				System.Drawing.Bitmap bmp = VSWAP.GetTextureBitmap(Palette, (uint)idx);

				int stride = VSWAP.TextureSize.Width * 4;

				for (int y = 0; y < VSWAP.TextureSize.Height; y++)
				{
					for (int x = 0; x < VSWAP.TextureSize.Width; x++)
					{
						System.Drawing.Color pixel = bmp.GetPixel(x, y);

						data[(stride * y) + (x * 4) + 0] = pixel.R;
						data[(stride * y) + (x * 4) + 1] = pixel.G;
						data[(stride * y) + (x * 4) + 2] = pixel.B;
						data[(stride * y) + (x * 4) + 3] = 255;
					}
				}

				img.CreateFromData(
					VSWAP.TextureSize.Width,
					VSWAP.TextureSize.Height,
					false, Image.Format.Rgba8, data);

				tex.CreateFromImage(img, (uint)Texture.FlagsEnum.Repeat);

				mat.AlbedoTexture = tex;
				mat.ParamsCullMode = SpatialMaterial.CullMode.Back;

				ret = mat;

				_vswapTexture.Add(idx, ret);
			}

			return ret;
		}

		public static Material GetSprite(int idx)
		{
			Material ret = _vswapSprite.ContainsKey(idx) ?
				_vswapSprite[idx] : null;

			if (ret == null)
			{
				byte[] data = new byte[VSWAP.SpriteSize.Width * VSWAP.SpriteSize.Height * 4];

				SpatialMaterial mat = new SpatialMaterial();

				ImageTexture tex = new ImageTexture();
				Image img = new Image();

				System.Drawing.Bitmap bmp = VSWAP.GetSpriteBitmap(Palette, (uint)idx);

				int stride = VSWAP.SpriteSize.Width * 4;

				for (int y = 0; y < VSWAP.SpriteSize.Height; y++)
				{
					for (int x = 0; x < VSWAP.SpriteSize.Width; x++)
					{
						System.Drawing.Color pixel = bmp.GetPixel(x, y);

						data[(stride * y) + (x * 4) + 0] = pixel.R;
						data[(stride * y) + (x * 4) + 1] = pixel.G;
						data[(stride * y) + (x * 4) + 2] = pixel.B;

						if (pixel.R == Transparent.r8 && 
							pixel.G == Transparent.g8 && 
							pixel.B == Transparent.b8)
						{
							data[(stride * y) + (x * 4) + 3] = 0;
						}
						else
						{
							data[(stride * y) + (x * 4) + 3] = 255;
						}
					}
				}

				img.CreateFromData(
					VSWAP.SpriteSize.Width,
					VSWAP.SpriteSize.Height,
					false, Image.Format.Rgba8, data);

				tex.CreateFromImage(img, (uint)0);

				mat.AlbedoTexture = tex;
				mat.ParamsCullMode = SpatialMaterial.CullMode.Disabled;
				mat.ParamsBillboardMode = SpatialMaterial.BillboardMode.FixedY;

				ret = mat;

				_vswapSprite.Add(idx, ret);
			}

			return ret;
		}

		public static AudioStreamSample GetSoundClip(DigitalSoundList sound)
		{
			return GetSoundClip((int)sound);
		}

		public static AudioStreamSample GetSoundClip(int idx)
		{
			AudioStreamSample result =
				_vswapSound.ContainsKey(idx) ? _vswapSound[idx] : null;

			if (result == null)
			{
				byte[] data = VSWAP.GetSoundData((uint)idx);

				if (data != null && data.Length > 0)
				{
					for (int i = 0; i < data.Length; i++)
					{
						unchecked
						{
							data[i] -= 0x80;
						}
					}

					result = new AudioStreamSample();

					result.Format = AudioStreamSample.FormatEnum.Format8Bits;
					result.MixRate = SoundSampleRate;
					result.LoopMode = AudioStreamSample.LoopModeEnum.Disabled;
					result.Data = data;
				}
			}

			return result;
		}

		public static WolfPalette Palette
		{
			get;
		} = new WolfPalette(Configuration.Files.WOLFPAL);

		public static GAMEMAPS Maps
		{
			get;
		} = new GAMEMAPS(
			Configuration.Assets.NUMMAPS,
			Configuration.Files.MAPHEAD,
			Configuration.Files.GAMEMAPS);

		public static VSWAP VSWAP
		{
			get;
		} = new VSWAP(
			Configuration.Files.VSWAP,
			new System.Drawing.Size(Configuration.Assets.TEXTUREWIDTH, Configuration.Assets.TEXTUREHEIGHT),
			new System.Drawing.Size(Configuration.Assets.SPRITEWIDTH, Configuration.Assets.SPRITEHEIGHT));

		public static VGAGRAPH VGAGRAPH
		{
			get;
		} = new VGAGRAPH(Configuration.Assets.GetDefinitions());
	}
}
