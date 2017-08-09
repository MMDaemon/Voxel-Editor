using OpenTK.Graphics.OpenGL;

namespace DMS.OpenGL
{
	public class FBOwithDepth : FBO
	{
		public FBOwithDepth(Texture texture) : base(texture)
		{
			Activate();
			depth = new RenderBuffer(RenderbufferStorage.DepthComponent32, texture.Width, texture.Height);
			depth.Attach(FramebufferAttachment.DepthAttachment);
			Deactivate();
		}

		protected override void DisposeResources()
		{
			base.DisposeResources();
			depth.Dispose();
		}

		private RenderBuffer depth;
	}
}