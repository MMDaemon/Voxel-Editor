﻿using System.Collections.Generic;
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

        private readonly List<(Key Key, KeyAction KeyAction)> _pressKeyActions;
        private readonly List<(Key Key, KeyAction KeyAction)> _holdKeyActions;
        private readonly List<(MouseButton MouseButton, KeyAction KeyAction)> _pressMouseButtonActions;
        private readonly List<(MouseButton MouseButton, KeyAction KeyAction)> _holdMouseButtonActions;

        private readonly Dictionary<Key, bool> _keyStatesBefore;
        private readonly Dictionary<Key, bool> _keyStates;
        private readonly Dictionary<MouseButton, bool> _mouseButtonStatesBefore;
        private readonly Dictionary<MouseButton, bool> _mouseButtonStates;

        public ModelInput ModelInput => CreateModelInput();

        public InputHandler(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;

            _pressKeyActions = new List<(Key Key, KeyAction KeyAction)>();
            _holdKeyActions = new List<(Key Key, KeyAction KeyAction)>();
            _pressMouseButtonActions = new List<(MouseButton MouseButton, KeyAction KeyAction)>();
            _holdMouseButtonActions = new List<(MouseButton MouseButton, KeyAction KeyAction)>();

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
            _pressKeyActions.Add((key, keyAction));
            if (!_keyStatesBefore.ContainsKey(key)) _keyStatesBefore.Add(key, false);
            if (!_keyStates.ContainsKey(key)) _keyStates.Add(key, false);
        }

        public void AddPressKeyAction(MouseButton mouseButton, KeyAction keyAction)
        {
            _pressMouseButtonActions.Add((mouseButton, keyAction));
            if (!_mouseButtonStatesBefore.ContainsKey(mouseButton)) _mouseButtonStatesBefore.Add(mouseButton, false);
            if (!_mouseButtonStates.ContainsKey(mouseButton)) _mouseButtonStates.Add(mouseButton, false);
        }

        public void AddHoldKeyAction(Key key, KeyAction keyAction)
        {
            _holdKeyActions.Add((key, keyAction));
            if (!_keyStates.ContainsKey(key)) _keyStates.Add(key, false);
        }

        public void AddHoldKeyAction(MouseButton mouseButton, KeyAction keyAction)
        {
            _holdMouseButtonActions.Add((mouseButton, keyAction));
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
            return new Vector2((float)(_gameWindow.Mouse.X - _gameWindow.Width / 2) / _gameWindow.Height * 2, (float)(_gameWindow.Mouse.Y - _gameWindow.Height / 2) / _gameWindow.Height * 2);
        }

        private IEnumerable<KeyAction> GetKeyActions()
        {
            List<KeyAction> keyActionList = new List<KeyAction>();

            //get keyActions

            foreach (var pressKeyAction in _pressKeyActions)
            {
                if (!keyActionList.Contains(pressKeyAction.KeyAction) && _keyStates[pressKeyAction.Key] && !_keyStatesBefore[pressKeyAction.Key])
                {
                    keyActionList.Add(pressKeyAction.KeyAction);
                }
            }

            foreach (var holdKeyAction in _holdKeyActions)
            {
                if (!keyActionList.Contains(holdKeyAction.KeyAction) && _keyStates[holdKeyAction.Key])
                {
                    keyActionList.Add(holdKeyAction.KeyAction);
                }
            }

            foreach (var pressMouseButtonAction in _pressMouseButtonActions)
            {
                if (!keyActionList.Contains(pressMouseButtonAction.KeyAction) && _mouseButtonStates[pressMouseButtonAction.MouseButton] && !_mouseButtonStatesBefore[pressMouseButtonAction.MouseButton])
                {
                    keyActionList.Add(pressMouseButtonAction.KeyAction);
                }
            }

            foreach (var holdMouseButtonAction in _holdMouseButtonActions)
            {
                if (!keyActionList.Contains(holdMouseButtonAction.KeyAction) && _mouseButtonStates[holdMouseButtonAction.MouseButton])
                {
                    keyActionList.Add(holdMouseButtonAction.KeyAction);
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