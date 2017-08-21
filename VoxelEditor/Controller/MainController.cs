﻿using System;
using System.Diagnostics;
using DMS.Application;
using OpenTK;
using VoxelEditor.Common.Enums;
using VoxelEditor.Model;
using VoxelEditor.View;
using VoxelEditor.View.Editor;
using VoxelEditor.Common.EventArguments;
using VoxelEditor.Model.Editor;
using VoxelEditor.Model.Menu;
using VoxelEditor.View.Menu;

namespace VoxelEditor.Controller
{
	class MainController
	{
		private readonly ExampleApplication _app;
		private readonly InputHandler _inputHandler;
		private readonly StateHandler _stateHandler;
		private IModel _model;
		private IView _view;
		private readonly Stopwatch _timeSource;
		
		public MainController()
		{
			 _app = new ExampleApplication();
			_inputHandler = new InputHandler((GameWindow)_app.GameWindow);
			_stateHandler = new StateHandler();
			InitializeStateHandler();
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

		private void InitializeStateHandler()
		{
			_stateHandler.AddStateInformation(State.Start, typeof(EditorModel), typeof(EditorView));
			_stateHandler.AddStateInformation(State.Menu, typeof(MenuModel), typeof(MenuView));
			_stateHandler.AddStateInformation(State.Editor, typeof(EditorModel), typeof(EditorView));
		}

		private void StateChanged(object sender, System.EventArgs e)
		{
			SetModelViewInstances(((StateChangedEventArgs)e).State);
		}

		private void SetModelViewInstances(State state)
		{
			StateInformation stateInformation = _stateHandler.GetStateInformation(state);
			_model = (IModel)Activator.CreateInstance(stateInformation.ModelType);
			_view = (IView)Activator.CreateInstance(stateInformation.ViewType);
		}
	}
}
