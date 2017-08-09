using System;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;
using DMS.Base;

namespace DMS.OpenGL
{
	[Serializable]
	public class VAOException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VAOException"/> class.
		/// </summary>
		/// <param name="msg">The error msg.</param>
		public VAOException(string msg) : base(msg) { }
	}

	public class VAO : Disposable
	{
		public VAO()
		{
			idVAO = GL.GenVertexArray();
		}

		public int IDLength { get; private set; } = 0;
		public PrimitiveType PrimitiveType { get; set; } = PrimitiveType.Triangles;
		public DrawElementsType DrawElementsType { get; private set; } = DrawElementsType.UnsignedShort;

		public void SetID<Index>(Index[] data) where Index : struct
		{
			if (ReferenceEquals(null, data)) return;
			if (0 == data.Length) return;
			Activate();
			var buffer = RequestBuffer(idBufferBinding, BufferTarget.ElementArrayBuffer);
			// set buffer data
			buffer.Set(data, BufferUsageHint.StaticDraw);
			//activate for state
			buffer.Activate();
			//cleanup state
			Deactivate();
			buffer.Deactivate();
			//save data for draw call
			DrawElementsType drawElementsType = GetDrawElementsType(typeof(Index));
			IDLength = data.Length;
			DrawElementsType = drawElementsType;
		}

		public void SetAttribute<DataElement>(int bindingID, DataElement[] data, VertexAttribPointerType type, int elementSize, bool perInstance = false) where DataElement : struct
		{
			if (-1 == bindingID) return; //if attribute not used in shader or wrong name
			Activate();
			var buffer = RequestBuffer(bindingID, BufferTarget.ArrayBuffer);
			buffer.Set(data, BufferUsageHint.StaticDraw);
			//activate for state
			buffer.Activate();
			//set data format
			int elementBytes = Marshal.SizeOf(typeof(DataElement));
			GL.VertexAttribPointer(bindingID, elementSize, type, false, elementBytes, 0);
			GL.EnableVertexAttribArray(bindingID);
			if (perInstance)
			{
				GL.VertexAttribDivisor(bindingID, 1);
			}
			//cleanup state
			Deactivate();
			buffer.Deactivate();
			GL.DisableVertexAttribArray(bindingID);
		}

		/// <summary>
		/// sets or updates a vertex attribute of type Matrix4
		/// Matrix4 is stored row-major, but OpenGL expects data to be column-major, so the Matrix4 inputs become transposed in the shader
		/// </summary>
		/// <param name="bindingID">shader binding location</param>
		/// <param name="data">array of Matrix4 inputs</param>
		/// <param name="perInstance"></param>
		public void SetAttribute(int bindingID, Matrix4[] data, bool perInstance = false)
		{
			if (-1 == bindingID) return; //if matrix not used in shader or wrong name
			Activate();
			var buffer = RequestBuffer(bindingID, BufferTarget.ArrayBuffer);
			// set buffer data
			buffer.Set(data, BufferUsageHint.StaticDraw);
			//activate for state
			buffer.Activate();
			//set data format
			int columnBytes = Marshal.SizeOf(typeof(Vector4));
			int elementBytes = Marshal.SizeOf(typeof(Matrix4));
			for (int i = 0; i < 4; i++)
			{
				GL.VertexAttribPointer(bindingID + i, 4, VertexAttribPointerType.Float, false, elementBytes, columnBytes * i);
				GL.EnableVertexAttribArray(bindingID + i);
				if (perInstance)
				{
					GL.VertexAttribDivisor(bindingID + i, 1);
				}
			}
			//cleanup state
			Deactivate();
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			for (int i = 0; i < 4; i++)
			{
				GL.DisableVertexAttribArray(bindingID + i);
			}
		}

		public void Activate()
		{
			GL.BindVertexArray(idVAO);
		}

		public void Deactivate()
		{
			GL.BindVertexArray(0);
		}

		public void DrawArrays(PrimitiveType type, int count, int start = 0)
		{
			Activate();
			GL.DrawArrays(type, start, count);
			Deactivate();
		}

		public void Draw(int instanceCount = 1)
		{
			if (0 == IDLength) throw new VAOException("Empty id data set! Draw yourself using active/deactivate!");
			Activate();
			GL.DrawElementsInstanced(PrimitiveType, IDLength, DrawElementsType, (IntPtr)0, instanceCount);
			Deactivate();
		}

		private int idVAO;
		private const int idBufferBinding = int.MaxValue;
		private Dictionary<int, BufferObject> boundBuffers = new Dictionary<int, BufferObject>();

		private static DrawElementsType GetDrawElementsType(Type type)
		{
			if (type == typeof(byte)) return DrawElementsType.UnsignedByte;
			if (type == typeof(ushort)) return DrawElementsType.UnsignedShort;
			if (type == typeof(uint)) return DrawElementsType.UnsignedInt;
			throw new VAOException("Invalid index type");
		}
		
		private BufferObject RequestBuffer(int bindingID, BufferTarget bufferTarget)
		{
			BufferObject buffer;
			if (!boundBuffers.TryGetValue(bindingID, out buffer))
			{
				buffer = new BufferObject(bufferTarget);
				boundBuffers[bindingID] = buffer;
			}
			return buffer;
		}

		protected override void DisposeResources()
		{
			foreach (var buffer in boundBuffers.Values)
			{
				buffer.Dispose();
			}
			boundBuffers.Clear();
			GL.DeleteVertexArray(idVAO);
			idVAO = 0;
		}
	}
}
