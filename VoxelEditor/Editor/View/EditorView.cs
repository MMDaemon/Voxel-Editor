using System.IO;
using DMS.Application;
using DMS.Base;
using DMS.OpenGL;
using VoxelEditor.Common.Transfer;
using VoxelEditor.Core.View;
using VoxelEditor.Registry.View;

namespace VoxelEditor.Editor.View
{
	internal class EditorView : IView
	{
		private readonly ViewRegistry _registry;
		private readonly EditorVisual _visual;
		private readonly EditorSound _sound;

		public EditorView(ViewRegistry registry)
		{
			_registry = registry;
			_visual = new EditorVisual();
			_sound = new EditorSound();
		}

		public void ProcessModelEvent()
		{
			//TODO implement
		}

		public void LoadResources(ResourceManager resourceManager)
		{
			if (ReferenceEquals(null, resourceManager.GetShader(_visual.ShaderName)))
			{
				var dir = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + @"\Resources\";
				resourceManager.AddShader(_visual.ShaderName, dir + "vertex.glsl", dir + "fragment.glsl"
					, Resourcen.vertex, Resourcen.fragment);
			}
		}

		public void ShaderChanged(string name, Shader shader)
		{
			_visual.ShaderChanged(name, shader);
		}

		public void Render(ViewModel viewModel)
		{
			_visual.Render(viewModel);
		}

		public void Resize(int width, int height)
		{
			_visual.Resize(width, height);
		}
	}
}
