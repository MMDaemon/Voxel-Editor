using System;
using DMS.Application;
using DMS.OpenGL;

namespace VoxelEditor.MVCInterfaces
{
	public interface IView
	{
		void ShaderChanged(string name, Shader shader);

		void LoadResources(ResourceManager resourceManager);

		void Render(IViewModel viewModel);

		void Resize(int width, int height);
	    void ProcessModelEvent(EventArgs e);
	}
}
