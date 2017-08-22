using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelEditor.Common
{
	internal static class Constant
	{
		public const int MaxMaterialAmount = 128;
		public const int ChunkSizeX = 32;
		public const int ChunkSizeY = 32;
		public const int ChunkSizeZ = 32;
		public const int MaxVoxelsPerChunk = ChunkSizeX*ChunkSizeY*ChunkSizeZ;
	}
}
