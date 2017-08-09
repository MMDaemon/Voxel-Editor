using System;
using DMS.OpenGL;
using DMS.ShaderDebugging;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace DMS.Application
{
	[Export(typeof(IResourceProvider))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class ResourceManager : IShaderProvider, IResourceProvider
	{
		public ResourceManager()
		{
			Instance = this;
		}

		public delegate void ShaderChangedHandler(string name, Shader shader);
		public event ShaderChangedHandler ShaderChanged;

		public void AddShader(string name, string vertexFile, string fragmentFile,
			byte[] vertexShaderResource = null, byte[] fragmentShaderResource = null)
		{
			var sfd = new ShaderFileDebugger(vertexFile, fragmentFile, vertexShaderResource, fragmentShaderResource);
			shaderWatcher.Add(name, sfd);
		}

		public void CheckForShaderChange()
		{
			foreach(var item in shaderWatcher)
			{
				if(item.Value.CheckForShaderChange())
				{
					ShaderChanged?.Invoke(item.Key, item.Value.Shader);
				}
			}
		}

		public Shader GetShader(string name)
		{
			ShaderFileDebugger shaderFD;
			if(shaderWatcher.TryGetValue(name, out shaderFD))
			{
				return shaderFD.Shader;
			}
			return null;
		}

		public void Add<RESOURCE_TYPE>(string name, IResource<RESOURCE_TYPE> resource) where RESOURCE_TYPE : IDisposable
		{
			resources.Add(name, resource); //throws exception if key exists
		}

		public IResource<RESOURCE_TYPE> Get<RESOURCE_TYPE>(string name) where RESOURCE_TYPE : IDisposable
		{
			object resource;
			if(resources.TryGetValue(name, out resource))
			{
				return resource as IResource<RESOURCE_TYPE>;
			}
			return null;
		}

		private Dictionary<string, ShaderFileDebugger> shaderWatcher = new Dictionary<string, ShaderFileDebugger>();
		private Dictionary<string, object> resources = new Dictionary<string, object>();

		public static ResourceManager Instance { get; private set; }
	}
}