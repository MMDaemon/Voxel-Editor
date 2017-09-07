using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MVCCore.Interfaces;
using VoxelEditor.Editor.ViewModel;
using VoxelUtils.Common.Enums;
using VoxelUtils.Registry.Model;

namespace VoxelEditor.Editor.Model
{
    public class EditorModel : ModelRegistryContainer, IModel
    {
        private float _lastUpdateTime;
        private float _deltaTime;
        private Vector2 _lastMousePosition;
        private Vector2 _deltaMousePosition;
        private readonly Player _player;
        public IViewModel ViewModel => CreateViewModel();

        public event EventHandler ModelEvent;
        public event StateChangedHandler StateChanged;

        private ModelRegistry _registry;

        public EditorModel(IModelRegistry registry) : base(registry)
        {
            _registry = (ModelRegistry)registry;
            _lastUpdateTime = 0.0f;
            _lastMousePosition = Vector2.Zero;
            _player = new Player();
        }

        public void Update(float absoluteTime, ModelInput input)
        {
            _deltaTime = absoluteTime - _lastUpdateTime;
            _lastUpdateTime = absoluteTime;
            _deltaMousePosition = input.MousePosition - _lastMousePosition;
            _lastMousePosition = input.MousePosition;
            HandleKeyActions(input.KeyActions.Cast<KeyAction>().ToList());
        }

        private void HandleKeyActions(ICollection<KeyAction> keyActions)
        {
            if (keyActions.Contains(KeyAction.MoveUp))
            {
                _player.Move(Vector3.UnitY, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.MoveDown))
            {
                _player.Move(-Vector3.UnitY, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.MoveForwards))
            {
                _player.Move(-Vector3.UnitZ, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.MoveBackwards))
            {
                _player.Move(Vector3.UnitZ, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.MoveLeft))
            {
                _player.Move(-Vector3.UnitX, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.MoveRight))
            {
                _player.Move(Vector3.UnitX, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.EnableCameraRotation))
            {
                CalculatePlayerRotation();
            }
        }

        private void CalculatePlayerRotation()
        {
            _player.Rotate(300 * new Vector2(-_deltaMousePosition.Y, _deltaMousePosition.X));
        }

        private EditorViewModel CreateViewModel()
        {
            EditorViewModel viewModel = new EditorViewModel(_player.Position, _player.Rotation);

            return viewModel;
        }

        private void OnModelEvent()
        {
            ModelEvent?.Invoke(null, null);
        }

        private void OnStateChanged(State state, bool temporary)
        {
            StateChanged?.Invoke((int)state, temporary);
        }
    }
}
