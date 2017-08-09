using DMS.Base;
using DMS.OpenGL;
using OpenTK.Graphics.OpenGL;
using System;

namespace DMS.HLGL
{
	//todo: stateSetGL + all gl classes into a manager class that handles dispose, state etc. do not use gl classes directly
	public class Image : Disposable
	{
		public Image(int width, int height, bool hasDepthBuffer = false, byte components = 4, bool floatingPoint = false): this(hasDepthBuffer)
		{
			var tex = TextureLoader.Create(width, height, components, floatingPoint);
			if (hasDepthBuffer)
			{
				fbo = new FBOwithDepth(tex);
			}
			else
			{
				fbo = new FBO(tex);
			}
		}

		public Image(bool hasDepthBuffer = false)
		{
			if (ReferenceEquals(null, stateSetGL)) stateSetGL = new StateSetGL();
			if (hasDepthBuffer)
			{
				actionClear = () => GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			}
			else
			{
				actionClear = () => GL.Clear(ClearBufferMask.ColorBufferBit);
			}
		}

		public Texture Texture { get { return fbo?.Texture; } }

		public void Clear()
		{
			stateSetGL.Fbo = fbo;
			actionClear();
		}

		public void Draw(DrawConfiguration config)
		{
			stateSetGL.Fbo = fbo;
			config.Draw(stateSetGL);
		}

		protected override void DisposeResources()
		{
			if (!ReferenceEquals(null, fbo)) fbo.Dispose();	
		}

		private FBO fbo = null;
		private Action actionClear = null;
		private static StateSetGL stateSetGL = null;
	}
}
