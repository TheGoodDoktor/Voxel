using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
    public interface IWorldBuilder
    {
        void Init(WorldData world);
        void BuildWorldChunk(WorldData world, Chunk chunk);
    }
}
