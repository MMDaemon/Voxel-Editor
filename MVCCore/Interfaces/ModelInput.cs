using System.Collections.Generic;
using System.Drawing;
using OpenTK.Input;

namespace MVCCore.Interfaces
{
	public class ModelInput
	{
		public MouseState MouseState { get; private set; }

	    public Point ScreenCenter { get; private set; }

		public List<int> KeyActions { get; private set; }

		public ModelInput(MouseState mouseState, Point screenCenter)
		{
			MouseState = mouseState;
		    ScreenCenter = screenCenter;
		    KeyActions = new List<int>();
		}
	}
}
