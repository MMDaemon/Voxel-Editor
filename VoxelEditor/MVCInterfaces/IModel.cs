using System;

namespace VoxelEditor.MVCInterfaces
{
	public delegate void StateChangedHandler(int state, bool temporary);

	public interface IModel
	{
		IViewModel ViewModel { get; }

		void Update(float absoluteTime, ModelInput input);

		event EventHandler ModelEvent;
		event StateChangedHandler StateChanged;
	}
}
