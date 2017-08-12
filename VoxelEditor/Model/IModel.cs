using System;
using VoxelEditor.Controller;

namespace VoxelEditor.Model
{
	internal interface IModel
	{
		ViewModel ViewModel { get; }

		void Update(float absoluteTime, ModelCommands commands);

		event EventHandler ModelEvent;
	}
}
