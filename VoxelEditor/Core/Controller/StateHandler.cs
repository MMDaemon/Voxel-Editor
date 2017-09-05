using System;
using System.Collections.Generic;
using VoxelEditor.Common.Enums;

namespace VoxelEditor.Core.Controller
{
	internal class StateHandler
	{
		private readonly Dictionary<State, Tuple<Type, Type>> _stateInformation;

		public StateHandler()
		{
			_stateInformation = new Dictionary<State, Tuple<Type, Type>>();
		}

		public void AddStateInformation(State state, Type modelType, Type viewType)
		{
			_stateInformation.Add(state, new Tuple<Type, Type>(modelType, viewType));
		}

		/// <summary>
		/// Returns the ModelType and ViewType bound to the given State
		/// </summary>
		/// <param name="state"></param>
		/// <returns>A tuple containing ModelType and ViewType</returns>
		public Tuple<Type, Type> GetStateInformation(State state)
		{
			return _stateInformation[state];
		}
	}
}
