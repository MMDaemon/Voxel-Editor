using System;
using System.Collections.Generic;
using VoxelEditor.Common.Enums;
using VoxelEditor.Model;
using VoxelEditor.View;

namespace VoxelEditor.Controller
{
	internal class StateHandler
	{
		private readonly Dictionary<State, StateInformation> _stateInformation;

		public StateHandler()
		{
			_stateInformation = new Dictionary<State, StateInformation>();
		}

		public void AddStateInformation(State state, Type modelType, Type viewType)
		{
			_stateInformation.Add(state, new StateInformation(modelType, viewType));
		}
		public StateInformation GetStateInformation(State state)
		{
			return _stateInformation[state];
		}
	}
}
