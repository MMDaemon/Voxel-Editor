using VoxelEditor.Common.EventArguments;
using VoxelEditor.Common.Transfer;

namespace VoxelEditor.View
{
	internal interface IView
	{
		void Render(ViewModel viewModel);

		void ProcessModelEvent(ModelEventArgs modelEventArgs);
	}
}
