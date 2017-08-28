using VoxelEditor.Common.EventArguments;
using VoxelEditor.Common.Transfer;

namespace VoxelEditor.Core.View
{
	internal interface IView
	{
		void Render(ViewModel viewModel);

		void ProcessModelEvent(ModelEventArgs modelEventArgs);
	}
}
