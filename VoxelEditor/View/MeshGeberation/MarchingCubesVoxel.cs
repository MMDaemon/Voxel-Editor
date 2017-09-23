using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VoxelUtils;
using VoxelUtils.Shared;

namespace VoxelEditor.View.MeshGeberation
{
    public class MarchingCubesVoxel
    {
        private List<Voxel> _voxels;
        private List<Vector3> _internalPositions;

        public MarchingCubesVoxel(List<Voxel> voxels)
        {
            _voxels = voxels;
            _internalPositions = new List<Vector3>();
        }
        private void InitiateInternalPositions()
        {
            if (_voxels[0].FillingQuantity<0.5 && _voxels[1].Amount<0.5)
            {
                
            }
            else if (_voxels[0].Amount<0.5 && _voxels[1].Amount>0.5)
            {
                
            }
        }
    }
}
