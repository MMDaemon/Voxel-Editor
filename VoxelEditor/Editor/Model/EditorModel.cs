using VoxelEditor.Common.Enums;
using VoxelEditor.Common.Transfer;
using VoxelEditor.Core.Model;
using VoxelEditor.Registry.Model;

namespace VoxelEditor.Editor.Model
{
	internal class EditorModel : IModel
	{
		public ViewModel ViewModel => CreateViewModel();

		public event ModelEventHandler ModelEvent;
		public event StateChangedHandler StateChanged;

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

		private void OnModelEvent()
		{
			ModelEvent?.Invoke();
		}

		private void OnStateChanged(State state, bool temporary)
		{
			StateChanged?.Invoke(state, temporary);
		}
	}
}
