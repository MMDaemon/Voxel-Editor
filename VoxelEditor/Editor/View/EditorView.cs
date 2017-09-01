using System.IO;
using DMS.Application;
using DMS.Base;
using DMS.OpenGL;
using VoxelEditor.Common.EventArguments;
using VoxelEditor.Common.Transfer;
using VoxelEditor.Core.View;
using VoxelEditor.Registry.View;

namespace VoxelEditor.Editor.View
{
	internal class EditorView : IView
	{
		private ViewRegistry _registry;
		private EditorVisual _visual;
		private EditorSound _sound;
		private Shader _menuShader;

		public void ShaderChanged(string name, Shader shader)
		{
			if (nameof(_menuShader) != name) return;
			_menuShader = shader;
			if (ReferenceEquals(shader, null)) return;
			UpdateMesh();
		}

		public void LoadResources(ResourceManager resourceManager)
		{
			if (ReferenceEquals(null, resourceManager.GetShader(nameof(_menuShader))))
			{
				var dir = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + @"\Resources\";
				resourceManager.AddShader(nameof(_menuShader), dir + "vertex.glsl", dir + "fragment.glsl"
					, Resourcen.vertex, Resourcen.fragment);
			}
		}

		public EditorView(ViewRegistry registry)
		{
			_registry = registry;
			_visual = new EditorVisual();
			_sound = new EditorSound();
		}

		public void ProcessModelEvent(ModelEventArgs modelEventArgs)
		{
			//TODO implement
		}

		public void Render(ViewModel viewModel)
		{
			//TODO implement
		}
		private void UpdateMesh()
		{
			//TODO implement
		}
	}
}
