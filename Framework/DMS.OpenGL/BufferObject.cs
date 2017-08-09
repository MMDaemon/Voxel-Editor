using DMS.Base;
using OpenTK.Graphics.OpenGL;
using System;
using System.Runtime.InteropServices;

namespace DMS.OpenGL
{
	public class BufferObject : Disposable
	{
		public BufferObject(BufferTarget bufferTarget)
		{
			BufferTarget = bufferTarget;
			switch(bufferTarget)
			{
				case BufferTarget.UniformBuffer: Type = ProgramInterface.UniformBlock; break;
				case BufferTarget.ShaderStorageBuffer: Type = ProgramInterface.ShaderStorageBlock; break;
			}
			GL.GenBuffers​(1, out bufferID);
		}

		public BufferTarget BufferTarget { get; private set; }
		public ProgramInterface Type { get; private set; }

		public void Activate()
		{
			GL.BindBuffer​(BufferTarget, bufferID);
		}

		public void ActivateBind(int index) //todo: more than one bound buffer is not working, but have different indices; test: glUniformBlockBinding
		{
			Activate();
			BufferRangeTarget target = (BufferRangeTarget)BufferTarget;
			GL.BindBufferBase​(target, index, bufferID);
		}

		public void Deactivate()
		{
			GL.BindBuffer​(BufferTarget, 0);
		}

		public void Set<DATA_ELEMENT_TYPE>(DATA_ELEMENT_TYPE[] data, BufferUsageHint usageHint) where DATA_ELEMENT_TYPE : struct
		{
			Activate();
			int elementBytes = Marshal.SizeOf(typeof(DATA_ELEMENT_TYPE));
			int bufferByteSize = data.Length * elementBytes;
			// set buffer data
			GL.BufferData(BufferTarget, (IntPtr)bufferByteSize, data, usageHint);
			//cleanup state
			Deactivate();
		}

		public void Set<DATA_TYPE>(DATA_TYPE data, BufferUsageHint usageHint) where DATA_TYPE : struct
		{
			Activate();
			var elementBytes = Marshal.SizeOf(typeof(DATA_TYPE));
			// set buffer data
			GL.BufferData(BufferTarget, elementBytes, ref data, usageHint);
			//cleanup state
			Deactivate();
		}

		protected override void DisposeResources()
		{
			if (-1 == bufferID) return;
			GL.DeleteBuffer(bufferID);
			bufferID = -1;
		}

		private int bufferID;
	}
}
