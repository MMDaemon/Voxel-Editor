using DMS.Application;
using VoxelEditor.Controller;

namespace VoxelEditor
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			ExampleApplication app = new ExampleApplication();
			MainController controller = new MainController();
			app.Update += controller.Update;
			app.Render += controller.Render;
			app.Run();
		}
	}
}
