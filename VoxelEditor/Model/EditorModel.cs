using System;
using VoxelEditor.Controller;

namespace VoxelEditor.Model
{
	internal class EditorModel : IModel
	{
		public ViewModel ViewModel => CreateViewModel();

		public event EventHandler ModelEvent;

		public void Update(float absoluteTime, ModelInput input)
		{
			//TODO implement
		}

		private ViewModel CreateViewModel()
		{
			return new ViewModel();
			//TODO implement
		}

		private void OnModelEvent(ModelEventArgs e)
		{
			ModelEvent?.Invoke(this, e);
		}
	}
}
