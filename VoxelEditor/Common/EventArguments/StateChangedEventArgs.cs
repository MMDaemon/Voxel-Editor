using System;
using VoxelEditor.Common.Enums;

namespace VoxelEditor.Common.EventArguments
{
	internal class StateChangedEventArgs: EventArgs
	{
		public State State { get; private set; }
		public bool Temporary { get; private set; }

		public StateChangedEventArgs(State state, bool temporary = false)
		{
			State = state;
			Temporary = temporary;
		}
	}
}
