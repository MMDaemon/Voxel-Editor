using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelEditor.Common.Shared
{
	internal class Voxel
	{
		public int MaterialId { get; private set; }
		public int Amount { get; private set; }

		public Voxel(int materialId, int amount)
		{
			MaterialId = materialId;
			Amount = amount;
		}

		public void AddMaterial(int amount)
		{
			Amount += amount;
		}

		public void TakeMaterial(int amount)
		{
			Amount -= amount;
		}
	}
}
