using System;
using OpenTK;
using VoxelEditor.Common.Transfer;

namespace VoxelEditor.Controller
{
	class InputHandler
	{
		private readonly GameWindow _gameWindow;

		public InputHandler(GameWindow gameWindow)
		{
			_gameWindow = gameWindow;
		}

		public ModelInput ModelInput
		{
			get
			{
				Console.WriteLine($"MousePos:({_gameWindow.Mouse.X},{_gameWindow.Mouse.Y})");
				return new ModelInput();
				//TODO implement
			}
		}
	}
}
