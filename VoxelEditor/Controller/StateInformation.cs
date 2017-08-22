using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelEditor.Model;
using VoxelEditor.View;

namespace VoxelEditor.Controller
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
