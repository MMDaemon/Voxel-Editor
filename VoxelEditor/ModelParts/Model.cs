using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelEditor.ControllerParts;

namespace VoxelEditor.ModelParts
{
	abstract class Model
	{
		private float _lastUpdateTime = 0.0f;
		protected float TimeAbsolute;
		protected float TimeDelta;

		public ViewModel ViewModel => CreateViewModel();

		public void Update(float absoluteTime, ModelCommands commands)
		{
			TimeAbsolute = absoluteTime;
			CalculateDeltaTime();
			UpdateFrame(commands);
		}

		protected abstract void UpdateFrame(ModelCommands commands);

		protected abstract ViewModel CreateViewModel();

		private void CalculateDeltaTime()
		{
			TimeDelta = TimeAbsolute - _lastUpdateTime;
			_lastUpdateTime = TimeAbsolute;
		}
		
	}
}
