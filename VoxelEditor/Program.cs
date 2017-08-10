using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMS.Application;
using VoxelEditor.ControllerParts;

namespace VoxelEditor
{
	class Program
	{
		static void Main(string[] args)
		{
			ExampleApplication app = new ExampleApplication();
			Controller controller = new Controller();
			app.Update += controller.Update;
			app.Render += controller.Render;
			app.Run();
		}
	}
}
