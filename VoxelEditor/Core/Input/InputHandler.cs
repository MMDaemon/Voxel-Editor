using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using VoxelEditor.Common.Enums;
using VoxelEditor.Common.Transfer;
using Vector2 = System.Numerics.Vector2;

namespace VoxelEditor.Core.Input
{
	class InputHandler
	{
		private readonly GameWindow _gameWindow;

		private readonly List<Tuple<Key, KeyAction>> _pressKeyActions;
		private readonly List<Tuple<Key, KeyAction>> _holdKeyActions;
		private readonly List<Tuple<MouseButton, KeyAction>> _pressMouseButtonActions;
		private readonly List<Tuple<MouseButton, KeyAction>> _holdMouseButtonActions;

		private readonly Dictionary<Key, bool> _keyStatesBefore;
		private readonly Dictionary<Key, bool> _keyStates;
		private readonly Dictionary<MouseButton, bool> _mouseButtonStatesBefore;
		private readonly Dictionary<MouseButton, bool> _mouseButtonStates;

		public ModelInput ModelInput => CreateModelInput();

		public InputHandler(GameWindow gameWindow)
		{
			_gameWindow = gameWindow;

			_pressKeyActions = new List<Tuple<Key, KeyAction>>();
			_holdKeyActions = new List<Tuple<Key, KeyAction>>();
			_pressMouseButtonActions = new List<Tuple<MouseButton, KeyAction>>();
			_holdMouseButtonActions = new List<Tuple<MouseButton, KeyAction>>();

			_keyStatesBefore = new Dictionary<Key, bool>();
			_keyStates = new Dictionary<Key, bool>();
			_mouseButtonStatesBefore = new Dictionary<MouseButton, bool>();
			_mouseButtonStates = new Dictionary<MouseButton, bool>();

			_gameWindow.KeyDown += (sender, args) => KeyDown(args.Key);
			_gameWindow.KeyUp += (sender, args) => KeyUp(args.Key);
			_gameWindow.MouseDown += (sender, args) => MouseDown(args.Button);
			_gameWindow.MouseUp += (sender, args) => MouseUp(args.Button);
		}

		public void AddPressKeyAction(Key key, KeyAction keyAction)
		{
			_pressKeyActions.Add(new Tuple<Key, KeyAction>(key, keyAction));
			if (!_keyStatesBefore.ContainsKey(key)) _keyStatesBefore.Add(key, false);
			if (!_keyStates.ContainsKey(key)) _keyStates.Add(key, false);
		}

		public void AddPressKeyAction(MouseButton mouseButton, KeyAction keyAction)
		{
			_pressMouseButtonActions.Add(new Tuple<MouseButton, KeyAction>(mouseButton, keyAction));
			if (!_mouseButtonStatesBefore.ContainsKey(mouseButton)) _mouseButtonStatesBefore.Add(mouseButton, false);
			if (!_mouseButtonStates.ContainsKey(mouseButton)) _mouseButtonStates.Add(mouseButton, false);
		}

		public void AddHoldKeyAction(Key key, KeyAction keyAction)
		{
			_holdKeyActions.Add(new Tuple<Key, KeyAction>(key, keyAction));
			if (!_keyStates.ContainsKey(key)) _keyStates.Add(key, false);
		}

		public void AddHoldKeyAction(MouseButton mouseButton, KeyAction keyAction)
		{
			_holdMouseButtonActions.Add(new Tuple<MouseButton, KeyAction>(mouseButton, keyAction));
			if (!_mouseButtonStates.ContainsKey(mouseButton)) _mouseButtonStates.Add(mouseButton, false);
		}

		private ModelInput CreateModelInput()
		{
			ModelInput modelInput = new ModelInput(GetMousePos());
			modelInput.KeyActions.AddRange(GetKeyActions());
			return modelInput;
		}

		private void KeyDown(Key key)
		{
			if (_keyStates.ContainsKey(key))
			{
				_keyStates[key] = true;
			}
		}

		private void KeyUp(Key key)
		{
			if (_keyStates.ContainsKey(key))
			{
				_keyStates[key] = false;
			}
		}

		private void MouseDown(MouseButton mouseButton)
		{
			if (_mouseButtonStates.ContainsKey(mouseButton))
			{
				_mouseButtonStates[mouseButton] = true;
			}
		}

		private void MouseUp(MouseButton mouseButton)
		{
			if (_mouseButtonStates.ContainsKey(mouseButton))
			{
				_mouseButtonStates[mouseButton] = false;
			}
		}

		private Vector2 GetMousePos()
		{
			return new Vector2((float)(_gameWindow.Mouse.X - _gameWindow.Width / 2) / _gameWindow.Height, (float)(_gameWindow.Mouse.Y - _gameWindow.Height / 2) / _gameWindow.Height);
		}

		private IEnumerable<KeyAction> GetKeyActions()
		{
			List<KeyAction> keyActionList = new List<KeyAction>();

			//get keyActions

			foreach (var pressKeyAction in _pressKeyActions)
			{
				if (!keyActionList.Contains(pressKeyAction.Item2) && _keyStates[pressKeyAction.Item1] && !_keyStatesBefore[pressKeyAction.Item1])
				{
					keyActionList.Add(pressKeyAction.Item2);
				}
			}

			foreach (var holdKeyAction in _holdKeyActions)
			{
				if (!keyActionList.Contains(holdKeyAction.Item2) && _keyStates[holdKeyAction.Item1])
				{
					keyActionList.Add(holdKeyAction.Item2);
				}
			}

			foreach (var pressMouseButtonAction in _pressMouseButtonActions)
			{
				if (!keyActionList.Contains(pressMouseButtonAction.Item2) && _mouseButtonStates[pressMouseButtonAction.Item1] && !_mouseButtonStatesBefore[pressMouseButtonAction.Item1])
				{
					keyActionList.Add(pressMouseButtonAction.Item2);
				}
			}

			foreach (var holdMouseButtonAction in _holdMouseButtonActions)
			{
				if (!keyActionList.Contains(holdMouseButtonAction.Item2) && _mouseButtonStates[holdMouseButtonAction.Item1])
				{
					keyActionList.Add(holdMouseButtonAction.Item2);
				}
			}

			//set before states

			foreach (var key in _keyStatesBefore.Keys)
			{
				_keyStatesBefore[key] = _keyStates[key];
			}

			foreach (var mouseButton in _mouseButtonStatesBefore.Keys)
			{
				_mouseButtonStatesBefore[mouseButton] = _mouseButtonStates[mouseButton];
			}

			return keyActionList;
		}
	}
}
