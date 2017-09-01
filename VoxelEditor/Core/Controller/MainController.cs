using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DMS.Application;
using DMS.Base;
using OpenTK;
using VoxelEditor.Common.Enums;
using VoxelEditor.Common.EventArguments;
using VoxelEditor.Core.Input;
using VoxelEditor.Core.Model;
using VoxelEditor.Core.View;
using VoxelEditor.Initialisation;
using VoxelEditor.Registry.Model;
using VoxelEditor.Registry.View;

namespace VoxelEditor.Core.Controller
{
	class MainController
	{
		private ExampleApplication _app;
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

		private void StateChanged(object sender, EventArgs e)
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
				if (stateInformation.ModelType.GetConstructor(new[] {typeof(ModelRegistry)}) != null)
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
			if (stateInformation.ViewType.GetConstructor(new[] { typeof(ViewRegistry) }) != null)
			{
				_view = (IView)Activator.CreateInstance(stateInformation.ViewType, _viewRegistry);
			}
			else
			{
				_view = (IView)Activator.CreateInstance(stateInformation.ViewType);
			}
			_app.ResourceManager.ShaderChanged += _view.ShaderChanged;
			_view.LoadResources(_app.ResourceManager);
		}
	}
}
