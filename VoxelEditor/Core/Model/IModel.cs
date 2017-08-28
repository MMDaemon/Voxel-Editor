using System;
using VoxelEditor.Common.Transfer;

namespace VoxelEditor.Core.Model
{
	internal interface IModel
	{
		ViewModel ViewModel { get; }

		void Update(float absoluteTime, ModelInput input);

		event EventHandler ModelEvent;
		event EventHandler StateChanged;
	}
}
