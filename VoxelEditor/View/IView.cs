using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelEditor.Controller;

namespace VoxelEditor.View
{
	internal interface IView
	{
		void Render(ViewModel viewModel);

		void ProcessModelEvent(ModelEventArgs modelEventArgs);
	}
}
