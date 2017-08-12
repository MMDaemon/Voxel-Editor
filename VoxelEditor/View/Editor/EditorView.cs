using VoxelEditor.Controller;

namespace VoxelEditor.View.Editor
{
	internal class EditorView : IView
	{
		private EditorVisual _visual;
		private EditorSound _sound;

		public EditorView()
		{
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
