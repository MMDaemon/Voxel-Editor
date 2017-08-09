using System;

namespace DMS.OpenGL
{
	public class DoubleBufferedFBO
	{
		public FBO ActiveFBO { get; private set; }
		public FBO InactiveFBO { get; private set; }

		public DoubleBufferedFBO(Func<Texture> creator)
		{
			ActiveFBO = new FBO(creator());
			InactiveFBO = new FBO(creator());
		}

		public void Activate()
		{
			ActiveFBO.Activate();
		}

		public void Deactivate()
		{
			ActiveFBO.Deactivate();
		}

		public void SwapBuffers()
		{
			var temp = ActiveFBO;
			ActiveFBO = InactiveFBO;
			InactiveFBO = ActiveFBO;
		}
	}
}
