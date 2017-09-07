using System.Numerics;
using VoxelEditor.Common.Enums;
using VoxelEditor.Common.Transfer;
using VoxelEditor.Core.Model;

namespace VoxelEditor.Menu.Model
{
	internal class MenuModel: IModel
	{
		public ViewModel ViewModel => CreateViewModel();

		public event ModelEventHandler ModelEvent;
		public event StateChangedHandler StateChanged;

		public void Update(float absoluteTime, ModelInput input)
		{
			//TODO implement
		}
		private ViewModel CreateViewModel()
		{
			return new ViewModel(Vector3.Zero, Vector2.Zero);
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
