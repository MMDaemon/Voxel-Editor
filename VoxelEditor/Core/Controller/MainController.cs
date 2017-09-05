using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DMS.Application;
using OpenTK;
using VoxelEditor.Common.Enums;
using VoxelEditor.Core.Input;
using VoxelEditor.Core.Model;
using VoxelEditor.Core.View;
using VoxelEditor.Initialisation;
using VoxelEditor.Registry.Model;
using VoxelEditor.Registry.View;

namespace VoxelEditor.Core.Controller
{
	internal class MainController
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
			_app.Resize += Resize;
			_model.ModelEvent += _view.ProcessModelEvent;
			_model.StateChanged += StateChanged;

			_timeSource.Start();
			_app.Run();
		}

		private void Update(float updatePeriod)
		{
			_model.Update((float)_timeSource.Elapsed.TotalSeconds, _inputHandler.ModelInput);
		}

		private void Render()
		{
			_view.Render(_model.ViewModel);
		}

		private void Resize(int width, int height)
		{
			_view.Resize(width, height);
		}

		private void StateChanged(State state, bool temporary)
		{
			if (temporary)
			{
				_inactiveModels.Add(_model);
			}
			SetModelViewInstances(state);
		}

		private void SetModelViewInstances(State state)
		{
			StateInformation stateInformation = _stateHandler.GetStateInformation(state);

			if (stateInformation.ViewType.GetConstructor(new[] { typeof(ViewRegistry) }) != null)
			{
				_view = (IView)Activator.CreateInstance(stateInformation.ViewType, _viewRegistry);
			}
			else
			{
				_view = (IView)Activator.CreateInstance(stateInformation.ViewType);
			}

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
				_model.ModelEvent += _view.ProcessModelEvent;
				_model.StateChanged += StateChanged;
			}
			_app.ResourceManager.ShaderChanged += _view.ShaderChanged;
			_view.LoadResources(_app.ResourceManager);
		}
	}
}
