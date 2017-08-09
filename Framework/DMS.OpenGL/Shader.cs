using System;
using OpenTK.Graphics.OpenGL;
using DMS.Base;

namespace DMS.OpenGL
{
	/// <summary>
	/// Shader class
	/// </summary>
	/// todo: rename to ShaderProgram and create Shader classes to compile individual (fragment, vertex, ...) shaders
	public class Shader : Disposable
	{
		public bool IsLinked { get; private set; } = false;

		public string LastLog { get; private set; }

		public int ProgramID { get; private set; } = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="Shader"/> class.
		/// </summary>
		public Shader()
		{
			ProgramID = GL.CreateProgram();
		}

		public void Compile(string sShader, ShaderType type)
		{
			IsLinked = false;
			int shaderObject = GL.CreateShader(type);
			if (0 == shaderObject) throw new ShaderCompileException(type, "Could not create " + type.ToString() + " object", string.Empty, sShader);
			// Compile vertex shader
			GL.ShaderSource(shaderObject, sShader);
			GL.CompileShader(shaderObject);
			int status_code;
			GL.GetShader(shaderObject, ShaderParameter.CompileStatus, out status_code);
			LastLog = GL.GetShaderInfoLog(shaderObject);
			if (1 != status_code)
			{
				GL.DeleteShader(shaderObject);
				throw new ShaderCompileException(type, "Error compiling  " + type.ToString(), LastLog, sShader);
			}
			GL.AttachShader(ProgramID, shaderObject);
			//shaderIDs.Add(shaderObject);
		}

		/// <summary>
		/// Begins this shader use.
		/// </summary>
		public void Activate()
		{
			GL.UseProgram(ProgramID);
		}

		/// <summary>
		/// Ends this shader use.
		/// </summary>
		public void Deactivate()
		{
			GL.UseProgram(0);
		}

		public int GetAttributeLocation(string name)
		{
			return GL.GetAttribLocation(ProgramID, name);
		}

		public int GetUniformLocation(string name)
		{
			return GL.GetUniformLocation(ProgramID, name);
			//return GL.GetProgramResourceIndex(ProgramID, ProgramInterface.Uniform, name); //alternative
		}

		public int GetShaderStorageBufferBindingIndex(string name)
		{
			return GL.GetProgramResourceIndex(ProgramID, ProgramInterface.ShaderStorageBlock, name);
		}

		public int GetResourceIndex(string name, ProgramInterface type)
		{
			return GL.GetProgramResourceIndex(ProgramID, type, name);
		}

		public int GetUniformBufferBindingIndex(string name)
		{
			return GL.GetProgramResourceIndex(ProgramID, ProgramInterface.UniformBlock, name);
		}

		public void Link()
		{
			try
			{
				GL.LinkProgram(ProgramID);
			}
			catch (Exception)
			{
				throw new ShaderException("Unknown Link error!", string.Empty);
			}
			int status_code;
			GL.GetProgram(ProgramID, GetProgramParameterName.LinkStatus, out status_code);
			if (1 != status_code)
			{
				throw new ShaderException("Error linking shader", GL.GetProgramInfoLog(ProgramID));
			}
			IsLinked = true;
		}

		protected override void DisposeResources()
		{
			if (0 != ProgramID)
			{
				GL.DeleteProgram(ProgramID);
			}
		}

		//private List<int> shaderIDs = new List<int>();

		//private void DetachShaders()
		//{
		//	foreach (int id in shaderIDs)
		//	{
		//		GL.DetachShader(m_ProgramID, id);
		//	}
		//	shaderIDs.Clear();
		//}
	}
}
