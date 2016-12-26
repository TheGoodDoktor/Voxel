using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
    [CreateAssetMenu(fileName = "TestWorldBuilder", menuName = "Voxel/World Builders/Test", order = 1)]
	public class TestWorldBuilder : WorldBuilder 
	{
        byte m_SolidBlock = (byte)BlockType.Solid;

        public override void Init(WorldData world)
        {
        }

        public override void BuildWorldChunk(WorldData world, Chunk chunk)
		{
			IntVec3 size = world.ChunkSizeBlocks;

			for(int x=0;x<size.x;x++)
			{
				for(int z=0;z<size.z;z++)
				{
					int height = Random.Range(1,10);
					for(int y=0;y<height;y++)
					{
						world.SetBlock(
							x + chunk.WorldPos.x,
							y + chunk.WorldPos.y,
							z + chunk.WorldPos.z,
							m_SolidBlock);
					}					
				}
			}
		}
	}
}
