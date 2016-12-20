using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
	public class TestWorldBuilder : IWorldBuilder 
	{
		public void BuildWorldChunk(WorldData world, Chunk chunk)
		{
			IntVec3 size = world.ChunkSizeBlocks;

			for(int x=0;x<size.x;x++)
			{
				for(int z=0;z<size.z;z++)
				{
					int height = Random.Range(1,10);
					for(int y=0;y<height;y++)
					{
						world.SetBlock(new IntVec3(
							x + chunk.WorldPos.x,
							y + chunk.WorldPos.y,
							z + chunk.WorldPos.z),
							BlockType.Solid);
					}
					
				}
			}
		}
	}
}
