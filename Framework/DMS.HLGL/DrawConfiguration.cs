using DMS.Application;
using DMS.Geometry;
using DMS.OpenGL;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace DMS.HLGL
{
	//todo: make this into a node with typed inputs and outputs
	public class DrawConfiguration
	{
		public bool BackfaceCulling { get; set; } = false;
		public int InstanceCount { get; set; } = 1;
		public bool ShaderPointSize { get; set; } = false;
		public Shader Shader { get; private set; }
		public VAO Vao { get; private set; }
		public bool ZBufferTest { get; set; } = false;

		public void Draw(StateSetGL stateSetGL)
		{
			stateSetGL.BackfaceCulling = BackfaceCulling;
			stateSetGL.ShaderPointSize = ShaderPointSize;
			stateSetGL.ZBufferTest = ZBufferTest;
			stateSetGL.Shader = Shader;

			BindTextures();
			ActivateBuffers();

			var vao = Vao;
			if (ReferenceEquals(null, vao))
			{
				if (1 == InstanceCount)
				{
					GL.DrawArrays(PrimitiveType.Quads, 0, 4); //todo: make this general -> mesh with only vertex count? particle system, sprites
				}
				else
				{
					GL.DrawArrays(PrimitiveType.Points, 0, InstanceCount);
				}
			}
			else
			{
				Vao.Draw(InstanceCount);
			}

			DeactivateBuffers();
			UnbindTextures();

		}

		public void SetInputTexture(string name, Image image)
		{
			textures[name] = image.Texture;
		}

		public void SetInputTexture(string name)
		{
			textures[name] = ResourceManager.Instance.Get<Texture>(name).Value;
		}

		public void UpdateInstanceAttribute(string name, int[] data)
		{
			Vao.SetAttribute(GetAttributeShaderLocationAndCheckVao(name), data, VertexAttribPointerType.Int, 1, true);
		}

		public void UpdateInstanceAttribute(string name, float[] data)
		{
			Vao.SetAttribute(GetAttributeShaderLocationAndCheckVao(name), data, VertexAttribPointerType.Float, 1, true);
		}

		public void UpdateInstanceAttribute(string name, System.Numerics.Vector2[] data)
		{
			Vao.SetAttribute(GetAttributeShaderLocationAndCheckVao(name), data, VertexAttribPointerType.Float, 2, true);
		}

		public void UpdateInstanceAttribute(string name, System.Numerics.Vector3[] data)
		{
			Vao.SetAttribute(GetAttributeShaderLocationAndCheckVao(name), data, VertexAttribPointerType.Float, 3, true);
		}

		public void UpdateInstanceAttribute(string name, System.Numerics.Vector4[] data)
		{
			Vao.SetAttribute(GetAttributeShaderLocationAndCheckVao(name), data, VertexAttribPointerType.Float, 4, true);
		}

		//public void UpdateInstanceAttribute<DATA_ELEMENT_TYPE>(string name, DATA_ELEMENT_TYPE[] data) where DATA_ELEMENT_TYPE : struct
		//{
		//	Vao.SetAttribute(GetAttributeShaderLocationAndCheckVao(name), data, VertexAttribPointerType.Float, 3, true);
		//}

		public void UpdateMeshShader(Mesh mesh, string shaderName)
		{
			if (string.IsNullOrWhiteSpace(shaderName)) throw new ArgumentException("A shaderName is required");
			var resShader = ResourceManager.Instance.Get<Shader>(shaderName);
			if (ReferenceEquals(null, resShader)) throw new ArgumentException("Shader '" + shaderName + "' does not exist");
			Shader = resShader.Value;
			//if (ReferenceEquals(null, mesh)) throw new ArgumentException("A mesh is required");
			Vao = ReferenceEquals(null, mesh) ? null : VAOLoader.FromMesh(mesh, Shader);
		}

		public void UpdateUniforms<DATA>(string name, DATA uniforms) where DATA : struct
		{
			BufferObject buffer;
			if (!buffers.TryGetValue(name, out buffer))
			{
				buffer = new BufferObject(BufferTarget.UniformBuffer);
				buffers.Add(name, buffer);
			}
			buffer.Set(uniforms, BufferUsageHint.StaticRead);
		}

		public void UpdateShaderBuffer<DATA_ELEMENT_TYPE>(string name, DATA_ELEMENT_TYPE[] uniformArray) where DATA_ELEMENT_TYPE : struct
		{
			BufferObject buffer;
			if (!buffers.TryGetValue(name, out buffer))
			{
				buffer = new BufferObject(BufferTarget.ShaderStorageBuffer);
				buffers.Add(name, buffer);
			}
			buffer.Set(uniformArray, BufferUsageHint.StaticCopy);
		}

		private Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
		private Dictionary<string, BufferObject> buffers = new Dictionary<string, BufferObject>();

		private void ActivateBuffers()
		{
			foreach (var uBuffer in buffers)
			{
				var bindingIndex = Shader.GetResourceIndex(uBuffer.Key, uBuffer.Value.Type);
				if (-1 == bindingIndex) throw new ArgumentException("Could not find shader parameters '" + uBuffer.Key + "'");
				uBuffer.Value.ActivateBind(bindingIndex);
			}
		}

		private void DeactivateBuffers()
		{
			foreach (var uBuffer in buffers)
			{
				uBuffer.Value.Deactivate();
			}
		}

		private void BindTextures()
		{
			int id = 0;
			if (ReferenceEquals(null, Shader))
			{
				foreach (var namedTex in textures)
				{
					GL.ActiveTexture(TextureUnit.Texture0 + id);
					namedTex.Value.Activate();
					++id;
				}
			}
			else
			{
				foreach (var namedTex in textures)
				{
					GL.ActiveTexture(TextureUnit.Texture0 + id);
					namedTex.Value.Activate();
					GL.Uniform1(Shader.GetUniformLocation(namedTex.Key), id);
					++id;
				}
			}
		}

		private void UnbindTextures()
		{
			int id = 0;
			foreach (var namedTex in textures)
			{
				GL.ActiveTexture(TextureUnit.Texture0 + id);
				namedTex.Value.Deactivate();
				++id;
			}
			GL.ActiveTexture(TextureUnit.Texture0);
		}

		private int GetAttributeShaderLocationAndCheckVao(string name)
		{
			if (ReferenceEquals(null, Vao)) throw new InvalidOperationException("Specify mesh before setting instance attributes");
			return Shader.GetAttributeLocation(name);
		}
	}
}