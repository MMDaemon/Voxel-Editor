using VoxelEditor.Common.Enums;
using VoxelEditor.Common.Transfer;

namespace VoxelEditor.Core.Model
{
	internal delegate void ModelEventHandler(/*TODO add arguments*/);
	internal delegate void StateChangedHandler(State state, bool temporary);

	internal interface IModel
	{
		ViewModel ViewModel { get; }

		void Update(float absoluteTime, ModelInput input);

		event ModelEventHandler ModelEvent;
		event StateChangedHandler StateChanged;
	}
}
