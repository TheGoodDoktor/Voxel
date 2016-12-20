using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
    public interface IWorldBuilder
    {
        void BuildWorldChunk(WorldData world, Chunk chunk);
    }
}
