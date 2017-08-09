using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.IO;

namespace DMS.OpenGL
{
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using SysDraw = System.Drawing.Imaging;

	public static class TextureLoader
	{
		public static Texture Create(int width, int height, byte components = 4, bool floatingPoint = false)
		{
			Debug.Assert(components < 5);
			PixelInternalFormat internalFormat = PixelInternalFormat.Rgba8;
			PixelFormat inputPixelFormat = PixelFormat.Rgba;
			PixelType type = PixelType.UnsignedByte;
			if(floatingPoint)
			{
				type = PixelType.Float;
				switch(components)
				{
					case 1: internalFormat = PixelInternalFormat.R32f; inputPixelFormat = PixelFormat.Red; break;
					case 2: internalFormat = PixelInternalFormat.Rg32f; inputPixelFormat = PixelFormat.Rg; break;
					case 3: internalFormat = PixelInternalFormat.Rgb32f; inputPixelFormat = PixelFormat.Rgb; break;
				}
			}
			else
			{
				switch (components)
				{
					case 1: internalFormat = PixelInternalFormat.R8; inputPixelFormat = PixelFormat.Red; break;
					case 2: internalFormat = PixelInternalFormat.Rg8; inputPixelFormat = PixelFormat.Rg; break;
					case 3: internalFormat = PixelInternalFormat.Rgb8; inputPixelFormat = PixelFormat.Rgb; break;
				}
			}
			return Texture.Create(width, height, internalFormat, inputPixelFormat, type);
		}


		public static Texture FromArray<TYPE>(TYPE[,] data, PixelInternalFormat internalFormat, PixelFormat format, PixelType type)
		{
			GCHandle pinnedArray = GCHandle.Alloc(data, GCHandleType.Pinned);
			try
			{
				IntPtr pointer = pinnedArray.AddrOfPinnedObject();
				var width = data.GetLength(0);
				var height = data.GetLength(1);
				Texture texture = new Texture();
				texture.FilterMipmap();
				texture.Activate();
				texture.LoadPixels(pointer, width, height, internalFormat, format, type);
				texture.Deactivate();
				return texture;
			}
			finally
			{
				pinnedArray.Free();
			}
		}

		public static Texture FromBitmap(Bitmap bitmap)
		{
			Texture texture = new Texture();
			texture.FilterMipmap();
			texture.Activate();
			//todo: 16bit channels
			using (Bitmap bmp = new Bitmap(bitmap))
			{
				bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
				var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), SysDraw.ImageLockMode.ReadOnly, bmp.PixelFormat);
				PixelInternalFormat internalFormat = SelectInternalPixelFormat(bmp.PixelFormat);
				OpenTK.Graphics.OpenGL.PixelFormat inputPixelFormat = SelectPixelFormat(bmp.PixelFormat);
				texture.LoadPixels(bmpData.Scan0, bmpData.Width, bmpData.Height, internalFormat, inputPixelFormat, PixelType.UnsignedByte);
				bmp.UnlockBits(bmpData);
			}
			texture.Deactivate();
			return texture;
		}

		public static Texture FromFile(string fileName)
		{
			if (String.IsNullOrEmpty(fileName))
			{
				throw new ArgumentException(fileName);
			}
			if (!File.Exists(fileName))
			{
				throw new FileLoadException(fileName);
			}
			return FromBitmap(new Bitmap(fileName));
		}

		public static void SaveToFile(Texture texture, string fileName, SysDraw.PixelFormat format = SysDraw.PixelFormat.Format32bppArgb)
		{
			using (var bitmap = SaveToBitmap(texture, format))
			{
				bitmap.Save(fileName);
			}
		}

		public static Bitmap SaveToBitmap(Texture texture, SysDraw.PixelFormat format = SysDraw.PixelFormat.Format32bppArgb)
		{
			try
			{ 
				var bmp = new Bitmap(texture.Width, texture.Height);
				texture.Activate();
				var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), SysDraw.ImageLockMode.WriteOnly, format);
				GL.GetTexImage(TextureTarget.Texture2D, 0, SelectPixelFormat(format), PixelType.UnsignedByte, data.Scan0);
				bmp.UnlockBits(data);
				texture.Deactivate();
				bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
				return bmp;
			}
			catch
			{
				texture.Deactivate();
				return null;
			}
		}

		public static PixelFormat SelectPixelFormat(SysDraw.PixelFormat pixelFormat)
		{
			switch (pixelFormat)
			{
				case SysDraw.PixelFormat.Format8bppIndexed: return PixelFormat.Red;
				case SysDraw.PixelFormat.Format24bppRgb: return PixelFormat.Bgr;
				case SysDraw.PixelFormat.Format32bppArgb: return PixelFormat.Bgra;
				default: throw new FileLoadException("Wrong pixel format " + pixelFormat.ToString());
			}
		}

		public static PixelInternalFormat SelectInternalPixelFormat(SysDraw.PixelFormat pixelFormat)
		{
			switch (pixelFormat)
			{
				case SysDraw.PixelFormat.Format8bppIndexed: return PixelInternalFormat.Luminance;
				case SysDraw.PixelFormat.Format24bppRgb: return PixelInternalFormat.Rgb;
				case SysDraw.PixelFormat.Format32bppArgb: return PixelInternalFormat.Rgba;
				default: throw new FileLoadException("Wrong pixel format " + pixelFormat.ToString());
			}
		}
	}
}
