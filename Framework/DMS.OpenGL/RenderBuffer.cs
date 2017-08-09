using System;
using DMS.Base;
using OpenTK.Graphics.OpenGL;

namespace DMS.OpenGL
{
	public class RenderBuffer : Disposable
	{
		public int Handle { get; private set; } = -1;

		public RenderBuffer(RenderbufferStorage type, int width, int height)
		{
			Handle = GL.GenRenderbuffer();
			Activate();
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, width, height);
			Deactivate();
		}

		public void Activate()
		{
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, Handle);
		}

		public void Attach(FramebufferAttachment attachmentPoint)
		{
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, attachmentPoint, RenderbufferTarget.Renderbuffer, Handle);
		}

		public void Deactivate()
		{
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
		}

		protected override void DisposeResources()
		{
			if (-1 != Handle) GL.DeleteRenderbuffer(Handle);
		}
	}
}
