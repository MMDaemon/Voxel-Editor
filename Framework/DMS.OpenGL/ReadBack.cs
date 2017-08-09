using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace DMS.OpenGL
{
	public class ReadBack
	{
		public static Bitmap FrameBuffer(int x, int y, int width, int height)
		{
			var format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
			var bmp = new Bitmap(width, height);
			BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, format);
			GL.ReadPixels(x, y, width, height, TextureLoader.SelectPixelFormat(format), PixelType.UnsignedByte, data.Scan0);
			bmp.UnlockBits(data);
			bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
			return bmp;
		}

		public static Bitmap FrameBuffer()
		{
			var viewport = new int[4];
			GL.GetInteger(GetPName.Viewport, viewport);
			return FrameBuffer(viewport[0], viewport[1], viewport[2], viewport[3]);
		}
	}
}
