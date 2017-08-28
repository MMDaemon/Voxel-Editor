using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DMS.Application;
using OpenTK;
using VoxelEditor.Common.Enums;
using VoxelEditor.Model;
using VoxelEditor.View;
using VoxelEditor.Common.EventArguments;
using VoxelEditor.Controller.Initialisation;
using VoxelEditor.Model.Registry;
using VoxelEditor.View.Registry;

namespace VoxelEditor.Controller
{
	class MainController
	{
		private readonly ExampleApplication _app;
		private readonly InputHandler _inputHandler;
		private readonly StateHandler _stateHandler;
		private readonly ModelRegistry _modelRegistry;
		private readonly ViewRegistry _viewRegistry;
		private readonly List<IModel> _inactiveModels;
		private IModel _model;
		private IView _view;
		private readonly Stopwatch _timeSource;

		public MainController()
		{
			_app = new ExampleApplication();
			InitializationHandler initializationHandler = new InitializationHandler();
			_inputHandler = new InputHandler((GameWindow)_app.GameWindow);
			_stateHandler = initializationHandler.InitializeStateHandler();
			_modelRegistry = initializationHandler.InitalizeModelRegistry();
			_viewRegistry = initializationHandler.InitializeViewRegistry();
			_inactiveModels = new List<IModel>();
			SetModelViewInstances(State.Start);

			_timeSource = new Stopwatch();

			_app.Update += Update;
			_app.Render += Render;
			_model.ModelEvent += (sender, args) => _view.ProcessModelEvent((ModelEventArgs)args);
			_model.StateChanged += StateChanged;

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

		private void StateChanged(object sender, System.EventArgs e)
		{
			if (((StateChangedEventArgs)e).Temporary)
			{
				_inactiveModels.Add(_model);
			}
			SetModelViewInstances(((StateChangedEventArgs)e).State);
		}

		private void SetModelViewInstances(State state)
		{
			StateInformation stateInformation = _stateHandler.GetStateInformation(state);
			IModel existingModel = (from inactiveModel in _inactiveModels
									where inactiveModel.GetType() == stateInformation.ModelType
									select inactiveModel).FirstOrDefault();
			if (existingModel != null)
			{
				_model = existingModel;
				_inactiveModels.Remove(existingModel);
			}
			else
			{
				if (stateInformation.ModelType.GetConstructor(new Type[] {typeof(ModelRegistry)}) != null)
				{
					_model = (IModel) Activator.CreateInstance(stateInformation.ModelType, _modelRegistry);
				}
				else
				{
					_model = (IModel)Activator.CreateInstance(stateInformation.ModelType);
				}
				_model.ModelEvent += (sender, args) => _view.ProcessModelEvent((ModelEventArgs)args);
				_model.StateChanged += StateChanged;
			}
			object[] viewRegistry = { _viewRegistry };
			if (stateInformation.ViewType.GetConstructor(new Type[] { typeof(ViewRegistry) }) != null)
			{
				_view = (IView)Activator.CreateInstance(stateInformation.ViewType, _viewRegistry);
			}
			else
			{
				_view = (IView)Activator.CreateInstance(stateInformation.ViewType);
			}
		}
	}
}
