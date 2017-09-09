using DMS.Application;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MVCCore.Handlers;
using MVCCore.Interfaces;

namespace MVCCore
{
    public class MainController
    {
        private readonly ExampleApplication _app;
        private readonly InputHandler _inputHandler;
        private readonly StateHandler _stateHandler;
        private readonly IModelRegistry _modelRegistry;
        private readonly IViewRegistry _viewRegistry;
        private readonly List<IModel> _inactiveModels;
        private IModel _model;
        private IView _view;
        private readonly Stopwatch _timeSource;

        public MainController(IInitializationHandler initializationHandler)
        {
            _app = new ExampleApplication();
            _inputHandler = initializationHandler.InitializeInputHandler((GameWindow)_app.GameWindow);
            _stateHandler = initializationHandler.InitializeStateHandler();
            _modelRegistry = initializationHandler.InitalizeModelRegistry();
            _viewRegistry = initializationHandler.InitializeViewRegistry();
            _inactiveModels = new List<IModel>();
            SetModelViewInstances(initializationHandler.InitialState);

            _timeSource = new Stopwatch();

            _app.Update += Update;
            _app.Render += Render;
            _app.Resize += Resize;
            _model.ModelEvent += (sender, e) => _view.ProcessModelEvent(e);
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
            _model.Resize(width, height);
        }

        private void StateChanged(int state, bool temporary)
        {
            if (temporary)
            {
                _inactiveModels.Add(_model);
            }
            SetModelViewInstances(state);
        }

        private void SetModelViewInstances(int state)
        {
            (Type ModelType, Type ViewType) stateInformation = _stateHandler.GetStateInformation(state);

            if (stateInformation.ViewType.GetConstructor(new[] { typeof(IViewRegistry) }) != null)
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
                if (stateInformation.ModelType.GetConstructor(new[] { typeof(IModelRegistry) }) != null)
                {
                    _model = (IModel)Activator.CreateInstance(stateInformation.ModelType, _modelRegistry);
                }
                else
                {
                    _model = (IModel)Activator.CreateInstance(stateInformation.ModelType);
                }
                _model.ModelEvent += (sender, e) => _view.ProcessModelEvent(e);
                _model.StateChanged += StateChanged;
            }
            _app.ResourceManager.ShaderChanged += _view.ShaderChanged;
            _view.LoadResources(_app.ResourceManager);
        }
    }
}
