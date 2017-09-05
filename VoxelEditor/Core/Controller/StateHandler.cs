using System;
using System.Collections.Generic;
using VoxelEditor.Common.Enums;

namespace VoxelEditor.Core.Controller
{
	internal class StateHandler
	{
        		private readonly Dictionary<State, (Type ModelType, Type ValueType)> _stateInformation;

		public StateHandler()
		{
			_stateInformation = new Dictionary<State, (Type ModelType, Type ValueType)>();
		}

		public void AddStateInformation(State state, Type modelType, Type viewType)
		{
			_stateInformation.Add(state, (modelType, viewType));
		}

		/// <summary>
		/// Returns the ModelType and ViewType bound to the given State
		/// </summary>
		/// <param name="state"></param>
		/// <returns>A tuple containing ModelType and ViewType</returns>
		public (Type ModelType, Type ValueType) GetStateInformation(State state)
		{
			return _stateInformation[state];
		}
	}
}
