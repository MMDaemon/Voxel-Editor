using System;
using VoxelEditor.Common.Transfer;
using VoxelEditor.Core.Model;

namespace VoxelEditor.Menu.Model
{
	internal class MenuModel: IModel
	{
		public ViewModel ViewModel => CreateViewModel();
		public void Update(float absoluteTime, ModelInput input)
		{
			//TODO implement
		}
		private ViewModel CreateViewModel()
		{
			return new ViewModel();
			//TODO implement
		}

		public event EventHandler ModelEvent;
		public event EventHandler StateChanged;
	}
}
