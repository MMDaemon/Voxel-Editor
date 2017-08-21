using System;
using VoxelEditor.Common.Enums;

namespace VoxelEditor.Common.EventArguments
{
	internal class StateChangedEventArgs: EventArgs
	{
		public State State { get; private set; }

		public StateChangedEventArgs(State state)
		{
			State = state;
		}
	}
}
