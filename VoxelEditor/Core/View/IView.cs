using DMS.Application;
using DMS.OpenGL;
using VoxelEditor.Common.Transfer;

namespace VoxelEditor.Core.View
{
	internal interface IView
	{
		void ShaderChanged(string name, Shader shader);

		void LoadResources(ResourceManager resourceManager);

		void Render(ViewModel viewModel);

		void Resize(int width, int height);

		void ProcessModelEvent();
	}
}
