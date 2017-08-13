using System.Diagnostics;
using DMS.Application;
using OpenTK;
using VoxelEditor.Model;
using VoxelEditor.View;
using VoxelEditor.View.Editor;

namespace VoxelEditor.Controller
{
	class MainController
	{
		private readonly ExampleApplication _app;
		private readonly InputHandler _inputHandler;
		private readonly IModel _model;
		private readonly IView _view;
		private readonly Stopwatch _timeSource;
		public MainController()
		{
			 _app = new ExampleApplication();
			_inputHandler = new InputHandler((GameWindow)_app.GameWindow);
			_model = new EditorModel();
			_view = new EditorView();
			_timeSource = new Stopwatch();

			_app.Update += Update;
			_app.Render += Render;
			_model.ModelEvent += (sender, args) => _view.ProcessModelEvent((ModelEventArgs)args);

			_timeSource.Start();
			_app.Run();
		}

		public void Update(float updatePeriod)
		{
			_model.Update((float)_timeSource.Elapsed.TotalSeconds, _inputHandler.ModelInput);
		}

		public void Render()
		{
			_view.Render(_model.ViewModel);
		}
	}
}
