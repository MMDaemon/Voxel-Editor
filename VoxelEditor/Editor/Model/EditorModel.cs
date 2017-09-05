using System;
using System.Numerics;
using VoxelEditor.Common.Enums;
using VoxelEditor.Common.Transfer;
using VoxelEditor.Core.Model;
using VoxelEditor.Registry.Model;

namespace VoxelEditor.Editor.Model
{
	internal class EditorModel : IModel
	{
		private float _lastUpdateTime;
		private Player _player;
		public ViewModel ViewModel => CreateViewModel();

		public event ModelEventHandler ModelEvent;
		public event StateChangedHandler StateChanged;

		private ModelRegistry _registry;

		public EditorModel(ModelRegistry registry)
		{
			_registry = registry;
			_lastUpdateTime = 0.0f;
			_player = new Player();
		}

		public void Update(float absoluteTime, ModelInput input)
		{
			var timeDelta = absoluteTime - _lastUpdateTime;
			_lastUpdateTime = absoluteTime;
			Console.WriteLine($"MousePos:({input.MousePos.X},{input.MousePos.Y})");
			if (input.KeyActions.Contains(KeyAction.MoveUp))
			{
				_player.Move(Vector3.UnitY, timeDelta);
			}
			if (input.KeyActions.Contains(KeyAction.MoveDown))
			{
				_player.Move(-Vector3.UnitY, timeDelta);
			}
			if (input.KeyActions.Contains(KeyAction.MoveForwards))
			{
				_player.Move(Vector3.UnitZ, timeDelta);
			}
			if (input.KeyActions.Contains(KeyAction.MoveBackwards))
			{
				_player.Move(-Vector3.UnitZ, timeDelta);
			}
			if (input.KeyActions.Contains(KeyAction.MoveLeft))
			{
				_player.Move(Vector3.UnitX, timeDelta);
			}
			if (input.KeyActions.Contains(KeyAction.MoveRight))
			{
				_player.Move(-Vector3.UnitX, timeDelta);
			}
		}

		private ViewModel CreateViewModel()
		{
			ViewModel viewModel = new ViewModel(_player.Position);

			return viewModel;
		}

		private void OnModelEvent()
		{
			ModelEvent?.Invoke();
		}

		private void OnStateChanged(State state, bool temporary)
		{
			StateChanged?.Invoke(state, temporary);
		}
	}
}
