using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
    public interface IMeshBuilder
    {
        void BuildMeshFromChunk(Chunk chunk);
    }


}
