using DMS.OpenGL;
using OpenTK.Graphics.OpenGL;

namespace DMS.HLGL
{
	public class StateSetGL
	{
		public bool BackfaceCulling
		{
			get	{ return backfaceCulling; }
			set
			{
				if (value == backfaceCulling) return;
				backfaceCulling = value;
				if (value) GL.Enable(EnableCap.CullFace); else GL.Disable(EnableCap.CullFace);
			}
		}

		public bool ShaderPointSize
		{
			get { return shaderPointSize; }
			set
			{
				if (value == shaderPointSize) return;
				shaderPointSize = value;
				if (value) GL.Enable(EnableCap.ProgramPointSize); else GL.Disable(EnableCap.ProgramPointSize);
			}
		}

		public FBO Fbo
		{
			get	{ return fbo; }

			set
			{
				if (ReferenceEquals(fbo, value)) return;
				fbo?.Deactivate();
				fbo = value;
				fbo?.Activate();
			}
		}

		//public VAO Vao
		//{
		//	get	{ return vao; }

		//	set
		//	{
		//		if (ReferenceEquals(vao, value)) return;
		//		vao?.Deactivate();
		//		vao = value;
		//		vao?.Activate();
		//	}
		//}

		public Shader Shader
		{
			get	{ return shader; }

			set
			{
				if (ReferenceEquals(shader, value)) return;
				shader?.Deactivate();
				shader = value;
				shader?.Activate();
			}
		}

		public bool ZBufferTest
		{
			get { return zBufferTest; }

			set
			{
				if (value == zBufferTest) return;
				zBufferTest = value;
				if (value) GL.Enable(EnableCap.DepthTest); else GL.Disable(EnableCap.DepthTest);
			}
		}

		private bool backfaceCulling = false;
		private FBO fbo = null;
		//private VAO vao = null;
		private Shader shader = null;
		private bool zBufferTest = false;
		private bool shaderPointSize = false;
	}
}
