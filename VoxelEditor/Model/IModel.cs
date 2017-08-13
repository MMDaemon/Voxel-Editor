using System;
using VoxelEditor.Controller;

namespace VoxelEditor.Model
{
	internal interface IModel
	{
		ViewModel ViewModel { get; }

		void Update(float absoluteTime, ModelInput input);

		event EventHandler ModelEvent;
	}
}
