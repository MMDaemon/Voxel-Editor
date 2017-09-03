using System.IO;
using DMS.Application;
using DMS.Base;
using DMS.OpenGL;
using VoxelEditor.Common.Transfer;
using VoxelEditor.Core.View;

namespace VoxelEditor.Menu.View
{
	internal class MenuView: IView
	{
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

		public void Render(ViewModel viewModel)
		{
			//TODO implement
		}

		public void ProcessModelEvent()
		{
			//TODO implement
		}

		private void UpdateMesh()
		{
			//TODO implement
		}
	}
}
