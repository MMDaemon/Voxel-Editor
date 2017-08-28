using VoxelEditor.Common.EventArguments;
using VoxelEditor.Common.Transfer;
using VoxelEditor.View.Registry;

namespace VoxelEditor.View.Editor
{
	internal class EditorView : IView
	{
		private ViewRegistry _registry;
		private EditorVisual _visual;
		private EditorSound _sound;
		
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
	}
}
