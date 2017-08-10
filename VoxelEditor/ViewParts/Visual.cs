using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelEditor.ControllerParts;

namespace VoxelEditor.ViewParts
{
	abstract class Visual
	{
		public abstract void Render(ViewModel viewModel);
	}
}
