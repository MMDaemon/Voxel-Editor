using System;
using VoxelEditor.Common.EventArguments;
using VoxelEditor.Common.Transfer;
using VoxelEditor.Core.Model;
using VoxelEditor.Registry.Model;

namespace VoxelEditor.Editor.Model
{
	internal class EditorModel : IModel
	{
		public ViewModel ViewModel => CreateViewModel();

		public event EventHandler ModelEvent;
		public event EventHandler StateChanged;

		private ModelRegistry _registry;

		public EditorModel(ModelRegistry registry)
		{
			_registry = registry;
		}

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

		private void OnStateChanged(StateChangedEventArgs e)
		{
			StateChanged?.Invoke(this, e);
		}
	}
}
