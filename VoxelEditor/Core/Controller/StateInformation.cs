using System;

namespace VoxelEditor.Core.Controller
{
	internal class StateInformation
	{
		public Type ModelType { get; private set; }
		public Type ViewType { get; private set; }

		public StateInformation(Type modelType, Type viewType)
		{
			ModelType = modelType;
			ViewType = viewType;
		}
	}
}
