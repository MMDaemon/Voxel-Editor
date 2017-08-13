using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace VoxelEditor.Controller
{
	class InputHandler
	{
		private GameWindow _gameWindow;

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
