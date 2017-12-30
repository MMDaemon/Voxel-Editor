using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace MVCCore.Interfaces
{
	public class ModelInput
	{
		public Vector2 MousePosition { get; private set; }

	    public Point ScreenCenter { get; private set; }

		public List<int> KeyActions { get; private set; }

		public ModelInput(Vector2 mousePosition, Point screenCenter)
		{
			MousePosition = mousePosition;
		    ScreenCenter = screenCenter;
		    KeyActions = new List<int>();
		}
	}
}
