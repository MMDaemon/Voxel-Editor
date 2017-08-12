using System.Diagnostics;
using VoxelEditor.Model;
using VoxelEditor.View;
using VoxelEditor.View.Editor;

namespace VoxelEditor.Controller
{
	class MainController
	{
		private readonly IModel _model;
		private readonly IView _view;
		private readonly Stopwatch _timeSource = new Stopwatch();
		public MainController()
		{
			_model = new EditorModel();
			_view = new EditorView();
			_model.ModelEvent += (sender, args) => _view.ProcessModelEvent((ModelEventArgs)args);
			_timeSource.Start();
		}

		public void Update(float updatePeriod)
		{
			_model.Update((float)_timeSource.Elapsed.TotalSeconds, new ModelCommands());
		}

		public void Render()
		{
			_view.Render(_model.ViewModel);
		}
	}
}
