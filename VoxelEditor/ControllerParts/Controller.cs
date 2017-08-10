using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelEditor.ModelParts;
using VoxelEditor.ViewParts;

namespace VoxelEditor.ControllerParts
{
	class Controller
	{
		private readonly Model _model;
		private readonly Visual _visual;
		private readonly Stopwatch timeSource = new Stopwatch();
		public Controller()
		{
			_model = new EditorModel();
			_visual = new EditorVisual();
			timeSource.Start();
		}

		public void Update(float updatePeriod)
		{
			_model.Update((float)timeSource.Elapsed.TotalSeconds, new ModelCommands());
		}

		public void Render()
		{
			_visual.Render(_model.ViewModel);
		}
	}
}
